using Epi;
using Epi.Fields;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Web;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewListViewModel : ListViewModelBase<DeepLink<View>>
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
        public RelayCommand RefreshCommand { get; private set; }

        public ViewListViewModel(Incident incident)
        {
            Title = "Forms";
            Incident = incident;
            Refresh();
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasOneSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasNonSystemSelectedItem);
            LinkCommand = new RelayCommand(Link, HasNonSystemSelectedItem);
            EnterDataCommand = new RelayCommand(EnterData, HasOneSelectedItem);
            ViewDataCommand = new RelayCommand(ViewData, HasOneSelectedItem);
            PublishToTemplateCommand = new RelayCommand(PublishToTemplate, HasOneSelectedItem);
            PublishToWebCommand = new RelayCommand(PublishToWeb, HasOneSelectedItem);
            PublishToMobileCommand = new RelayCommand(PublishToMobile, HasOneSelectedItem);
            ImportFromProjectCommand = new RelayCommand(ImportFromProject, HasOneSelectedItem);
            ImportFromPackageCommand = new RelayCommand(ImportFromPackage, HasOneSelectedItem);
            ImportFromFileCommand = new RelayCommand(ImportFromFile, HasOneSelectedItem);
            ImportFromWebCommand = new RelayCommand(ImportFromWeb, HasOneSelectedItem);
            ImportFromMobileCommand = new RelayCommand(ImportFromMobile, HasOneSelectedItem);
            ExportToPackageCommand = new RelayCommand(ExportToPackage, HasOneSelectedItem);
            ExportToFileCommand = new RelayCommand(ExportToFile, HasOneSelectedItem);
            AnalyzeClassicCommand = new RelayCommand(AnalyzeClassic, HasOneSelectedItem);
            AnalyzeVisualCommand = new RelayCommand(AnalyzeVisual, HasOneSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            SelectedItemChanged += (sender, e) =>
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
            Messenger.Default.Register<RefreshMessage<View>>(this, msg => Refresh());
            Messenger.Default.Register<RefreshMessage<Incident>>(this, msg => Refresh());
        }

        protected override IEnumerable<DeepLink<View>> GetItems()
        {
            IEnumerable<DeepLink<View>> items;
            if (Incident == null)
            {
                items = DataContext.ViewLinks.SelectDeepLinks();
            }
            else
            {
                items = DataContext.ViewLinks.SelectDeepLinksByIncidentId(Incident.IncidentId);
            }
            return items.OrderBy(item => item.Item.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(DeepLink<View> item)
        {
            yield return item.Item.Name;
            if (Incident == null)
            {
                yield return item.Incident?.Name;
            }
        }

        public bool HasNonSystemSelectedItem()
        {
            return HasOneSelectedItem() && !DataContext.IsResponderView(SelectedItem.Item);
        }

        public void Create()
        {
            TemplateInfo template = DataContext.CreateNewViewTemplate();
            string namePrefix = Incident == null ? null : Incident.Name + "_";
            Wrapper wrapper = MakeView.InstantiateViewTemplate.Create(DataContext.Project.FilePath, template.Path, namePrefix);
            wrapper.Event += (sender, e) =>
            {
                if (e.Type == WrapperEventType.ViewCreated)
                {
                    if (Incident != null)
                    {
                        ViewLink viewLink = DataContext.ViewLinks.Create();
                        viewLink.ViewId = e.Properties.Id;
                        viewLink.IncidentId = Incident.IncidentId;
                        DataContext.ViewLinks.Save(viewLink);
                    }
                    Messenger.Default.Send(new RefreshMessage<View>());
                }
            };
            wrapper.Invoke();
        }

        public void Edit()
        {
            MakeView.OpenView.Create(DataContext.Project.FilePath, SelectedItem.Item.Name).Invoke();
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
                DataContext.Assignments.DeleteByViewId(SelectedItem.Item.Id);
                DataContext.ViewLinks.DeleteByViewId(SelectedItem.Item.Id);
                DataContext.WebSurveys.DeleteByViewId(SelectedItem.Item.Id);
                DataContext.Project.DeleteView(SelectedItem.Item);
                Messenger.Default.Send(new RefreshMessage<View>());
            };
            Messenger.Default.Send(msg);
        }

        public void Link()
        {
            Messenger.Default.Send(new ShowMessage
            {
                ViewModel = new ViewLinkViewModel(SelectedItem)
                {
                    Active = true
                }
            });
        }

        public void EnterData()
        {
            if (DataContext.IsResponderLinkedView(SelectedItem.Item))
            {
                Messenger.Default.Send(new ShowMessage
                {
                    ViewModel = new PrepopulateViewModel(SelectedItem)
                    {
                        Active = true
                    }
                });
            }
            else
            {
                DataContext.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Item);
                Enter.OpenNewRecord.Create(DataContext.Project.FilePath, SelectedItem.Item.Name).Invoke();
            }
        }

        public void ViewData()
        {
            Main.OpenRecordListView(SelectedItem.Item);
        }

        public void PublishToTemplate()
        {
            Wrapper wrapper = MakeView.CreateTemplate.Create(DataContext.Project.FilePath, SelectedItem.Item.Name);
            wrapper.Event += (sender, e) =>
            {
                if (e.Type == WrapperEventType.TemplateCreated)
                {
                    Messenger.Default.Send(new RefreshMessage<TemplateInfo>());
                }
            };
            wrapper.Invoke();
        }

        private void ShowUnsupportedMessage(string message, IEnumerable<Field> fields)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(message);
            builder.AppendLine();
            builder.Append(string.Join(", ", fields.Select(field => string.Format("{0} ({1})", field.Name, field.FieldType))));
            Messenger.Default.Send(new AlertMessage
            {
                Message = builder.ToString()
            });
        }

        public void PublishToWeb()
        {
            ICollection<Field> fields = SelectedItem.Item.Fields.Cast<Field>()
                .Where(field => !field.FieldType.IsWebSupported())
                .ToList();
            if (fields.Count > 0)
            {
                ShowUnsupportedMessage("The following fields are not supported for publication to web:", fields);
            }
            else
            {
                ConfigurationError error;
                if (Service.IsConfigured(out error, true))
                {
                    SurveyViewModel survey = new SurveyViewModel(SelectedItem.Item);
                    survey.Activate();
                    Messenger.Default.Send(new ShowMessage
                    {
                        ViewModel = survey
                    });
                }
                else
                {
                    SurveyViewModel.RequestConfiguration(error);
                }
            }
        }

        public void PublishToMobile()
        {
            ICollection<Field> fields = SelectedItem.Item.Fields.Cast<Field>()
                .Where(field => !field.FieldType.IsMobileSupported())
                .ToList();
            if (fields.Count > 0)
            {
                ShowUnsupportedMessage("The following fields are not supported for publication to mobile:", fields);
            }
            else
            {
                MakeView.PublishToMobile.Create(DataContext.Project.FilePath, SelectedItem.Item.Name).Invoke();
            }
        }

        public void ImportFromProject()
        {
            DataContext.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Item);
            ImportExport.ImportFromView(SelectedItem.Item);
        }

        public void ImportFromPackage()
        {
            DataContext.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Item);
            ImportExport.ImportFromPackage(SelectedItem.Item);
        }

        public void ImportFromFile()
        {
            DataContext.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Item);
            Analysis.Import.Create(DataContext.Project.FilePath, SelectedItem.Item.Name).Invoke();
        }

        public void ImportFromWeb()
        {
            DataContext.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Item);
            if (!SelectedItem.Item.IsWebSurvey())
            {
                Messenger.Default.Send(new AlertMessage
                {
                    Message = "Form has not been published to web."
                });
                return;
            }
            ConfigurationError error = ConfigurationError.None;
            Survey survey = null;
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
                survey = Service.GetSurvey(SelectedItem.Item);
                if (survey == null)
                {
                    return;
                }
                ViewEntityRepository<ViewEntity> entities = new ViewEntityRepository<ViewEntity>(DataContext.Driver, SelectedItem.Item);
                try
                {
                    foreach (Record record in Service.GetRecords(survey))
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
                }
                catch (Exception ex)
                {
                    Log.Logger.Warn("Failed to import data from web", ex);
                }
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
                    Messenger.Default.Send(new ToastMessage
                    {
                        Message = "Data has been imported from web."
                    });
                }
            };
            Messenger.Default.Send(msg);
        }

        public void ImportFromMobile()
        {
            DataContext.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Item);
            ImportExport.ImportFromMobile(SelectedItem.Item);
        }

        public void ExportToPackage()
        {
            DataContext.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Item);
            ImportExport.ExportToPackage(SelectedItem.Item);
        }

        public void ExportToFile()
        {
            DataContext.Project.CollectedData.EnsureDataTablesExist(SelectedItem.Item);
            Analysis.Export.Create(DataContext.Project.FilePath, SelectedItem.Item.Name).Invoke();
        }

        public void AnalyzeClassic()
        {
            Messenger.Default.Send(new ShowMessage
            {
                ViewModel = new PgmViewModel(SelectedItem)
                {
                    Active = true
                }
            });
        }

        public void AnalyzeVisual()
        {
            Messenger.Default.Send(new ShowMessage
            {
                ViewModel = new CanvasViewModel(SelectedItem)
                {
                    Active = true
                }
            });
        }
    }
}
