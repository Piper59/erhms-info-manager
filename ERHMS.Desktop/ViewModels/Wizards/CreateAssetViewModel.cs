using Epi;
using ERHMS.Common.Naming;
using ERHMS.Common.Text;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Shared;
using ERHMS.Desktop.Wizards;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analytics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public abstract class CreateAssetViewModel : WizardViewModel
    {
        public class SetViewViewModel : StepViewModel<CreateAssetViewModel>
        {
            public static async Task<SetViewViewModel> CreateAsync(CreateAssetViewModel wizard)
            {
                SetViewViewModel result = new SetViewViewModel(wizard);
                await result.InitializeAsync();
                return result;
            }

            public override string Title => Strings.CreateAsset_Lead_SetView;
            public ViewListCollectionView Views { get; private set; }

            private SetViewViewModel(CreateAssetViewModel wizard)
                : base(wizard) { }

            private async Task InitializeAsync()
            {
                Views = await ViewListCollectionView.CreateAsync(Wizard.Project);
            }

            public override bool CanContinue()
            {
                return Views.HasCurrent();
            }

            public override async Task ContinueAsync()
            {
                Wizard.View = Views.CurrentItem;
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                IStep step = await progress.Run(() =>
                {
                    return SetFilePathViewModel.CreateAsync(Wizard, this);
                });
                GoToStep(step);
            }
        }

        public class SetFilePathViewModel : StepViewModel<CreateAssetViewModel>
        {
            public static async Task<SetFilePathViewModel> CreateAsync(CreateAssetViewModel wizard, IStep antecedent)
            {
                SetFilePathViewModel result = new SetFilePathViewModel(wizard, antecedent);
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

            private SetFilePathViewModel(CreateAssetViewModel wizard, IStep antecedent)
                : base(wizard, antecedent)
            {
                fileDialog = ServiceLocator.Resolve<IFileDialogService>();
                fileDialog.Filter = wizard.FileFilter;
                BrowseCommand = new SyncCommand(Browse);
            }

            private async Task InitializeAsync()
            {
                FilePath = await Task.Run(() =>
                {
                    string directoryPath = Wizard.Project.Location;
                    string extension = Wizard.FileExtension;
                    string fileName = $"{Wizard.View.Name}{extension}";
                    FileNameUniquifier fileNames = new FileNameUniquifier(directoryPath, extension);
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
                if (!Comparers.Path.Equals(directoryPath, Wizard.Project.Location))
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
                Wizard.FilePath = FilePath;
                GoToStep(new CommitViewModel(Wizard, this));
                return Task.CompletedTask;
            }
        }

        public class CommitViewModel : StepViewModel<CreateAssetViewModel>
        {
            public override string Title => Strings.Lead_Commit;
            public override string ContinueAction => Strings.AccessText_Finish;
            public DetailsViewModel Details { get; }

            public CommitViewModel(CreateAssetViewModel wizard, IStep antecedent)
                : base(wizard, antecedent)
            {
                Details = new DetailsViewModel
                {
                    { Strings.Label_View, wizard.View.Name },
                    { Strings.Label_File, Path.GetFileName(wizard.FilePath) },
                    { Strings.Label_Location, Path.GetDirectoryName(wizard.FilePath) }
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
                    Asset asset = Wizard.CreateCore();
                    using (Stream stream = File.Open(Wizard.FilePath, FileMode.Create, FileAccess.Write))
                    {
                        asset.Save(stream);
                    }
                });
                Commit(true);
                GoToStep(new CloseViewModel(Wizard, this));
            }
        }

        public class CloseViewModel : StepViewModel<CreateAssetViewModel>
        {
            public override string Title => Strings.CreateAsset_Lead_Close;
            public override string ContinueAction => Strings.AccessText_Close;

            private bool openInEpiInfo = true;
            public bool OpenInEpiInfo
            {
                get { return openInEpiInfo; }
                set { SetProperty(ref openInEpiInfo, value); }
            }

            public CloseViewModel(CreateAssetViewModel wizard, IStep antecedent)
                : base(wizard, antecedent) { }

            public override bool CanContinue()
            {
                return true;
            }

            public override Task ContinueAsync()
            {
                Wizard.OpenInEpiInfo = OpenInEpiInfo;
                Close();
                return Task.CompletedTask;
            }
        }

        protected abstract Module Module { get; }
        protected abstract string FileExtension { get; }
        protected abstract string FileFilter { get; }
        public Project Project { get; }
        protected View View { get; set; }
        public string FilePath { get; private set; }
        public bool OpenInEpiInfo { get; private set; }

        protected CreateAssetViewModel(Project project)
        {
            Project = project;
        }

        protected async Task InitializeAsync()
        {
            Step = await SetViewViewModel.CreateAsync(this);
        }

        protected abstract Asset CreateCore();
    }
}
