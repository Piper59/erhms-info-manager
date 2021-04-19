using ERHMS.Common;
using ERHMS.Desktop.Infrastructure.Services;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Integration;
using ERHMS.Desktop.Views.Integration;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Action = System.Action;

namespace ERHMS.Desktop
{
    public static class Integration
    {
        static Integration()
        {
            App.ConfigureLog();
            ConfigureServices();
        }

        private static void ConfigureServices()
        {
            ServiceLocator.Install<IDialogService>(() => new DialogService());
            ServiceLocator.Install<IProgressService>(() => new ProgressService());
            ServiceLocator.Install<IWindowingService>(() => new WinFormsWindowingService());
        }

        private static void RunSynchronously(Task task)
        {
            DispatcherFrame frame = new DispatcherFrame();
            task.ContinueWith(_ => frame.Continue = false);
            Dispatcher.PushFrame(frame);
            task.Wait();
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
                App.OnNonFatalException(ex);
            }
            Log.Instance.Debug($"Executed integration command: {methodName}");
        }

        public static string GetWorkerId(string firstName, string lastName, string emailAddress, string globalRecordId)
        {
            string workerId = null;
            Run(() =>
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                progress.Title = ResXResources.Lead_LoadingWorkers;
                GetWorkerIdViewModel dataContext = null;
                Task task = progress.RunAsync(async () =>
                {
                    dataContext = await GetWorkerIdViewModel.CreateAsync(
                        firstName,
                        lastName,
                        emailAddress,
                        globalRecordId);
                });
                RunSynchronously(task);
                Window window = new GetWorkerIdView
                {
                    DataContext = dataContext
                };
                IWindowingService windowing = ServiceLocator.Resolve<IWindowingService>();
                if (windowing.ShowDialog(window) == true)
                {
                    workerId = dataContext.Workers.Items.SelectedItem.Value.GlobalRecordId;
                }
            });
            return workerId;
        }
    }
}
