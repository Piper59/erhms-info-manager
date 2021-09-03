using ERHMS.Common.Linq;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.ViewModels.Utilities;
using ERHMS.Desktop.Views.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace ERHMS.Desktop.Utilities
{
    public class GetWorkerId : Utility.Graphical
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string WorkerId { get; set; }

        public override IEnumerable<string> Parameters
        {
            get
            {
                yield return FirstName;
                yield return LastName;
                yield return EmailAddress;
                yield return WorkerId;
            }
            set
            {
                using (IEnumerator<string> enumerator = value.GetEnumerator())
                {
                    FirstName = enumerator.GetNext();
                    LastName = enumerator.GetNext();
                    EmailAddress = enumerator.GetNext();
                    WorkerId = enumerator.GetNext();
                }
            }
        }

        protected override string Body => Strings.Body_GetWorkerId;

        public override async Task<string> ExecuteAsync()
        {
            GetWorkerIdViewModel dataContext =
                await GetWorkerIdViewModel.CreateAsync(FirstName, LastName, EmailAddress, WorkerId);
            Window window = new GetWorkerIdView
            {
                DataContext = dataContext,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
            return dataContext.WorkerId;
        }
    }
}
