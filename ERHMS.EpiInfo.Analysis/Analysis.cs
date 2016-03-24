using Epi;
using Epi.Windows.Analysis.Dialogs;
using ERHMS.Utility;
using System;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Windows.Forms;
using View = Epi.View;

namespace ERHMS.EpiInfo.Analysis
{
    public class Analysis : Wrapper
    {
        private static Process Execute(Expression<Action<string[]>> expression, params string[] args)
        {
            return Execute(Module.Analysis, expression, args);
        }

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

        public static Process ImportFromFile(View view)
        {
            return Execute(args => Main_ImportFromFile(args), view.Project.FilePath, view.Name);
        }

        internal static void Main_ImportFromFile(string[] args)
        {
            string projectPath = args[0];
            string viewName = args[1];
            using (Project project = new Project(new FileInfo(projectPath)))
            {
                View view = project.Views[viewName];
                using (MainForm form = new MainForm())
                {
                    form.Load += (sender, e) =>
                    {
                        using (ReadDialog dialog = new ReadDialog(form))
                        {
                            if (dialog.ShowDialog() != DialogResult.OK)
                            {
                                return;
                            }
                            form.AddAndExecuteCommand(dialog.CommandText);
                        }
                        DataTable input = form.GetCurrentReadOutput();
                        MappingCollection mappings;
                        using (MappingDialog dialog = new MappingDialog(form, input, view))
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
                        string csvFileName = Path.ChangeExtension(csv.Name, "#csv");
                        form.AddCommand(string.Format(
                            "WRITE APPEND \"TEXT\" {{{0}}} : [{1}] {2}",
                            csvConnectionString,
                            csvFileName,
                            string.Join(" ", mappings.Targets)));
                        form.AddCommand(GetReadCommand(projectPath, viewName));
                        form.AddCommand(string.Format(
                            "MERGE {{{0}}}:{1} {2} :: {3}",
                            csvConnectionString,
                            csvFileName,
                            ColumnNames.GLOBAL_RECORD_ID,
                            mappings.GetKeyTarget()));
                        form.ExecuteCommands();
                    };
                    Application.Run(form);
                }
            }
        }
    }
}
