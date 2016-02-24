using Epi;
using Epi.Data;
using ERHMS.EpiInfo.Data;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

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
            View view = Project.GetViewByName((string)Views.SelectedItem);
            ViewData.ItemsSource = Driver.GetUndeletedViewData(view).DefaultView;
        }
    }
}
