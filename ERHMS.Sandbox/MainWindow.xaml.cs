using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analysis;
using ERHMS.EpiInfo.AnalysisDashboard;
using ERHMS.EpiInfo.Communication;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Enter;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Action = System.Action;
using Project = ERHMS.EpiInfo.Project;

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
        private View view;
        private IDataDriver driver;
        private ViewEntityRepository<Surveillance> surveillances;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            App.Current.Service.RefreshingViewData += Service_RefreshingData;
            App.Current.Service.RefreshingRecordData += Service_RefreshingData;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            App.Current.Service.RefreshingViewData -= Service_RefreshingData;
            App.Current.Service.RefreshingRecordData -= Service_RefreshingData;
        }

        private void Service_RefreshingData(object sender, ViewEventArgs e)
        {
            if (project == null || project.FilePath != e.ProjectPath || e.ViewName != "Surveillance")
            {
                return;
            }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Refresh();
            }));
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Epi Info 7 Sample Project (Sample.prj)|Sample.prj"
            };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                project = new Project(dialog.FileName);
                view = project.Views["Surveillance"];
                driver = DataDriverFactory.CreateDataDriver(project);
                surveillances = new ViewEntityRepository<Surveillance>(driver, view);
            }
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Refresh()
        {
            int selectedIndex = Data.SelectedIndex;
            Data.ItemsSource = surveillances.Select();
            Data.SelectedIndex = selectedIndex;
        }

        private void DataSelect_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void DataInsert_Click(object sender, RoutedEventArgs e)
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
            Refresh();
        }

        private void DataUpdate_Click(object sender, RoutedEventArgs e)
        {
            Surveillance surveillance = (Surveillance)Data.SelectedItem;
            if (surveillance == null)
            {
                return;
            }
            surveillance.Week = new Random().Next(1, 52);
            surveillances.Save(surveillance);
            Refresh();
        }

        private void DataDelete_Click(object sender, RoutedEventArgs e)
        {
            Surveillance surveillance = (Surveillance)Data.SelectedItem;
            if (surveillance == null)
            {
                return;
            }
            surveillances.Delete(surveillance);
            Refresh();
        }

        private void DataUndelete_Click(object sender, RoutedEventArgs e)
        {
            Surveillance surveillance = (Surveillance)Data.SelectedItem;
            if (surveillance == null)
            {
                return;
            }
            surveillances.Undelete(surveillance);
            Refresh();
        }

        private void AnalysisImport_Click(object sender, RoutedEventArgs e)
        {
            Analysis.Import(view);
        }

        private void AnalysisExport_Click(object sender, RoutedEventArgs e)
        {
            Analysis.Export(view);
        }

        private void DashboardOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Epi Info 7 Dashboard Canvas File (*.cvs7)|*.cvs7",
                CheckFileExists = false
            };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                FileInfo file = new FileInfo(dialog.FileName);
                if (file.Exists)
                {
                    Canvas canvas;
                    if (!Canvas.TryRead(file, out canvas))
                    {
                        return;
                    }
                    AnalysisDashboard.OpenCanvas(canvas);
                }
                else
                {
                    Canvas canvas = Canvas.CreateForView(view, file);
                    AnalysisDashboard.OpenCanvas(canvas);
                }
            }
        }

        private void EnterInsert_Click(object sender, RoutedEventArgs e)
        {
            Enter.OpenView(view, new
            {
                FirstName = "John",
                LastName = "Doe"
            });
        }

        private void EnterUpdate_Click(object sender, RoutedEventArgs e)
        {
            Surveillance surveillance = (Surveillance)Data.SelectedItem;
            if (surveillance == null)
            {
                return;
            }
            Enter.OpenRecord(view, surveillance.UniqueKey.Value);
        }
    }
}
