using ERHMS.Common.Linq;
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string GlobalRecordId { get; set; }
        public string Instructions => ResXResources.Body_GetWorkerId;

        public IEnumerable<string> Parameters
        {
            get
            {
                yield return FirstName;
                yield return LastName;
                yield return EmailAddress;
                yield return GlobalRecordId;
            }
            set
            {
                IEnumerator<string> enumerator = value.GetEnumerator();
                FirstName = enumerator.GetNext();
                LastName = enumerator.GetNext();
                EmailAddress = enumerator.GetNext();
                GlobalRecordId = enumerator.GetNext();
            }
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
