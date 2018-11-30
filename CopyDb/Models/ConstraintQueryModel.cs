namespace CopyDb.Models
{
    public class ConstraintQueryModel
    {
        public string FkConstraintName { get; set; }
        public string FkTableName { get; set; }
        public string FkColumnName { get; set; }
        public string FkOrdinalPosition { get; set; }
        public string ReferencedConstraintName { get; set; }
        public string ReferencedTableName { get; set; }
        public string ReferencedColumnName { get; set; }
        public string ReferencedOrdinalPosition { get; set; }
    }
}