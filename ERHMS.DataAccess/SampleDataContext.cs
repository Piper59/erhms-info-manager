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

        public static DataContext Create()
        {
            Log.Logger.DebugFormat("Creating sample data context");
            Configuration configuration = Configuration.GetNewInstance();
            string projectPath = Path.Combine(configuration.Directories.Project, "Sample", "Sample" + Project.FileExtension);
            Directory.CreateDirectory(Path.GetDirectoryName(projectPath));
            Assembly assembly = Assembly.GetExecutingAssembly();
            assembly.CopyManifestResourceTo("ERHMS.DataAccess.Resources.Sample.Sample.prj", projectPath);
            ProjectInfo projectInfo = ProjectInfo.Get(projectPath);
            projectInfo.SetAccessConnectionString();
            assembly.CopyManifestResourceTo("ERHMS.DataAccess.Resources.Sample.Sample.mdb", Path.ChangeExtension(projectPath, ".mdb"));
            SampleDataContext dataContext = new SampleDataContext(new Project(projectPath));
            dataContext.InsertPgm("Safety Messages", "ERHMS.DataAccess.Resources.Sample.SafetyMessages.pgm7");
            dataContext.InsertCanvas("Air Quality", "ERHMS.DataAccess.Resources.Sample.AirQuality.cvs7");
            dataContext.InsertCanvas("Symptoms", "ERHMS.DataAccess.Resources.Sample.Symptoms.cvs7");
            return dataContext;
        }

        private Incident Incident { get; set; }

        public SampleDataContext(Project project)
            : base(project)
        {
            Incident = Incidents.Select().Single();
        }

        private void InsertPgm(string pgmName, string resourceName)
        {
            Pgm pgm = new Pgm
            {
                Name = pgmName,
                Content = Assembly.GetExecutingAssembly().GetManifestResourceText(resourceName)
            };
            pgm.Content = ReadCommandPattern.Replace(pgm.Content, Project.FilePath);
            Project.InsertPgm(pgm);
            PgmLink pgmLink = PgmLinks.Create();
            pgmLink.PgmId = pgm.PgmId;
            pgmLink.IncidentId = Incident.IncidentId;
            PgmLinks.Save(pgmLink);
        }

        private void InsertCanvas(string canvasName, string resourceName)
        {
            Canvas canvas = new Canvas
            {
                Name = canvasName,
                Content = Assembly.GetExecutingAssembly().GetManifestResourceText(resourceName)
            };
            canvas.SetProjectPath(Project.FilePath);
            Project.InsertCanvas(canvas);
            CanvasLink canvasLink = CanvasLinks.Create();
            canvasLink.CanvasId = canvas.CanvasId;
            canvasLink.IncidentId = Incident.IncidentId;
            CanvasLinks.Save(canvasLink);
        }
    }
}
