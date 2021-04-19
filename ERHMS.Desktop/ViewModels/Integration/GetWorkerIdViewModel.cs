using ERHMS.Desktop.ViewModels.Collections;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels.Integration
{
    public class GetWorkerIdViewModel
    {
        public static async Task<GetWorkerIdViewModel> CreateAsync(
            string firstName,
            string lastName,
            string emailAddress,
            string globalRecordId)
        {
            GetWorkerIdViewModel result = new GetWorkerIdViewModel(firstName, lastName, emailAddress, globalRecordId);
            await result.InitializeAsync();
            return result;
        }

        public string FirstName { get; }
        public string LastName { get; }
        public string EmailAddress { get; }
        public string GlobalRecordId { get; }
        public WorkerCollectionViewModel Workers { get; private set; }

        private GetWorkerIdViewModel(string firstName, string lastName, string emailAddress, string globalRecordId)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            GlobalRecordId = globalRecordId;
        }

        private async Task InitializeAsync()
        {
            Workers = await WorkerCollectionViewModel.CreateAsync(FirstName, LastName, EmailAddress, GlobalRecordId);
        }
    }
}
