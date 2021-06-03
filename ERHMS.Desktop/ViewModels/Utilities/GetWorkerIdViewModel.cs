using ERHMS.Desktop.ViewModels.Collections;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels.Utilities
{
    public class GetWorkerIdViewModel
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string EmailAddress { get; }
        public WorkerCollectionViewModel Workers { get; private set; }

        public string WorkerId
        {
            get { return Workers.CurrentValue?.GlobalRecordId; }
            set { Workers.MoveCurrentToGlobalRecordId(value); }
        }

        public GetWorkerIdViewModel(string firstName, string lastName, string emailAddress)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            Workers = new WorkerCollectionViewModel();
        }

        public async Task InitializeAsync()
        {
            await Workers.InitializeAsync(FirstName, LastName, EmailAddress);
        }
    }
}
