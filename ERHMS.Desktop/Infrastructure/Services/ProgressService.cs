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
        private static readonly TimeSpan Delay = TimeSpan.FromSeconds(1.0);

        private ProgressViewModel viewModel;

        public Application Application { get; }

        public ProgressService(Application application)
        {
            Application = application;
        }

        public void Report(string value)
        {
            Application.Dispatcher.Invoke(() => viewModel.Status = value);
        }

        private async Task RunCoreAsync(string title, bool canBeCanceled, Func<CancellationToken, Task> action)
        {
            try
            {
                using (viewModel = new ProgressViewModel(title, canBeCanceled))
                {
                    Window owner = Application.GetActiveOrMainWindow();
                    NonBlockingModalDialog dialog = new NonBlockingModalDialog(new ProgressView
                    {
                        Owner = owner,
                        DataContext = viewModel
                    });
                    using (WindowDisabler.Begin(owner))
                    using (CancellationTokenSource completionTokenSource = new CancellationTokenSource())
                    {
                        Task task = action(viewModel.CancellationToken);
                        Task continuation = task.ContinueWith(_ => completionTokenSource.Cancel());
                        try
                        {
                            await Task.Delay(Delay, completionTokenSource.Token);
                        }
                        catch (TaskCanceledException) { }
                        using (completionTokenSource.IsCancellationRequested ? null : dialog.BeginShow())
                        {
                            await task;
                            await continuation;
                        }
                        await dialog.EndShowAsync();
                    }
                }
            }
            finally
            {
                viewModel = null;
            }
        }

        public Task RunAsync(string title, Action action)
        {
            return RunCoreAsync(title, false, _ => Task.Run(action));
        }

        public Task RunAsync(string title, Func<Task> action)
        {
            return RunCoreAsync(title, false, _ => action());
        }

        public Task RunAsync(string title, Action<CancellationToken> action)
        {
            return RunCoreAsync(title, true, cancellationToken => Task.Run(() => action(cancellationToken)));
        }

        public Task RunAsync(string title, Func<CancellationToken, Task> action)
        {
            return RunCoreAsync(title, true, action);
        }
    }
}
