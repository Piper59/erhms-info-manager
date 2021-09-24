using ERHMS.Common.Linq;
using ERHMS.Common.Logging;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
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
            GetWorkerIdView window = new GetWorkerIdView
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            try
            {
                window.DataContext =
                    await GetWorkerIdViewModel.CreateAsync(FirstName, LastName, EmailAddress, WorkerId);
            }
            catch (Exception ex)
            {
                Log.Instance.Warn(ex);
                IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
                dialog.Severity = DialogSeverity.Warning;
                dialog.Lead = Strings.GetWorkerId_Lead_Error;
                dialog.Body = Strings.GetWorkerId_Body_Error;
                dialog.Details = ex.ToString();
                dialog.Buttons = DialogButtonCollection.Close;
                dialog.Show();
                return null;
            }
            window.ShowDialog();
            return window.DataContext.WorkerId;
        }
    }
}
