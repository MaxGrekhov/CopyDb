using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CopyDb.Models;
using Dapper;

namespace CopyDb.Core
{
    public class SqlServerDbProvider : BaseDbProvider
    {
        public SqlServerDbProvider(string schema, string connectionString) : base(schema, new SqlConnection(connectionString))
        {
            BeginEscape = "[";
            EndEscape = "]";
        }

        protected override string GetPaginationSql(TableInfo info, int page, int count)
        {
            return $"order by {BeginEscape}{info.Columns[0].Name}{EndEscape} offset @offset rows fetch next @limit rows only";
        }

        public override async Task Insert(TableInfo info, DataTable table)
        {
            using (var copy = new SqlBulkCopy(Connection as SqlConnection, SqlBulkCopyOptions.KeepIdentity, Transaction as SqlTransaction))
            {
                foreach (var column in info.Columns)
                    copy.ColumnMappings.Add(column.Name, column.Name);
                copy.DestinationTableName = $"{BeginEscape}{Schema}{EndEscape}.{BeginEscape}{info.Name}{EndEscape}";
                await copy.WriteToServerAsync(table);
            }
        }
    }
}
