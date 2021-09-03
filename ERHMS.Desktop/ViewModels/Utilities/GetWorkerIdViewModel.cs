using ERHMS.Desktop.ViewModels.Collections;
using ERHMS.Domain.Data;
using ERHMS.EpiInfo.Data;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels.Utilities
{
    public class GetWorkerIdViewModel
    {
        public static async Task<GetWorkerIdViewModel> CreateAsync(
            string firstName,
            string lastName,
            string emailAddress,
            string workerId)
        {
            GetWorkerIdViewModel result = new GetWorkerIdViewModel(firstName, lastName, emailAddress, workerId);
            await result.InitializeAsync();
            return result;
        }

        public string FirstName { get; }
        public string LastName { get; }
        public string EmailAddress { get; }
        public string WorkerId { get; private set; }
        public WorkerCollectionViewModel Workers { get; private set; }

        private GetWorkerIdViewModel(string firstName, string lastName, string emailAddress, string workerId)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            WorkerId = workerId;
        }

        private async Task InitializeAsync()
        {
            Workers = await WorkerCollectionViewModel.CreateAsync(FirstName, LastName, EmailAddress);
            Workers.Items.MoveCurrentTo(
                worker => Record.GlobalRecordIdComparer.Equals(worker.GlobalRecordId, WorkerId));
            Workers.Committed += Workers_Committed;
        }

        private void Workers_Committed(object sender, RecordEventArgs<Worker> e)
        {
            WorkerId = e.Record.GlobalRecordId;
        }
    }
}
