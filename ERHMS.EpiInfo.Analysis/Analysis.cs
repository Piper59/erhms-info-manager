using Epi;
using Epi.Fields;
using Epi.Windows.Analysis.Dialogs;
using ERHMS.EpiInfo.Communication;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using View = Epi.View;

namespace ERHMS.EpiInfo.Analysis
{
    public class Analysis : Wrapper
    {
        private static string GetReadCommand(string projectPath, string viewName)
        {
            return string.Format("READ {{{0}}}:{1}", projectPath, viewName);
        }

        private static string GetCsvConnectionString(FileInfo csv)
        {
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.Provider = "Microsoft.Jet.OLEDB.4.0";
            builder.DataSource = csv.DirectoryName;
            builder["Extended Properties"] = "text;HDR=Yes;FMT=Delimited";
            return builder.ConnectionString;
        }

        [STAThread]
        internal static void Main(string[] args)
        {
            MainBase(typeof(Analysis), args);
        }

        public static Process Execute()
        {
            return Execute(args => Main_Execute(args));
        }
        private static void Main_Execute(string[] args)
        {
            using (MainForm form = new MainForm())
            {
                Application.Run(form);
            }
        }

        public static Process OpenPgm(Pgm pgm)
        {
            FileInfo file = IOExtensions.GetTemporaryFile(extension: Pgm.FileExtension);
            File.WriteAllText(file.FullName, pgm.Content);
            return Execute(args => Main_OpenPgm(args), file.FullName);
        }
        private static void Main_OpenPgm(string[] args)
        {
            string pgmPath = args[0];
            using (MainForm form = new MainForm())
            {
                form.Load += (sender, e) =>
                {
                    form.Commands = File.ReadAllText(pgmPath);
                };
                Application.Run(form);
            }
        }

        public static Process Import(View target)
        {
            return Execute(args => Main_Import(args), target.Project.FilePath, target.Name);
        }
        private static void Main_Import(string[] args)
        {
            string projectPath = args[0];
            string viewName = args[1];
            using (Project project = new Project(projectPath))
            {
                View view = project.Views[viewName];
                using (MainForm form = new MainForm())
                {
                    form.Load += (sender, e) =>
                    {
                        string command;
                        using (ReadDialog dialog = new ReadDialog(form))
                        {
                            if (dialog.ShowDialog() != DialogResult.OK)
                            {
                                return;
                            }
                            command = dialog.CommandText;
                        }
                        form.AddCommand(command);
                        form.ExecuteCommand(command, () =>
                        {
                            IEnumerable<string> sources = form.GetOutput().Columns
                                .Cast<DataColumn>()
                                .Select(column => column.ColumnName);
                            IEnumerable<string> targets = view.Fields.TableColumnFields
                                .Cast<Field>()
                                .Select(field => field.Name);
                            MappingCollection mappings;
                            using (MappingDialog dialog = new MappingDialog(form, sources, targets))
                            {
                                if (dialog.ShowDialog() != DialogResult.OK)
                                {
                                    return;
                                }
                                mappings = dialog.GetMappings();
                            }
                            form.AddCommand(mappings.GetCommands());
                            FileInfo csv = IOExtensions.GetTemporaryFile(extension: ".csv");
                            string csvConnectionString = GetCsvConnectionString(csv);
                            string csvFileName = csv.Name.Replace(".", "#");
                            form.AddCommand(string.Format(
                                "WRITE REPLACE \"TEXT\" {{{0}}} : [{1}] {2}",
                                csvConnectionString,
                                csvFileName,
                                string.Join(" ", mappings.EscapedTargets)));
                            form.AddCommand(GetReadCommand(projectPath, viewName));
                            form.AddCommand(string.Format(
                                "MERGE {{{0}}}:{1} {2} :: {3}",
                                csvConnectionString,
                                csvFileName,
                                ColumnNames.GLOBAL_RECORD_ID,
                                mappings.GetKeyTarget()));
                            form.ExecuteCommands(() =>
                            {
                                IService service = Service.Connect();
                                if (service != null)
                                {
                                    service.OnViewDataImported(projectPath, viewName);
                                }
                            });
                        });
                    };
                    Application.Run(form);
                }
            }
        }

        public static Process Export(View source)
        {
            return Execute(args => Main_Export(args), source.Project.FilePath, source.Name);
        }
        private static void Main_Export(string[] args)
        {
            string projectPath = args[0];
            string viewName = args[1];
            using (MainForm form = new MainForm())
            {
                form.Load += (sender, e) =>
                {
                    string command = GetReadCommand(projectPath, viewName);
                    form.AddCommand(command);
                    form.ExecuteCommand(command, () =>
                    {
                        using (WriteDialog dialog = new WriteDialog(form))
                        {
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                form.AddCommand(dialog.CommandText);
                                if (dialog.ProcessingMode == CommandDesignDialog.CommandProcessingMode.Save_And_Execute)
                                {
                                    form.ExecuteCommand(dialog.CommandText);
                                }
                            }
                        }
                    });
                };
                Application.Run(form);
            }
        }
    }
}
