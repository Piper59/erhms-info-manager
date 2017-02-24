namespace ERHMS.EpiInfo
{
    public enum RecStatus
    {
        Deleted = 0,
        Undeleted = 1
    }

    public static class RecStatusExtensions
    {
        public static int ToValue(this RecStatus @this)
        {
            return (int)@this;
        }

        public static RecStatus FromValue(int value)
        {
            return value == 0 ? RecStatus.Deleted : RecStatus.Undeleted;
        }
    }
}
