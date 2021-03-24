using System;

namespace ERHMS.Desktop
{
    public static class Integration
    {
        public static string WorkerInfo_GetGlobalRecordId(
            string firstName,
            string lastName,
            string emailAddress,
            string globalRecordId)
        {
            return Guid.Empty.ToString();
        }
    }
}
