using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analysis;
using ERHMS.EpiInfo.AnalysisDashboard;
using ERHMS.EpiInfo.Enter;
using ERHMS.EpiInfo.ImportExport;
using ERHMS.EpiInfo.MakeView;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class FormListViewModel : DocumentViewModel
    {
        private static readonly ICollection<Func<View, string>> FilterPropertyAccessors = new Func<View, string>[]
        {
            view => view.Name
        };

        public Incident Incident { get; private set; }

        public string IncidentId
        {
            get { return Incident == null ? null : Incident.IncidentId; }
        }

        private string filter;
        public string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                if (!Set(() => Filter, ref filter, value))
                {
                    return;
                }
                Views.Refresh();
            }
        }

        private ICollectionView views;
        public ICollectionView Views
        {
            get { return views; }
            set { Set(() => Views, ref views, value); }
        }

        private View selectedView;
        public View SelectedView
        {
            get
            {
                return selectedView;
            }
            set
            {
                if (!Set(() => SelectedView, ref selectedView, value))
                {
                    return;
                }
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
            }
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

        public FormListViewModel(Incident incident)
        {
            if (incident == null)
            {
                Title = "Forms";
            }
            else
            {
                Title = string.Format("{0} Forms", incident.Name);
            }
            Incident = incident;
            Refresh();
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasSelectedView);
            DeleteCommand = new RelayCommand(Delete, HasSelectedView);
            EnterDataCommand = new RelayCommand(EnterData, HasSelectedView);
            ViewDataCommand = new RelayCommand(ViewData, HasSelectedView);
            PublishToTemplateCommand = new RelayCommand(PublishToTemplate, HasSelectedView);
            PublishToWebCommand = new RelayCommand(PublishToWeb, HasSelectedView);
            PublishToMobileCommand = new RelayCommand(PublishToMobile, HasSelectedView);
            ImportFromProjectCommand = new RelayCommand(ImportFromProject, HasSelectedView);
            ImportFromPackageCommand = new RelayCommand(ImportFromPackage, HasSelectedView);
            ImportFromFileCommand = new RelayCommand(ImportFromFile, HasSelectedView);
            ImportFromWebCommand = new RelayCommand(ImportFromWeb, HasSelectedView);
            ImportFromMobileCommand = new RelayCommand(ImportFromMobile, HasSelectedView);
            ExportToPackageCommand = new RelayCommand(ExportToPackage, HasSelectedView);
            ExportToFileCommand = new RelayCommand(ExportToFile, HasSelectedView);
            AnalyzeClassicCommand = new RelayCommand(AnalyzeClassic, HasSelectedView);
            CreatePgmCommand = new RelayCommand(CreatePgm, HasPgmName);
            CancelPgmCommand = new RelayCommand(CancelPgm);
            AnalyzeVisualCommand = new RelayCommand(AnalyzeVisual, HasSelectedView);
            CreateCanvasCommand = new RelayCommand(CreateCanvas, HasCanvasName);
            CancelCanvasCommand = new RelayCommand(CancelCanvas);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<View>>(this, OnRefreshMessage);
        }

        public bool HasSelectedView()
        {
            return SelectedView != null;
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
            MakeView.OpenView(SelectedView);
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
                ViewLink viewLink = DataContext.ViewLinks.SelectByViewId(SelectedView.Id);
                if (viewLink != null)
                {
                    DataContext.ViewLinks.Delete(viewLink);
                }
                DataContext.Project.DeleteView(SelectedView.Id);
                Messenger.Default.Send(new RefreshMessage<View>(IncidentId));
            };
            Messenger.Default.Send(msg);
        }

        public void EnterData()
        {
            Enter.OpenView(SelectedView);
        }

        public void ViewData()
        {
            // TODO: Implement
        }

        public void PublishToTemplate()
        {
            MakeView.CreateTemplate(SelectedView);
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
            if (ImportExport.ImportFromView(SelectedView))
            {
                App.Current.Service.OnViewDataImported(SelectedView.Project.FilePath, SelectedView.Name);
            }
        }

        public void ImportFromPackage()
        {
            if (ImportExport.ImportFromPackage(SelectedView))
            {
                App.Current.Service.OnViewDataImported(SelectedView.Project.FilePath, SelectedView.Name);
            }
        }

        public void ImportFromFile()
        {
            Analysis.Import(SelectedView);
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
            ImportExport.ExportToPackage(SelectedView);
        }

        public void ExportToFile()
        {
            Analysis.Export(SelectedView);
        }

        public void AnalyzeClassic()
        {
            CreatingPgm = true;
        }

        public void CreatePgm()
        {
            Pgm pgm = new Pgm
            {
                Name = PgmName,
                Content = Pgm.GetContentForView(SelectedView)
            };
            DataContext.Project.InsertPgm(pgm);
            if (Incident != null)
            {
                PgmLink pgmLink = DataContext.PgmLinks.Create();
                pgmLink.PgmId = pgm.PgmId;
                pgmLink.IncidentId = IncidentId;
                DataContext.PgmLinks.Save(pgmLink);
            }
            Messenger.Default.Send(new RefreshMessage<Pgm>(IncidentId));
            CreatingPgm = false;
            PgmName = "";
            Analysis.OpenPgm(pgm);
        }

        public void CancelPgm()
        {
            CreatingPgm = false;
            PgmName = "";
        }

        public void AnalyzeVisual()
        {
            CreatingCanvas = true;
        }

        public void CreateCanvas()
        {
            Canvas canvas = new Canvas
            {
                Name = CanvasName,
                Content = Canvas.GetContentForView(SelectedView)
            };
            DataContext.Project.InsertCanvas(canvas);
            if (Incident != null)
            {
                CanvasLink canvasLink = DataContext.CanvasLinks.Create();
                canvasLink.CanvasId = canvas.CanvasId;
                canvasLink.IncidentId = IncidentId;
                DataContext.CanvasLinks.Save(canvasLink);
            }
            Messenger.Default.Send(new RefreshMessage<Canvas>(IncidentId));
            CreatingCanvas = false;
            CanvasName = "";
            AnalysisDashboard.OpenCanvas(DataContext.Project, canvas, IncidentId);
        }

        public void CancelCanvas()
        {
            CreatingCanvas = false;
            CanvasName = "";
        }

        public void Refresh()
        {
            Views = CollectionViewSource.GetDefaultView(DataContext.GetLinkedViews(IncidentId).OrderBy(view => view.Name));
            Views.Filter = MatchesFilter;
        }

        private bool MatchesFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(Filter))
            {
                return true;
            }
            View view = (View)item;
            foreach (Func<View, string> accessor in FilterPropertyAccessors)
            {
                string property = accessor(view);
                if (property != null && property.ContainsIgnoreCase(Filter))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnRefreshMessage(RefreshMessage<View> msg)
        {
            if (msg.IncidentId == IncidentId)
            {
                Refresh();
            }
        }
    }
}
