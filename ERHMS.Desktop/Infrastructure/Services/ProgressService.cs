using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class ProgressService : IProgressService
    {
        private readonly ProgressViewModel dataContext;

        public string Lead
        {
            get { return dataContext.Lead; }
            set { dataContext.Lead = value; }
        }

        public bool CanBeCanceled
        {
            get { return dataContext.CanBeCanceled; }
            set { dataContext.CanBeCanceled = value; }
        }

        public CancellationToken CancellationToken => dataContext.CancellationToken;
        public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(1.0);

        public ProgressService()
        {
            dataContext = new ProgressViewModel
            {
                Lead = Strings.Lead_Working
            };
        }

        public void Report(string value)
        {
            dataContext.Body = value;
        }

        private async Task<TTask> RunCore<TTask>(Func<TTask> action)
            where TTask : Task
        {
            Window owner = Application.Current.GetActiveWindow();
            using (WindowDisabler.Begin(owner))
            {
                ProgressView window = new ProgressView
                {
                    DataContext = dataContext
                };
                window.SetOwner(owner);
                TTask task = action();
                CancellationTokenSource delaying = new CancellationTokenSource();
                Task continuation = task.ContinueWith(
                    _ =>
                    {
                        delaying.Cancel();
                        window.CanClose = true;
                        window.Close();
                    },
                    TaskScheduler.FromCurrentSynchronizationContext());
                try
                {
                    await Task.Delay(Delay, delaying.Token);
                }
                catch (TaskCanceledException) { }
                if (!task.IsCompleted)
                {
                    window.ShowDialog();
                }
                await task;
                await continuation;
                return task;
            }
        }

        public Task Run(Func<Task> action)
        {
            return RunCore(action).Unwrap();
        }

        public Task<TResult> Run<TResult>(Func<Task<TResult>> action)
        {
            return RunCore(action).Unwrap();
        }
    }
}
