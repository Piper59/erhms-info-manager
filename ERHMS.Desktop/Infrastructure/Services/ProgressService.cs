using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class ProgressService : IProgressService
    {
        private readonly IWindowingService windowing;
        private string status;

        public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(1.0);
        public string Title { get; set; }

        public ProgressService()
        {
            windowing = ServiceLocator.Resolve<IWindowingService>();
        }

        private event EventHandler StatusUpdated;
        private void OnStatusUpdated(EventArgs e) => StatusUpdated?.Invoke(this, e);
        private void OnStatusUpdated() => OnStatusUpdated(EventArgs.Empty);

        public void Report(string value)
        {
            status = value;
            OnStatusUpdated();
        }

        private async Task RunCoreAsync(bool canBeCanceled, Func<CancellationToken, Task> action)
        {
            using (ProgressViewModel dataContext = new ProgressViewModel(Title, canBeCanceled))
            {
                void ProgressService_StatusUpdated(object sender, EventArgs e)
                {
                    dataContext.Status = status;
                }

                dataContext.Status = status;
                StatusUpdated += ProgressService_StatusUpdated;
                try
                {
                    ProgressView window = new ProgressView
                    {
                        DataContext = dataContext
                    };
                    using (windowing.Disable())
                    {
                        Task task = action(dataContext.CancellationToken);
                        Task continuation = task.ContinueWith(
                            _ =>
                            {
                                window.CanClose = true;
                                window.Close();
                            },
                            TaskScheduler.FromCurrentSynchronizationContext());
                        await Task.Run(() =>
                        {
                            task.Wait(Delay);
                        });
                        if (!task.IsCompleted)
                        {
                            windowing.ShowDialog(window);
                        }
                        await task;
                        await continuation;
                    }
                }
                finally
                {
                    StatusUpdated -= ProgressService_StatusUpdated;
                }
            }
        }

        public Task RunAsync(Action action)
        {
            return RunCoreAsync(false, _ => Task.Run(action));
        }

        public Task RunAsync(Func<Task> action)
        {
            return RunCoreAsync(false, _ => action());
        }

        public Task RunAsync(Action<CancellationToken> action)
        {
            return RunCoreAsync(true, cancellationToken => Task.Run(() => action(cancellationToken)));
        }

        public Task RunAsync(Func<CancellationToken, Task> action)
        {
            return RunCoreAsync(true, action);
        }
    }
}
