using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using System;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    [ContextSafe]
    public class StartViewModel : DocumentViewModel
    {
        public ICommand ShowDataSourcesCommand { get; private set; }
        public ICommand ShowHelpCommand { get; private set; }
        public ICommand ShowRespondersCommand { get; private set; }
        public ICommand ShowIncidentsCommand { get; private set; }
        public ICommand ShowViewsCommand { get; private set; }
        public ICommand ShowTemplatesCommand { get; private set; }
        public ICommand ShowPgmsCommand { get; private set; }
        public ICommand ShowCanvasesCommand { get; private set; }

        public StartViewModel()
        {
            Title = "Start";
            ShowDataSourcesCommand = new Command(ShowDataSources);
            ShowHelpCommand = new Command(ShowHelp);
            ShowRespondersCommand = new AsyncCommand(ShowRespondersAsync);
            ShowIncidentsCommand = new AsyncCommand(ShowIncidentsAsync);
            ShowViewsCommand = new AsyncCommand(ShowViewsAsync);
            ShowTemplatesCommand = new AsyncCommand(ShowTemplatesAsync);
            ShowPgmsCommand = new AsyncCommand(ShowPgmsAsync);
            ShowCanvasesCommand = new AsyncCommand(ShowCanvasesAsync);
        }

        private async Task<bool> ValidateAsync()
        {
            if (Context == null)
            {
                if (await ServiceLocator.Dialog.ConfirmAsync(Resources.GetStarted, title: "Help"))
                {
                    ServiceLocator.Document.ShowByType(() => new DataSourceListViewModel());
                }
                return false;
            }
            return true;
        }

        private async Task ShowAsync<TDocument>(Func<TDocument> constructor)
            where TDocument : DocumentViewModel
        {
            if (!await ValidateAsync())
            {
                return;
            }
            ServiceLocator.Document.ShowByType(constructor);
        }

        public void ShowDataSources()
        {
            ServiceLocator.Document.ShowByType(() => new DataSourceListViewModel());
        }

        public void ShowHelp()
        {
            ServiceLocator.Document.ShowByType(() => new HelpViewModel());
        }

        public async Task ShowRespondersAsync()
        {
            await ShowAsync(() => new ResponderListViewModel());
        }

        public async Task ShowIncidentsAsync()
        {
            await ShowAsync(() => new IncidentListViewModel());
        }

        public async Task ShowViewsAsync()
        {
            await ShowAsync(() => new ViewListViewModel(null));
        }

        public async Task ShowTemplatesAsync()
        {
            await ShowAsync(() => new TemplateListViewModel(null));
        }

        public async Task ShowPgmsAsync()
        {
            await ShowAsync(() => new PgmListViewModel(null));
        }

        public async Task ShowCanvasesAsync()
        {
            await ShowAsync(() => new CanvasListViewModel(null));
        }
    }
}
