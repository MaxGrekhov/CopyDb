using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CopyDb.Common;
using CopyDb.Models;
using Microsoft.Extensions.Configuration;

namespace CopyDb.Core
{
    public interface ICopyService
    {
        Task<bool> Run();
    }

    public class CopyService : ICopyService
    {
        private readonly IConfiguration _config;
        private readonly ILoggerService<CopyService> _logger;

        public CopyService(IConfiguration config, ILoggerService<CopyService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<bool> Run()
        {
            try
            {
                var config = _config.Get<AppConfig>() ?? new AppConfig();
                var source = GetProvider(config.Source?.Type, config.Source?.Schema, config.Source?.Connection);
                var destination = GetProvider(config.Destination?.Type, config.Destination?.Schema, config.Destination?.Connection);
                if (source == null || destination == null)
                    return false;
                var srcTables = await source.GetInfos(config.Source?.Filter);
                var destTables = await destination.GetInfos(config.Destination?.Filter);
                if (SimpleSchemaComparer(srcTables, destTables, config.CheckTypes))
                {
                    await Delete(destTables, destination);
                    await Copy(destTables, source, destination, config.PageSize);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }
            return true;
        }

        private IDbProvider GetProvider(string type, string schema, string connection)
        {
            switch (type?.ToLower())
            {
                case "sqlserver": return new SqlServerDbProvider(schema, connection);
                case "postgres": return new PostgresDbProvider(schema, connection);
                case "mysql": return new MySqlDbProvider(schema, connection);
            }
            _logger.Error($"Unknown db provider: {type?.ToLower()}");
            return null;
        }

        private bool SimpleSchemaComparer(List<TableInfo> source, List<TableInfo> destination, bool checkTypes)
        {
            var result = true;
            var (commonTables, onlySrcTables, onlyDestTables) =
                Diff(source, destination, (src, dest) => string.Equals(src.Name, dest.Name, StringComparison.InvariantCultureIgnoreCase));
            if (onlySrcTables.Count > 0 || onlyDestTables.Count > 0)
            {
                if (onlySrcTables.Count > 0)
                    _logger.Error("Source have unique tables: " + string.Join(", ", onlySrcTables));
                if (onlyDestTables.Count > 0)
                    _logger.Error("Destination have unique tables: " + string.Join(", ", onlyDestTables));
                result = false;
            }

            Func<ColumnInfo, ColumnInfo, bool> columnComparer;
            if (checkTypes)
                columnComparer = (src, dest) => string.Equals(src.Name, dest.Name, StringComparison.InvariantCultureIgnoreCase) &&
                    src.Type == dest.Type && src.IsNullable == dest.IsNullable;
            else
                columnComparer = (src, dest) => string.Equals(src.Name, dest.Name, StringComparison.InvariantCultureIgnoreCase);
            foreach (var common in commonTables)
            {
                var (_, onlySrcColumns, onlyDestColumns) =
                    Diff(common.src.Columns, common.dest.Columns, columnComparer);
                if (onlySrcColumns.Count > 0 || onlyDestColumns.Count > 0)
                {
                    if (onlySrcColumns.Count > 0)
                        _logger.Error("Source table " + common.src.Name + " have unique columns: " + string.Join(", ", onlySrcColumns));
                    if (onlyDestColumns.Count > 0)
                        _logger.Error("Destination table " + common.dest.Name + " have unique columns: " + string.Join(", ", onlyDestColumns));
                    result = false;
                }
            }
            return result;
        }

        private (List<(T src, T dest)> common, List<T> onlySrc, List<T> onlyDest) Diff<T>(List<T> src, List<T> dest, Func<T, T, bool> comparer)
        {
            var onlySrc = src.Where(s => dest.All(d => !comparer(s, d))).ToList();
            var onlyDest = dest.Where(d => src.All(s => !comparer(d, s))).ToList();
            var common = src.Select(s => (src: s, dest: dest.FirstOrDefault(d => comparer(s, d)))).Where(x => x.dest != null).ToList();
            return (common, onlySrc, onlyDest);
        }

        private async Task Delete(List<TableInfo> infos, IDbProvider destination)
        {
            infos = infos.ToList();
            var orderedList = new List<TableInfo>();
            while (infos.Count > 0)
            {
                var processed = orderedList.Select(x => x.Name).ToList();
                var info = infos.FirstOrDefault(x => x.References.Count(d => x.Name != d && !processed.Contains(d)) == 0);
                if (info == null)
                    throw new Exception($"Unable to resolve dependencies to delete. processed: {string.Join(", ", processed)}. remaining: {string.Join(", ", infos.Select(x => x.Name))}.");
                infos.Remove(info);
                orderedList.Add(info);
            }
            _logger.Trace($"ordered to delete: {string.Join(", ", orderedList.Select(x => x.Name))}");

            foreach (var info in orderedList)
            {
                await destination.Delete(info);
                _logger.Info($"Rows deleted from '{info.Name}'");
            }
        }

        private async Task Copy(List<TableInfo> infos, IDbProvider source, IDbProvider destination, int pageSize)
        {
            infos = infos.ToList();
            var orderedList = new List<TableInfo>();
            while (infos.Count > 0)
            {
                var processed = orderedList.Select(x => x.Name).ToList();
                var info = infos.FirstOrDefault(x => x.Dependencies.Count(d => x.Name != d && !processed.Contains(d)) == 0);
                if (info == null)
                    throw new Exception($"Unable to resolve dependencies to copy. processed: {string.Join(", ", processed)}. remaining: {string.Join(", ", infos.Select(x => x.Name))}.");
                infos.Remove(info);
                orderedList.Add(info);
            }
            _logger.Trace($"ordered to copy: {string.Join(", ", orderedList.Select(x => x.Name))}");
            var sw = new Stopwatch();
            foreach (var info in orderedList)
            {
                using (destination.BeginTransaction())
                {
                    sw.Restart();
                    if (info.Columns.Count > 0)
                    {
                        await destination.BeforeInsert(info);
                        int page = 0;
                        while (true)
                        {
                            page++;
                            var table = await source.Select(info, page, pageSize);
                            if (table.Rows.Count > 0)
                                await destination.Insert(info, table);
                            _logger.Info($"Copied {(page - 1) * pageSize + table.Rows.Count} rows to '{info.Name}' elapsed time {sw.ElapsedMilliseconds / 1000} sec");
                            if (table.Rows.Count != pageSize)
                                break;
                        }
                        await destination.AfterInsert(info);
                    }
                    sw.Stop();
                    destination.Commit();
                }
            }
        }
    }
}
