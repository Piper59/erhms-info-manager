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
        private string status;

        public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(1.0);
        public string Title { get; set; }

        private event EventHandler Reporting;
        private void OnReporting(EventArgs e) => Reporting?.Invoke(this, e);
        private void OnReporting() => OnReporting(EventArgs.Empty);

        public void Report(string value)
        {
            status = value;
            OnReporting();
        }

        private async Task RunCoreAsync(bool canBeCanceled, Func<CancellationToken, Task> action)
        {
            ProgressViewModel dataContext = new ProgressViewModel(Title, canBeCanceled);

            void ProgressService_Reporting(object sender, EventArgs e)
            {
                dataContext.Status = status;
            }

            dataContext.Status = status;
            Reporting += ProgressService_Reporting;
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
                    Task task = action(dataContext.CancellationToken);
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
                }
            }
            finally
            {
                Reporting -= ProgressService_Reporting;
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
