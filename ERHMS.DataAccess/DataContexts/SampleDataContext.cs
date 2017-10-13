using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.Utility;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.DataAccess
{
    public class SampleDataContext : DataContext
    {
        private static readonly Regex ReadCommandPattern = new Regex(@"(?<=READ \{)[^}]+(?=})");

        public static string GetFilePath()
        {
            return Path.Combine(Configuration.GetNewInstance().Directories.Project, "Sample", "Sample" + Project.FileExtension);
        }

        public static bool Exists()
        {
            return File.Exists(GetFilePath());
        }

        public static DataContext Create()
        {
            Log.Logger.DebugFormat("Creating sample data context");
            string projectPath = GetFilePath();
            Directory.CreateDirectory(Path.GetDirectoryName(projectPath));
            Assembly assembly = Assembly.GetExecutingAssembly();
            assembly.CopyManifestResourceTo("ERHMS.DataAccess.Resources.Sample.Sample.prj", projectPath);
            ProjectInfo projectInfo = ProjectInfo.Get(projectPath);
            projectInfo.SetAccessDatabase();
            string databasePath = Path.ChangeExtension(projectPath, OleDbExtensions.FileExtensions.Access);
            assembly.CopyManifestResourceTo("ERHMS.DataAccess.Resources.Sample.Sample.mdb", databasePath);
            Project project = new Project(projectPath);
            project.Version = assembly.GetName().Version;
            project.Save();
            SampleDataContext context = new SampleDataContext(project);
            context.InsertPgm("Safety Messages", "ERHMS.DataAccess.Resources.Sample.SafetyMessages.pgm7");
            context.InsertCanvas("Air Quality", "ERHMS.DataAccess.Resources.Sample.AirQuality.cvs7");
            context.InsertCanvas("Symptoms", "ERHMS.DataAccess.Resources.Sample.Symptoms.cvs7");
            context.Upgrade();
            return context;
        }

        private Incident Incident { get; set; }

        public SampleDataContext(Project project)
            : base(project)
        {
            Incident = Incidents.Select().Single();
        }

        private void InsertPgm(string pgmName, string resourceName)
        {
            EpiInfo.Pgm pgm = new EpiInfo.Pgm
            {
                Name = pgmName,
                Content = Assembly.GetExecutingAssembly().GetManifestResourceText(resourceName)
            };
            pgm.Content = ReadCommandPattern.Replace(pgm.Content, Project.FilePath);
            Project.InsertPgm(pgm);
            PgmLinks.Save(new PgmLink(true)
            {
                PgmId = pgm.PgmId,
                IncidentId = Incident.IncidentId
            });
        }

        private void InsertCanvas(string canvasName, string resourceName)
        {
            EpiInfo.Canvas canvas = new EpiInfo.Canvas
            {
                Name = canvasName,
                Content = Assembly.GetExecutingAssembly().GetManifestResourceText(resourceName)
            };
            canvas.SetProjectPath(Project.FilePath);
            Project.InsertCanvas(canvas);
            CanvasLinks.Save(new CanvasLink(true)
            {
                CanvasId = canvas.CanvasId,
                IncidentId = Incident.IncidentId
            });
        }
    }
}
