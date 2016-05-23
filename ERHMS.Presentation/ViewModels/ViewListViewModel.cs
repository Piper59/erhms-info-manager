using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analysis;
using ERHMS.EpiInfo.AnalysisDashboard;
using ERHMS.EpiInfo.Enter;
using ERHMS.EpiInfo.ImportExport;
using ERHMS.EpiInfo.MakeView;
using ERHMS.EpiInfo.Web;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewListViewModel : ListViewModelBase<View>
    {
        public class ResponderInternalViewModel : ViewModelBase
        {
            private bool active;
            public bool Active
            {
                get { return active; }
                set { Set(() => Active, ref active, value); }
            }

            public View View { get; private set; }

            private ICollection<Responder> responders;
            public ICollection<Responder> Responders
            {
                get { return responders; }
                set { Set(() => Responders, ref responders, value); }
            }

            private Responder selectedResponder;
            public Responder SelectedResponder
            {
                get { return selectedResponder; }
                set { Set(() => SelectedResponder, ref selectedResponder, value); }
            }

            public RelayCommand ContinueCommand { get; private set; }
            public RelayCommand CancelCommand { get; private set; }

            public ResponderInternalViewModel()
            {
                Refresh();
                ContinueCommand = new RelayCommand(Continue);
                CancelCommand = new RelayCommand(Cancel);
                Messenger.Default.Register<RefreshListMessage<Responder>>(this, OnRefreshResponderListMessage);
            }

            private void Refresh()
            {
                Responders = DataContext.Responders.SelectByDeleted(false)
                    .OrderBy(responder => responder.LastName)
                    .ThenBy(responder => responder.FirstName)
                    .ToList();
            }

            public void Reset(View view)
            {
                View = view;
                SelectedResponder = null;
            }

            public void Continue()
            {
                if (SelectedResponder == null)
                {
                    Enter.OpenView(View);
                }
                else
                {
                    Enter.OpenView(View, new
                    {
                        ResponderId = SelectedResponder.ResponderId
                    });
                }
                Active = false;
            }

            public void Cancel()
            {
                Active = false;
            }

            private void OnRefreshResponderListMessage(RefreshListMessage<Responder> msg)
            {
                Refresh();
            }
        }

        public Incident Incident { get; private set; }

        public string IncidentId
        {
            get { return Incident == null ? null : Incident.IncidentId; }
        }

        public ResponderInternalViewModel ResponderModel { get; private set; }
        public SurveyViewModel SurveyModel { get; private set; }
        public AnalysisViewModel PgmModel { get; private set; }
        public AnalysisViewModel CanvasModel { get; private set; }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand EnterDataCommand { get; private set; }
        public RelayCommand ViewDataCommand { get; private set; }
        public RelayCommand PublishToTemplateCommand { get; private set; }
        public RelayCommand PublishToWebCommand { get; private set; }
        public RelayCommand PublishToMobileCommand { get; private set; }
        public RelayCommand ImportFromProjectCommand { get; private set; }
        public RelayCommand ImportFromPackageCommand { get; private set; }
        public RelayCommand ImportFromFileCommand { get; private set; }
        public RelayCommand ImportFromWebCommand { get; private set; }
        public RelayCommand ImportFromMobileCommand { get; private set; }
        public RelayCommand ExportToPackageCommand { get; private set; }
        public RelayCommand ExportToFileCommand { get; private set; }
        public RelayCommand AnalyzeClassicCommand { get; private set; }
        public RelayCommand AnalyzeVisualCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public ViewListViewModel(Incident incident)
        {
            Incident = incident;
            UpdateTitle();
            Refresh();
            Selecting += (sender, e) =>
            {
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                EnterDataCommand.RaiseCanExecuteChanged();
                ViewDataCommand.RaiseCanExecuteChanged();
                PublishToTemplateCommand.RaiseCanExecuteChanged();
                PublishToWebCommand.RaiseCanExecuteChanged();
                PublishToMobileCommand.RaiseCanExecuteChanged();
                ImportFromProjectCommand.RaiseCanExecuteChanged();
                ImportFromPackageCommand.RaiseCanExecuteChanged();
                ImportFromFileCommand.RaiseCanExecuteChanged();
                ImportFromWebCommand.RaiseCanExecuteChanged();
                ImportFromMobileCommand.RaiseCanExecuteChanged();
                ExportToPackageCommand.RaiseCanExecuteChanged();
                ExportToFileCommand.RaiseCanExecuteChanged();
                AnalyzeClassicCommand.RaiseCanExecuteChanged();
                AnalyzeVisualCommand.RaiseCanExecuteChanged();
            };
            ResponderModel = new ResponderInternalViewModel();
            SurveyModel = new SurveyViewModel();
            PgmModel = new AnalysisViewModel(CreatePgm);
            CanvasModel = new AnalysisViewModel(CreateCanvas);
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasSelectedItem);
            EnterDataCommand = new RelayCommand(EnterData, HasSelectedItem);
            ViewDataCommand = new RelayCommand(ViewData, HasSelectedItem);
            PublishToTemplateCommand = new RelayCommand(PublishToTemplate, HasSelectedItem);
            PublishToWebCommand = new RelayCommand(PublishToWeb, HasSelectedItem);
            PublishToMobileCommand = new RelayCommand(PublishToMobile, HasSelectedItem);
            ImportFromProjectCommand = new RelayCommand(ImportFromProject, HasSelectedItem);
            ImportFromPackageCommand = new RelayCommand(ImportFromPackage, HasSelectedItem);
            ImportFromFileCommand = new RelayCommand(ImportFromFile, HasSelectedItem);
            ImportFromWebCommand = new RelayCommand(ImportFromWeb, HasSelectedItem);
            ImportFromMobileCommand = new RelayCommand(ImportFromMobile, HasSelectedItem);
            ExportToPackageCommand = new RelayCommand(ExportToPackage, HasSelectedItem);
            ExportToFileCommand = new RelayCommand(ExportToFile, HasSelectedItem);
            AnalyzeClassicCommand = new RelayCommand(AnalyzeClassic, HasSelectedItem);
            AnalyzeVisualCommand = new RelayCommand(AnalyzeVisual, HasSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<Incident>>(this, OnRefreshIncidentMessage);
            Messenger.Default.Register<RefreshListMessage<View>>(this, OnRefreshViewListMessage);
        }

        private void UpdateTitle()
        {
            Title = GetTitleWithIncidentName("Forms", Incident);
        }

        protected override ICollectionView GetItems()
        {
            IEnumerable<View> views;
            if (Incident == null)
            {
                views = DataContext.GetUnlinkedViews();
            }
            else
            {
                views = DataContext.GetLinkedViews(IncidentId);
            }
            return CollectionViewSource.GetDefaultView(views.OrderBy(view => view.Name));
        }

        protected override IEnumerable<string> GetFilteredValues(View item)
        {
            yield return item.Name;
        }

        private bool IsResponderView(View view)
        {
            return view.Name.EqualsIgnoreCase(DataContext.Responders.View.Name);
        }

        private bool IsResponderLinkedView(View view)
        {
            Responder responder;
            return !IsResponderView(view) && view.Fields.Contains(nameof(responder.ResponderId));
        }

        public void Create()
        {
            string prefix = Incident == null ? null : Incident.Name;
            MakeView.AddView(DataContext.Project, prefix, IncidentId);
        }

        public void Edit()
        {
            MakeView.OpenView(SelectedItem);
        }

        public void Delete()
        {
            if (IsResponderView(SelectedItem))
            {
                Messenger.Default.Send(new NotifyMessage("The selected form cannot be deleted."));
            }
            else
            {
                ConfirmMessage msg = new ConfirmMessage("Delete", "Delete the selected form?");
                msg.Confirmed += (sender, e) =>
                {
                    DataContext.Assignments.DeleteByViewId(SelectedItem.Id);
                    DataContext.ViewLinks.DeleteByViewId(SelectedItem.Id);
                    DataContext.WebSurveys.DeleteByViewId(SelectedItem.Id);
                    DataContext.Project.DeleteView(SelectedItem);
                    Messenger.Default.Send(new RefreshListMessage<View>(IncidentId));
                };
                Messenger.Default.Send(msg);
            }
        }

        public void EnterData()
        {
            if (IsResponderLinkedView(SelectedItem))
            {
                ResponderModel.Reset(SelectedItem);
                ResponderModel.Active = true;
            }
            else
            {
                Enter.OpenView(SelectedItem);
            }
        }

        public void ViewData()
        {
            Locator.Main.OpenRecordListView(SelectedItem);
        }

        public void PublishToTemplate()
        {
            MakeView.CreateTemplate(SelectedItem);
        }

        public void PublishToWeb()
        {
            Service service = new Service();
            if (service.IsConfigured())
            {
                SurveyModel.Activate(SelectedItem);
            }
            else
            {
                SurveyViewModel.RequestConfiguration();
            }
        }

        public void PublishToMobile()
        {
            MakeView.PublishToMobile(SelectedItem);
        }

        public void ImportFromProject()
        {
            if (ImportExport.ImportFromView(SelectedItem))
            {
                Messenger.Default.Send(new RefreshDataMessage(SelectedItem));
            }
        }

        public void ImportFromPackage()
        {
            if (ImportExport.ImportFromPackage(SelectedItem))
            {
                Messenger.Default.Send(new RefreshDataMessage(SelectedItem));
            }
        }

        public void ImportFromFile()
        {
            Analysis.Import(SelectedItem);
        }

        public void ImportFromWeb()
        {
            SurveyViewModel surveyModel = new SurveyViewModel(SelectedItem);
            if (surveyModel.Import())
            {
                Messenger.Default.Send(new RefreshDataMessage(SelectedItem));
            }
        }

        public void ImportFromMobile()
        {
            if (ImportExport.ImportFromMobile(SelectedItem))
            {
                Messenger.Default.Send(new RefreshDataMessage(SelectedItem));
            }
        }

        public void ExportToPackage()
        {
            ImportExport.ExportToPackage(SelectedItem);
        }

        public void ExportToFile()
        {
            Analysis.Export(SelectedItem);
        }

        public void AnalyzeClassic()
        {
            PgmModel.Reset();
            PgmModel.Active = true;
        }

        public void AnalyzeVisual()
        {
            CanvasModel.Reset();
            CanvasModel.Active = true;
        }

        public void CreatePgm()
        {
            Pgm pgm = new Pgm
            {
                Name = PgmModel.Name,
                Content = Pgm.GetContentForView(SelectedItem)
            };
            DataContext.Project.InsertPgm(pgm);
            if (Incident != null)
            {
                PgmLink pgmLink = DataContext.PgmLinks.Create();
                pgmLink.PgmId = pgm.PgmId;
                pgmLink.IncidentId = IncidentId;
                DataContext.PgmLinks.Save(pgmLink);
            }
            Messenger.Default.Send(new RefreshListMessage<Pgm>(IncidentId));
            PgmModel.Active = false;
            Analysis.OpenPgm(pgm, true);
        }

        public void CreateCanvas()
        {
            Canvas canvas = new Canvas
            {
                Name = CanvasModel.Name,
                Content = Canvas.GetContentForView(SelectedItem)
            };
            DataContext.Project.InsertCanvas(canvas);
            if (Incident != null)
            {
                CanvasLink canvasLink = DataContext.CanvasLinks.Create();
                canvasLink.CanvasId = canvas.CanvasId;
                canvasLink.IncidentId = IncidentId;
                DataContext.CanvasLinks.Save(canvasLink);
            }
            Messenger.Default.Send(new RefreshListMessage<Canvas>(IncidentId));
            CanvasModel.Active = false;
            AnalysisDashboard.OpenCanvas(DataContext.Project, canvas, IncidentId);
        }

        private void OnRefreshIncidentMessage(RefreshMessage<Incident> msg)
        {
            if (msg.Entity == Incident)
            {
                UpdateTitle();
            }
        }

        private void OnRefreshViewListMessage(RefreshListMessage<View> msg)
        {
            if (StringExtensions.EqualsIgnoreCase(msg.IncidentId, IncidentId))
            {
                Refresh();
            }
        }
    }
}
