using Epi.Fields;
using Epi.Windows.Analysis.Dialogs;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using View = Epi.View;

namespace ERHMS.EpiInfo.Wrappers
{
    public class Analysis : Wrapper
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            MainBase(typeof(Analysis), args);
        }

        public static Wrapper OpenPgm(string name, string content, bool execute)
        {
            return Create(() => Main_OpenPgm(name, content, execute));
        }
        private static void Main_OpenPgm(string name, string content, bool execute)
        {
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
                if (e.CloseReason == CloseReason.UserClosing && form.Commands != content)
                {
                    string message = "Save changes to this program before exiting?";
                    DialogResult result = MessageBox.Show(form, message, "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        if (form.SavePgm(name))
                        {
                            RaiseEvent(WrapperEventType.PgmSaved);
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
            Project project = new Project(projectPath);
            View view = project.Views[viewName];
            MainForm form = new MainForm();
            form.Shown += (sender, e) =>
            {
                string command;
                using (ReadDialog dialog = new ReadDialog(form))
                {
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    if (dialog.ShowDialog(form) != DialogResult.OK)
                    {
                        string message = "No data source was selected. Import has been canceled. Close Epi Info?";
                        if (MessageBox.Show(form, message, "Close?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            form.Close();
                            return;
                        }
                    }
                    command = dialog.CommandText;
                }
                form.AddCommand(command);
                form.ExecuteCommand(command, true, () =>
                {
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
                    form.ExecuteCommands(true, () =>
                    {
                        RaiseEvent(WrapperEventType.ViewDataImported);
                        string message = "Data has been imported. Close Epi Info?";
                        if (MessageBox.Show(form, message, "Close?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            form.Close();
                        }
                    });
                });
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
                form.ExecuteCommand(command, true, () =>
                {
                    using (WriteDialog dialog = new WriteDialog(form))
                    {
                        dialog.StartPosition = FormStartPosition.CenterParent;
                        DialogResult result = dialog.ShowDialog(form);
                        if (result == DialogResult.OK)
                        {
                            form.AddCommand(dialog.CommandText);
                            if (dialog.ProcessingMode == CommandDesignDialog.CommandProcessingMode.Save_And_Execute)
                            {
                                form.ExecuteCommand(dialog.CommandText, false, () =>
                                {
                                    string message = "Data has been exported. Close Epi Info?";
                                    if (MessageBox.Show(form, message, "Close?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                    {
                                        form.Close();
                                    }
                                });
                            }
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            string message = "Export has been canceled. Close Epi Info?";
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
