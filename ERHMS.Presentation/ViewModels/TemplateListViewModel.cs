using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class TemplateListViewModel : ListViewModelBase<TemplateInfo>
    {
        public Incident Incident { get; private set; }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public TemplateListViewModel(Incident incident)
        {
            Title = "Templates";
            Incident = incident;
            Refresh();
            CreateCommand = new RelayCommand(Create, HasOneSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasOneSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            SelectedItemChanged += (sender, e) =>
            {
                CreateCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            };
            Messenger.Default.Register<RefreshMessage<TemplateInfo>>(this, msg => Refresh());
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
            string namePrefix = Incident == null ? "" : Incident.Name + "_";
            Wrapper wrapper = MakeView.InstantiateViewTemplate.Create(DataContext.Project.FilePath, SelectedItem.Path, namePrefix);
            wrapper.Event += (sender, e) =>
            {
                if (e.Type == WrapperEventType.ViewCreated)
                {
                    Messenger.Default.Send(new RefreshMessage<View>());
                }
            };
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
                Messenger.Default.Send(new RefreshMessage<TemplateInfo>());
            };
            Messenger.Default.Send(msg);
        }
    }
}
