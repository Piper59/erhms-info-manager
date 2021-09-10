using Epi;
using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Shared;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public static class ExportRecordsViewModels
    {
        public class State
        {
            public View View { get; }
            public string FilePath { get; set; }

            public State(View view)
            {
                View = view;
            }
        }

        public class SetStrategyViewModel : StepViewModel<State>
        {
            public override string Title => Strings.ExportRecords_Lead_SetStrategy;

            public ICommand ExportToPackageCommand { get; }
            public ICommand ExportToFileCommand { get; }

            public SetStrategyViewModel(State state)
                : base(state)
            {
                ExportToPackageCommand = new SyncCommand(ExportToPackage);
                ExportToFileCommand = new SyncCommand(ExportToFile);
            }

            public void ExportToFile()
            {
                Wizard.GoForward(new SetFilePathViewModel(State));
            }

            public void ExportToPackage()
            {
                Wizard.Close();
                State.View.ExportToPackage();
            }
        }

        public class SetFilePathViewModel : StepViewModel<State>
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

            public SetFilePathViewModel(State state)
                : base(state)
            {
                fileDialog = ServiceLocator.Resolve<IFileDialogService>();
                fileDialog.Filter = Strings.FileDialog_Filter_CsvFiles;
                BrowseCommand = new SyncCommand(Browse);
            }

            public void Browse()
            {
                if (!fileDialog.Save().GetValueOrDefault())
                {
                    return;
                }
                FilePath = fileDialog.FileName;
                Command.OnCanExecuteChanged();
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
                    { Strings.Label_File, state.FilePath }
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
                progress.CanBeCanceled = true;
                try
                {
                    await progress.Run(() =>
                    {
                        State.View.LoadFields();
                        using (Stream stream = File.Open(State.FilePath, FileMode.Create, FileAccess.Write))
                        using (TextWriter writer = new StreamWriter(stream))
                        using (RecordExporter exporter = new RecordExporter(State.View, writer))
                        {
                            exporter.Progress = new FormattingProgress<int>(progress, Strings.Body_ExportingRow);
                            exporter.Export(progress.CancellationToken);
                        }
                    });
                    Wizard.Result = true;
                }
                catch (OperationCanceledException)
                {
                    Wizard.Result = false;
                }
                Wizard.Committed = true;
                Wizard.GoForward(new CloseViewModel(State));
            }
        }

        public class CloseViewModel : StepViewModel<State>
        {
            public override string Title =>
                Wizard.Result.GetValueOrDefault()
                ? Strings.ExportRecords_Lead_Close_Success
                : Strings.ExportRecords_Lead_Close_Failure;
            public override string ContinueAction => Strings.AccessText_Close;

            public CloseViewModel(State state)
                : base(state) { }

            public override bool CanContinue()
            {
                return true;
            }

            public override Task ContinueAsync()
            {
                Wizard.Close();
                return Task.CompletedTask;
            }
        }

        public static WizardViewModel GetWizard(View view)
        {
            State state = new State(view);
            StepViewModel step = new SetStrategyViewModel(state);
            return new WizardViewModel(step);
        }
    }
}
