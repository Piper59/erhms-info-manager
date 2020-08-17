namespace ERHMS.EpiInfo
{
    public enum InvalidViewNameReason
    {
        None,
        Empty,
        InvalidChar,
        InvalidBeginning,
        TooLong,
        Exists,
        IsConflict
    }
}
