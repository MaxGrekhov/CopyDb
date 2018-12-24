using System.Collections.Generic;

namespace CopyDb.Models
{
    public class DbConfig
    {
        public string Type { get; set; }
        public string Schema { get; set; }
        public string Connection { get; set; }
        public string Filter { get; set; }
    }

    public class TableConfig
    {
        public string Name { get; set; }
        public List<ColumnConfig> Columns { get; set; }
    }

    public class ColumnConfig
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class AppConfig
    {
        public int PageSize { get; set; } = 10000;
        public bool Force { get; set; }
        public bool Delete { get; set; } = true;
        public bool Copy { get; set; } = true;
        public bool CheckTypes { get; set; }
        public List<TableConfig> Tables { get; set; }
        public DbConfig Source { get; set; }
        public DbConfig Destination { get; set; }
    }
}
