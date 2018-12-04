using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CopyDb.Common;
using CopyDb.Models;
using Dapper;

namespace CopyDb.Core
{
    public abstract class BaseDbProvider : IDbProvider
    {
        protected readonly string Schema;
        protected readonly IDbConnection Connection;
        protected string BeginEscape { get; set; }
        protected string EndEscape { get; set; }


        protected string ColumnsSql = @"
                select table_name, column_name, column_default, is_nullable, data_type
                from information_schema.columns
                where table_schema = @schema
                order by table_name, ordinal_position;";

        protected string ConstraintSql = @"
                select  
                     kcu1.constraint_name as fkconstraintname 
                    ,kcu1.table_name as fktablename 
                    ,kcu1.column_name as fkcolumnname 
                    ,kcu1.ordinal_position as fkordinalposition 
                    ,kcu2.constraint_name as referencedconstraintname 
                    ,kcu2.table_name as referencedtablename 
                    ,kcu2.column_name as referencedcolumnname 
                    ,kcu2.ordinal_position as referencedordinalposition 
                from information_schema.referential_constraints as rc 

                inner join information_schema.key_column_usage as kcu1 
                    on kcu1.constraint_catalog = rc.constraint_catalog  
                    and kcu1.constraint_schema = rc.constraint_schema 
                    and kcu1.constraint_name = rc.constraint_name 

                inner join information_schema.key_column_usage as kcu2 
                    on kcu2.constraint_catalog = rc.unique_constraint_catalog  
                    and kcu2.constraint_schema = rc.unique_constraint_schema 
                    and kcu2.constraint_name = rc.unique_constraint_name 
                    and kcu2.ordinal_position = kcu1.ordinal_position 
                where rc.constraint_schema = @schema
                order by fktablename, fkcolumnname, referencedtablename, referencedcolumnname";

        protected IDbTransaction Transaction;

        protected BaseDbProvider(string schema, IDbConnection connection)
        {
            Schema = schema;
            Connection = connection;
            Connection.Open();
        }

        public IDisposable BeginTransaction()
        {
            Transaction = Connection.BeginTransaction();

            return new Disposable(() =>
            {
                Transaction?.Dispose();
                Transaction = null;
            });
        }

        public void Commit()
        {
            Transaction.Commit();
        }

        public async Task<List<TableInfo>> GetInfos(string filter)
        {
            var columns = await Connection.QueryAsync<TableQueryModel>(ColumnsSql, new { schema = Schema }, Transaction);
            var infos = columns.GroupBy(x => x.TableName)
                .Select(x => new TableInfo
                {
                    Name = x.Key,
                    Columns = x.Select(c => new ColumnInfo
                    {
                        Name = c.ColumnName,
                        Type = c.DataType,
                        DefaultValue = c.ColumnDefault,
                        IsNullable = c.IsNullable?.ToLower() == "yes"
                    }).ToList()
                })
                .ToList();
            if (!string.IsNullOrEmpty(filter))
                infos = infos.Where(x => Regex.IsMatch(x.Name, filter)).ToList();
            var constraints = (await Connection.QueryAsync<ConstraintQueryModel>(ConstraintSql, new { schema = Schema })).AsList();
            foreach (var info in infos)
            {
                info.Dependencies = constraints
                    .Where(x => x.FkTableName == info.Name)
                    .Select(x => x.ReferencedTableName)
                    .ToList();
                info.References = constraints
                    .Where(x => x.ReferencedTableName == info.Name)
                    .Select(x => x.FkTableName)
                    .ToList();
            }

            return infos;
        }

        public virtual async Task<DataTable> Select(TableInfo info, int page, int count)
        {
            var sb = new StringBuilder();
            sb.Append("select ");
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
            sb.Append(" from ");
            sb.Append(BeginEscape);
            sb.Append(Schema);
            sb.Append(EndEscape);
            sb.Append(".");
            sb.Append(BeginEscape);
            sb.Append(info.Name);
            sb.Append(EndEscape);
            sb.Append(" ");
            sb.Append(GetPaginationSql(info, page, count));
            var reader = await Connection.ExecuteReaderAsync(sb.ToString(),
                new { offset = (page - 1) * count, limit = count }, Transaction);
            var table = new DataTable();
            table.Load(reader);
            return table;
        }

        protected abstract string GetPaginationSql(TableInfo info, int page, int count);

        public virtual Task BeforeInsert(TableInfo info)
        {
            return Task.CompletedTask;
        }

        public abstract Task Insert(TableInfo info, DataTable table);

        public virtual Task AfterInsert(TableInfo info)
        {
            return Task.CompletedTask;
        }

        public virtual async Task Delete(TableInfo info)
        {
            var sql = $@"delete from {BeginEscape}{Schema}{EndEscape}.{BeginEscape}{info.Name}{EndEscape} where 1=1;";
            await Connection.ExecuteAsync(sql, null, Transaction);
        }

        public void Dispose()
        {
            Transaction?.Dispose();
            Connection?.Dispose();
        }
    }
}
