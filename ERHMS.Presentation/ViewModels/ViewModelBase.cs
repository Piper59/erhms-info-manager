using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.Presentation.Messages;
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

        private bool closed;
        public bool Closed
        {
            get { return closed; }
            private set { Set(() => Closed, ref closed, value); }
        }

        public RelayCommand CloseCommand { get; private set; }

        public ViewModelBase()
        {
            CloseCommand = new RelayCommand(Close);
        }

        public void Close()
        {
            Closed = true;
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
    }
}
