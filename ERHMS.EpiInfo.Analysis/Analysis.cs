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

        public static Process OpenPgm(Project project, Pgm pgm, bool execute, string tag = null)
        {
            FileInfo file = IOExtensions.GetTemporaryFile(extension: Pgm.FileExtension);
            File.WriteAllText(file.FullName, pgm.Content);
            return Execute(args => Main_OpenPgm(args), project.FilePath, pgm.PgmId.ToString(), pgm.Name, file.FullName, execute.ToString(), tag);
        }
        private static void Main_OpenPgm(string[] args)
        {
            string projectPath = args[0];
            int pgmId = int.Parse(args[1]);
            string pgmName = args[2];
            string pgmPath = args[3];
            bool execute = bool.Parse(args[4]);
            string tag = args[5];
            if (tag == "")
            {
                tag = null;
            }
            using (MainForm form = new MainForm())
            {
                form.Load += (sender, e) =>
                {
                    form.Commands = File.ReadAllText(pgmPath);
                    if (execute)
                    {
                        form.ExecuteCommands();
                    }
                };
                form.FormClosing += (sender, e) =>
                {
                    if (e.CloseReason == CloseReason.UserClosing)
                    {
                        DialogResult result = MessageBox.Show(
                            "Save changes to this program before exiting?",
                            "Save?",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            if (form.SavePgm(pgmName))
                            {
                                IService service = Service.Connect();
                                if (service != null)
                                {
                                    service.OnPgmSaved(projectPath, pgmId, tag);
                                }
                            }
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            e.Cancel = true;
                        }
                    }
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
                            dialog.StartPosition = FormStartPosition.CenterParent;
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
                            MappingCollection mappings = null;
                            while (true)
                            {
                                using (MappingDialog dialog = new MappingDialog(form, sources, targets))
                                {
                                    dialog.StartPosition = FormStartPosition.CenterParent;
                                    if (dialog.ShowDialog() == DialogResult.OK)
                                    {
                                        mappings = dialog.GetMappings();
                                    }
                                }
                                if (mappings == null || mappings.Count == 0)
                                {
                                    DialogResult result = MessageBox.Show(
                                        "No variables have been mapped. Cancel import?",
                                        "Cancel?",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Error);
                                    if (result == DialogResult.Yes)
                                    {
                                        return;
                                    }
                                }
                                else
                                {
                                    break;
                                }
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
                            form.AddCommand("TYPEOUT \"Importing data...\"");
                            form.AddCommand(string.Format(
                                "MERGE {{{0}}}:{1} {2} :: {3}",
                                csvConnectionString,
                                csvFileName,
                                ColumnNames.GLOBAL_RECORD_ID,
                                mappings.GetKeyTarget()));
                            form.AddCommand("TYPEOUT \"Data has been imported.\"");
                            form.Enabled = false;
                            form.ExecuteCommands(() =>
                            {
                                form.Enabled = true;
                                IService service = Service.Connect();
                                if (service != null)
                                {
                                    service.OnViewDataImported(projectPath, viewName);
                                }
                                if (MessageBox.Show("Data has been imported. Close Epi Info?", "Close?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    form.Close();
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
                            dialog.StartPosition = FormStartPosition.CenterParent;
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                form.AddCommand(dialog.CommandText);
                                if (dialog.ProcessingMode == CommandDesignDialog.CommandProcessingMode.Save_And_Execute)
                                {
                                    form.ExecuteCommand(dialog.CommandText, () =>
                                    {
                                        if (MessageBox.Show("Data has been exported. Close Epi Info?", "Close?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                        {
                                            form.Close();
                                        }
                                    });
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
