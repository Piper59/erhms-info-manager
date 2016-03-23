using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using Microsoft.Win32;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows;

namespace ERHMS.Sandbox
{
    public partial class MainWindow : Window
    {
        private class Addict : TableEntity
        {
            public override string Guid
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            public double? Clinic
            {
                get { return GetProperty<double?>("Clinic"); }
                set { SetProperty("Clinic", value); }
            }

            public double? Status
            {
                get { return GetProperty<double?>("Status"); }
                set { SetProperty("Status", value); }
            }

            public double? SurvivalTimeInDays
            {
                get { return GetProperty<double?>("Survival_Time_Days"); }
                set { SetProperty("Survival_Time_Days", value); }
            }

            public double? PrisonRecord
            {
                get { return GetProperty<double?>("Prison_Record"); }
                set { SetProperty("Prison_Record", value); }
            }

            public double? MethadoneDoseInMgPerDay
            {
                get { return GetProperty<double?>("Methadone_dose__mg_day_"); }
                set { SetProperty("Methadone_dose__mg_day_", value); }
            }
        }

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
        private CodeRepository sexes;
        private TableEntityRepository<Addict> addicts;
        private ViewEntityRepository<Surveillance> surveillances;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Epi Info 7 Sample Project (Sample.prj)|Sample.prj";
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                project = new Project(new FileInfo(dialog.FileName));
                driver = DataDriverFactory.CreateDataDriver(project);
                sexes = new CodeRepository(driver, "codeSex", "Sex", false);
                addicts = new TableEntityRepository<Addict>(driver, "Addicts");
                surveillances = new ViewEntityRepository<Surveillance>(driver, project.Views["Surveillance"]);
            }
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private IEnumerable GetDataSource(string tableName)
        {
            switch (tableName)
            {
                case "codeSex":
                    return sexes.Select();
                case "Addicts":
                    return addicts.Select();
                case "Surveillance":
                    return surveillances.Select();
                default:
                    return null;
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            Data.ItemsSource = GetDataSource((string)TableName.SelectedValue);
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
        }

        private Surveillance GetSurveillanceByName(string firstName, string lastName)
        {
            DataParameterCollection parameters = new DataParameterCollection(driver);
            parameters.AddByValue(firstName);
            parameters.AddByValue(lastName);
            string sql = parameters.Format("FirstName = {0} AND LastName = {1}");
            return surveillances.Select(new DataPredicate(sql, parameters)).Single();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Surveillance surveillance = GetSurveillanceByName("John", "Smith");
            surveillance.Week = new Random().Next(1, 52);
            surveillances.Save(surveillance);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Surveillance surveillance = GetSurveillanceByName("John", "Smith");
            surveillances.Delete(surveillance);
        }

        private void Undelete_Click(object sender, RoutedEventArgs e)
        {
            Surveillance surveillance = GetSurveillanceByName("John", "Smith");
            surveillances.Undelete(surveillance);
        }
    }
}
