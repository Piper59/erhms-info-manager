using ERHMS.Common.Linq;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.ViewModels.Utilities;
using ERHMS.Desktop.Views.Utilities;
using System;
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
        public string GlobalRecordId { get; set; }
        public string WorkerId => Output;

        public override IEnumerable<string> Parameters
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
                using (IEnumerator<string> enumerator = value.GetEnumerator())
                {
                    FirstName = enumerator.GetNext();
                    LastName = enumerator.GetNext();
                    EmailAddress = enumerator.GetNext();
                    GlobalRecordId = enumerator.GetNext();
                }
            }
        }

        protected override string Instructions => Strings.Body_GetWorkerId;

        public override async Task ExecuteAsync()
        {
            GetWorkerIdViewModel dataContext = new GetWorkerIdViewModel(FirstName, LastName, EmailAddress);
            await dataContext.InitializeAsync();
            dataContext.WorkerId = GlobalRecordId;
            Window window = new GetWorkerIdView
            {
                DataContext = dataContext,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            if (window.ShowDialog() == true)
            {
                Console.Out.Write(dataContext.WorkerId);
            }
        }
    }
}
