namespace ERHMS.EpiInfo
{
    public static class RecordStatus
    {
        public const short Deleted = 0;
        public const short Undeleted = 1;

        public static bool IsDeleted(int value)
        {
            return value == Deleted;
        }
    }
}
