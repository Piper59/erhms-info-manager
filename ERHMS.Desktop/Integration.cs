using ERHMS.Common;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.ViewModels.Integration;
using ERHMS.Desktop.Views;
using ERHMS.Desktop.Views.Integration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
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

        private static void ConfigureServices()
        {
            ServiceLocator.Install<IDialogService>(() => new DialogService());
        }

        private static void Run(Action action, [CallerMemberName] string methodName = null)
        {
            App.ConfigureLog();
            ConfigureServices();
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
                    ResXResources.Lead_NonFatalException,
                    ex.Message,
                    ex.ToString(),
                    DialogButtonCollection.Close);
            }
            Log.Instance.Debug($"Executed integration command: {methodName}");
        }

        private static bool? ShowDialog(Window dialog)
        {
            new WindowInteropHelper(dialog)
            {
                Owner = Process.GetCurrentProcess().MainWindowHandle
            };
            return dialog.ShowDialog();
        }

        public static string GetWorkerId(string firstName, string lastName, string emailAddress)
        {
            string workerId = null;
            Run(() =>
            {
                Window dialog = new GetWorkerIdView
                {
                    DataContext = new GetWorkerIdViewModel()
                };
                if (ShowDialog(dialog) == true)
                {
                    // TODO
                }
            });
            return workerId;
        }
    }
}
