using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CopyDb.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Dapper;
using MySql.Data.MySqlClient;

namespace CopyDb.Core
{
    public class MySqlDbProvider : BaseDbProvider
    {
        public MySqlDbProvider(string schema, string connectionString) : base(schema, new MySqlConnection(connectionString))
        {
            BeginEscape = "`";
            EndEscape = "`";
            ConstraintSql = @"
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
                    and kcu1.table_name = rc.table_name

                inner join information_schema.key_column_usage as kcu2 
                    on kcu2.constraint_catalog = rc.unique_constraint_catalog  
                    and kcu2.constraint_schema = rc.unique_constraint_schema 
                    and kcu2.constraint_name = rc.unique_constraint_name 
                    and kcu2.ordinal_position = kcu1.ordinal_position 
                    and kcu2.table_name = rc.referenced_table_name
                where rc.constraint_schema = @schema
                order by fktablename, fkcolumnname, referencedtablename, referencedcolumnname";
        }

        protected override string GetPaginationSql(TableInfo info, int page, int count)
        {
            return $"order by {BeginEscape}{info.Columns[0].Name}{EndEscape} limit @offset, @limit";
        }

        public override async Task Insert(TableInfo info, DataTable table)
        {
            var filepath = "./temp.csv";
            using (var file = File.CreateText(filepath))
            using (var csv = new CsvWriter(file, new Configuration { Delimiter = ";" }))
            {
                foreach (var column in info.Columns)
                    csv.WriteField(column.Name);
                csv.NextRecord();
                foreach (DataRow row in table.Rows)
                {
                    foreach (var column in info.Columns)
                    {
                        var value = row[column.Name];
                        switch (value)
                        {
                            case DBNull _:
                            case null:
                                csv.WriteConvertedField(@"\N");
                                break;
                            case DateTime dateTime:
                                csv.WriteConvertedField(dateTime.ToString("s", CultureInfo.InvariantCulture));
                                break;
                            case bool boolean:
                                csv.WriteConvertedField(boolean ? "1" : "0");
                                break;
                            default:
                                csv.WriteField(string.Format(CultureInfo.InvariantCulture, "{0}", value));
                                break;
                        }
                    }
                    csv.NextRecord();
                }
            }
            var fileInfo = new FileInfo(filepath);
            var loader = new MySqlBulkLoader(Connection as MySqlConnection)
            {
                TableName = $"{BeginEscape}{Schema}{EndEscape}.{BeginEscape}{info.Name}{EndEscape}",
                FieldTerminator = ";",
                EscapeCharacter = '\\',
                FieldQuotationCharacter = '"',
                FieldQuotationOptional = true,
                LineTerminator = Environment.NewLine,
                FileName = fileInfo.FullName,
                NumberOfLinesToSkip = 1
            };
            await loader.LoadAsync();
            //   fileInfo.Delete();
        }
    }
}
