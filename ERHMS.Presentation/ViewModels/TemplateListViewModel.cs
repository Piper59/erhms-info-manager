using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.MakeView;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class TemplateListViewModel : ListViewModelBase<Template>
    {
        public Incident Incident { get; private set; }

        public string IncidentId
        {
            get { return Incident == null ? null : Incident.IncidentId; }
        }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public TemplateListViewModel(Incident incident)
        {
            Title = "Templates";
            Incident = incident;
            Refresh();
            Selecting += (sender, e) =>
            {
                CreateCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            };
            CreateCommand = new RelayCommand(Create, HasSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshListMessage<Template>>(this, OnRefreshTemplateListMessage);
        }

        protected override ICollectionView GetItems()
        {
            return CollectionViewSource.GetDefaultView(DataContext.GetTemplates(TemplateLevel.View).OrderBy(template => template.Name));
        }

        protected override IEnumerable<string> GetFilteredValues(Template item)
        {
            yield return item.Name;
            yield return item.Description;
        }

        public void Create()
        {
            string prefix = Incident == null ? null : Incident.Name;
            MakeView.InstantiateTemplate(DataContext.Project, SelectedItem, prefix, IncidentId).Invoke();
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage("Delete", "Delete the selected template?");
            msg.Confirmed += (sender, e) =>
            {
                SelectedItem.Delete();
                Messenger.Default.Send(new RefreshListMessage<Template>());
            };
            Messenger.Default.Send(msg);
        }

        private void OnRefreshTemplateListMessage(RefreshListMessage<Template> msg)
        {
            Refresh();
        }
    }
}
