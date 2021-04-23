using ERHMS.Desktop.Properties;
using ERHMS.Desktop.ViewModels.Utilities;
using ERHMS.Desktop.Views.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace ERHMS.Desktop.Utilities
{
    public class GetWorkerId : IUtility
    {
        public string Instructions => ResXResources.Instructions_GetWorkerId;

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string GlobalRecordId { get; set; }

        public IEnumerable<string> GetParameters()
        {
            yield return FirstName;
            yield return LastName;
            yield return EmailAddress;
            yield return GlobalRecordId;
        }

        public void ParseParameters(IReadOnlyList<string> parameters)
        {
            FirstName = parameters[0];
            LastName = parameters[1];
            EmailAddress = parameters[2];
            GlobalRecordId = parameters[3];
        }

        public async Task<string> ExecuteAsync()
        {
            GetWorkerIdViewModel dataContext =
                await GetWorkerIdViewModel.CreateAsync(FirstName, LastName, EmailAddress);
            dataContext.WorkerId = GlobalRecordId;
            Window window = new GetWorkerIdView
            {
                DataContext = dataContext,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            return window.ShowDialog() == true ? dataContext.WorkerId : null;
        }
    }
}
