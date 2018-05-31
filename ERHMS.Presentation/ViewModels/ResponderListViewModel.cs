using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class ResponderListViewModel : DocumentViewModel
    {
        public class ResponderListChildViewModel : ListViewModel<Responder>
        {
            public ResponderListChildViewModel()
            {
                Refresh();
            }

            protected override IEnumerable<Responder> GetItems()
            {
                return Context.Responders.SelectUndeleted()
                    .OrderBy(responder => responder.FullName, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(responder => responder.EmailAddress, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Responder item)
            {
                return ListViewModelExtensions.GetFilteredValues(item);
            }
        }

        public ResponderListChildViewModel Responders { get; private set; }

        public ICommand CreateCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand MergeAutomatedCommand { get; private set; }
        public ICommand MergeSelectedCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand EmailCommand { get; private set; }
        public ICommand ImportFromProjectCommand { get; private set; }
        public ICommand ImportFromPackageCommand { get; private set; }
        public ICommand ImportFromFileCommand { get; private set; }
        public ICommand ImportFromWebCommand { get; private set; }
        public ICommand ImportFromMobileCommand { get; private set; }
        public ICommand ExportToPackageCommand { get; private set; }
        public ICommand ExportToFileCommand { get; private set; }
        public ICommand AnalyzeClassicCommand { get; private set; }
        public ICommand AnalyzeVisualCommand { get; private set; }

        public ResponderListViewModel()
        {
            Title = "Responders";
            Responders = new ResponderListChildViewModel();
            CreateCommand = new Command(Create);
            EditCommand = new Command(Edit, Responders.HasOneSelectedItem);
            MergeAutomatedCommand = new AsyncCommand(MergeAutomatedAsync);
            MergeSelectedCommand = new AsyncCommand(MergeSelectedAsync, Responders.HasAnySelectedItems);
            DeleteCommand = new AsyncCommand(DeleteAsync, Responders.HasOneSelectedItem);
            EmailCommand = new Command(Email, Responders.HasAnySelectedItems);
            ImportFromProjectCommand = new Command(ImportFromProject);
            ImportFromPackageCommand = new Command(ImportFromPackage);
            ImportFromFileCommand = new AsyncCommand(ImportFromFileAsync);
            ImportFromWebCommand = new AsyncCommand(ImportFromWebAsync);
            ImportFromMobileCommand = new Command(ImportFromMobile);
            ExportToPackageCommand = new Command(ExportToPackage);
            ExportToFileCommand = new AsyncCommand(ExportToFileAsync);
            AnalyzeClassicCommand = new AsyncCommand(AnalyzeClassicAsync);
            AnalyzeVisualCommand = new AsyncCommand(AnalyzeVisualAsync);
        }

        public void Create()
        {
            ServiceLocator.Document.Show(() => new ResponderViewModel(new Responder(true)));
        }

        public void Edit()
        {
            ServiceLocator.Document.Show(
                model => model.Responder.Equals(Responders.SelectedItems.First()),
                () => new ResponderViewModel(Context.Responders.Refresh(Responders.SelectedItems.First())));
        }

        public async Task MergeAutomatedAsync()
        {
            ICollection<Tuple<Responder, Responder>> pairs = new List<Tuple<Responder, Responder>>();
            await ServiceLocator.Dialog.BlockAsync(Resources.ResponderPairSearching, () =>
            {
                IList<Responder> responders = Context.Responders.SelectUndeleted()
                    .OrderBy(responder => responder.FullName, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                ILookup<string, string> uniquePairs = Context.UniquePairs.SelectLookup();
                for (int index1 = 0; index1 < responders.Count; index1++)
                {
                    Responder responder1 = responders[index1];
                    for (int index2 = index1 + 1; index2 < responders.Count; index2++)
                    {
                        Responder responder2 = responders[index2];
                        if (responder1.IsSimilar(responder2) && !uniquePairs[responder1.ResponderId].Contains(responder2.ResponderId))
                        {
                            pairs.Add(Tuple.Create(responder1, responder2));
                        }
                    }
                }
            });
            if (pairs.Count == 0)
            {
                await ServiceLocator.Dialog.AlertAsync(Resources.ResponderPairSearchEmpty, "Help");
            }
            else
            {
                ServiceLocator.Document.Show(() => new MergeAutomatedViewModel(pairs));
            }
        }

        public async Task MergeSelectedAsync()
        {
            IList<Responder> responders = Responders.SelectedItems.ToList();
            if (responders.Count != 2)
            {
                await ServiceLocator.Dialog.AlertAsync(Resources.ResponderPairNotSelected);
                return;
            }
            ServiceLocator.Document.Show(() => new MergeSelectedViewModel(
                Context.Responders.Refresh(responders[0]),
                Context.Responders.Refresh(responders[1])));
        }

        public async Task DeleteAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.ResponderConfirmDelete, "Delete"))
            {
                Responder responder = Responders.SelectedItems.First();
                responder.Deleted = true;
                Context.Responders.Save(responder);
                ServiceLocator.Data.Refresh(typeof(Responder));
            }
        }

        public void Email()
        {
            ServiceLocator.Document.Show(() => new EmailViewModel(Context.Responders.Refresh(Responders.SelectedItems)));
        }

        public void ImportFromProject()
        {
            ImportExport.ImportFromProject(Context.Responders.View.Id);
        }

        public void ImportFromPackage()
        {
            ImportExport.ImportFromPackage(Context.Responders.View.Id);
        }

        public async Task ImportFromFileAsync()
        {
            await ImportExport.ImportFromFileAsync(Context.Responders.View.Id);
        }

        public async Task ImportFromWebAsync()
        {
            await ImportExport.ImportFromWebAsync(Context.Responders.View.Id);
        }

        public void ImportFromMobile()
        {
            ImportExport.ImportFromMobile(Context.Responders.View.Id);
        }

        public void ExportToPackage()
        {
            ImportExport.ExportToPackage(Context.Responders.View.Id);
        }

        public async Task ExportToFileAsync()
        {
            await ImportExport.ExportToFileAsync(Context.Responders.View.Id);
        }

        public async Task AnalyzeClassicAsync()
        {
            PgmViewModel model = new PgmViewModel(Context.Responders.View.Id);
            await ServiceLocator.Dialog.ShowAsync(model);
        }

        public async Task AnalyzeVisualAsync()
        {
            CanvasViewModel model = new CanvasViewModel(Context.Responders.View.Id);
            await ServiceLocator.Dialog.ShowAsync(model);
        }
    }
}
