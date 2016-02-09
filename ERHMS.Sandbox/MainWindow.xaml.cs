using Epi;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Documents;

namespace ERHMS.Sandbox
{
    public partial class MainWindow : Window
    {
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
                Views.Inlines.Clear();
                using (Project project = new Project(dialog.FileName))
                {
                    foreach (View view in project.Views)
                    {
                        Views.Inlines.Add(new Run(view.Name));
                        Views.Inlines.Add(new LineBreak());
                    }
                }
            }
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
