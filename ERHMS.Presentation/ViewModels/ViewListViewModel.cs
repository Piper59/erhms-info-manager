using Epi;
using Epi.Fields;
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using Template = ERHMS.EpiInfo.Template;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewListViewModel : ListViewModelBase<Link<View>>
    {
        public class LinkInternalViewModel : LinkViewModelBase
        {
            public Link<View> View { get; private set; }

            public void Reset(Link<View> view)
            {
                Reset(view.IncidentId);
                View = view;
            }

            public override void Link()
            {
                DataContext.ViewLinks.DeleteByViewId(View.Data.Id);
                ViewLink viewLink = DataContext.ViewLinks.Create();
                viewLink.ViewId = View.Data.Id;
                viewLink.IncidentId = SelectedIncidentId;
                DataContext.ViewLinks.Save(viewLink);
                Messenger.Default.Send(new RefreshListMessage<View>(SelectedIncidentId));
                Active = false;
            }

            public override void Unlink()
            {
                DataContext.ViewLinks.DeleteByViewId(View.Data.Id);
                Messenger.Default.Send(new RefreshListMessage<View>(SelectedIncidentId));
                Active = false;
            }
        }

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
                    Enter.OpenView(View).Invoke();
                }
                else
                {
                    Enter.OpenView(View, new
                    {
                        ResponderId = SelectedResponder.ResponderId
                    }).Invoke();
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

        public LinkInternalViewModel LinkModel { get; private set; }
        public ResponderInternalViewModel ResponderModel { get; private set; }
        public SurveyViewModel SurveyModel { get; private set; }
        public AnalysisViewModel PgmModel { get; private set; }
        public AnalysisViewModel CanvasModel { get; private set; }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand IncidentCommand { get; private set; }
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
                IncidentCommand.RaiseCanExecuteChanged();
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
            LinkModel = new LinkInternalViewModel();
            ResponderModel = new ResponderInternalViewModel();
            SurveyModel = new SurveyViewModel();
            PgmModel = new AnalysisViewModel(CreatePgm);
            CanvasModel = new AnalysisViewModel(CreateCanvas);
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasNonSystemSelectedView);
            IncidentCommand = new RelayCommand(Link, HasNonSystemSelectedView);
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
            Title = GetTitle("Forms", Incident);
        }

        protected override ICollectionView GetItems()
        {
            IEnumerable<Link<View>> views;
            if (Incident == null)
            {
                views = DataContext.GetLinkedViews().Where(view => view.Incident == null || !view.Incident.Deleted);
            }
            else
            {
                views = DataContext.GetLinkedViews(IncidentId).Select(view => new Link<View>(view, Incident));
            }
            return CollectionViewSource.GetDefaultView(views.OrderBy(view => view.Data.Name));
        }

        protected override IEnumerable<string> GetFilteredValues(Link<View> item)
        {
            yield return item.Data.Name;
            yield return item.IncidentName;
        }

        public bool HasNonSystemSelectedView()
        {
            return HasSelectedItem() && !DataContext.IsResponderView(SelectedItem.Data);
        }

        public void Create()
        {
            string prefix = Incident == null ? null : Incident.Name;
            FileInfo templateFile = IOExtensions.GetTemporaryFile("ERHMS_{0:N}.xml");
            Assembly.GetAssembly(typeof(Responder)).CopyManifestResourceTo("ERHMS.Domain.Templates.Forms.Empty.xml", templateFile);
            Template template = Template.Get(templateFile);
            MakeView.InstantiateTemplate(DataContext.Project, template, prefix, IncidentId).Invoke();
        }

        public void Edit()
        {
            MakeView.OpenView(SelectedItem.Data).Invoke();
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage("Delete", "Delete the selected form?");
            msg.Confirmed += (sender, e) =>
            {
                DataContext.Assignments.DeleteByViewId(SelectedItem.Data.Id);
                DataContext.ViewLinks.DeleteByViewId(SelectedItem.Data.Id);
                DataContext.WebSurveys.DeleteByViewId(SelectedItem.Data.Id);
                DataContext.Project.DeleteView(SelectedItem.Data);
                Messenger.Default.Send(new RefreshListMessage<View>(SelectedItem.IncidentId));
            };
            Messenger.Default.Send(msg);
        }

        public void Link()
        {
            LinkModel.Reset(SelectedItem);
            LinkModel.Active = true;
        }

        public void EnterData()
        {
            if (DataContext.IsResponderLinkedView(SelectedItem.Data))
            {
                ResponderModel.Reset(SelectedItem.Data);
                ResponderModel.Active = true;
            }
            else
            {
                Enter.OpenView(SelectedItem.Data).Invoke();
            }
        }

        public void ViewData()
        {
            SelectedItem.Data.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Data);
            Locator.Main.OpenRecordListView(SelectedItem.Data);
        }

        public void PublishToTemplate()
        {
            MakeView.CreateTemplate(SelectedItem.Data).Invoke();
        }

        private void NotifyUnsupported(string message, IEnumerable<Field> fields)
        {
            string fieldList = string.Join(", ", fields.Select(field => string.Format("{0} ({1})", field.Name, field.FieldType)));
            Messenger.Default.Send(new NotifyMessage(string.Format("{0}{1}{1}{2}", message, Environment.NewLine, fieldList)));
        }

        public void PublishToWeb()
        {
            ICollection<Field> fields = SelectedItem.Data.Fields.Cast<Field>()
                .Where(field => !field.FieldType.IsWebSupported())
                .ToList();
            if (fields.Count > 0)
            {
                NotifyUnsupported("The following fields are not supported for publication to web:", fields);
            }
            else
            {
                Service service = new Service();
                if (service.IsConfigured())
                {
                    SurveyModel.Activate(SelectedItem.Data);
                }
                else
                {
                    SurveyViewModel.RequestConfiguration();
                }
            }
        }

        public void PublishToMobile()
        {
            ICollection<Field> fields = SelectedItem.Data.Fields.Cast<Field>()
                .Where(field => !field.FieldType.IsMobileSupported())
                .ToList();
            if (fields.Count > 0)
            {
                NotifyUnsupported("The following fields are not supported for publication to mobile:", fields);
            }
            else
            {
                SelectedItem.Data.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Data);
                MakeView.PublishToMobile(SelectedItem.Data).Invoke();
            }
        }

        public void ImportFromProject()
        {
            SelectedItem.Data.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Data);
            if (ImportExport.ImportFromView(SelectedItem.Data))
            {
                Messenger.Default.Send(new RefreshDataMessage(SelectedItem.Data));
            }
        }

        public void ImportFromPackage()
        {
            SelectedItem.Data.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Data);
            if (ImportExport.ImportFromPackage(SelectedItem.Data))
            {
                Messenger.Default.Send(new RefreshDataMessage(SelectedItem.Data));
            }
        }

        public void ImportFromFile()
        {
            SelectedItem.Data.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Data);
            Analysis.Import(SelectedItem.Data).Invoke();
        }

        public void ImportFromWeb()
        {
            SelectedItem.Data.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Data);
            SurveyViewModel surveyModel = new SurveyViewModel(SelectedItem.Data);
            if (surveyModel.Import())
            {
                Messenger.Default.Send(new RefreshDataMessage(SelectedItem.Data));
            }
        }

        public void ImportFromMobile()
        {
            SelectedItem.Data.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Data);
            if (ImportExport.ImportFromMobile(SelectedItem.Data))
            {
                Messenger.Default.Send(new RefreshDataMessage(SelectedItem.Data));
            }
        }

        public void ExportToPackage()
        {
            SelectedItem.Data.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Data);
            ImportExport.ExportToPackage(SelectedItem.Data);
        }

        public void ExportToFile()
        {
            SelectedItem.Data.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Data);
            Analysis.Export(SelectedItem.Data).Invoke();
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
                Content = Pgm.GetContentForView(SelectedItem.Data)
            };
            DataContext.Project.InsertPgm(pgm);
            if (SelectedItem.Incident != null)
            {
                PgmLink pgmLink = DataContext.PgmLinks.Create();
                pgmLink.PgmId = pgm.PgmId;
                pgmLink.IncidentId = SelectedItem.IncidentId;
                DataContext.PgmLinks.Save(pgmLink);
            }
            Messenger.Default.Send(new RefreshListMessage<Pgm>(SelectedItem.IncidentId));
            PgmModel.Active = false;
            SelectedItem.Data.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Data);
            Analysis.OpenPgm(DataContext.Project, pgm, true, SelectedItem.IncidentId).Invoke();
        }

        public void CreateCanvas()
        {
            Canvas canvas = new Canvas
            {
                Name = CanvasModel.Name,
                Content = Canvas.GetContentForView(SelectedItem.Data)
            };
            DataContext.Project.InsertCanvas(canvas);
            if (SelectedItem.Incident != null)
            {
                CanvasLink canvasLink = DataContext.CanvasLinks.Create();
                canvasLink.CanvasId = canvas.CanvasId;
                canvasLink.IncidentId = SelectedItem.IncidentId;
                DataContext.CanvasLinks.Save(canvasLink);
            }
            Messenger.Default.Send(new RefreshListMessage<Canvas>(SelectedItem.IncidentId));
            CanvasModel.Active = false;
            SelectedItem.Data.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Data);
            AnalysisDashboard.OpenCanvas(DataContext.Project, canvas, SelectedItem.IncidentId).Invoke();
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
            if (Incident == null || StringExtensions.EqualsIgnoreCase(msg.IncidentId, IncidentId))
            {
                Refresh();
            }
        }
    }
}
