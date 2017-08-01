using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class TemplateListViewModel : ListViewModel<TemplateInfo>
    {
        public static void Create(IServiceManager services, TemplateInfo template, Incident incident)
        {
            string prefix = incident == null ? "" : incident.Name + "_";
            Wrapper wrapper = MakeView.InstantiateViewTemplate.Create(services.Context.Project.FilePath, template.FilePath, prefix);
            wrapper.Event += (sender, e) =>
            {
                if (e.Type == WrapperEventType.ViewCreated)
                {
                    if (incident != null)
                    {
                        services.Context.ViewLinks.Save(new ViewLink
                        {
                            ViewId = e.Properties.ViewId,
                            IncidentId = incident.IncidentId
                        });
                    }
                    Messenger.Default.Send(new RefreshMessage(typeof(View)));
                }
            };
            wrapper.Invoke();
        }

        public Incident Incident { get; private set; }

        private RelayCommand createCommand;
        public ICommand CreateCommand
        {
            get { return createCommand ?? (createCommand = new RelayCommand(Create, HasSelectedItem)); }
        }

        private RelayCommand deleteCommand;
        public ICommand DeleteCommand
        {
            get { return deleteCommand ?? (deleteCommand = new RelayCommand(Delete, HasSelectedItem)); }
        }

        public TemplateListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Templates";
            Incident = incident;
            SelectionChanged += (sender, e) =>
            {
                createCommand.RaiseCanExecuteChanged();
                deleteCommand.RaiseCanExecuteChanged();
            };
            Refresh();
        }

        protected override IEnumerable<TemplateInfo> GetItems()
        {
            return TemplateInfo.GetByLevel(TemplateLevel.View).OrderBy(item => item.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(TemplateInfo item)
        {
            yield return item.Name;
            yield return item.Description;
        }

        public void Create()
        {
            Create(Services, SelectedItem, Incident);
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Delete",
                Message = "Delete the selected template?"
            };
            msg.Confirmed += (sender, e) =>
            {
                SelectedItem.Delete();
                MessengerInstance.Send(new RefreshMessage(typeof(TemplateInfo)));
            };
            MessengerInstance.Send(msg);
        }
    }
}
