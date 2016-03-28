using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analysis;
using ERHMS.EpiInfo.Communication;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace ERHMS.Sandbox
{
    public partial class MainWindow : Window
    {
        private class Surveillance : ViewEntity
        {
            public string FirstName
            {
                get { return GetProperty<string>("FirstName"); }
                set { SetProperty("FirstName", value); }
            }

            public string LastName
            {
                get { return GetProperty<string>("LastName"); }
                set { SetProperty("LastName", value); }
            }

            public DateTime? BirthDate
            {
                get { return GetProperty<DateTime?>("BirthDate"); }
                set { SetProperty("BirthDate", value); }
            }

            public double? Year
            {
                get { return GetProperty<double?>("Year"); }
                set { SetProperty("Year", value); }
            }

            public double? Week
            {
                get { return GetProperty<double?>("Week"); }
                set { SetProperty("Week", value); }
            }

            public bool? Pregnant
            {
                get { return GetProperty<bool?>("Pregnant"); }
                set { SetProperty("Pregnant", value); }
            }
        }

        private Project project;
        private IDataDriver driver;
        private ViewEntityRepository<Surveillance> surveillances;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            App.Current.Service.RefreshingView += Service_RefreshingView;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            App.Current.Service.RefreshingView -= Service_RefreshingView;
        }

        private void Service_RefreshingView(object sender, ViewEventArgs e)
        {
            if (project == null || project.FilePath != e.ProjectPath || e.ViewName != "Surveillance")
            {
                return;
            }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Data.ItemsSource = surveillances.Select();
            }));
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Epi Info 7 Sample Project (Sample.prj)|Sample.prj";
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                project = new Project(new FileInfo(dialog.FileName));
                driver = DataDriverFactory.CreateDataDriver(project);
                surveillances = new ViewEntityRepository<Surveillance>(driver, project.Views["Surveillance"]);
                Buttons.IsEnabled = true;
            }
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            Data.ItemsSource = surveillances.Select();
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            Surveillance surveillance = surveillances.Create();
            surveillance.FirstName = "Steven";
            surveillance.LastName = "Williams";
            surveillance.BirthDate = new DateTime(1984, 10, 7);
            surveillance.Year = 2007;
            surveillance.Week = new Random().Next(1, 52);
            surveillance.Pregnant = false;
            surveillances.Save(surveillance);
            MessageBox.Show(surveillance.UniqueKey.ToString(), Title);
            Data.ItemsSource = surveillances.Select();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Surveillance surveillance = (Surveillance)Data.SelectedItem;
            if (surveillance == null)
            {
                return;
            }
            surveillance.Week = new Random().Next(1, 52);
            surveillances.Save(surveillance);
            Data.ItemsSource = surveillances.Select();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Surveillance surveillance = (Surveillance)Data.SelectedItem;
            if (surveillance == null)
            {
                return;
            }
            surveillances.Delete(surveillance);
            Data.ItemsSource = surveillances.Select();
        }

        private void Undelete_Click(object sender, RoutedEventArgs e)
        {
            Surveillance surveillance = (Surveillance)Data.SelectedItem;
            if (surveillance == null)
            {
                return;
            }
            surveillances.Undelete(surveillance);
            Data.ItemsSource = surveillances.Select();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            Analysis.ImportFromFile(surveillances.View);
        }
    }
}
