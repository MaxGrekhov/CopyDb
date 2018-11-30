using System.Collections.Generic;
using System.Diagnostics;

namespace CopyDb.Models
{
    [DebuggerDisplay("Table: {" + nameof(Name) + "}")]
    public class TableInfo
    {
        public string Name { get; set; }
        public List<ColumnInfo> Columns { get; set; }
        public List<string> Dependencies { get; set; }
        public List<string> References { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
