using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Wizards;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public class ExportRecordsViewModel : WizardViewModel
    {
        public class SetFormatViewModel : StepViewModel<ExportRecordsViewModel>
        {
            public override string Title => Strings.ExportRecords_Lead_SetFormat;

            public ICommand ExportToCsvCommand { get; }
            public ICommand ExportToEdp7Command { get; }

            public SetFormatViewModel(ExportRecordsViewModel wizard)
                : base(wizard)
            {
                ExportToCsvCommand = new SyncCommand(ExportToCsv);
                ExportToEdp7Command = new SyncCommand(ExportToEdp7);
            }

            public void ExportToCsv()
            {
                GoToStep(new SetFilePathViewModel(Wizard, this));
            }

            public void ExportToEdp7()
            {
                Close();
                Wizard.View.ExportToPackage();
            }
        }

        public class SetFilePathViewModel : StepViewModel<ExportRecordsViewModel>
        {
            private readonly IFileDialogService fileDialog;

            public override string Title => Strings.ExportRecords_Lead_SetFilePath;

            private string filePath;
            public string FilePath
            {
                get { return filePath; }
                private set { SetProperty(ref filePath, value); }
            }

            public ICommand BrowseCommand { get; }

            public SetFilePathViewModel(ExportRecordsViewModel wizard, IStep antecedent)
                : base(wizard, antecedent)
            {
                fileDialog = ServiceLocator.Resolve<IFileDialogService>();
                fileDialog.Filter = Strings.FileDialog_Filter_CsvFiles;
                BrowseCommand = new SyncCommand(Browse);
            }

            public void Browse()
            {
                if (fileDialog.Save() != true)
                {
                    return;
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

        public class CommitViewModel : StepViewModel<ExportRecordsViewModel>
        {
            private static IProgress<int> GetProgress(IProgressService progress)
            {
                return new Progress<int>(rowNumber =>
                {
                    progress.Report(string.Format(Strings.Body_ExportingRow, rowNumber));
                });
            }

            public override string Title => Strings.Lead_Commit;
            public override string ContinueAction => Strings.AccessText_Finish;
            public DetailsViewModel Details { get; }

            public CommitViewModel(ExportRecordsViewModel wizard, IStep antecedent)
                : base(wizard, antecedent)
            {
                Details = new DetailsViewModel
                {
                    { Strings.Label_File, wizard.FilePath }
                };
            }

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                progress.Lead = Strings.Lead_ExportingRecords;
                await progress.Run(() =>
                {
                    Wizard.View.LoadFields();
                    using (Stream stream = File.Open(Wizard.FilePath, FileMode.Create, FileAccess.Write))
                    using (TextWriter writer = new StreamWriter(stream))
                    using (RecordExporter exporter = new RecordExporter(Wizard.View, writer))
                    {
                        exporter.Export(GetProgress(progress));
                    }
                });
                Commit(true);
                GoToStep(new CloseViewModel(Wizard, this));
            }
        }

        public class CloseViewModel : StepViewModel<ExportRecordsViewModel>
        {
            public override string Title => Strings.ExportRecords_Lead_Close;
            public override string ContinueAction => Strings.AccessText_Close;

            public CloseViewModel(ExportRecordsViewModel wizard, IStep antecedent)
                : base(wizard, antecedent) { }

            public override bool CanContinue()
            {
                return true;
            }

            public override Task ContinueAsync()
            {
                Close();
                return Task.CompletedTask;
            }
        }

        public View View { get; }
        private string FilePath { get; set; }

        public ExportRecordsViewModel(View view)
        {
            View = view;
            Step = new SetFormatViewModel(this);
        }
    }
}
