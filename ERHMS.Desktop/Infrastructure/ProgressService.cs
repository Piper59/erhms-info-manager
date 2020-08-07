using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ERHMS.Desktop.Infrastructure
{
    public class ProgressService : IProgressService
    {
        private readonly Application application;
        private readonly ProgressViewModel dataContext;

        public bool IsUserCancellationRequested => dataContext.IsUserCancellationRequested;

        public ProgressService(Application application, string taskName, bool canUserCancel)
        {
            this.application = application;
            dataContext = new ProgressViewModel(taskName, canUserCancel);
        }

        public async void Report(string value)
        {
            await application.Dispatcher.InvokeAsync(() => dataContext.Status = value);
        }

        public async Task RunAsync(Action action, CancellationToken token)
        {
            Window owner = application.MainWindow;
            ProgressView window = new ProgressView
            {
                Owner = owner,
                DataContext = dataContext
            };
            // TODO: Disable owner?
            // Do something less heavy-handed?
            Mouse.OverrideCursor = Cursors.AppStarting;
            try
            {
                Task task = Task.Run(action, token);
                window.BeginShowDialog();
                await task;
            }
            finally
            {
                Mouse.OverrideCursor = null;
                window.EndShowDialog();
            }
        }

        public Task RunAsync(Action action) => RunAsync(action, CancellationToken.None);
    }
}
