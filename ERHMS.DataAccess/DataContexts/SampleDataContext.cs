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
            string path = GetFilePath();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            Assembly assembly = Assembly.GetExecutingAssembly();
            assembly.CopyManifestResourceTo("ERHMS.DataAccess.Resources.Sample.Sample.prj", path);
            ProjectInfo projectInfo = ProjectInfo.Get(path);
            projectInfo.SetAccessConnectionString();
            assembly.CopyManifestResourceTo("ERHMS.DataAccess.Resources.Sample.Sample.mdb", Path.ChangeExtension(path, ".mdb"));
            SampleDataContext context = new SampleDataContext(new Project(path));
            context.InsertPgm("Safety Messages", "ERHMS.DataAccess.Resources.Sample.SafetyMessages.pgm7");
            context.InsertCanvas("Air Quality", "ERHMS.DataAccess.Resources.Sample.AirQuality.cvs7");
            context.InsertCanvas("Symptoms", "ERHMS.DataAccess.Resources.Sample.Symptoms.cvs7");
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
            PgmLinks.Save(new PgmLink
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
            CanvasLinks.Save(new CanvasLink
            {
                CanvasId = canvas.CanvasId,
                IncidentId = Incident.IncidentId
            });
        }
    }
}
