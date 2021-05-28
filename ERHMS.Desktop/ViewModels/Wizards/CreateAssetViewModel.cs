using Epi;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analytics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public abstract class CreateAssetViewModel : WizardViewModel
    {
        public class CollectViewModel : StepViewModel<CreateAssetViewModel>
        {
            public override string Title => ResXResources.Lead_CreateAsset_Collect;

            private readonly List<View> views;
            public ICollectionView Views { get; }

            public View CurrentView => (View)Views.CurrentItem;

            public CollectViewModel(CreateAssetViewModel wizard)
                : base(wizard)
            {
                views = new List<View>();
                Views = new ListCollectionView(views);
            }

            public async Task InitializeAsync()
            {
                await Task.Run(() =>
                {
                    Wizard.Project.LoadViews();
                    views.Clear();
                    views.AddRange(Wizard.Project.Views.Cast<View>());
                });
                Views.Refresh();
            }

            private string GetFilePath()
            {
                IFileDialogService fileDialog = ServiceLocator.Resolve<IFileDialogService>();
                fileDialog.InitialDirectory = Wizard.Project.Location;
                fileDialog.FileName = $"{CurrentView.Name}{Wizard.FileExtension}";
                fileDialog.Filter = Wizard.FileFilter;
                if (fileDialog.Save() != true)
                {
                    return null;
                }
                string filePath = fileDialog.FileName;
                string location = Path.GetDirectoryName(filePath);
                if (location.Equals(Wizard.Project.Location, StringComparison.OrdinalIgnoreCase))
                {
                    return filePath;
                }
                IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
                dialog.Severity = DialogSeverity.Warning;
                dialog.Lead = ResXResources.Lead_ConfirmOrphanAssetCreation;
                dialog.Body = string.Format(ResXResources.Body_ConfirmOrphanAssetCreation, location);
                dialog.Buttons = DialogButtonCollection.YesOrNo;
                return dialog.Show() == true ? filePath : null;
            }

            public override bool CanContinue()
            {
                return Views.HasCurrent();
            }

            public override Task ContinueAsync()
            {
                Wizard.View = CurrentView;
                string filePath = GetFilePath();
                if (filePath != null)
                {
                    Wizard.Asset = new FileInfo(filePath);
                    ContinueTo(new ConfirmViewModel());
                }
                return Task.CompletedTask;
            }
        }

        public class ConfirmViewModel : StepViewModel<CreateAssetViewModel>
        {
            public override string Title => ResXResources.Lead_CreateAsset_Confirm;
            public override string ContinueAction => ResXResources.AccessText_Finish;

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                await Wizard.CreateAsync();
                Commit();
                SetResult(true);
                ContinueTo(new CompleteViewModel());
            }
        }

        public class CompleteViewModel : StepViewModel<CreateAssetViewModel>
        {
            public override string Title => ResXResources.Lead_CreateAsset_Complete;
            public override string ContinueAction => ResXResources.AccessText_Close;

            private bool open = true;
            public bool Open
            {
                get { return open; }
                set { SetProperty(ref open, value); }
            }

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                if (open)
                {
                    await Wizard.OpenAsync();
                }
                Close();
            }
        }

        protected abstract Module Module { get; }
        protected abstract string FileExtension { get; }
        protected abstract string FileFilter { get; }
        public Project Project { get; }
        public View View { get; private set; }
        public FileInfo Asset { get; private set; }

        protected CreateAssetViewModel(Project project)
        {
            Project = project;
        }

        protected async Task InitializeAsync()
        {
            CollectViewModel step = new CollectViewModel(this);
            await step.InitializeAsync();
            this.step = step;
        }

        protected abstract Asset GetAsset();

        protected async Task CreateAsync()
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Title = ResXResources.Lead_CreatingAsset;
            await progress.RunAsync(() =>
            {
                Asset asset = GetAsset();
                using (Stream stream = File.Open(Asset.FullName, FileMode.Create, FileAccess.Write))
                {
                    asset.Save(stream);
                }
            });
        }

        protected async Task OpenAsync()
        {
            await MainViewModel.Instance.StartEpiInfoAsync(Module, Asset.FullName);
        }
    }
}
