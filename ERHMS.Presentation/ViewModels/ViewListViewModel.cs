using Epi.Fields;
using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Web;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewListViewModel : DocumentViewModel
    {
        public class ViewListChildViewModel : ListViewModel<View>
        {
            public Incident Incident { get; private set; }

            public ViewListChildViewModel(Incident incident)
            {
                Incident = incident;
                Refresh();
            }

            protected override IEnumerable<View> GetItems()
            {
                IEnumerable<View> views = Incident == null
                    ? Context.Views.SelectUndeleted()
                    : Context.Views.SelectByIncidentId(Incident.IncidentId);
                return views.OrderBy(view => view.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(View item)
            {
                yield return item.Name;
                yield return item.Incident?.Name;
            }

            public bool HasNonSystemSelectedItem()
            {
                return HasSelectedItem() && SelectedItem.ViewId != Context.Responders.View.Id;
            }

            public void SelectById(int viewId)
            {
                SelectedObject = Items.FirstOrDefault(view => view.ViewId == viewId);
            }
        }

        public Incident Incident { get; private set; }
        public ViewListChildViewModel Views { get; private set; }

        public ICommand CreateCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand LinkCommand { get; private set; }
        public ICommand EnterDataCommand { get; private set; }
        public ICommand ViewDataCommand { get; private set; }
        public ICommand PublishToTemplateCommand { get; private set; }
        public ICommand PublishToWebCommand { get; private set; }
        public ICommand PublishToMobileCommand { get; private set; }
        public ICommand ImportFromProjectCommand { get; private set; }
        public ICommand ImportFromPackageCommand { get; private set; }
        public ICommand ImportFromFileCommand { get; private set; }
        public ICommand ImportFromWebCommand { get; private set; }
        public ICommand ImportFromMobileCommand { get; private set; }
        public ICommand ExportToPackageCommand { get; private set; }
        public ICommand ExportToFileCommand { get; private set; }
        public ICommand AnalyzeClassicCommand { get; private set; }
        public ICommand AnalyzeVisualCommand { get; private set; }

        public ViewListViewModel(Incident incident)
        {
            Title = "Forms";
            Incident = incident;
            Views = new ViewListChildViewModel(incident);
            CreateCommand = new AsyncCommand(CreateAsync);
            EditCommand = new AsyncCommand(EditAsync, Views.HasSelectedItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, Views.HasNonSystemSelectedItem);
            LinkCommand = new AsyncCommand(LinkAsync, Views.HasNonSystemSelectedItem);
            EnterDataCommand = new AsyncCommand(EnterDataAsync, Views.HasSelectedItem);
            ViewDataCommand = new Command(ViewData, Views.HasSelectedItem);
            PublishToTemplateCommand = new AsyncCommand(PublishToTemplateAsync, Views.HasSelectedItem);
            PublishToWebCommand = new AsyncCommand(PublishToWebAsync, Views.HasSelectedItem);
            PublishToMobileCommand = new AsyncCommand(PublishToMobileAsync, Views.HasSelectedItem);
            ImportFromProjectCommand = new Command(ImportFromProject, Views.HasSelectedItem);
            ImportFromPackageCommand = new Command(ImportFromPackage, Views.HasSelectedItem);
            ImportFromFileCommand = new AsyncCommand(ImportFromFileAsync, Views.HasSelectedItem);
            ImportFromWebCommand = new AsyncCommand(ImportFromWebAsync, Views.HasSelectedItem);
            ImportFromMobileCommand = new Command(ImportFromMobile, Views.HasSelectedItem);
            ExportToPackageCommand = new Command(ExportToPackage, Views.HasSelectedItem);
            ExportToFileCommand = new AsyncCommand(ExportToFileAsync, Views.HasSelectedItem);
            AnalyzeClassicCommand = new AsyncCommand(AnalyzeClassicAsync, Views.HasSelectedItem);
            AnalyzeVisualCommand = new AsyncCommand(AnalyzeVisualAsync, Views.HasSelectedItem);
        }

        public async Task CreateAsync()
        {
            string prefix = Incident == null ? "" : Incident.Name + "_";
            Wrapper wrapper = MakeView.InstantiateViewTemplate.Create(Context.Project.FilePath, DataContext.GetViewTemplate().FilePath, prefix);
            wrapper.AddViewCreatedHandler(Incident);
            await ServiceLocator.Wrapper.InvokeAsync(wrapper);
        }

        public async Task EditAsync()
        {
            Wrapper wrapper = MakeView.OpenView.Create(Context.Project.FilePath, Views.SelectedItem.Name);
            await ServiceLocator.Wrapper.InvokeAsync(wrapper);
        }

        public async Task DeleteAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.ViewConfirmDelete, "Delete"))
            {
                Context.Assignments.DeleteByViewId(Views.SelectedItem.ViewId);
                Context.ViewLinks.DeleteByViewId(Views.SelectedItem.ViewId);
                Context.WebSurveys.DeleteByViewId(Views.SelectedItem.ViewId);
                Context.Project.DeleteView(Views.SelectedItem.ViewId);
                ServiceLocator.Data.Refresh(typeof(View));
            };
        }

        public async Task LinkAsync()
        {
            ViewLinkViewModel model = new ViewLinkViewModel(Context.Views.Refresh(Views.SelectedItem));
            await ServiceLocator.Dialog.ShowAsync(model);
        }

        public async Task EnterDataAsync()
        {
            View view = Context.Views.Refresh(Views.SelectedItem);
            if (view.HasResponderIdField)
            {
                PrepopulateViewModel model = new PrepopulateViewModel(view);
                await ServiceLocator.Dialog.ShowAsync(model);
            }
            else
            {
                Context.Project.CollectedData.EnsureDataTablesExist(view.ViewId);
                Wrapper wrapper = Enter.OpenNewRecord.Create(Context.Project.FilePath, view.Name);
                wrapper.AddRecordSavedHandler();
                await ServiceLocator.Wrapper.InvokeAsync(wrapper);
            }
        }

        public void ViewData()
        {
            Context.Project.CollectedData.EnsureDataTablesExist(Views.SelectedItem.ViewId);
            ServiceLocator.Document.Show(
                model => model.View.Id == Views.SelectedItem.ViewId,
                () => new ViewEntityListViewModel(Context.Project.GetViewById(Views.SelectedItem.ViewId)));
        }

        public async Task PublishToTemplateAsync()
        {
            Wrapper wrapper = MakeView.CreateTemplate.Create(Context.Project.FilePath, Views.SelectedItem.Name);
            wrapper.Event += (sender, e) =>
            {
                if (e.Type != "TemplateCreated")
                {
                    return;
                }
                ServiceLocator.Dispatcher.Post(() =>
                {
                    ServiceLocator.Data.Refresh(typeof(TemplateInfo));
                    if (Incident == null)
                    {
                        TemplateListViewModel model = ServiceLocator.Document.ShowByType(() => new TemplateListViewModel(null));
                        model.Templates.SelectByPath(e.Properties.Path);
                    }
                    else
                    {
                        IncidentViewModel model = ServiceLocator.Document.Show(
                            _model => _model.Incident.Equals(Incident),
                            () => new IncidentViewModel(Incident));
                        model.Templates.Active = true;
                        model.Templates.Templates.SelectByPath(e.Properties.Path);
                    }
                });
            };
            await ServiceLocator.Wrapper.InvokeAsync(wrapper);
        }

        private async Task<bool> ValidateAsync(string target, Epi.View view, Func<Field, bool> supported)
        {
            ICollection<Field> fields = view.Fields.Cast<Field>()
                .Where(field => !supported(field))
                .ToList();
            if (fields.Count > 0)
            {
                IEnumerable<string> fieldNames = fields.Select(field => string.Format("{0} ({1})", field.Name, field.FieldType));
                string message = string.Format(Resources.ViewFieldUnsupported, target, string.Join(Environment.NewLine, fieldNames));
                await ServiceLocator.Dialog.AlertAsync(message);
                return false;
            }
            return true;
        }

        public async Task PublishToWebAsync()
        {
            Epi.View view = Context.Project.GetViewById(Views.SelectedItem.ViewId);
            if (!await ValidateAsync("web", view, field => field.FieldType.IsWebSupported()))
            {
                return;
            }
            ConfigurationError error;
            if (Service.IsConfigured(out error, local: true))
            {
                SurveyViewModel model = new SurveyViewModel(view);
                if (await model.InitializeAsync())
                {
                    await ServiceLocator.Dialog.ShowAsync(model);
                }
            }
            else
            {
                await ServiceLocator.Dialog.AlertAsync(string.Format(Resources.WebConfigure, error.GetErrorMessage()));
                ServiceLocator.Document.ShowSettings();
            }
        }

        public async Task PublishToMobileAsync()
        {
            Epi.View view = Context.Project.GetViewById(Views.SelectedItem.ViewId);
            if (!await ValidateAsync("mobile", view, field => field.FieldType.IsMobileSupported()))
            {
                return;
            }
            Wrapper wrapper = MakeView.PublishToMobile.Create(Context.Project.FilePath, Views.SelectedItem.Name);
            wrapper.Invoke();
        }

        public void ImportFromProject()
        {
            ImportExport.ImportFromProject(Views.SelectedItem.ViewId);
        }

        public void ImportFromPackage()
        {
            ImportExport.ImportFromPackage(Views.SelectedItem.ViewId);
        }

        public async Task ImportFromFileAsync()
        {
            await ImportExport.ImportFromFileAsync(Views.SelectedItem.ViewId);
        }

        public async Task ImportFromWebAsync()
        {
            await ImportExport.ImportFromWebAsync(Views.SelectedItem.ViewId);
        }

        public void ImportFromMobile()
        {
            ImportExport.ImportFromMobile(Views.SelectedItem.ViewId);
        }

        public void ExportToPackage()
        {
            ImportExport.ExportToPackage(Views.SelectedItem.ViewId);
        }

        public async Task ExportToFileAsync()
        {
            await ImportExport.ExportToFileAsync(Views.SelectedItem.ViewId);
        }

        public async Task AnalyzeClassicAsync()
        {
            PgmViewModel model = new PgmViewModel(Views.SelectedItem.ViewId);
            await ServiceLocator.Dialog.ShowAsync(model);
        }

        public async Task AnalyzeVisualAsync()
        {
            CanvasViewModel model = new CanvasViewModel(Views.SelectedItem.ViewId);
            await ServiceLocator.Dialog.ShowAsync(model);
        }
    }
}
