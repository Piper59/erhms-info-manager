﻿using Epi;
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
using System.Reflection;
using System.Windows.Data;

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
            DeleteCommand = new RelayCommand(Delete, HasSelectedItem);
            IncidentCommand = new RelayCommand(Link, HasSelectedItem);
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

        public void Create()
        {
            string prefix = Incident == null ? null : Incident.Name;
            EpiInfo.Template template = EpiInfo.Template.GetFromResource(Assembly.GetAssembly(typeof(Responder)), "ERHMS.Domain.Templates.Forms.Empty.xml");
            MakeView.InstantiateTemplate(DataContext.Project, template, prefix, IncidentId);
        }

        public void Edit()
        {
            MakeView.OpenView(SelectedItem.Data);
        }

        public void Delete()
        {
            if (DataContext.IsResponderView(SelectedItem.Data))
            {
                Messenger.Default.Send(new NotifyMessage("The selected form cannot be deleted."));
            }
            else
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
                Enter.OpenView(SelectedItem.Data);
            }
        }

        public void ViewData()
        {
            SelectedItem.Data.CreateDataTables();
            Locator.Main.OpenRecordListView(SelectedItem.Data);
        }

        public void PublishToTemplate()
        {
            MakeView.CreateTemplate(SelectedItem.Data);
        }

        public void PublishToWeb()
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

        public void PublishToMobile()
        {
            SelectedItem.Data.CreateDataTables();
            MakeView.PublishToMobile(SelectedItem.Data);
        }

        public void ImportFromProject()
        {
            SelectedItem.Data.CreateDataTables();
            if (ImportExport.ImportFromView(SelectedItem.Data))
            {
                Messenger.Default.Send(new RefreshDataMessage(SelectedItem.Data));
            }
        }

        public void ImportFromPackage()
        {
            SelectedItem.Data.CreateDataTables();
            if (ImportExport.ImportFromPackage(SelectedItem.Data))
            {
                Messenger.Default.Send(new RefreshDataMessage(SelectedItem.Data));
            }
        }

        public void ImportFromFile()
        {
            SelectedItem.Data.CreateDataTables();
            Analysis.Import(SelectedItem.Data);
        }

        public void ImportFromWeb()
        {
            SelectedItem.Data.CreateDataTables();
            SurveyViewModel surveyModel = new SurveyViewModel(SelectedItem.Data);
            if (surveyModel.Import())
            {
                Messenger.Default.Send(new RefreshDataMessage(SelectedItem.Data));
            }
        }

        public void ImportFromMobile()
        {
            SelectedItem.Data.CreateDataTables();
            if (ImportExport.ImportFromMobile(SelectedItem.Data))
            {
                Messenger.Default.Send(new RefreshDataMessage(SelectedItem.Data));
            }
        }

        public void ExportToPackage()
        {
            SelectedItem.Data.CreateDataTables();
            ImportExport.ExportToPackage(SelectedItem.Data);
        }

        public void ExportToFile()
        {
            SelectedItem.Data.CreateDataTables();
            Analysis.Export(SelectedItem.Data);
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
            SelectedItem.Data.CreateDataTables();
            Analysis.OpenPgm(DataContext.Project, pgm, true, SelectedItem.IncidentId);
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
            SelectedItem.Data.CreateDataTables();
            AnalysisDashboard.OpenCanvas(DataContext.Project, canvas, SelectedItem.IncidentId);
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
