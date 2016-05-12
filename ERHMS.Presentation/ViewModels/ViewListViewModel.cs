using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analysis;
using ERHMS.EpiInfo.AnalysisDashboard;
using ERHMS.EpiInfo.Enter;
using ERHMS.EpiInfo.ImportExport;
using ERHMS.EpiInfo.MakeView;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewListViewModel : ListViewModelBase<View>
    {
        public Incident Incident { get; private set; }

        public string IncidentId
        {
            get { return Incident == null ? null : Incident.IncidentId; }
        }

        private bool creatingPgm;
        public bool CreatingPgm
        {
            get { return creatingPgm; }
            set { Set(() => CreatingPgm, ref creatingPgm, value); }
        }

        private bool creatingCanvas;
        public bool CreatingCanvas
        {
            get { return creatingCanvas; }
            set { Set(() => CreatingCanvas, ref creatingCanvas, value); }
        }

        private string pgmName;
        public string PgmName
        {
            get
            {
                return pgmName;
            }
            set
            {
                if (Set(() => PgmName, ref pgmName, value))
                {
                    CreatePgmCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string canvasName;
        public string CanvasName
        {
            get
            {
                return canvasName;
            }
            set
            {
                if (Set(() => CanvasName, ref canvasName, value))
                {
                    CreateCanvasCommand.RaiseCanExecuteChanged();
                }
            }
        }

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
        public RelayCommand CreatePgmCommand { get; private set; }
        public RelayCommand CancelPgmCommand { get; private set; }
        public RelayCommand AnalyzeVisualCommand { get; private set; }
        public RelayCommand CreateCanvasCommand { get; private set; }
        public RelayCommand CancelCanvasCommand { get; private set; }
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
            CreatePgmCommand = new RelayCommand(CreatePgm, HasPgmName);
            CancelPgmCommand = new RelayCommand(CancelPgm);
            AnalyzeVisualCommand = new RelayCommand(AnalyzeVisual, HasSelectedItem);
            CreateCanvasCommand = new RelayCommand(CreateCanvas, HasCanvasName);
            CancelCanvasCommand = new RelayCommand(CancelCanvas);
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

        public bool HasPgmName()
        {
            return !string.IsNullOrWhiteSpace(PgmName);
        }

        public bool HasCanvasName()
        {
            return !string.IsNullOrWhiteSpace(CanvasName);
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
                DataContext.Project.DeleteView(SelectedItem.Id);
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
            // TODO: Implement
        }

        public void PublishToMobile()
        {
            // TODO: Implement
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
            // TODO: Implement
        }

        public void ImportFromMobile()
        {
            // TODO: Implement
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
            PgmName = "";
            CreatingPgm = true;
        }

        public void AnalyzeVisual()
        {
            CanvasName = "";
            CreatingCanvas = true;
        }

        public void CreatePgm()
        {
            Pgm pgm = new Pgm
            {
                Name = PgmName,
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
            CreatingPgm = false;
            Analysis.OpenPgm(pgm, true);
        }

        public void CreateCanvas()
        {
            Canvas canvas = new Canvas
            {
                Name = CanvasName,
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
            CreatingCanvas = false;
            AnalysisDashboard.OpenCanvas(DataContext.Project, canvas, IncidentId);
        }

        public void CancelPgm()
        {
            CreatingPgm = false;
        }

        public void CancelCanvas()
        {
            CreatingCanvas = false;
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
            if (msg.IncidentId.Equals(IncidentId, StringComparison.OrdinalIgnoreCase))
            {
                Refresh();
            }
        }
    }
}
