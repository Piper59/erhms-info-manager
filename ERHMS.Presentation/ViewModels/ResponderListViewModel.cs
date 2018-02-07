using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
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
            public ResponderListChildViewModel(IServiceManager services)
                : base(services)
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
                yield return item.LastName;
                yield return item.FirstName;
                yield return item.EmailAddress;
                yield return item.City;
                yield return item.State;
                yield return item.OrganizationName;
                yield return item.Occupation;
            }
        }

        public ResponderListChildViewModel Responders { get; private set; }
        public ImportExportViewModel ImportExport { get; private set; }

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

        public ResponderListViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Responders";
            Responders = new ResponderListChildViewModel(services);
            ImportExport = new ImportExportViewModel(services);
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
            Services.Document.Show(() => new ResponderViewModel(Services, new Responder(true)));
        }

        public void Edit()
        {
            Services.Document.Show(
                model => model.Responder.Equals(Responders.SelectedItems.First()),
                () => new ResponderViewModel(Services, Context.Responders.Refresh(Responders.SelectedItems.First())));
        }

        public async Task MergeAutomatedAsync()
        {
            ICollection<Tuple<Responder, Responder>> pairs = new List<Tuple<Responder, Responder>>();
            await Services.Dialog.BlockAsync("Searching for potentially duplicate responders \u2026", () =>
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
                        if (!uniquePairs[responder1.ResponderId].Contains(responder2.ResponderId, StringComparer.OrdinalIgnoreCase)
                            && responder1.IsSimilar(responder2))
                        {
                            pairs.Add(Tuple.Create(responder1, responder2));
                        }
                    }
                }
            });
            if (pairs.Count == 0)
            {
                string message = string.Join(" ", new string[]
                {
                    "No potentially duplicate responders found.",
                    "You may still perform a merge by selecting two responders from the list and clicking Merge > Selected."
                });
                await Services.Dialog.AlertAsync(message, "Help");
            }
            else
            {
                Services.Document.Show(() => new MergeAutomatedViewModel(Services, pairs));
            }
        }

        public async Task MergeSelectedAsync()
        {
            IList<Responder> responders = Responders.SelectedItems.ToList();
            if (responders.Count != 2)
            {
                await Services.Dialog.AlertAsync("Please select two responders to merge.");
                return;
            }
            Services.Document.Show(() => new MergeSelectedViewModel(
                Services,
                Context.Responders.Refresh(responders[0]),
                Context.Responders.Refresh(responders[1])));
        }

        public async Task DeleteAsync()
        {
            if (await Services.Dialog.ConfirmAsync("Delete the selected responder?", "Delete"))
            {
                Responder responder = Responders.SelectedItems.First();
                responder.Deleted = true;
                Context.Responders.Save(responder);
                Services.Data.Refresh(typeof(Responder));
            }
        }

        public void Email()
        {
            Services.Document.Show(() => new EmailViewModel(Services, Context.Responders.Refresh(Responders.SelectedItems)));
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
            using (PgmViewModel model = new PgmViewModel(Services, Context.Responders.View.Id))
            {
                await Services.Dialog.ShowAsync(model);
            }
        }

        public async Task AnalyzeVisualAsync()
        {
            using (CanvasViewModel model = new CanvasViewModel(Services, Context.Responders.View.Id))
            {
                await Services.Dialog.ShowAsync(model);
            }
        }

        public override void Dispose()
        {
            Responders.Dispose();
            ImportExport.Dispose();
            base.Dispose();
        }
    }
}
