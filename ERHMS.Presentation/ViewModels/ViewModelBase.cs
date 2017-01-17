using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        public static ViewModelLocator Locator
        {
            get { return App.Current.Locator; }
        }

        protected static DataContext DataContext
        {
            get { return Locator.Main.DataSource; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            protected set { Set(() => Title, ref title, value); }
        }

        private bool dirty;
        public virtual bool Dirty
        {
            get { return dirty; }
            set { Set(() => Dirty, ref dirty, value); }
        }

        private bool closed;
        public bool Closed
        {
            get { return closed; }
            private set { Set(() => Closed, ref closed, value); }
        }

        public RelayCommand CloseCommand { get; private set; }

        protected ViewModelBase()
        {
            PropertyChanged += (sender, e) =>
            {
                if (GetType().GetProperty(e.PropertyName).GetCustomAttribute<DirtyCheckAttribute>() != null)
                {
                    OnDirtyCheckPropertyChanged(sender, e);
                }
            };
            CloseCommand = new RelayCommand(Close);
        }

        public event EventHandler AfterClosed;
        private void OnAfterClosed(EventArgs e)
        {
            AfterClosed?.Invoke(this, e);
        }
        private void OnAfterClosed()
        {
            OnAfterClosed(EventArgs.Empty);
        }

        protected void OnDirtyCheckPropertyChanged(object sender, EventArgs e)
        {
            Dirty = true;
        }

        private void CloseInternal()
        {
            Closed = true;
            OnAfterClosed();
        }

        public void Close()
        {
            if (Closed)
            {
                return;
            }
            if (Dirty)
            {
                ConfirmMessage msg = new ConfirmMessage(
                    "Close",
                    string.Format("There may be unsaved changes. Are you sure you want to close {0}?", Title));
                msg.Confirmed += (sender, e) =>
                {
                    CloseInternal();
                };
                Messenger.Default.Send(msg);
            }
            else
            {
                CloseInternal();
            }
        }

        protected string GetTitle(string title, Incident incident)
        {
            if (incident == null)
            {
                return title;
            }
            else
            {
                string incidentName = incident.New ? "New Incident" : incident.Name;
                return string.Format("{0} {1}", incidentName, title).Trim();
            }
        }

        protected void NotifyRequired(IEnumerable<string> fields)
        {
            Messenger.Default.Send(new NotifyMessage(string.Format(
                "The following fields are required:{0}{0}{1}",
                Environment.NewLine,
                string.Join(", ", fields))));
        }

        protected void NotifyInvalid(IEnumerable<string> fields)
        {
            Messenger.Default.Send(new NotifyMessage(string.Format(
                "The following fields are invalid:{0}{0}{1}",
                Environment.NewLine,
                string.Join(", ", fields))));
        }
    }
}
