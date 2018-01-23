using ERHMS.Presentation.Commands;
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

        public StartViewModel(IServiceManager services)
            : base(services)
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
                if (await Services.Dialog.ConfirmAsync(Services.String.GetStarted, title: "Help"))
                {
                    Services.Document.ShowByType(() => new DataSourceListViewModel(Services));
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
            Services.Document.ShowByType(constructor);
        }

        public void ShowDataSources()
        {
            Services.Document.ShowByType(() => new DataSourceListViewModel(Services));
        }

        public void ShowHelp()
        {
            Services.Document.ShowByType(() => new HelpViewModel(Services));
        }

        public async Task ShowRespondersAsync()
        {
            await ShowAsync(() => new ResponderListViewModel(Services));
        }

        public async Task ShowIncidentsAsync()
        {
            await ShowAsync(() => new IncidentListViewModel(Services));
        }

        public async Task ShowViewsAsync()
        {
            await ShowAsync(() => new ViewListViewModel(Services, null));
        }

        public async Task ShowTemplatesAsync()
        {
            await ShowAsync(() => new TemplateListViewModel(Services, null));
        }

        public async Task ShowPgmsAsync()
        {
            await ShowAsync(() => new PgmListViewModel(Services, null));
        }

        public async Task ShowCanvasesAsync()
        {
            await ShowAsync(() => new CanvasListViewModel(Services, null));
        }
    }
}
