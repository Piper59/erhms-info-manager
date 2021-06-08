using Epi;
using ERHMS.Common.Naming;
using ERHMS.Common.Text;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Wizards;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analytics;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public abstract class CreateAssetViewModel : WizardViewModel
    {
        public class SetViewViewModel : StepViewModel<CreateAssetViewModel>
        {
            public override string Title => Strings.Lead_CreateAsset_SetView;

            private readonly List<View> views;
            public ICollectionView Views { get; }

            public View CurrentView => (View)Views.CurrentItem;

            public SetViewViewModel(CreateAssetViewModel wizard)
                : base(wizard)
            {
                views = new List<View>();
                Views = new ListCollectionView(views);
            }

            public async Task InitializeAsync()
            {
                IEnumerable<View> views = await Task.Run(() =>
                {
                    Wizard.Project.LoadViews();
                    return Wizard.Project.Views.Cast<View>().ToList();
                });
                this.views.Clear();
                this.views.AddRange(views);
                Views.Refresh();
            }

            public override bool CanContinue()
            {
                return Views.HasCurrent();
            }

            public override async Task ContinueAsync()
            {
                Wizard.View = CurrentView;
                await ContinueToAsync(async () =>
                {
                    SetFileInfoViewModel step = new SetFileInfoViewModel(Wizard, this);
                    await step.InitializeAsync();
                    return step;
                });
            }
        }

        public class SetFileInfoViewModel : StepViewModel<CreateAssetViewModel>
        {
            public override string Title => Strings.Lead_CreateAsset_SetFileInfo;

            private FileInfo fileInfo;
            public FileInfo FileInfo
            {
                get { return fileInfo; }
                set { SetProperty(ref fileInfo, value); }
            }

            public ICommand BrowseCommand { get; }

            public SetFileInfoViewModel(CreateAssetViewModel wizard, IStep antecedent)
                : base(wizard, antecedent)
            {
                BrowseCommand = new SyncCommand(Browse);
            }

            public async Task InitializeAsync()
            {
                string fileName = $"{Wizard.View.Name}{Wizard.FileExtension}";
                await Task.Run(() =>
                {
                    FileNameUniquifier fileNames =
                        new FileNameUniquifier(Wizard.Project.Location, Wizard.FileExtension);
                    if (fileNames.Exists(fileName))
                    {
                        fileName = fileNames.Uniquify(fileName);
                    }
                });
                fileInfo = new FileInfo(Path.Combine(Wizard.Project.Location, fileName));
            }

            public void Browse()
            {
                IFileDialogService fileDialog = ServiceLocator.Resolve<IFileDialogService>();
                fileDialog.InitialDirectory = this.fileInfo.DirectoryName;
                fileDialog.FileName = this.fileInfo.Name;
                fileDialog.Filter = Wizard.FileFilter;
                if (fileDialog.Save() != true)
                {
                    return;
                }
                FileInfo fileInfo = new FileInfo(fileDialog.FileName);
                if (!Comparers.Path.Equals(fileInfo.DirectoryName, Wizard.Project.Location))
                {
                    IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
                    dialog.Severity = DialogSeverity.Warning;
                    dialog.Lead = Strings.Lead_ConfirmOrphanAssetCreation;
                    dialog.Body = string.Format(Strings.Body_ConfirmOrphanAssetCreation, fileInfo.DirectoryName);
                    dialog.Buttons = DialogButtonCollection.ActionOrCancel(Strings.AccessText_Continue);
                    if (dialog.Show() != true)
                    {
                        return;
                    }
                }
                FileInfo = fileInfo;
            }

            public override bool CanContinue()
            {
                return fileInfo != null;
            }

            public override Task ContinueAsync()
            {
                Wizard.FileInfo = fileInfo;
                ContinueTo(new CommitViewModel(Wizard, this));
                return Task.CompletedTask;
            }
        }

        public class CommitViewModel : StepViewModel<CreateAssetViewModel>
        {
            public override string Title => Strings.Lead_CreateAsset_Commit;
            public override string ContinueAction => Strings.AccessText_Finish;
            public DetailsViewModel Details { get; }

            public CommitViewModel(CreateAssetViewModel wizard, IStep antecedent)
                : base(wizard, antecedent)
            {
                Details = new DetailsViewModel
                {
                    { Strings.Key_View, wizard.View.Name },
                    { Strings.Key_FileName, wizard.FileInfo.Name },
                    { Strings.Key_Location, wizard.FileInfo.DirectoryName }
                };
            }

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                progress.Title = Strings.Lead_CreatingAsset;
                await progress.Run(() =>
                {
                    Asset asset = Wizard.CreateCore();
                    using (Stream stream = Wizard.FileInfo.Open(FileMode.Create, FileAccess.Write))
                    {
                        asset.Save(stream);
                    }
                });
                Commit();
                SetResult(true);
                ContinueTo(new CloseViewModel(Wizard, this));
            }
        }

        public class CloseViewModel : StepViewModel<CreateAssetViewModel>
        {
            public override string Title => Strings.Lead_CreateAsset_Close;
            public override string ContinueAction => Strings.AccessText_Close;

            private bool opening = true;
            public bool Opening
            {
                get { return opening; }
                set { SetProperty(ref opening, value); }
            }

            public CloseViewModel(CreateAssetViewModel wizard, IStep antecedent)
                : base(wizard, antecedent) { }

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                if (opening)
                {
                    await MainViewModel.Instance.StartEpiInfoAsync(Wizard.Module, Wizard.FileInfo.FullName);
                }
                Close();
            }
        }

        protected abstract Module Module { get; }
        protected abstract string FileExtension { get; }
        protected abstract string FileFilter { get; }
        public Project Project { get; }
        public View View { get; private set; }
        public FileInfo FileInfo { get; private set; }

        protected CreateAssetViewModel(Project project)
        {
            Project = project;
        }

        public async Task InitializeAsync()
        {
            SetViewViewModel step = new SetViewViewModel(this);
            await step.InitializeAsync();
            Initialize(step);
        }

        protected abstract Asset CreateCore();
    }
}
