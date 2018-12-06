﻿namespace CopyDb.Models
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
        public int PageSize { get; set; } = 10000;
        public bool Force { get; set; }
        public bool Delete { get; set; } = true;
        public bool Copy { get; set; } = true;
        public bool CheckTypes { get; set; }
        public DbInfo Source { get; set; }
        public DbInfo Destination { get; set; }
    }
}
