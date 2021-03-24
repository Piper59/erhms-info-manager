using ERHMS.Common;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using Action = System.Action;

namespace ERHMS.Desktop
{
    public static class Integration
    {
        private class DialogService : IDialogService
        {
            public bool? Show(
                DialogType dialogType,
                string lead,
                string body,
                string details,
                IReadOnlyCollection<DialogButton> buttons)
            {
                Window dialog = new DialogView
                {
                    DataContext = new DialogViewModel(dialogType, lead, body, details, buttons),
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                return dialog.ShowDialog();
            }
        }

        static Integration()
        {
            App.ConfigureLog();
            ConfigureServices();
        }

        private static void ConfigureServices()
        {
            ServiceLocator.Install<IDialogService>(() => new DialogService());
        }

        private static void Run(Action action, [CallerMemberName] string methodName = null)
        {
            Log.Instance.Debug($"Executing integration command: {methodName}");
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
                ServiceLocator.Resolve<IDialogService>().Show(
                    DialogType.Error,
                    ResXResources.Lead_CaughtNonFatalException,
                    ex.Message,
                    ex.ToString(),
                    DialogButtonCollection.Close);
            }
            Log.Instance.Debug($"Executed integration command: {methodName}");
        }

        public static string WorkerInfo_GetGlobalRecordId(
            string firstName,
            string lastName,
            string emailAddress,
            string globalRecordId)
        {
            Run(() =>
            {
                // TODO
            });
            return globalRecordId;
        }
    }
}
