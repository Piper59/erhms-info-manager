using Epi.Fields;
using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using ERHMS.EpiInfo.Projects;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public partial class ProjectViewModel
    {
        public class ViewsChildViewModel : ObservableObject
        {
            public class Item : ObservableObject, ISelectable
            {
                public Epi.View View { get; }
                public string Title { get; }
                public int FieldCount { get; }
                public int RecordCount { get; }

                private bool isSelected;
                public bool IsSelected
                {
                    get { return isSelected; }
                    set { SetProperty(ref isSelected, value); }
                }

                public Item(Epi.View view)
                {
                    View = view;
                    Title = view.Pages[0].Fields.OfType<LabelField>()
                        .OrderBy(field => field.TabIndex)
                        .FirstOrDefault(field => field.Name.StartsWith("Title", StringComparison.OrdinalIgnoreCase))
                        ?.PromptText;
                    FieldCount = view.Fields.OfType<IInputField>().Count();
                    RecordRepository repository = new RecordRepository(view);
                    if (repository.TableExists())
                    {
                        RecordCount = repository.Count(repository.GetWhereDeletedClause(false));
                    }
                }

                public override int GetHashCode()
                {
                    return View.Id.GetHashCode();
                }

                public override bool Equals(object obj)
                {
                    return obj is Item item && item.View.Id == View.Id;
                }
            }

            public Project Project { get; }

            private readonly CustomCollectionView<Item> items;
            public ICustomCollectionView<Item> Items => items;

            public ICommand CreateCommand { get; }
            public ICommand CustomizeCommand { get; }
            public ICommand ViewDataCommand { get; }
            public ICommand EnterDataCommand { get; }
            public ICommand ExportDataCommand { get; }
            public ICommand ImportDataCommand { get; }
            public ICommand DeleteCommand { get; }

            public ViewsChildViewModel(Project project)
            {
                Project = project;
                items = new CustomCollectionView<Item>();
                RefreshDataInternal();
                CreateCommand = new AsyncCommand(CreateAsync);
                CustomizeCommand = new AsyncCommand(CustomizeAsync, items.HasSelectedItem);
                ViewDataCommand = new AsyncCommand(ViewDataAsync, items.HasSelectedItem);
                EnterDataCommand = new AsyncCommand(EnterDataAsync, items.HasSelectedItem);
                ExportDataCommand = new SyncCommand(ExportData, items.HasSelectedItem);
                ImportDataCommand = new SyncCommand(ImportData, items.HasSelectedItem);
                DeleteCommand = new AsyncCommand(DeleteAsync, items.HasSelectedItem);
            }

            private void RefreshDataInternal()
            {
                items.Source.Clear();
                items.Source.AddRange(Project.Views.Cast<Epi.View>().Select(view => new Item(view)));
            }

            public void RefreshData()
            {
                Project.LoadViews();
                RefreshDataInternal();
            }

            public void RefreshView()
            {
                items.Refresh();
            }

            public async Task CreateAsync()
            {
                CreateViewWizard wizard = null;
                {
                    IProgressService progress = ServiceProvider.Resolve<IProgressService>();
                    progress.Title = ResX.LoadingProjectTitle;
                    await progress.RunAsync(() =>
                    {
                        Project.LoadViews();
                        wizard = new CreateViewWizard(Project);
                    });
                }
                bool? result = ServiceProvider.Resolve<IWizardService>().Run(wizard);
                if (result.GetValueOrDefault())
                {
                    IProgressService progress = ServiceProvider.Resolve<IProgressService>();
                    progress.Title = ResX.LoadingProjectTitle;
                    await progress.RunAsync(() =>
                    {
                        items.Source.Add(new Item(wizard.View));
                    });
                    items.Refresh();
                }
            }

            public async Task CustomizeAsync()
            {
                Epi.View view = items.SelectedItem.View;
                await MainViewModel.Current.StartEpiInfoAsync(
                    Module.MakeView,
                    $"/project:{view.Project.FilePath}",
                    $"/view:{view.Name}");
            }

            public async Task ViewDataAsync()
            {
                ViewViewModel content = null;
                IProgressService progress = ServiceProvider.Resolve<IProgressService>();
                progress.Title = ResX.LoadingViewTitle;
                await progress.RunAsync(() =>
                {
                    content = new ViewViewModel(Project, items.SelectedItem.View);
                });
                MainViewModel.Current.Content = content;
            }

            public async Task EnterDataAsync()
            {
                Epi.View view = items.SelectedItem.View;
                await MainViewModel.Current.StartEpiInfoAsync(
                    Module.Enter,
                    $"/project:{view.Project.FilePath}",
                    $"/view:{view.Name}",
                    "/record:*");
            }

            public void ExportData()
            {
                throw new System.NotImplementedException();
            }

            public void ImportData()
            {
                throw new System.NotImplementedException();
            }

            public async Task DeleteAsync()
            {
                bool? result = ServiceProvider.Resolve<IDialogService>().Show(new DialogInfo(DialogInfoPreset.Warning)
                {
                    Lead = string.Format(ResX.DeleteViewWarningLead, items.SelectedItem.View.Name),
                    Body = ResX.DeleteViewWarningBody,
                    Buttons = new DialogButtonCollection
                    {
                        new DialogButton(true, "_Delete", true, false),
                        new DialogButton(false, "_Cancel", false, true)
                    }
                });
                if (result.GetValueOrDefault())
                {
                    try
                    {
                        IProgressService progress = ServiceProvider.Resolve<IProgressService>();
                        progress.Title = ResX.DeletingViewTitle;
                        await progress.RunAsync(() =>
                        {
                            Project.DeleteView(items.SelectedItem.View);
                        });
                        items.Source.Remove(items.SelectedItem);
                        items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        ServiceProvider.Resolve<IDialogService>().Show(new DialogInfo(DialogInfoPreset.Error)
                        {
                            Lead = ResX.DeletingViewErrorLead,
                            Body = ex.Message,
                            Details = ex.ToString()
                        });
                    }
                }
            }
        }
    }
}
