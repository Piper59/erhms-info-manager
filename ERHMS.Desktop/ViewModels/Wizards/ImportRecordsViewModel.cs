using Epi;
using Epi.Fields;
using ERHMS.Common.Linq;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Shared;
using ERHMS.Desktop.Wizards;
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
    public class ImportRecordsViewModel : WizardViewModel, IDisposable
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
                Targets.MoveCurrentTo(target => !target.IsEmpty && NameComparer.Default.Equals(target.Name, source));
            }
        }

        public class MappingCollection : List<Mapping>
        {
            public static MappingCollection FromImporter(RecordImporter importer)
            {
                IEnumerable<Field> fields = importer.View.Fields.DataFields.Cast<Field>()
                    .OrderBy(field => field, new FieldComparer.ByName());
                return new MappingCollection(importer.Headers, fields);
            }

            public MappingCollection(IEnumerable<string> headers, IEnumerable<Field> fields)
            {
                IEnumerable<Target> targets = fields.Select(field => new Target(field))
                    .Prepend(Target.Empty)
                    .ToList();
                foreach (Iterator<string> header in headers.Iterate())
                {
                    Add(new Mapping(header.Index, header.Value, targets));
                }
            }
        }

        public class SetFormatViewModel : StepViewModel<ImportRecordsViewModel>
        {
            public override string Title => Strings.ImportRecords_Lead_SetFormat;

            public ICommand ImportFromCsvCommand { get; }
            public ICommand ImportFromEdp7Command { get; }

            public SetFormatViewModel(ImportRecordsViewModel wizard)
                : base(wizard)
            {
                ImportFromCsvCommand = new SyncCommand(ImportFromCsv);
                ImportFromEdp7Command = new SyncCommand(ImportFromEdp7);
            }

            public void ImportFromCsv()
            {
                GoToStep(new SetFilePathViewModel(Wizard, this));
            }

            public void ImportFromEdp7()
            {
                Close();
                Wizard.View.ImportFromPackage();
                SetResult(true);
            }
        }

        public class SetFilePathViewModel : StepViewModel<ImportRecordsViewModel>
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

            public SetFilePathViewModel(ImportRecordsViewModel wizard, IStep antecedent)
                : base(wizard, antecedent)
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
                Wizard.FilePath = FilePath;
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                IStep step = await progress.Run(() =>
                {
                    return SetMappingsViewModel.CreateAsync(Wizard, this);
                });
                GoToStep(step);
            }
        }

        public class SetMappingsViewModel : StepViewModel<ImportRecordsViewModel>
        {
            public static async Task<SetMappingsViewModel> CreateAsync(ImportRecordsViewModel wizard, IStep antecedent)
            {
                SetMappingsViewModel result = new SetMappingsViewModel(wizard, antecedent);
                await result.InitializeAsync();
                return result;
            }

            public override string Title => Strings.ImportRecords_Lead_SetMappings;
            private RecordImporter Importer { get; set; }
            public MappingCollection Mappings { get; private set; }

            private SetMappingsViewModel(ImportRecordsViewModel wizard, IStep antecedent)
                : base(wizard, antecedent) { }

            private async Task InitializeAsync()
            {
                await Task.Run(() =>
                {
                    Stream stream = null;
                    TextReader reader = null;
                    Importer = null;
                    try
                    {
                        stream = File.Open(Wizard.FilePath, FileMode.Open, FileAccess.Read);
                        reader = new StreamReader(stream);
                        Importer = new RecordImporter(Wizard.View, reader);
                    }
                    catch
                    {
                        Importer?.Dispose();
                        reader?.Dispose();
                        stream?.Dispose();
                        throw;
                    }
                    Wizard.View.LoadFields();
                    Mappings = MappingCollection.FromImporter(Importer);
                });
            }

            public override void Return()
            {
                Importer.Dispose();
                base.Return();
            }

            public override bool CanContinue()
            {
                return Mappings.Any(mapping => !mapping.Target.IsEmpty);
            }

            public override Task ContinueAsync()
            {
                Wizard.Importer = Importer;
                Wizard.Mappings = Mappings;
                GoToStep(new CommitViewModel(Wizard, this));
                return Task.CompletedTask;
            }
        }

        public class CommitViewModel : StepViewModel<ImportRecordsViewModel>
        {
            private static IProgress<int> GetProgress(IProgressService progress)
            {
                return new Progress<int>(rowNumber =>
                {
                    progress.Report(string.Format(Strings.Body_ImportingRow, rowNumber));
                });
            }

            public override string Title => Strings.Lead_Commit;
            public override string ContinueAction => Strings.AccessText_Finish;
            public DetailsViewModel Details { get; }

            public CommitViewModel(ImportRecordsViewModel wizard, IStep antecedent)
                : base(wizard, antecedent)
            {
                Details = new DetailsViewModel
                {
                    { Strings.Label_File, wizard.FilePath },
                    { Strings.Label_Mappings, wizard.Mappings }
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
                bool result = await progress.Run(() =>
                {
                    Wizard.Importer.Reset();
                    foreach (Mapping mapping in Wizard.Mappings)
                    {
                        if (mapping.Target.IsEmpty)
                        {
                            continue;
                        }
                        Wizard.Importer.Map(mapping.Index, mapping.Target.Field);
                    }
                    try
                    {
                        return Wizard.Importer.Import(GetProgress(progress));
                    }
                    finally
                    {
                        Wizard.Importer.Dispose();
                    }
                });
                Commit(result);
                GoToStep(new CloseViewModel(Wizard, this));
            }
        }

        public class CloseViewModel : StepViewModel<ImportRecordsViewModel>
        {
            public override string Title =>
                Wizard.Succeeded
                ? Strings.ImportRecords_Lead_Close_Success
                : Strings.ImportRecords_Lead_Close_Failure;
            public override string ContinueAction => Strings.AccessText_Close;
            public string Errors { get; }

            public CloseViewModel(ImportRecordsViewModel wizard, IStep antecedent)
                : base(wizard, antecedent)
            {
                StringBuilder errors = new StringBuilder();
                foreach (Exception error in wizard.Importer.Errors)
                {
                    errors.AppendLine(error.Message);
                    if (error.InnerException != null)
                    {
                        errors.AppendLine($"  {error.InnerException.Message}");
                    }
                }
                Errors = errors.ToString().Trim();
                if (Errors.Length == 0)
                {
                    Errors = null;
                }
            }

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
        public string FilePath { get; private set; }
        private RecordImporter Importer { get; set; }
        private MappingCollection Mappings { get; set; }

        public ImportRecordsViewModel(View view)
        {
            View = view;
            Step = new SetFormatViewModel(this);
        }

        public void Dispose()
        {
            Importer?.Dispose();
        }
    }
}
