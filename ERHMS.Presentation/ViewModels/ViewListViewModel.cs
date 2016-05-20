using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analysis;
using ERHMS.EpiInfo.AnalysisDashboard;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Enter;
using ERHMS.EpiInfo.ImportExport;
using ERHMS.EpiInfo.MakeView;
using ERHMS.EpiInfo.Web;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Action = System.Action;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewListViewModel : ListViewModelBase<View>
    {
        public class SurveyViewModel : ViewModelBase
        {
            private static void RequestConfigurationInternal(string message)
            {
                NotifyMessage msg = new NotifyMessage(message);
                msg.Dismissed += (sender, e) =>
                {
                    Locator.Main.OpenSettingsView();
                };
                Messenger.Default.Send(msg);
            }

            public static void RequestConfiguration()
            {
                RequestConfigurationInternal("Please configure web survey settings.");
            }

            public static void RequestConfiguration(string reason)
            {
                RequestConfigurationInternal(string.Format("{0} Please verify web survey settings.", reason));
            }

            public static void RequestConfiguration(ConfigurationError error)
            {
                RequestConfiguration(error.GetMessage());
            }

            public ICollection<ResponseType> ResponseTypes { get; private set; }

            private bool active;
            public bool Active
            {
                get { return active; }
                set { Set(() => Active, ref active, value); }
            }

            public View View { get; private set; }

            private Survey survey;
            public Survey Survey
            {
                get { return survey; }
                set { Set(() => Survey, ref survey, value); }
            }

            public RelayCommand PublishCommand { get; private set; }
            public RelayCommand CancelCommand { get; private set; }

            public SurveyViewModel()
            {
                ResponseTypes = EnumExtensions.GetValues<ResponseType>()
                    .Where(responseType => responseType != ResponseType.Unspecified)
                    .ToList();
                PublishCommand = new RelayCommand(Publish);
                CancelCommand = new RelayCommand(Cancel);
            }

            public void Activate(View view)
            {
                View = view;
                if (view.IsPublished())
                {
                    ConfigurationError error = ConfigurationError.None;
                    BlockMessage msg = new BlockMessage("Retrieving web survey details \u2026");
                    msg.Executing += (sender, e) =>
                    {
                        Service service = new Service();
                        error = service.CheckConfiguration();
                        if (error == ConfigurationError.None)
                        {
                            Survey = service.GetSurvey(view);
                        }
                    };
                    msg.Executed += (sender, e) =>
                    {
                        if (error != ConfigurationError.None)
                        {
                            RequestConfiguration(error);
                        }
                        else if (Survey == null)
                        {
                            RequestConfiguration("Failed to retrieve web survey details.");
                        }
                        else
                        {
                            Active = true;
                        }
                    };
                    Messenger.Default.Send(msg);
                }
                else
                {
                    DateTime now = DateTime.Now;
                    Survey = new Survey
                    {
                        Title = view.Name,
                        StartDate = now,
                        EndDate = now.AddMonths(1),
                        ResponseType = ResponseType.Single,
                        Intro = "",
                        Outro = "",
                        Draft = false,
                        PublishKey = Guid.NewGuid()
                    };
                    Active = true;
                }
            }

            public void Publish()
            {
                // TODO: Validate fields
                bool success = false;
                ConfigurationError error = ConfigurationError.None;
                BlockMessage msg = new BlockMessage("Publishing form to web \u2026");
                msg.Executing += (sender, e) =>
                {
                    Service service = new Service();
                    error = service.CheckConfiguration();
                    if (error != ConfigurationError.None)
                    {
                        return;
                    }
                    if (View.IsPublished())
                    {
                        success = service.Republish(View, Survey);
                    }
                    else
                    {
                        success = service.Publish(View, Survey);
                        if (success)
                        {
                            WebSurvey webSurvey = DataContext.WebSurveys.Create();
                            webSurvey.WebSurveyId = Survey.SurveyId;
                            webSurvey.ViewId = View.Id;
                            webSurvey.PublishKey = Survey.PublishKey.ToString();
                            DataContext.WebSurveys.Save(webSurvey);
                        }
                    }
                };
                msg.Executed += (sender, e) =>
                {
                    if (error != ConfigurationError.None)
                    {
                        RequestConfiguration(error);
                    }
                    else if (!success)
                    {
                        RequestConfiguration("Failed to publish form to web.");
                    }
                    else
                    {
                        Active = false;
                        Messenger.Default.Send(new ToastMessage("Form has been published to web."));
                    }
                };
                Messenger.Default.Send(msg);
            }

            public void Cancel()
            {
                Active = false;
            }
        }

        public class AnalysisViewModel : ViewModelBase
        {
            private bool active;
            public bool Active
            {
                get { return active; }
                set { Set(() => Active, ref active, value); }
            }

            private string name;
            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    if (Set(() => Name, ref name, value))
                    {
                        CreateCommand.RaiseCanExecuteChanged();
                    }
                }
            }

            public Action Callback { get; private set; }

            public RelayCommand CreateCommand { get; private set; }
            public RelayCommand CancelCommand { get; private set; }

            public AnalysisViewModel(Action callback)
            {
                Callback = callback;
                CreateCommand = new RelayCommand(Create, HasName);
                CancelCommand = new RelayCommand(Cancel);
            }

            public bool HasName()
            {
                return !string.IsNullOrWhiteSpace(Name);
            }

            public void Activate()
            {
                Name = "";
                Active = true;
            }

            public void Create()
            {
                Callback();
            }

            public void Cancel()
            {
                Active = false;
            }
        }

        public Incident Incident { get; private set; }

        public string IncidentId
        {
            get { return Incident == null ? null : Incident.IncidentId; }
        }

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
            if (Incident == null)
            {
                Title = "Forms";
            }
            else
            {
                string incidentName = Incident.New ? "New Incident" : Incident.Name;
                Title = string.Format("{0} Forms", incidentName).Trim();
            }
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

        public void Create()
        {
            MakeView.AddView(DataContext.Project, Incident == null ? null : Incident.Name, IncidentId);
        }

        public void Edit()
        {
            MakeView.OpenView(SelectedItem);
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage(
                "Delete?",
                "Are you sure you want to delete this form?",
                "Delete",
                "Don't Delete");
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

        public void EnterData()
        {
            Enter.OpenView(SelectedItem);
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
                App.Current.Service.OnViewDataImported(SelectedItem.Project.FilePath, SelectedItem.Name);
            }
        }

        public void ImportFromPackage()
        {
            if (ImportExport.ImportFromPackage(SelectedItem))
            {
                App.Current.Service.OnViewDataImported(SelectedItem.Project.FilePath, SelectedItem.Name);
            }
        }

        public void ImportFromFile()
        {
            Analysis.Import(SelectedItem);
        }

        public void ImportFromWeb()
        {
            if (!SelectedItem.IsPublished())
            {
                Messenger.Default.Send(new NotifyMessage("Form has not been published to web."));
                return;
            }
            bool success = false;
            ConfigurationError error = ConfigurationError.None;
            Survey survey = null;
            BlockMessage msg = new BlockMessage("Importing data from web \u2026");
            msg.Executing += (sender, e) =>
            {
                Service service = new Service();
                error = service.CheckConfiguration();
                if (error != ConfigurationError.None)
                {
                    return;
                }
                survey = service.GetSurvey(SelectedItem);
                if (survey == null)
                {
                    return;
                }
                ViewEntityRepository<ViewEntity> entities = new ViewEntityRepository<ViewEntity>(DataContext.Driver, SelectedItem);
                foreach (Record record in service.GetRecords(SelectedItem, survey))
                {
                    ViewEntity entity = entities.SelectByGlobalRecordId(record.GlobalRecordId);
                    if (entity == null)
                    {
                        entity = entities.Create();
                        entity.GlobalRecordId = record.GlobalRecordId;
                    }
                    foreach (string key in record.Keys)
                    {
                        Type type = entities.GetDataType(key);
                        entity.SetProperty(key, record.GetValue(key, type));
                    }
                    entities.Save(entity);
                }
                success = true;
            };
            msg.Executed += (sender, e) =>
            {
                if (error != ConfigurationError.None)
                {
                    SurveyViewModel.RequestConfiguration(error);
                }
                else if (survey == null)
                {
                    SurveyViewModel.RequestConfiguration("Failed to retrieve web survey details.");
                }
                else if (!success)
                {
                    SurveyViewModel.RequestConfiguration("Failed to import data from web.");
                }
                else
                {
                    Messenger.Default.Send(new ToastMessage("Data has been imported from web."));
                }
            };
            Messenger.Default.Send(msg);
        }

        public void ImportFromMobile()
        {
            if (ImportExport.ImportFromMobile(SelectedItem))
            {
                App.Current.Service.OnViewDataImported(SelectedItem.Project.FilePath, SelectedItem.Name);
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
            PgmModel.Activate();
        }

        public void AnalyzeVisual()
        {
            CanvasModel.Activate();
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
            if (string.Equals(msg.IncidentId, IncidentId, StringComparison.OrdinalIgnoreCase))
            {
                Refresh();
            }
        }
    }
}
