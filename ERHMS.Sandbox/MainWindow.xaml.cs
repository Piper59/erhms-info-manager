using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using Microsoft.Win32;
using System;
using System.Collections;
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

        private IDataDriver driver;
        private CodeRepository sexes;
        private TableRepository<Addict> addicts;
        private ViewRepository<Surveillance> surveillances;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Epi Info 7 Sample Database (Sample.mdb)|Sample.mdb";
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                driver = AccessDriver.Create(dialog.FileName);
                sexes = new CodeRepository(driver, "codeSex", "Sex", false);
                addicts = new TableRepository<Addict>(driver, "Addicts");
                surveillances = new ViewRepository<Surveillance>(driver, "Surveillance");
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
            MessageBox.Show(surveillance.UniqueKey.ToString());
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
