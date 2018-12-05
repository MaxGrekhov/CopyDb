namespace CopyDb.Models
{
    public class DbInfo
    {
        public string Type { get; set; }
        public string Schema { get; set; }
        public string Connection { get; set; }
        public string Filter { get; set; }
    }

    public class AppConfig
    {
        public int PageSize { get; set; } = 1000;
        public bool Force { get; set; }
        public bool CheckTypes { get; set; }
        public DbInfo Source { get; set; }
        public DbInfo Destination { get; set; }
    }
}
