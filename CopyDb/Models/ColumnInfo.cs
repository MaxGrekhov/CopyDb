using System.Diagnostics;

namespace CopyDb.Models
{
    [DebuggerDisplay("Column: {" + nameof(Name) + "}")]
    public class ColumnInfo
    {
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public string Type { get; set; }
        public bool IsNullable { get; set; }

        public override string ToString()
        {
            return $"({Name} {Type} {(IsNullable ? "NULL" : "NOT NULL")} {(string.IsNullOrEmpty(DefaultValue) ? "" : "DEFAULT" + DefaultValue)})";
        }
    }
}