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
    public class ProgressService : IProgressService
    {
        public Application Application { get; }
        public Dispatcher Dispatcher => Application.Dispatcher;
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
            await Dispatcher.InvokeAsync(() => DataContext.Status = value);
        }

        public async Task RunAsync(Action action)
        {
            Window owner = Application.MainWindow;
            ProgressView window = new ProgressView
            {
                DataContext = DataContext,
                Owner = owner
            };
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Task task = Task.Run(action);
                window.BeginShowDialog();
                await task;
            }
            finally
            {
                Mouse.OverrideCursor = null;
                window.EndShowDialog();
            }
        }
    }
}
