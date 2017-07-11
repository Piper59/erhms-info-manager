using ERHMS.DataAccess;
using ERHMS.Presentation.Infrastructure;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        protected MainViewModel Main
        {
            get { return MainViewModel.Instance; }
        }

        protected DataContext DataContext
        {
            get { return Main.DataContext; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            protected set { Set(nameof(Title), ref title, value); }
        }

        private bool dirty;
        public virtual bool Dirty
        {
            get { return dirty; }
            protected set { Set(nameof(Dirty), ref dirty, value); }
        }

        public RelayCommand CloseCommand { get; private set; }

        protected ViewModelBase()
        {
            CloseCommand = new RelayCommand(Close);
            PropertyChanged += (sender, e) =>
            {
                if (GetType().GetProperty(e.PropertyName).HasCustomAttribute<DirtyCheckAttribute>())
                {
                    OnDirtyCheckPropertyChanged(sender, e);
                }
            };
        }

        public event EventHandler Closing;
        private void OnClosing(EventArgs e)
        {
            Closing?.Invoke(this, e);
        }
        private void OnClosing()
        {
            OnClosing(EventArgs.Empty);
        }

        private void OnDirtyCheckPropertyChanged(object sender, EventArgs e)
        {
            Dirty = true;
        }

        protected void AddDirtyCheck(INotifyPropertyChanged entity)
        {
            entity.PropertyChanged += OnDirtyCheckPropertyChanged;
            Closing += (sender, e) =>
            {
                entity.PropertyChanged -= OnDirtyCheckPropertyChanged;
            };
        }

        protected void RemoveDirtyCheck(INotifyPropertyChanged entity)
        {
            entity.PropertyChanged -= OnDirtyCheckPropertyChanged;
        }

        public void Close()
        {
            if (Dirty)
            {
                ConfirmMessage msg = new ConfirmMessage
                {
                    Verb = "Close",
                    Message = string.Format("There may be unsaved changes. Are you sure you want to close {0}?", Title),
                };
                msg.Confirmed += (sender, e) =>
                {
                    OnClosing();
                };
                Messenger.Default.Send(msg);
            }
            else
            {
                OnClosing();
            }
        }

        private void ShowValidationMessage(IEnumerable<string> fields, string reason)
        {
            StringBuilder message = new StringBuilder();
            message.AppendFormat("The following fields are {0}:", reason);
            message.AppendLine();
            message.AppendLine();
            message.Append(string.Join(", ", fields));
            AlertMessage msg = new AlertMessage
            {
                Message = message.ToString()
            };
            Messenger.Default.Send(msg);
        }

        protected void ShowRequiredMessage(IEnumerable<string> fields)
        {
            ShowValidationMessage(fields, "required");
        }

        protected void ShowInvalidMessage(IEnumerable<string> fields)
        {
            ShowValidationMessage(fields, "invalid");
        }
    }
}
