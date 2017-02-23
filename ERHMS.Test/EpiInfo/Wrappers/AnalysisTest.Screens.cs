using Epi;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public partial class AnalysisTest
    {
        private class MainFormScreen : AutomationElementX
        {
            public readonly AutomationElementX txtTextArea;
            public readonly AutomationElementX statusStrip1;

            public MainFormScreen()
                : base(AutomationExtensions.FindFirstTopLevel(AutomationElement.AutomationIdProperty, "AnalysisMainForm"))
            {
                txtTextArea = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "txtTextArea");
                statusStrip1 = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "statusStrip1");
            }

            public AutomationElementX GetCloseDialogScreen()
            {
                return FindFirstX(TreeScope.Children, AutomationElement.NameProperty, "Close?");
            }

            public MappingDialogScreen GetMappingDialogScreen()
            {
                return new MappingDialogScreen(FindFirst(TreeScope.Children, AutomationElement.AutomationIdProperty, "MappingDialog"));
            }

            public PgmDialogScreen GetPgmDialogScreen()
            {
                return new PgmDialogScreen(FindFirst(TreeScope.Children, AutomationElement.AutomationIdProperty, "PgmDialog"));
            }

            public ReadDialogScreen GetReadDialogScreen()
            {
                return new ReadDialogScreen(FindFirst(TreeScope.Children, AutomationElement.AutomationIdProperty, "ReadDialog"));
            }

            public AutomationElementX GetSaveDialogScreen()
            {
                return FindFirstX(TreeScope.Children, AutomationElement.NameProperty, "Save?");
            }

            public WriteDialogScreen GetWriteDialogScreen()
            {
                return new WriteDialogScreen(FindFirst(TreeScope.Children, AutomationElement.AutomationIdProperty, "WriteDialog"));
            }

            public void WaitForReady()
            {
                AutomationExtensions.TryWait(() => statusStrip1.GetChildren().Any(child => child.Element.Current.Name == SharedStrings.READY));
            }
        }

        private class CsvExistingFileDialogScreen : AutomationElementX
        {
            public readonly AutomationElementX txtFileName;
            public readonly AutomationElementX btnOK;

            public CsvExistingFileDialogScreen(AutomationElement element)
                : base(element)
            {
                txtFileName = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "txtFileName");
                btnOK = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "btnOK");
            }
        }

        private class MappingDialogScreen : AutomationElementX
        {
            public readonly AutomationElementX dgvMappings;
            public readonly AutomationElementX btnOk;

            public MappingDialogScreen(AutomationElement element)
                : base(element)
            {
                dgvMappings = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "dgvMappings");
                btnOk = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "btnOk");
            }

            public void SetMapping(string source, string target)
            {
                foreach (AutomationElementX row in dgvMappings.GetChildren())
                {
                    IList<AutomationElementX> cells = row.GetChildren().ToList();
                    if (cells[0].Element.Current.Name.StartsWith("Source Row") && cells[0].Value.Current.Value == source)
                    {
                        cells[1].Value.SetValue(target);
                    }
                }
            }
        }

        private class OpenDialogScreen : AutomationElementX
        {
            public readonly AutomationElementX txtFileName;

            public OpenDialogScreen(AutomationElement element)
                : base(element)
            {
                txtFileName = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "1148");
            }
        }

        private class PgmDialogScreen : AutomationElementX
        {
            public readonly AutomationElementX btnFindProject;
            public readonly AutomationElementX cmbPrograms;
            public readonly AutomationElementX btnOK;

            public PgmDialogScreen(AutomationElement element)
                : base(element)
            {
                btnFindProject = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "btnFindProject");
                cmbPrograms = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "cmbPrograms");
                btnOK = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "btnOK");
            }

            public OpenDialogScreen GetOpenDialogScreen()
            {
                return new OpenDialogScreen(FindFirst(TreeScope.Children, AutomationElement.NameProperty, SharedStrings.SELECT_PROJECT));
            }
        }

        private class ReadDialogScreen : AutomationElementX
        {
            public readonly AutomationElementX cmbDataSourcePlugIns;
            public readonly AutomationElementX btnFindDataSource;
            public readonly AutomationElementX lvDataSourceObjects;
            public readonly AutomationElementX btnOK;

            public ReadDialogScreen(AutomationElement element)
                : base(element)
            {
                cmbDataSourcePlugIns = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "cmbDataSourcePlugIns");
                btnFindDataSource = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "btnFindDataSource");
                lvDataSourceObjects = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "lvDataSourceObjects");
                btnOK = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "btnOK");
            }

            public CsvExistingFileDialogScreen GetCsvExistingFileDialogScreen()
            {
                return new CsvExistingFileDialogScreen(FindFirst(TreeScope.Children, AutomationElement.AutomationIdProperty, "CsvExistingFileDialog"));
            }
        }

        private class WriteDialogScreen : AutomationElementX
        {
            public readonly AutomationElementX lbxVariables;
            public readonly AutomationElementX rdbReplace;
            public readonly AutomationElementX cmbOutputFormat;
            public readonly AutomationElementX btnGetFile;
            public readonly AutomationElementX txtFileName;
            public readonly AutomationElementX btnOK;

            public WriteDialogScreen(AutomationElement element)
                : base(element)
            {
                lbxVariables = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "lbxVariables");
                rdbReplace = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "rdbReplace");
                cmbOutputFormat = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "cmbOutputFormat");
                btnGetFile = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "btnGetFile");
                txtFileName = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "1001");
                btnOK = FindFirstX(TreeScope.Descendants, AutomationElement.AutomationIdProperty, "btnOK");
            }

            public CsvExistingFileDialogScreen GetCsvExistingFileDialogScreen()
            {
                return new CsvExistingFileDialogScreen(FindFirst(TreeScope.Children, AutomationElement.AutomationIdProperty, "CsvExistingFileDialog"));
            }
        }
    }
}
