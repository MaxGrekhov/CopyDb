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


        private bool _hasIdentity;

        public override async Task BeforeInsert(TableInfo info)
        {
            var hasIdentitySql =
                $@"select case when exists (select 1 from SYS.IDENTITY_COLUMNS where OBJECT_NAME(OBJECT_ID) = '{info.Name}') then 1 else 0 end";
            _hasIdentity = await Connection.ExecuteScalarAsync<bool>(hasIdentitySql, null, Transaction);
            if (_hasIdentity)
            {
                var sql = $"SET IDENTITY_INSERT {BeginEscape}{Schema}{EndEscape}.{BeginEscape}{info.Name}{EndEscape} ON";
                await Connection.ExecuteAsync(sql, null, Transaction);
            }
        }

        public override async Task AfterInsert(TableInfo info)
        {
            if (_hasIdentity)
            {
                var sql = $"SET IDENTITY_INSERT {BeginEscape}{Schema}{EndEscape}.{BeginEscape}{info.Name}{EndEscape} OFF";
                await Connection.ExecuteAsync(sql, null, Transaction);
            }
        }
    }
}
