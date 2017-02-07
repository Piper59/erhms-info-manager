namespace ERHMS.EpiInfo
{
    public enum InvalidViewNameReason
    {
        None,
        Empty,
        InvalidChar,
        InvalidFirstChar,
        TooLong,
        ViewExists,
        TableExists
    }
}
