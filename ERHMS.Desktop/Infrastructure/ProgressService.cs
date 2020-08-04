using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ERHMS.Desktop.Infrastructure
{
    internal class ProgressService : IProgressService
    {
        private static readonly TimeSpan ShowDialogDelay = TimeSpan.FromSeconds(1.0);

        public Application Application { get; }
        private Dispatcher Dispatcher => Application.Dispatcher;
        public ProgressViewModel DataContext { get; }

        public ProgressService(Application application, string taskName)
        {
            Application = application;
            DataContext = new ProgressViewModel
            {
                TaskName = taskName
            };
        }

        public async void Report(string value)
        {
            await Dispatcher.InvokeAsync(() => DataContext.Progress = value);
        }

        public async Task RunAsync(Action action)
        {
            Window owner = Application.MainWindow;
            Window window = new ProgressView
            {
                DataContext = DataContext,
                Owner = owner
            };
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Task task = Task.Run(action);
                DispatcherOperation operation = null;
                try
                {
                    await Task.Delay(ShowDialogDelay);
                    operation = Dispatcher.InvokeAsync(() =>
                    {
                        if (!DataContext.Complete)
                        {
                            window.ShowDialog();
                        }
                    });
                    await task;
                }
                finally
                {
                    DataContext.Complete = true;
                    await operation;
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }
}
