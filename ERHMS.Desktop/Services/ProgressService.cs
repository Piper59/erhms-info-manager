using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ERHMS.Desktop.Services
{
    internal class ProgressService : IProgressService
    {
        private readonly Application application;
        private readonly ProgressViewModel dataContext;

        public string Title
        {
            get { return dataContext.Title; }
            set { dataContext.Title = value; }
        }

        public bool CanUserCancel
        {
            get { return dataContext.CanUserCancel; }
            set { dataContext.CanUserCancel = value; }
        }

        public bool IsUserCancellationRequested => dataContext.IsUserCancellationRequested;

        public ProgressService(Application application)
        {
            this.application = application;
            dataContext = new ProgressViewModel();
        }

        public async void Report(string value)
        {
            await application.Dispatcher.InvokeAsync(() => dataContext.Status = value);
        }

        public async Task RunAsync(Action action, CancellationToken token)
        {
            Window owner = application.GetActiveWindow();
            ProgressView window = new ProgressView
            {
                Owner = owner,
                DataContext = dataContext
            };
            if (owner != null)
            {
                owner.IsEnabled = false;
            }
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                Task task = Task.Run(action, token);
                window.BeginShowDialog();
                await task;
            }
            finally
            {
                Mouse.OverrideCursor = null;
                if (owner != null)
                {
                    owner.IsEnabled = true;
                }
                window.EndShowDialog();
            }
        }

        public Task RunAsync(Action action)
        {
            return RunAsync(action, CancellationToken.None);
        }
    }
}
