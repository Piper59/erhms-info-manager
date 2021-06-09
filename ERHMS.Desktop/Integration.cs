using ERHMS.Desktop.Utilities;

namespace ERHMS.Desktop
{
    public static class Integration
    {
        public static string GetWorkerId(string firstName, string lastName, string emailAddress, string globalRecordId)
        {
            GetWorkerId utility = new GetWorkerId
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                GlobalRecordId = globalRecordId
            };
            utility.Invoke();
            return utility.WorkerId;
        }
    }
}
