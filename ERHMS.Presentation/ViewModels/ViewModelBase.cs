using ERHMS.DataAccess;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        protected static bool ValidateDateRange(DateTime? startDate, DateTime? endDate)
        {
            return !startDate.HasValue || !endDate.HasValue || startDate.Value <= endDate.Value;
        }

        public IServiceManager Services { get; private set; }

        public IDocumentManager Documents
        {
            get { return Services.Documents; }
        }

        public IDialogManager Dialogs
        {
            get { return Services.Dialogs; }
        }

        public DataContext Context
        {
            get { return Services.Context; }
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

        protected ViewModelBase(IServiceManager services)
        {
            Services = services;
            CloseCommand = new RelayCommand(Close, CanClose);
            services.ContextChanged += (sender, e) =>
            {
                RaisePropertyChanged(nameof(Context));
            };
            PropertyChanged += (sender, e) =>
            {
                if (GetType().GetProperty(e.PropertyName).HasCustomAttribute<DirtyCheckAttribute>())
                {
                    Dirty = true;
                }
            };
        }

        public event EventHandler Closed;
        protected virtual void OnClosed(EventArgs e)
        {
            Closed?.Invoke(this, e);
        }
        protected virtual void OnClosed()
        {
            OnClosed(EventArgs.Empty);
        }

        protected virtual bool CanClose()
        {
            return true;
        }

        protected void AddDirtyCheck(INotifyPropertyChanged child)
        {
            child.PropertyChanged += Child_PropertyChanged;
            Closed += (sender, e) =>
            {
                RemoveDirtyCheck(child);
            };
        }

        protected void RemoveDirtyCheck(INotifyPropertyChanged child)
        {
            child.PropertyChanged -= Child_PropertyChanged;
        }

        private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dirty = true;
        }

        public void Close(bool confirm)
        {
            if (Dirty && confirm)
            {
                ConfirmMessage msg = new ConfirmMessage
                {
                    Verb = "Close",
                    Message = string.Format("There may be unsaved changes. Are you sure you want to close {0}?", Title),
                };
                msg.Confirmed += (sender, e) =>
                {
                    OnClosed();
                };
                MessengerInstance.Send(msg);
            }
            else
            {
                OnClosed();
            }
        }

        public virtual void Close()
        {
            Close(true);
        }

        protected void ShowValidationMessage(ValidationError error, IEnumerable<string> fields)
        {
            StringBuilder message = new StringBuilder();
            message.AppendFormat("The following fields are {0}:", error.ToString().ToLower());
            message.AppendLine();
            message.AppendLine();
            foreach (string field in fields)
            {
                message.AppendLine(field);
            }
            MessengerInstance.Send(new AlertMessage
            {
                Message = message.ToString().Trim()
            });
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", GetType(), Title);
        }
    }
}
