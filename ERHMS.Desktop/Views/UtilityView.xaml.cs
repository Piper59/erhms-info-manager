using ERHMS.Desktop.Utilities;
using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using ResXResources = ERHMS.Desktop.Properties.Resources;

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

            public void Report(string value)
            {
                Action action = () => Parent.AppendToLog(value);
                if (Parent.CheckAccess())
                {
                    action();
                }
                else
                {
                    Parent.Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
                }
            }
        }

        private IProgress<string> progress;

        public new Utility DataContext
        {
            get { return (Utility)base.DataContext; }
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
            UnregisterDataContext(e.OldValue as Utility);
            RegisterDataContext(e.NewValue as Utility);
        }

        private void RegisterDataContext(Utility dataContext)
        {
            if (dataContext != null)
            {
                dataContext.Progress = progress;
                dataContext.CloseRequested += DataContext_CloseRequested;
            }
        }

        private void UnregisterDataContext(Utility dataContext)
        {
            if (dataContext != null)
            {
                dataContext.Progress = null;
                dataContext.CloseRequested -= DataContext_CloseRequested;
            }
        }

        private void AppendToLog(string message)
        {
            Log.AppendText(message + Environment.NewLine);
            Log.ScrollToEnd();
        }

        private void DataContext_CloseRequested(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!DataContext.Done)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine("This utility is still running.");
                message.AppendLine();
                message.Append("Close anyway?");
                MessageBoxResult result = MessageBox.Show(
                    message.ToString(),
                    ResXResources.AppTitle,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
            base.OnClosing(e);
        }
    }
}
