using Epi;
using Epi.Data;
using ERHMS.Utility;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace ERHMS.EpiInfo
{
    public class Project : Epi.Project
    {
        public new const string FileExtension = ".prj";

        public static Project Create(string name, string description, DirectoryInfo location, string driver, string connectionString)
        {
            string safeConnectionString = DataExtensions.ToSafeString(connectionString);
            Log.Current.DebugFormat("Creating project: {0}, {1}, {2}, {3}", name, location.FullName, driver, safeConnectionString);
            Project project = new Project
            {
                Name = name,
                Description = description,
                Location = location,
                CollectedDataDriver = driver,
                CollectedDataConnectionString = connectionString
            };
            project.CollectedDataDbInfo.DBCnnStringBuilder.ConnectionString = connectionString;
            project.CollectedData.Initialize(project.CollectedDataDbInfo, driver, true);
            project.MetadataSource = MetadataSource.SameDb;
            project.Metadata.AttachDbDriver(project.CollectedData.GetDbDriver());
            project.Save();
            return project;
        }

        public new DirectoryInfo Location
        {
            get { return new DirectoryInfo(base.Location); }
            set { base.Location = value.FullName; }
        }

        public FileInfo File
        {
            get { return new FileInfo(FilePath); }
        }

        public IDbDriver Driver
        {
            get { return CollectedData.GetDbDriver(); }
        }

        private Project() { }

        public Project(string path)
            : base(path)
        {
            Log.Current.DebugFormat("Opening project: {0}", path);
        }

        public Project(FileInfo file)
            : this(file.FullName)
        { }

        public override void Save()
        {
            Log.Current.DebugFormat("Saving project: {0}", File.FullName);
            base.Save();
        }

        public DataTable GetFieldsAsDataTable()
        {
            string sql = "SELECT * FROM metaFields";
            return Driver.Select(Driver.CreateQuery(sql));
        }

        public IEnumerable<View> GetViews()
        {
            return Metadata.GetViews().Cast<View>();
        }

        public DataTable GetPgmsAsDataTable()
        {
            return Metadata.GetPgms();
        }

        public void DeleteView(int viewId)
        {
            ViewDeleter deleter = new ViewDeleter(this);
            deleter.DeleteViewAndDescendants(viewId);
        }

        public void DeletePgm(int pgmId)
        {
            Metadata.DeletePgm(pgmId);
        }
    }
}
