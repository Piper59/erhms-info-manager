using Epi.Fields;
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

namespace ERHMS.Presentation.ViewModels
{
    public class ViewListViewModel : ListViewModel<View>
    {
        public Incident Incident { get; private set; }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand LinkCommand { get; private set; }
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

        public ViewListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Forms";
            Incident = incident;
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasNonSystemSelectedItem);
            LinkCommand = new RelayCommand(Link, HasNonSystemSelectedItem);
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
            Refresh();
            SelectionChanged += (sender, e) =>
            {
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                LinkCommand.RaiseCanExecuteChanged();
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
            Dialogs.InvokeAsync(MakeView.OpenView.Create(Context.Project.FilePath, SelectedItem.Name));
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
            Dialogs.ShowAsync(new ViewLinkViewModel(Services, Context.Views.SelectById(SelectedItem.ViewId)));
        }

        public void EnterData()
        {
            View view = Context.Views.SelectById(SelectedItem.ViewId);
            if (view.HasResponderIdField)
            {
                Dialogs.ShowAsync(new PrepopulateViewModel(Services, view));
            }
            else
            {
                Context.Project.CollectedData.EnsureDataTablesExist(view.ViewId);
                Dialogs.InvokeAsync(Enter.OpenNewRecord.Create(Context.Project.FilePath, view.Name));
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
            Dialogs.InvokeAsync(wrapper);
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
                foreach (Field field in fields)
                {
                    message.AppendFormat("{0} ({1})", field.Name, field.FieldType);
                    message.AppendLine();
                }
                MessengerInstance.Send(new AlertMessage
                {
                    Message = message.ToString().Trim()
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
            Dialogs.InvokeAsync(Analysis.Import.Create(Context.Project.FilePath, SelectedItem.Name));
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
            bool unlinked = false;
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
                ILookup<string, Responder> responders = Context.Responders.Select()
                    .ToLookup(responder => responder.EmailAddress, StringComparer.OrdinalIgnoreCase);
                ViewEntityRepository<ViewEntity> entities = new ViewEntityRepository<ViewEntity>(Context, view);
                try
                {
                    foreach (Record record in Service.GetRecords(survey))
                    {
                        if (record.ContainsKey("ResponderID") && record.ContainsKey("ResponderEmailAddress"))
                        {
                            Responder responder = responders[record["ResponderEmailAddress"]].FirstOrDefault();
                            if (responder == null)
                            {
                                unlinked = true;
                            }
                            else
                            {
                                record["ResponderID"] = responder.ResponderId;
                            }
                        }
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
                    Log.Logger.Warn("Failed to import data from web", exception);
                    Documents.ShowSettings("Failed to import data from web.", exception);
                }
                else
                {
                    MessengerInstance.Send(new ToastMessage
                    {
                        Message = "Data has been imported from web."
                    });
                    if (unlinked)
                    {
                        ShowUnlinkedMessage(view);
                    }
                }
            };
            MessengerInstance.Send(msg);
        }

        private void ShowUnlinkedMessage(Epi.View view)
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Review",
                Message = "One or more records could not be linked to a responder. Review these records?"
            };
            msg.Confirmed += (sender, e) =>
            {
                Documents.ShowRecords(view);
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
            Dialogs.InvokeAsync(Analysis.Export.Create(Context.Project.FilePath, SelectedItem.Name));
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
