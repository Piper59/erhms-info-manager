using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace ERHMS.Desktop.Views
{
    public partial class UtilityView : Window
    {
        private class ProgressLogger : IProgress<string>
        {
            public UtilityView Parent { get; }

            public ProgressLogger(UtilityView parent)
            {
                Parent = parent;
            }

            public async void Report(string value)
            {
                await Parent.Dispatcher.InvokeAsync(() => Parent.AppendToLog(value), DispatcherPriority.Background);
            }
        }

        private IProgress<string> progress;

        public new UtilityViewModel DataContext
        {
            get { return (UtilityViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public UtilityView()
        {
            InitializeComponent();
            progress = new ProgressLogger(this);
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UnregisterDataContext(e.OldValue as UtilityViewModel);
            RegisterDataContext(e.NewValue as UtilityViewModel);
        }

        private void RegisterDataContext(UtilityViewModel dataContext)
        {
            if (dataContext != null)
            {
                dataContext.Utility.Progress = progress;
                dataContext.ExitRequested += DataContext_ExitRequested;
            }
        }

        private void UnregisterDataContext(UtilityViewModel dataContext)
        {
            if (dataContext != null)
            {
                dataContext.Utility.Progress = null;
                dataContext.ExitRequested -= DataContext_ExitRequested;
            }
        }

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            await DataContext.Run();
        }

        private void AppendToLog(string message)
        {
            Log.AppendText(message + Environment.NewLine);
            Log.ScrollToEnd();
        }

        private void DataContext_ExitRequested(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!DataContext.Done)
            {
                DialogInfo info = new DialogInfo(DialogInfoPreset.Warning)
                {
                    Lead = "This utility is still running",
                    Body = "Exit anyway?",
                    Buttons = new DialogButtonCollection
                    {
                        new DialogButton(true, "Exit", false, false),
                        new DialogButton(false, "Don't exit", false, true),
                    }
                };
                if (!ServiceLocator.Dialog.Show(info).GetValueOrDefault())
                {
                    e.Cancel = true;
                }
            }
            base.OnClosing(e);
        }
    }
}
