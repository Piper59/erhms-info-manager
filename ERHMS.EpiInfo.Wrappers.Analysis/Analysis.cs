using Epi.Fields;
using Epi.Windows.Analysis.Dialogs;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using View = Epi.View;

namespace ERHMS.EpiInfo.Wrappers
{
    public class Analysis
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            Wrapper.MainBase(args);
        }

        public class OpenPgm : Wrapper
        {
            private static string name;
            private static string content;
            private static bool execute;
            private static MainForm form;

            public static Wrapper Create(string name, string content, bool execute)
            {
                return Create(() => Execute(name, content, execute));
            }

            private static void Execute(string name, string content, bool execute)
            {
                OpenPgm.name = name;
                OpenPgm.content = content.Trim().NormalizeNewLines();
                OpenPgm.execute = execute;
                form = new MainForm();
                form.Shown += Form_Shown;
                form.FormClosing += Form_FormClosing;
                Application.Run(form);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                form.Commands = content;
                if (execute)
                {
                    form.ExecuteCommands(true);
                }
            }

            private static void Form_FormClosing(object sender, FormClosingEventArgs e)
            {
                string commands = form.Commands.Trim().NormalizeNewLines();
                if (e.CloseReason == CloseReason.UserClosing && commands != content)
                {
                    string message = "Save changes to this program before exiting?";
                    DialogResult result = MessageBox.Show(form, message, "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        if (form.SavePgm(name))
                        {
                            RaiseEvent(WrapperEventType.PgmSaved);
                        }
                        else
                        {
                            e.Cancel = true;
                        }
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        public class Import : Wrapper
        {
            private static string projectPath;
            private static string viewName;
            private static View view;
            private static MainForm form;

            public static Wrapper Create(string projectPath, string viewName)
            {
                return Create(() => Execute(projectPath, viewName));
            }

            private static void Execute(string projectPath, string viewName)
            {
                Import.projectPath = projectPath;
                Import.viewName = viewName;
                form = new MainForm();
                form.Shown += Form_Shown;
                Application.Run(form);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                BackgroundWorker worker = form.GetBackgroundWorker("Loading...", true);
                worker.DoWork += Step1;
                worker.RunWorkerCompleted += Step2;
                worker.RunWorkerAsync();
            }

            private static void Step1(object sender, DoWorkEventArgs e)
            {
                Project project = new Project(projectPath);
                view = project.Views[viewName];
            }

            private static void Step2(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Panic(form, "An error occurred while loading the project.");
                    return;
                }
                string command;
                using (ReadDialog dialog = new ReadDialog(form))
                {
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    if (dialog.ShowDialog(form) != DialogResult.OK)
                    {
                        TryClose(form, "No data source was selected. Import has been canceled.", MessageBoxIcon.Warning);
                        return;
                    }
                    command = dialog.CommandText;
                }
                form.AddCommand(command);
                form.ExecuteCommand(command, true, Step3);
            }

            private static void Step3(Exception ex)
            {
                if (ex != null)
                {
                    Panic(form, "An error occurred while reading data.");
                    return;
                }
                ICollection<string> sources = form.GetOutput().Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToList();
                ICollection<string> targets = view.Fields.TableColumnFields.Cast<Field>().Select(field => field.Name).ToList();
                MappingCollection mappings = null;
                while (true)
                {
                    using (MappingDialog dialog = new MappingDialog(form, sources, targets))
                    {
                        dialog.StartPosition = FormStartPosition.CenterParent;
                        if (dialog.ShowDialog(form) == DialogResult.OK)
                        {
                            mappings = dialog.GetMappings();
                        }
                    }
                    if (mappings == null || mappings.Count == 0)
                    {
                        string message = "No variables were mapped. Retry mapping or cancel import?";
                        if (MessageBox.Show(form, message, "Retry?", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                        {
                            form.Close();
                            return;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                form.AddCommand(mappings.GetCommands());
                string csvPath = IOExtensions.GetTempFileName("ERHMS_{0:N}.csv");
                form.AddCommand(Commands.WriteCsv(csvPath, mappings.GetTargets()));
                form.AddCommand(Commands.Read(projectPath, viewName));
                form.AddCommand(Commands.MergeCsv(csvPath, mappings.GetKeyTarget()));
                form.ExecuteCommands(true, Step4);
            }

            private static void Step4(Exception ex)
            {
                if (ex != null)
                {
                    Panic(form, "An error occurred while importing data.");
                    return;
                }
                RaiseEvent(WrapperEventType.ViewDataImported);
                TryClose(form, "Data has been imported.");
            }
        }

        public class Export : Wrapper
        {
            private static string projectPath;
            private static string viewName;
            private static MainForm form;

            public static Wrapper Create(string projectPath, string viewName)
            {
                return Create(() => Execute(projectPath, viewName));
            }

            private static void Execute(string projectPath, string viewName)
            {
                Export.projectPath = projectPath;
                Export.viewName = viewName;
                form = new MainForm();
                form.Shown += Form_Shown;
                Application.Run(form);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                string command = Commands.Read(projectPath, viewName);
                form.AddCommand(command);
                form.ExecuteCommand(command, true, Step1);
            }

            private static void Step1(Exception ex)
            {
                if (ex != null)
                {
                    Panic(form, "An error occurred while reading data.");
                    return;
                }
                using (WriteDialog dialog = new WriteDialog(form))
                {
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    DialogResult result = dialog.ShowDialog(form);
                    if (result == DialogResult.OK)
                    {
                        form.AddCommand(dialog.CommandText);
                        if (dialog.ProcessingMode == CommandDesignDialog.CommandProcessingMode.Save_And_Execute)
                        {
                            form.ExecuteCommand(dialog.CommandText, false, Step2);
                        }
                    }
                    else
                    {
                        TryClose(form, "Export has been canceled.", MessageBoxIcon.Warning);
                    }
                }
            }

            private static void Step2(Exception ex)
            {
                if (ex != null)
                {
                    Panic(form, "An error occurred while exporting data.");
                    return;
                }
                TryClose(form, "Data has been exported.");
            }
        }
    }
}
