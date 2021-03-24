namespace ERHMS.EpiInfo.Data
{
    public enum RecordStatus : short
    {
        Deleted = 0,
        Undeleted = 1
    }

    public static class RecordStatusExtensions
    {
        public static bool IsEquivalent(this RecordStatus @this, RecordStatus value)
        {
            return @this == RecordStatus.Deleted ? value == RecordStatus.Deleted : value != RecordStatus.Deleted;
        }

        public static bool IsEquivalent(this RecordStatus @this, bool deleted)
        {
            return @this == RecordStatus.Deleted ? deleted : !deleted;
        }
    }
}
