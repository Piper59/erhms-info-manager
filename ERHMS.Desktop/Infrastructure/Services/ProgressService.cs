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
        private ProgressViewModel dataContext;

        public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(1.0);
        public string Lead { get; set; } = Strings.Lead_Working;
        public bool CanBeCanceled { get; set; } = false;

        private async Task<TTask> RunCore<TTask>(Func<TTask> action)
            where TTask : Task
        {
            dataContext = new ProgressViewModel(Lead, CanBeCanceled);
            try
            {
                Window owner = Application.Current.GetActiveWindow();
                ProgressView window = new ProgressView
                {
                    DataContext = dataContext
                };
                window.SetOwner(owner);
                using (WindowDisabler.Begin(owner))
                {
                    CancellationTokenSource completionTokenSource = new CancellationTokenSource();
                    TTask task = action();
                    Task continuation = task.ContinueWith(
                        _ =>
                        {
                            completionTokenSource.Cancel();
                            dataContext.Done = true;
                            window.Close();
                        },
                        TaskScheduler.FromCurrentSynchronizationContext());
                    try
                    {
                        await Task.Delay(Delay, completionTokenSource.Token);
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
            finally
            {
                dataContext = null;
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

        public void ThrowIfCancellationRequested()
        {
            if (dataContext == null)
            {
                throw new InvalidOperationException("Task is not running.");
            }
            if (!CanBeCanceled)
            {
                throw new InvalidOperationException("Task cannot be canceled.");
            }
            dataContext.CancellationToken.ThrowIfCancellationRequested();
        }

        public void Report(string value)
        {
            if (dataContext == null)
            {
                throw new InvalidOperationException("Task is not running.");
            }
            dataContext.Status = value;
        }
    }
}
