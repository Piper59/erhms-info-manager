using Epi;
using Epi.Fields;
using ERHMS.Common;
using ERHMS.Common.Linq;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Shared;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using ERHMS.EpiInfo.Metadata;
using ERHMS.EpiInfo.Naming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public static class ImportRecordsViewModels
    {
        public class Target
        {
            public static Target Empty { get; } = new Target(null, Strings.Placeholder_Ignored);

            public Field Field { get; }
            public string Name { get; }
            public bool IsEmpty => Field == null;

            private Target(Field field, string name)
            {
                Field = field;
                Name = name;
            }

            public Target(Field field)
                : this(field, field.Name) { }

            public bool IsMatch(string source)
            {
                return !IsEmpty && NameComparer.Default.Equals(Name, source);
            }
        }

        public class Mapping
        {
            public int Index { get; }
            public string Source { get; }
            public ListCollectionView<Target> Targets { get; }
            public Target Target => Targets.CurrentItem;

            public Mapping(int index, string source, IEnumerable<Target> targets)
            {
                Index = index;
                Source = source;
                Targets = new ListCollectionView<Target>(targets);
                Targets.MoveCurrentTo(target => target.IsMatch(source));
            }
        }

        public class MappingCollection : List<Mapping> { }

        public class State
        {
            public View View { get; }
            public string FilePath { get; set; }
            public RecordImporter Importer { get; set; }
            public MappingCollection Mappings { get; set; }

            public State(View view)
            {
                View = view;
            }
        }

        public class SetStrategyViewModel : StepViewModel<State>
        {
            public override string Title => Strings.ImportRecords_Lead_SetStrategy;

            public ICommand ImportFromViewCommand { get; }
            public ICommand ImportFromPackageCommand { get; }
            public ICommand ImportFromFileCommand { get; }

            public SetStrategyViewModel(State state)
                : base(state)
            {
                ImportFromViewCommand = new SyncCommand(ImportFromView);
                ImportFromPackageCommand = new SyncCommand(ImportFromPackage);
                ImportFromFileCommand = new SyncCommand(ImportFromFile);
            }

            public void ImportFromView()
            {
                Wizard.Close();
                State.View.ImportFromView();
                Wizard.Result = true;
            }

            public void ImportFromPackage()
            {
                Wizard.Close();
                State.View.ImportFromPackage();
                Wizard.Result = true;
            }

            public void ImportFromFile()
            {
                Wizard.GoForward(new SetFilePathViewModel(State));
            }
        }

        public class SetFilePathViewModel : StepViewModel<State>
        {
            private readonly IFileDialogService fileDialog;

            public override string Title => Strings.ImportRecords_Lead_SetFilePath;

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
                fileDialog.Filters = new string[]
                {
                    Strings.FileDialog_Filter_CsvFiles,
                    Strings.FileDialog_Filter_AllFiles
                };
                BrowseCommand = new SyncCommand(Browse);
            }

            public void Browse()
            {
                if (fileDialog.Open() != true)
                {
                    return;
                }
                FilePath = fileDialog.FileName;
            }

            public override bool CanContinue()
            {
                return FilePath != null;
            }

            public override async Task ContinueAsync()
            {
                State.FilePath = FilePath;
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                Wizard.GoForward(await progress.Run(() =>
                {
                    return SetMappingsViewModel.CreateAsync(State);
                }));
            }
        }

        public class SetMappingsViewModel : StepViewModel<State>
        {
            public static async Task<SetMappingsViewModel> CreateAsync(State state)
            {
                SetMappingsViewModel result = new SetMappingsViewModel(state);
                await result.InitializeAsync();
                return result;
            }

            public override string Title => Strings.ImportRecords_Lead_SetMappings;
            public RecordImporter Importer { get; private set; }
            public MappingCollection Mappings { get; private set; }

            private SetMappingsViewModel(State state)
                : base(state) { }

            private async Task InitializeAsync()
            {
                await Task.Run(() =>
                {
                    Stream stream = null;
                    TextReader reader = null;
                    Importer = null;
                    try
                    {
                        stream = File.Open(State.FilePath, FileMode.Open, FileAccess.Read);
                        reader = new StreamReader(stream);
                        Importer = new RecordImporter(State.View, reader);
                    }
                    catch
                    {
                        Importer?.Dispose();
                        reader?.Dispose();
                        stream?.Dispose();
                        throw;
                    }
                    Mappings = new MappingCollection();
                    State.View.LoadFields();
                    IEnumerable<Target> targets = State.View.Fields.DataFields.Cast<Field>()
                        .OrderBy(field => field, new FieldComparer.ByName())
                        .Select(field => new Target(field))
                        .Prepend(Target.Empty)
                        .ToList();
                    foreach (Iterator<string> header in Importer.Headers.Iterate())
                    {
                        Mappings.Add(new Mapping(header.Index, header.Value, targets));
                    }
                });
            }

            public override bool CanContinue()
            {
                return Mappings.Any(mapping => !mapping.Target.IsEmpty);
            }

            public override Task ContinueAsync()
            {
                State.Importer = Importer;
                State.Mappings = Mappings;
                Wizard.GoForward(new CommitViewModel(State));
                return Task.CompletedTask;
            }

            public override void Dispose()
            {
                Importer.Dispose();
                base.Dispose();
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
                    { Strings.Label_File, state.FilePath },
                    { Strings.Label_Mappings, state.Mappings }
                };
            }

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                progress.Lead = Strings.Lead_ImportingRecords;
                progress.CanBeCanceled = true;
                try
                {
                    bool result = await progress.Run(() =>
                    {
                        State.Importer.Reset();
                        foreach (Mapping mapping in State.Mappings)
                        {
                            if (mapping.Target.IsEmpty)
                            {
                                continue;
                            }
                            State.Importer.Map(mapping.Index, mapping.Target.Field);
                        }
                        State.Importer.Progress = new FormattingProgress<int>(progress, Strings.Body_ImportingRow);
                        return State.Importer.Import(progress.CancellationToken);
                    });
                    Wizard.Result = result;
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
                Wizard.Result == true
                ? Strings.ImportRecords_Lead_Close_Success
                : Strings.ImportRecords_Lead_Close_Failure;
            public override string ContinueAction => Strings.AccessText_Close;
            public string Errors { get; }

            public CloseViewModel(State state)
                : base(state)
            {
                if (state.Importer.Errors.Count() > 0)
                {
                    StringBuilder errors = new StringBuilder();
                    foreach (Exception error in state.Importer.Errors)
                    {
                        errors.AppendLine(error.Message);
                        if (error.InnerException != null)
                        {
                            errors.AppendLine($"  {error.InnerException.Message}");
                        }
                    }
                    Errors = errors.ToString().Trim();
                }
            }

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
