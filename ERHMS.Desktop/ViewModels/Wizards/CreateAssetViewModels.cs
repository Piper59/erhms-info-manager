using Epi;
using ERHMS.Common.Naming;
using ERHMS.Common.Text;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Shared;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analytics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public static class CreateAssetViewModels
    {
        public class State
        {
            public Module Module { get; }
            public string FileExtension => Asset.GetFileExtension(Module);
            public string FileFilter => Infrastructure.FileFilter.FromModule(Module);
            public Project Project { get; }
            public View View { get; set; }
            public string FilePath { get; set; }

            public State(Module module, Project project)
            {
                Module = module;
                Project = project;
            }
        }

        public class SetViewViewModel : StepViewModel<State>
        {
            public static async Task<SetViewViewModel> CreateAsync(State state)
            {
                SetViewViewModel result = new SetViewViewModel(state);
                await result.InitializeAsync();
                return result;
            }

            public override string Title => Strings.CreateAsset_Lead_SetView;
            public ViewListCollectionView Views { get; private set; }

            private SetViewViewModel(State state)
                : base(state) { }

            private async Task InitializeAsync()
            {
                Views = await ViewListCollectionView.CreateAsync(State.Project);
            }

            public override bool CanContinue()
            {
                return Views.HasCurrent();
            }

            public override async Task ContinueAsync()
            {
                State.View = Views.CurrentItem;
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                Wizard.GoForward(await progress.Run(() =>
                {
                    return SetFilePathViewModel.CreateAsync(State);
                }));
            }
        }

        public class SetFilePathViewModel : StepViewModel<State>
        {
            public static async Task<SetFilePathViewModel> CreateAsync(State state)
            {
                SetFilePathViewModel result = new SetFilePathViewModel(state);
                await result.InitializeAsync();
                return result;
            }

            private readonly IFileDialogService fileDialog;

            public override string Title => Strings.CreateAsset_Lead_SetFilePath;

            private string filePath;
            public string FilePath
            {
                get { return filePath; }
                private set { SetProperty(ref filePath, value); }
            }

            public ICommand BrowseCommand { get; }

            private SetFilePathViewModel(State state)
                : base(state)
            {
                fileDialog = ServiceLocator.Resolve<IFileDialogService>();
                fileDialog.Filter = state.FileFilter;
                BrowseCommand = new SyncCommand(Browse);
            }

            private async Task InitializeAsync()
            {
                FilePath = await Task.Run(() =>
                {
                    string directoryPath = State.Project.Location;
                    string fileName = $"{State.View.Name}{State.FileExtension}";
                    FileNameUniquifier fileNames = new FileNameUniquifier(directoryPath, State.FileExtension);
                    if (fileNames.Exists(fileName))
                    {
                        fileName = fileNames.Uniquify(fileName);
                    }
                    return Path.Combine(directoryPath, fileName);
                });
                fileDialog.FileName = FilePath;
            }

            public void Browse()
            {
                if (fileDialog.Save() != true)
                {
                    return;
                }
                string directoryPath = Path.GetDirectoryName(fileDialog.FileName);
                if (!Comparers.Path.Equals(directoryPath, State.Project.Location))
                {
                    IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
                    dialog.Severity = DialogSeverity.Warning;
                    dialog.Lead = Strings.Lead_ConfirmOrphanAssetCreation;
                    dialog.Body = string.Format(Strings.Body_ConfirmOrphanAssetCreation, directoryPath);
                    dialog.Buttons = DialogButtonCollection.ActionOrCancel(Strings.AccessText_Continue);
                    if (dialog.Show() != true)
                    {
                        return;
                    }
                }
                FilePath = fileDialog.FileName;
            }

            public override bool CanContinue()
            {
                return FilePath != null;
            }

            public override Task ContinueAsync()
            {
                State.FilePath = FilePath;
                Wizard.GoForward(new CommitViewModel(State));
                return Task.CompletedTask;
            }
        }

        public class CommitViewModel : StepViewModel<State>
        {
            public override string Title => Strings.Lead_Commit;
            public override string ContinueAction => Strings.AccessText_Finish;
            public DetailsViewModel Details { get; }

            public CommitViewModel(State state)
                : base(state)
            {
                Details = new DetailsViewModel
                {
                    { Strings.Label_View, state.View.Name },
                    { Strings.Label_File, Path.GetFileName(state.FilePath) },
                    { Strings.Label_Location, Path.GetDirectoryName(state.FilePath) }
                };
            }

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                progress.Lead = Strings.Lead_CreatingAsset;
                await progress.Run(() =>
                {
                    Asset asset = Asset.Create(State.Module, State.View);
                    using (Stream stream = File.Open(State.FilePath, FileMode.Create, FileAccess.Write))
                    {
                        asset.Save(stream);
                    }
                });
                Wizard.Result = true;
                Wizard.Committed = true;
                Wizard.GoForward(new CloseViewModel(State));
            }
        }

        public class CloseViewModel : StepViewModel<State>
        {
            public override string Title => Strings.CreateAsset_Lead_Close;
            public override string ContinueAction => Strings.AccessText_Close;

            private bool openInEpiInfo = true;
            public bool OpenInEpiInfo
            {
                get { return openInEpiInfo; }
                set { SetProperty(ref openInEpiInfo, value); }
            }

            public CloseViewModel(State state)
                : base(state) { }

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                if (OpenInEpiInfo)
                {
                    await Integration.StartAsync(State.Module, State.FilePath);
                }
                Wizard.Close();
            }
        }

        public static async Task<WizardViewModel> GetWizardAsync(Module module, Project project)
        {
            State state = new State(module, project);
            StepViewModel step = await SetViewViewModel.CreateAsync(state);
            return new WizardViewModel(step);
        }
    }
}
