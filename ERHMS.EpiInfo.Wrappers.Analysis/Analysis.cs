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
    // TODO: Clean up
    // Separate into one file per main method?
    // Consider ERHMS.Test.Wrappers.Test formatting as well
    // Put callbacks into methods called Step1, Step2, ...
    public class Analysis : Wrapper
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            MainBase(args);
        }

        public static Wrapper OpenPgm(string name, string content, bool execute)
        {
            return Create(() => Main_OpenPgm(name, content, execute));
        }
        private static void Main_OpenPgm(string name, string content, bool execute)
        {
            content = content.Trim().NormalizeNewLines();
            MainForm form = new MainForm();
            form.Shown += (sender, e) =>
            {
                form.Commands = content;
                if (execute)
                {
                    form.ExecuteCommands(true);
                }
            };
            form.FormClosing += (sender, e) =>
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
            };
            Application.Run(form);
        }

        public static Wrapper Import(string projectPath, string viewName)
        {
            return Create(() => Main_Import(projectPath, viewName));
        }
        private static void Main_Import(string projectPath, string viewName)
        {
            MainForm form = new MainForm();
            form.Shown += (sender, e) =>
            {
                Project project = null;
                View view = null;
                BackgroundWorker worker = form.GetBackgroundWorker("Loading...", true);
                worker.DoWork += (_sender, _e) =>
                {
                    project = new Project(projectPath);
                    view = project.Views[viewName];
                };
                worker.RunWorkerCompleted += (_sender, _e) =>
                {
                    if (_e.Error != null)
                    {
                        string message = "An error occurred while loading the project. Epi Info\u2122 must shut down.";
                        MessageBox.Show(form, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        form.Close();
                        return;
                    }
                    string command;
                    using (ReadDialog dialog = new ReadDialog(form))
                    {
                        dialog.StartPosition = FormStartPosition.CenterParent;
                        if (dialog.ShowDialog(form) != DialogResult.OK)
                        {
                            string message = "No data source was selected. Import has been canceled. Close Epi Info\u2122?";
                            if (MessageBox.Show(form, message, "Close?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                            {
                                form.Close();
                                return;
                            }
                        }
                        command = dialog.CommandText;
                    }
                    form.AddCommand(command);
                    form.ExecuteCommand(command, true, ex =>
                    {
                        if (ex != null)
                        {
                            string message = "An error occurred while reading data. Epi Info\u2122 must shut down.";
                            MessageBox.Show(form, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            form.Close();
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
                        form.AddCommand(Commands.Type("Importing data..."));
                        form.AddCommand(Commands.MergeCsv(csvPath, mappings.GetKeyTarget()));
                        form.AddCommand(Commands.Type("Data has been imported."));
                        form.ExecuteCommands(true, _ex =>
                        {
                            if (_ex != null)
                            {
                                string _message = "An error occurred while importing data. Epi Info\u2122 must shut down.";
                                MessageBox.Show(form, _message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                form.Close();
                                return;
                            }
                            RaiseEvent(WrapperEventType.ViewDataImported);
                            string message = "Data has been imported. Close Epi Info\u2122?";
                            if (MessageBox.Show(form, message, "Close?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                form.Close();
                            }
                        });
                    });
                };
                worker.RunWorkerAsync();
            };
            Application.Run(form);
        }

        public static Wrapper Export(string projectPath, string viewName)
        {
            return Create(() => Main_Export(projectPath, viewName));
        }
        private static void Main_Export(string projectPath, string viewName)
        {
            MainForm form = new MainForm();
            form.Shown += (sender, e) =>
            {
                string command = Commands.Read(projectPath, viewName);
                form.AddCommand(command);
                form.ExecuteCommand(command, true, ex =>
                {
                    if (ex != null)
                    {
                        string message = "An error occurred while reading data. Epi Info\u2122 must shut down.";
                        MessageBox.Show(form, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        form.Close();
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
                                form.ExecuteCommand(dialog.CommandText, false, _ex =>
                                {
                                    if (_ex != null)
                                    {
                                        string _message = "An error occurred while exporting data. Epi Info\u2122 must shut down.";
                                        MessageBox.Show(form, _message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        form.Close();
                                        return;
                                    }
                                    string message = "Data has been exported. Close Epi Info\u2122?";
                                    if (MessageBox.Show(form, message, "Close?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                    {
                                        form.Close();
                                    }
                                });
                            }
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            string message = "Export has been canceled. Close Epi Info\u2122?";
                            if (MessageBox.Show(form, message, "Close?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                            {
                                form.Close();
                            }
                        }
                    }
                });
            };
            Application.Run(form);
        }
    }
}
