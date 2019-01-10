using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CopyDb.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Dapper;
using Npgsql;

namespace CopyDb.Core
{
    public class PostgresDbProvider : BaseDbProvider
    {
        public PostgresDbProvider(string schema, string connectionString) : base(schema, new NpgsqlConnection(connectionString))
        {
            BeginEscape = "\"";
            EndEscape = "\"";
        }

        protected override string GetPaginationSql(TableInfo info, int page, int count)
        {
            return $"order by {BeginEscape}{info.Columns[0].Name}{EndEscape} limit @limit offset @offset";
        }

        public override Task Insert(TableInfo info, DataTable table)
        {
            var sb = new StringBuilder();
            sb.Append("COPY ");
            sb.Append($@"{BeginEscape}{Schema}{EndEscape}.{BeginEscape}{info.Name}{EndEscape}");
            sb.Append(" (");
            var first = true;
            foreach (var column in info.Columns)
            {
                if (first)
                    first = false;
                else
                    sb.Append(",");
                sb.Append(BeginEscape);
                sb.Append(column.Name);
                sb.Append(EndEscape);
            }
            sb.Append(@") FROM STDIN DELIMITER ';' QUOTE '""' ESCAPE '\' NULL '@NULL' csv");
            using (var writer = ((NpgsqlConnection)Connection).BeginTextImport(sb.ToString()))
            using (var csv = new CsvWriter(writer, new Configuration { Delimiter = ";", Escape = '\\' }))
            {
                foreach (DataRow row in table.Rows)
                {
                    foreach (var column in info.Columns)
                    {
                        var value = row[column.Name];
                        switch (value)
                        {
                            case DBNull _:
                            case null:
                                csv.WriteConvertedField("@NULL");
                                break;
                            case DateTime dateTime:
                                csv.WriteConvertedField(dateTime.ToString("s", CultureInfo.InvariantCulture));
                                break;
                            default:
                                csv.WriteField(string.Format(CultureInfo.InvariantCulture, "{0}", value));
                                break;
                        }
                    }
                    csv.NextRecord();
                }
            }
            return Task.CompletedTask;
        }

        public override async Task AfterInsert(TableInfo info)
        {
            foreach (var column in info.Columns)
            {
                if (column.DefaultValue?.StartsWith("nextval") == true)
                {
                    var sql = $@"select setval(pg_get_serial_sequence('{BeginEscape}{Schema}{EndEscape}.{BeginEscape}{info.Name}{EndEscape}', '{column.Name}'),
                        coalesce((select max({BeginEscape}{column.Name}{EndEscape}) from {BeginEscape}{Schema}{EndEscape}.{BeginEscape}{info.Name}{EndEscape}), 1));";
                    await Connection.ExecuteAsync(sql, null, Transaction);
                }
            }
        }
    }
}
