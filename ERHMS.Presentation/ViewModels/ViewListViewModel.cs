﻿using Epi.Fields;
using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Web;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewListViewModel : ListViewModel<View>
    {
        public Incident Incident { get; private set; }

        private RelayCommand createCommand;
        public ICommand CreateCommand
        {
            get { return createCommand ?? (createCommand = new RelayCommand(Create, HasSelectedItem)); }
        }

        private RelayCommand editCommand;
        public ICommand EditCommand
        {
            get { return editCommand ?? (editCommand = new RelayCommand(Edit, HasSelectedItem)); }
        }

        private RelayCommand deleteCommand;
        public ICommand DeleteCommand
        {
            get { return deleteCommand ?? (deleteCommand = new RelayCommand(Delete, HasNonSystemSelectedItem)); }
        }

        private RelayCommand linkCommand;
        public ICommand LinkCommand
        {
            get { return linkCommand ?? (linkCommand = new RelayCommand(Link, HasNonSystemSelectedItem)); }
        }

        private RelayCommand enterDataCommand;
        public ICommand EnterDataCommand
        {
            get { return enterDataCommand ?? (enterDataCommand = new RelayCommand(EnterData, HasSelectedItem)); }
        }

        private RelayCommand viewDataCommand;
        public ICommand ViewDataCommand
        {
            get { return viewDataCommand ?? (viewDataCommand = new RelayCommand(ViewData, HasSelectedItem)); }
        }

        private RelayCommand publishToTemplateCommand;
        public ICommand PublishToTemplateCommand
        {
            get { return publishToTemplateCommand ?? (publishToTemplateCommand = new RelayCommand(PublishToTemplate, HasSelectedItem)); }
        }

        private RelayCommand publishToWebCommand;
        public ICommand PublishToWebCommand
        {
            get { return publishToWebCommand ?? (publishToWebCommand = new RelayCommand(PublishToWeb, HasSelectedItem)); }
        }

        private RelayCommand publishToMobileCommand;
        public ICommand PublishToMobileCommand
        {
            get { return publishToMobileCommand ?? (publishToMobileCommand = new RelayCommand(PublishToMobile, HasSelectedItem)); }
        }

        private RelayCommand importFromProjectCommand;
        public ICommand ImportFromProjectCommand
        {
            get { return importFromProjectCommand ?? (importFromProjectCommand = new RelayCommand(ImportFromProject, HasSelectedItem)); }
        }

        private RelayCommand importFromPackageCommand;
        public ICommand ImportFromPackageCommand
        {
            get { return importFromPackageCommand ?? (importFromPackageCommand = new RelayCommand(ImportFromPackage, HasSelectedItem)); }
        }

        private RelayCommand importFromFileCommand;
        public ICommand ImportFromFileCommand
        {
            get { return importFromFileCommand ?? (importFromFileCommand = new RelayCommand(ImportFromFile, HasSelectedItem)); }
        }

        private RelayCommand importFromWebCommand;
        public ICommand ImportFromWebCommand
        {
            get { return importFromWebCommand ?? (importFromWebCommand = new RelayCommand(ImportFromWeb, HasSelectedItem)); }
        }

        private RelayCommand importFromMobileCommand;
        public ICommand ImportFromMobileCommand
        {
            get { return importFromMobileCommand ?? (importFromMobileCommand = new RelayCommand(ImportFromMobile, HasSelectedItem)); }
        }

        private RelayCommand exportToPackageCommand;
        public ICommand ExportToPackageCommand
        {
            get { return exportToPackageCommand ?? (exportToPackageCommand = new RelayCommand(ExportToPackage, HasSelectedItem)); }
        }

        private RelayCommand exportToFileCommand;
        public ICommand ExportToFileCommand
        {
            get { return exportToFileCommand ?? (exportToFileCommand = new RelayCommand(ExportToFile, HasSelectedItem)); }
        }

        private RelayCommand analyzeClassicCommand;
        public ICommand AnalyzeClassicCommand
        {
            get { return analyzeClassicCommand ?? (analyzeClassicCommand = new RelayCommand(AnalyzeClassic, HasSelectedItem)); }
        }

        private RelayCommand analyzeVisualCommand;
        public ICommand AnalyzeVisualCommand
        {
            get { return analyzeVisualCommand ?? (analyzeVisualCommand = new RelayCommand(AnalyzeVisual, HasSelectedItem)); }
        }

        public ViewListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Forms";
            Incident = incident;
            SelectionChanged += (sender, e) =>
            {
                editCommand.RaiseCanExecuteChanged();
                deleteCommand.RaiseCanExecuteChanged();
                linkCommand.RaiseCanExecuteChanged();
                enterDataCommand.RaiseCanExecuteChanged();
                viewDataCommand.RaiseCanExecuteChanged();
                publishToTemplateCommand.RaiseCanExecuteChanged();
                publishToWebCommand.RaiseCanExecuteChanged();
                publishToMobileCommand.RaiseCanExecuteChanged();
                importFromProjectCommand.RaiseCanExecuteChanged();
                importFromPackageCommand.RaiseCanExecuteChanged();
                importFromFileCommand.RaiseCanExecuteChanged();
                importFromWebCommand.RaiseCanExecuteChanged();
                importFromMobileCommand.RaiseCanExecuteChanged();
                exportToPackageCommand.RaiseCanExecuteChanged();
                exportToFileCommand.RaiseCanExecuteChanged();
                analyzeClassicCommand.RaiseCanExecuteChanged();
                analyzeVisualCommand.RaiseCanExecuteChanged();
            };
            Refresh();
        }

        public bool HasNonSystemSelectedItem()
        {
            return HasSelectedItem() && SelectedItem.Name != Context.Responders.View.Name;
        }

        protected override IEnumerable<View> GetItems()
        {
            IEnumerable<View> views;
            if (Incident == null)
            {
                views = Context.Views.SelectUndeleted();
            }
            else
            {
                views = Context.Views.SelectByIncidentId(Incident.IncidentId);
            }
            return views.OrderBy(view => view.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(View item)
        {
            yield return item.Name;
            yield return item.Incident?.Name;
        }

        public void Create()
        {
            TemplateListViewModel.Create(Services, DataContext.GetNewViewTemplate(), Incident);
        }

        public void Edit()
        {
            MakeView.OpenView.Create(Context.Project.FilePath, SelectedItem.Name).Invoke();
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Delete",
                Message = "Delete the selected form?"
            };
            msg.Confirmed += (sender, e) =>
            {
                Context.Assignments.DeleteByViewId(SelectedItem.ViewId);
                Context.ViewLinks.DeleteByViewId(SelectedItem.ViewId);
                Context.WebSurveys.DeleteByViewId(SelectedItem.ViewId);
                Context.Project.DeleteView(SelectedItem.ViewId);
                MessengerInstance.Send(new RefreshMessage(typeof(View)));
            };
            MessengerInstance.Send(msg);
        }

        public void Link()
        {
            Dialogs.ShowAsync(new ViewLinkViewModel(Services, SelectedItem));
        }

        public void EnterData()
        {
            if (SelectedItem.HasResponderIdField)
            {
                Dialogs.ShowAsync(new PrepopulateViewModel(Services, SelectedItem));
            }
            else
            {
                Context.Project.CollectedData.EnsureDataTablesExist(SelectedItem.ViewId);
                Enter.OpenNewRecord.Create(Context.Project.FilePath, SelectedItem.Name).Invoke();
            }
        }

        public void ViewData()
        {
            Context.Project.CollectedData.EnsureDataTablesExist(SelectedItem.ViewId);
            Documents.ShowRecords(Context.Project.GetViewById(SelectedItem.ViewId));
        }

        public void PublishToTemplate()
        {
            Wrapper wrapper = MakeView.CreateTemplate.Create(Context.Project.FilePath, SelectedItem.Name);
            wrapper.Event += (sender, e) =>
            {
                if (e.Type == WrapperEventType.TemplateCreated)
                {
                    MessengerInstance.Send(new RefreshMessage(typeof(TemplateInfo)));
                }
            };
            wrapper.Invoke();
        }

        private bool Validate(string target, Epi.View view, Func<Field, bool> unsupported)
        {
            ICollection<Field> fields = view.Fields.Cast<Field>()
                .Where(unsupported)
                .ToList();
            if (fields.Count > 0)
            {
                StringBuilder message = new StringBuilder();
                message.AppendFormat("The following fields are unsupported for publication to {0}:", target);
                message.AppendLine();
                message.AppendLine();
                message.Append(string.Join(", ", fields.Select(field => string.Format("{0} ({1})", field.Name, field.FieldType))));
                MessengerInstance.Send(new AlertMessage
                {
                    Message = message.ToString()
                });
                return false;
            }
            return true;
        }

        public void PublishToWeb()
        {
            Epi.View view = Context.Project.GetViewById(SelectedItem.ViewId);
            if (!Validate("web", view, field => !field.FieldType.IsWebSupported()))
            {
                return;
            }
            ConfigurationError error;
            if (Service.IsConfigured(out error, true))
            {
                SurveyViewModel survey = new SurveyViewModel(Services, view);
                survey.Open();
            }
            else
            {
                Documents.ShowSettings(SurveyViewModel.GetErrorMessage(error));
            }
        }

        public void PublishToMobile()
        {
            Epi.View view = Context.Project.GetViewById(SelectedItem.ViewId);
            if (!Validate("mobile", view, field => !field.FieldType.IsMobileSupported()))
            {
                return;
            }
            MakeView.PublishToMobile.Create(Context.Project.FilePath, SelectedItem.Name).Invoke();
        }

        public void ImportFromProject()
        {
            Context.Project.CollectedData.EnsureDataTablesExist(SelectedItem.ViewId);
            ImportExport.ImportFromView(Dialogs.Win32Window, Context.Project.GetViewById(SelectedItem.ViewId));
        }

        public void ImportFromPackage()
        {
            Context.Project.CollectedData.EnsureDataTablesExist(SelectedItem.ViewId);
            ImportExport.ImportFromPackage(Dialogs.Win32Window, Context.Project.GetViewById(SelectedItem.ViewId));
        }

        public void ImportFromFile()
        {
            Context.Project.CollectedData.EnsureDataTablesExist(SelectedItem.ViewId);
            Analysis.Import.Create(Context.Project.FilePath, SelectedItem.Name).Invoke();
        }

        public void ImportFromWeb()
        {
            Epi.View view = Context.Project.GetViewById(SelectedItem.ViewId);
            if (!view.IsWebSurvey())
            {
                MessengerInstance.Send(new AlertMessage
                {
                    Message = "Form has not been published to web."
                });
                return;
            }
            Context.Project.CollectedData.EnsureDataTablesExist(SelectedItem.ViewId);
            ConfigurationError error = ConfigurationError.None;
            if (!Service.IsConfigured(out error, true))
            {
                Documents.ShowSettings(SurveyViewModel.GetErrorMessage(error));
                return;
            }
            Survey survey = null;
            Exception exception = null;
            bool success = false;
            BlockMessage msg = new BlockMessage
            {
                Message = "Importing data from web \u2026"
            };
            msg.Executing += (sender, e) =>
            {
                if (!Service.IsConfigured(out error))
                {
                    return;
                }
                survey = Service.GetSurvey(view.WebSurveyId);
                if (survey == null)
                {
                    return;
                }
                ViewEntityRepository<ViewEntity> entities = new ViewEntityRepository<ViewEntity>(Context, view);
                try
                {
                    foreach (Record record in Service.GetRecords(survey))
                    {
                        entities.Save(record);
                    }
                    success = true;
                }
                catch (Exception ex)
                {
                    Log.Logger.Warn("Failed to import data from web", ex);
                    exception = ex;
                }
            };
            msg.Executed += (sender, e) =>
            {
                if (error != ConfigurationError.None)
                {
                    Documents.ShowSettings(SurveyViewModel.GetErrorMessage(error));
                }
                else if (survey == null)
                {
                    Documents.ShowSettings(SurveyViewModel.GetErrorMessage("Failed to retrieve web survey details."));
                }
                else if (!success)
                {
                    Documents.ShowSettings("Failed to import data from web.", exception);
                }
                else
                {
                    MessengerInstance.Send(new ToastMessage
                    {
                        Message = "Data has been imported from web."
                    });
                }
            };
            MessengerInstance.Send(msg);
        }

        public void ImportFromMobile()
        {
            Context.Project.CollectedData.EnsureDataTablesExist(SelectedItem.ViewId);
            ImportExport.ImportFromMobile(Dialogs.Win32Window, Context.Project.GetViewById(SelectedItem.ViewId));
        }

        public void ExportToPackage()
        {
            Context.Project.CollectedData.EnsureDataTablesExist(SelectedItem.ViewId);
            ImportExport.ExportToPackage(Dialogs.Win32Window, Context.Project.GetViewById(SelectedItem.ViewId));
        }

        public void ExportToFile()
        {
            Context.Project.CollectedData.EnsureDataTablesExist(SelectedItem.ViewId);
            Analysis.Export.Create(Context.Project.FilePath, SelectedItem.Name).Invoke();
        }

        public void AnalyzeClassic()
        {
            Dialogs.ShowAsync(new PgmViewModel(Services, SelectedItem));
        }

        public void AnalyzeVisual()
        {
            Dialogs.ShowAsync(new CanvasViewModel(Services, SelectedItem));
        }
    }
}
