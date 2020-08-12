using ERHMS.Data;
using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo.Data;
using ERHMS.EpiInfo.Projects;
using System.Linq;

namespace ERHMS.Desktop
{
    public static class Integration
    {
        public static string LookUpWorkerId(string firstName, string lastName)
        {
            WorkerProject project = new WorkerProject(Settings.Default.WorkerProjectPath);
            RecordRepository repository = new RecordRepository(project.WorkerInfoView);
            string clauses = $"WHERE {repository.Quote("FirstName")} = @FirstName AND {repository.Quote("LastName")} = @LastName";
            ParameterCollection parameters = new ParameterCollection
            {
                { "@FirstName", firstName },
                { "@LastName", lastName }
            };
            return repository.Select(clauses, parameters).FirstOrDefault()?.GlobalRecordId;
        }
    }
}
