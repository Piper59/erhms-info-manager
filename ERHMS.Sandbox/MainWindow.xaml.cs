using Epi;
using Epi.Collections;
using Epi.Data;
using Epi.Fields;
using ERHMS.EpiInfo.Data;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ERHMS.Sandbox
{
    public partial class MainWindow : Window
    {
        private Project Project { get; set; }

        private IDbDriver Driver
        {
            get { return Project.CollectedData.GetDbDriver(); }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Epi Info 7 Project Files (*.prj)|*.prj";
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                Views.Items.Clear();
                if (Project != null)
                {
                    Project.Dispose();
                }
                Project = new Project(dialog.FileName);
                foreach (View view in Project.Views)
                {
                    Views.Items.Add(view.Name);
                }
            }
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Views_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Views.SelectedItem == null)
            {
                return;
            }
            View view = Project.Views[(string)Views.SelectedItem];
            IDictionary<string, ICollection> fieldCollections = new Dictionary<string, ICollection>
            {
                { "All fields", view.Fields },
                { "Text fields", view.Fields.TextFields },
                { "Mirror fields", view.Fields.MirrorFields },
                { "Input fields", view.Fields.InputFields },
                { "Data fields", view.Fields.DataFields },
                { "Table column fields", view.Fields.TableColumnFields },
                { "Related fields", view.Fields.RelatedFields },
                { "Grid fields", view.Fields.GridFields }
            };
            Output.Inlines.Clear();
            foreach (KeyValuePair<string, ICollection> fieldCollection in fieldCollections)
            {
                Output.Inlines.AddRange(GetFieldInlines(fieldCollection.Key, fieldCollection.Value));
                Output.Inlines.Add(new LineBreak());
            }
        }

        private IEnumerable<Inline> GetFieldInlines(string title, ICollection fields)
        {
            yield return new Bold(new Run(string.Format("{0} ({1})", title, fields.Count)));
            yield return new LineBreak();
            foreach (IField field in fields)
            {
                yield return new Run(field.Name);
                yield return new LineBreak();
            }
        }
    }
}
