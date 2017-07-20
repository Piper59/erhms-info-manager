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
                : base(AutomationElement.RootElement.FindFirst(TreeScope.Children, id: "AnalysisMainForm"))
            {
                txtTextArea = FindFirstX(TreeScope.Descendants, id: "txtTextArea");
                statusStrip1 = FindFirstX(TreeScope.Descendants, id: "statusStrip1");
            }

            public void WaitForReady()
            {
                AutomationExtensions.TryWait(() => statusStrip1.Element.GetChildren().Any(child => child.Current.Name == SharedStrings.READY));
            }

            public AutomationElementX GetCloseDialogScreen()
            {
                return FindFirstX(TreeScope.Children, name: "Close?");
            }

            public MappingDialogScreen GetMappingDialogScreen()
            {
                return new MappingDialogScreen(FindFirst(TreeScope.Children, id: "MappingDialog"));
            }

            public ReadDialogScreen GetReadDialogScreen()
            {
                return new ReadDialogScreen(FindFirst(TreeScope.Children, id: "ReadDialog"));
            }

            public WriteDialogScreen GetWriteDialogScreen()
            {
                return new WriteDialogScreen(FindFirst(TreeScope.Children, id: "WriteDialog"));
            }
        }

        private class CsvExistingFileDialogScreen : AutomationElementX
        {
            public readonly AutomationElementX txtFileName;
            public readonly AutomationElementX btnOK;

            public CsvExistingFileDialogScreen(AutomationElement element)
                : base(element)
            {
                txtFileName = FindFirstX(TreeScope.Descendants, id: "txtFileName");
                btnOK = FindFirstX(TreeScope.Descendants, id: "btnOK");
            }
        }

        private class MappingDialogScreen : AutomationElementX
        {
            public readonly AutomationElementX dgvMappings;
            public readonly AutomationElementX btnOk;

            public MappingDialogScreen(AutomationElement element)
                : base(element)
            {
                dgvMappings = FindFirstX(TreeScope.Descendants, id: "dgvMappings");
                btnOk = FindFirstX(TreeScope.Descendants, id: "btnOk");
            }

            public void SetMapping(string source, string target)
            {
                foreach (AutomationElement row in dgvMappings.Element.GetChildren())
                {
                    IList<AutomationElementX> cells = row.GetChildren()
                        .Select(cell => new AutomationElementX(cell))
                        .ToList();
                    if (cells[0].Element.Current.Name.StartsWith("Source Row") && cells[0].Value.Current.Value == source)
                    {
                        cells[1].Value.SetValue(target);
                    }
                }
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
                cmbDataSourcePlugIns = FindFirstX(TreeScope.Descendants, id: "cmbDataSourcePlugIns");
                btnFindDataSource = FindFirstX(TreeScope.Descendants, id: "btnFindDataSource");
                lvDataSourceObjects = FindFirstX(TreeScope.Descendants, id: "lvDataSourceObjects");
                btnOK = FindFirstX(TreeScope.Descendants, id: "btnOK");
            }

            public CsvExistingFileDialogScreen GetCsvExistingFileDialogScreen()
            {
                return new CsvExistingFileDialogScreen(FindFirst(TreeScope.Children, id: "CsvExistingFileDialog"));
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
                lbxVariables = FindFirstX(TreeScope.Descendants, id: "lbxVariables");
                rdbReplace = FindFirstX(TreeScope.Descendants, id: "rdbReplace");
                cmbOutputFormat = FindFirstX(TreeScope.Descendants, id: "cmbOutputFormat");
                btnGetFile = FindFirstX(TreeScope.Descendants, id: "btnGetFile");
                txtFileName = FindFirstX(TreeScope.Descendants, id: "1001");
                btnOK = FindFirstX(TreeScope.Descendants, id: "btnOK");
            }

            public CsvExistingFileDialogScreen GetCsvExistingFileDialogScreen()
            {
                return new CsvExistingFileDialogScreen(FindFirst(TreeScope.Children, id: "CsvExistingFileDialog"));
            }
        }
    }
}
