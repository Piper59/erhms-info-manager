using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class ResponderLinkViewModel : DialogViewModel
    {
        public class ResponderListChildViewModel : ListViewModel<Responder>
        {
            public ResponderListChildViewModel(IServiceManager services)
                : base(services)
            {
                Refresh();
            }

            protected override IEnumerable<Responder> GetItems()
            {
                return Context.Responders.Select().OrderBy(responder => responder.FullName);
            }

            public void Select(string responderId)
            {
                SelectedItem = TypedItems.SingleOrDefault(responder => responder.ResponderId.EqualsIgnoreCase(responderId));
            }
        }

        private string fieldName;

        public ViewEntityRepository<ViewEntity> Entities { get; private set; }
        public ViewEntity Entity { get; private set; }
        public ResponderListChildViewModel Responders { get; private set; }

        public RelayCommand LinkCommand { get; private set; }
        public RelayCommand UnlinkCommand { get; private set; }

        public ResponderLinkViewModel(IServiceManager services, ViewEntityRepository<ViewEntity> entities, ViewEntity entity)
            : base(services)
        {
            Title = "Link to Responder";
            Entities = entities;
            Entity = entity;
            fieldName = entities.View.Fields["ResponderID"].Name;
            Responders = new ResponderListChildViewModel(services);
            string responderId = entity.GetProperty(fieldName) as string;
            if (responderId != null)
            {
                Responders.Select(responderId);
            }
            LinkCommand = new RelayCommand(Link, Responders.HasSelectedItem);
            UnlinkCommand = new RelayCommand(Unlink);
            Responders.SelectionChanged += (sender, e) =>
            {
                LinkCommand.RaiseCanExecuteChanged();
            };
        }

        public void Link()
        {
            Entity.SetProperty(fieldName, Responders.SelectedItem.ResponderId);
            Entities.Save(Entity);
            Close();
        }

        public void Unlink()
        {
            Entity.SetProperty(fieldName, null);
            Entities.Save(Entity);
            Close();
        }
    }
}
