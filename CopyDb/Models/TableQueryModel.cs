namespace CopyDb.Models
{
    public class TableQueryModel
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ColumnDefault { get; set; }
        public string IsNullable { get; set; }
        public string DataType { get; set; }
    }
}