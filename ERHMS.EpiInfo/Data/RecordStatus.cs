namespace ERHMS.EpiInfo.Data
{
    public static class RecordStatus
    {
        public static short FromDeleted(bool deleted)
        {
            return deleted ? Deleted : Undeleted;
        }

        public static bool ToDeleted(short? recordStatus)
        {
            return recordStatus == Deleted;
        }

        public const short Deleted = 0;
        public const short Undeleted = 1;
    }
}
