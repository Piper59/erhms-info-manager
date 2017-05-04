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
    public class CanvasListViewModel : ListViewModelBase<DeepLink<Canvas>>
    {
        public Incident Incident { get; private set; }

        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand LinkCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public CanvasListViewModel(Incident incident)
        {
            Title = "Dashboards";
            Incident = incident;
            Refresh();
            OpenCommand = new RelayCommand(Open, HasOneSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasOneSelectedItem);
            LinkCommand = new RelayCommand(Link, HasOneSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            SelectedItemChanged += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                LinkCommand.RaiseCanExecuteChanged();
            };
            Messenger.Default.Register<RefreshMessage<Canvas>>(this, msg => Refresh());
            Messenger.Default.Register<RefreshMessage<Incident>>(this, msg => Refresh());
        }

        protected override IEnumerable<DeepLink<Canvas>> GetItems()
        {
            IEnumerable<DeepLink<Canvas>> items;
            if (Incident == null)
            {
                items = DataContext.CanvasLinks.SelectDeepLinks();
            }
            else
            {
                items = DataContext.CanvasLinks.SelectDeepLinksByIncidentId(Incident.IncidentId);
            }
            return items.OrderBy(item => item.Item.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(DeepLink<Canvas> item)
        {
            yield return item.Item.Name;
            if (Incident == null)
            {
                yield return item.Incident?.Name;
            }
        }

        public void Open()
        {
            Canvas canvas = DataContext.Project.GetCanvasById(SelectedItem.Item.CanvasId);
            Wrapper wrapper = AnalysisDashboard.OpenCanvas.Create(DataContext.Project.FilePath, canvas.Content);
            wrapper.Event += (sender, e) =>
            {
                if (e.Type == WrapperEventType.CanvasSaved)
                {
                    canvas.Content = e.Properties.Content;
                    DataContext.Project.UpdateCanvas(canvas);
                    Messenger.Default.Send(new RefreshMessage<Canvas>());
                }
            };
            wrapper.Invoke();
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Delete",
                Message = "Delete the selected dashboard?"
            };
            msg.Confirmed += (sender, e) =>
            {
                DataContext.CanvasLinks.DeleteByCanvasId(SelectedItem.Item.CanvasId);
                DataContext.Project.DeleteCanvas(SelectedItem.Item);
                Messenger.Default.Send(new RefreshMessage<Canvas>());
            };
            Messenger.Default.Send(msg);
        }

        public void Link()
        {
            Messenger.Default.Send(new ShowMessage
            {
                ViewModel = new CanvasLinkViewModel(SelectedItem)
                {
                    Active = true
                }
            });
        }
    }
}
