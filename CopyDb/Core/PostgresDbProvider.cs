using System.Threading.Tasks;
using CopyDb.Models;
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
            return "limit @limit offset @offset";
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
