using ERHMS.Desktop.Utilities;

namespace ERHMS.Desktop
{
    public static class Integration
    {
        public static string GetWorkerId(string firstName, string lastName, string emailAddress, string globalRecordId)
        {
            return Utility.Invoke(new GetWorkerId
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                GlobalRecordId = globalRecordId
            });
        }
    }
}
