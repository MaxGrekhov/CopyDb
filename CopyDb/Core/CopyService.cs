using System;
using System.Collections.Generic;
using System.Data;
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

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            Dapper.SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime2);
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
                var diff = SimpleSchemaComparer(srcTables, destTables, config.Tables, config.CheckTypes);
                if (diff.isSame || config.Force)
                {
                    if (config.Delete)
                        await Delete(diff.common.Select(x => x.dest).ToList(), destination);
                    if (config.Copy)
                        await Copy(diff.common, config.Tables, source, destination, config.PageSize);
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

        private (bool isSame, List<(TableInfo src, TableInfo dest)> common) SimpleSchemaComparer(List<TableInfo> source, List<TableInfo> destination, List<TableConfig> tablesConfig, bool checkTypes)
        {
            var result = (isSame: true, common: new List<(TableInfo src, TableInfo dest)>());

            var (commonTables, onlySrcTables, onlyDestTables) =
                Diff(source, destination, (src, dest) => StringEquals(src.Name, dest.Name));

            if (onlySrcTables.Count > 0 || onlyDestTables.Count > 0)
            {
                if (onlySrcTables.Count > 0)
                    _logger.Error("Source have unique tables: " + string.Join(", ", onlySrcTables));
                if (onlyDestTables.Count > 0)
                    _logger.Error("Destination have unique tables: " + string.Join(", ", onlyDestTables));
                result.isSame = false;
            }

            bool ColumnComparer(ColumnInfo src, ColumnInfo dest) => StringEquals(src.Name, dest.Name)
                && (!checkTypes || src.Type == dest.Type && src.IsNullable == dest.IsNullable);

            foreach (var common in commonTables)
            {
                var columns = Diff(common.src.Columns, common.dest.Columns, ColumnComparer);

                var tableConfig = tablesConfig?.FirstOrDefault(x => StringEquals(x.Name, common.dest.Name));

                var (commonColumns, onlySrcColumns, onlyDestColumns) = ColumnsCorrection(columns, tableConfig);

                var destTableInfo = new TableInfo
                {
                    Name = common.dest.Name,
                    Columns = commonColumns.Select(x => x.dest).ToList(),
                    Dependencies = common.dest.Dependencies,
                    References = common.dest.References
                };

                var srcTableInfo = new TableInfo
                {
                    Name = common.src.Name,
                    Columns = commonColumns.Select(x => x.src).Where(x => x != null).ToList(),
                    Dependencies = common.src.Dependencies,
                    References = common.src.References
                };

                result.common.Add((srcTableInfo, destTableInfo));

                if (onlySrcColumns.Count > 0 || onlyDestColumns.Count > 0)
                {
                    if (onlySrcColumns.Count > 0)
                        _logger.Error("Source table " + common.src.Name + " have unique columns: " + string.Join(", ", onlySrcColumns));
                    if (onlyDestColumns.Count > 0)
                        _logger.Error("Destination table " + common.dest.Name + " have unique columns: " + string.Join(", ", onlyDestColumns));
                    result.isSame = false;
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

        private (List<(ColumnInfo src, ColumnInfo dest)> common, List<ColumnInfo> onlySrc, List<ColumnInfo> onlyDest) ColumnsCorrection((List<(ColumnInfo src, ColumnInfo dest)> common, List<ColumnInfo> onlySrc, List<ColumnInfo> onlyDest) columns, TableConfig tableConfig)
        {
            if (tableConfig != null)
            {
                var isHere = columns.onlyDest.Where(dest => tableConfig.Columns.Any(x => StringEquals(x.Name, dest.Name))).ToList();

                isHere.ForEach(x => columns.onlyDest.Remove(x));

                foreach (var columnInfo in isHere)
                    columns.common.Add((null, columnInfo));
            }
            return columns;
        }

        private async Task Delete(List<TableInfo> infos, IDbProvider destination)
        {
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

        private async Task Copy(List<(TableInfo src, TableInfo dest)> infos, List<TableConfig> tablesConfig, IDbProvider source, IDbProvider destination, int pageSize)
        {
            var orderedList = new List<(TableInfo src, TableInfo dest)>();
            while (infos.Count > 0)
            {
                var processed = orderedList.Select(x => x.dest.Name).ToList();
                var info = infos.FirstOrDefault(x => x.dest.Dependencies.Count(d => x.dest.Name != d && !processed.Contains(d)) == 0);
                if (info.dest == null)
                    throw new Exception($"Unable to resolve dependencies to copy. processed: {string.Join(", ", processed)}. remaining: {string.Join(", ", infos.Select(x => x.dest.Name))}.");
                infos.Remove(info);
                orderedList.Add(info);
            }
            _logger.Trace($"ordered to copy: {string.Join(", ", orderedList.Select(x => x.dest.Name))}");
            var sw = new Stopwatch();
            foreach (var info in orderedList)
            {
                var tableConfig = tablesConfig?.FirstOrDefault(x => StringEquals(x.Name, info.dest.Name));

                using (destination.BeginTransaction())
                {
                    sw.Restart();
                    if (info.dest.Columns.Count > 0)
                    {
                        await destination.BeforeInsert(info.dest);
                        int page = 0;
                        while (true)
                        {
                            page++;
                            var table = await source.Select(info.src, page, pageSize);
                            if (table.Rows.Count > 0)
                            {
                                DataTableCorrection(table, tableConfig);
                                await destination.Insert(info.dest, table);
                            }
                            _logger.Info($"Copied {(page - 1) * pageSize + table.Rows.Count} rows to '{info.dest.Name}' elapsed time {sw.ElapsedMilliseconds / 1000} sec");
                            if (table.Rows.Count != pageSize)
                                break;
                        }
                        await destination.AfterInsert(info.dest);
                    }
                    destination.Commit();
                    sw.Stop();
                }
            }
        }

        private void DataTableCorrection(DataTable table, TableConfig tableConfig)
        {
            if (tableConfig?.Columns?.Count > 0)
            {
                foreach (var column in tableConfig.Columns)
                {
                    var dataColumn = table.Columns.Cast<DataColumn>()
                                         .FirstOrDefault(x => StringEquals(x.ColumnName, column.Name))
                                     ?? table.Columns.Add(column.Name);
                    foreach (DataRow row in table.Rows)
                        row[dataColumn.ColumnName] = column.Value;
                }
            }
        }

        private bool StringEquals(string first, string second)
        {
            return string.Equals(first, second, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
