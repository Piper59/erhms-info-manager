using ERHMS.Data;
using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo.Data;
using ERHMS.EpiInfo.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ERHMS.Desktop
{
    // TODO: Add logging
    public static class Integration
    {
        public static string LookUpWorkerId(string firstName, string lastName, string workerId)
        {
            try
            {
                WorkerProject project = new WorkerProject(Settings.Default.WorkerProjectPath);
                RecordRepository repository = new RecordRepository(project.WorkerInfoView);
                string clauses = $"WHERE {repository.Quote("FirstName")} = @FirstName AND {repository.Quote("LastName")} = @LastName";
                ParameterCollection parameters = new ParameterCollection
                {
                    { "@FirstName", firstName },
                    { "@LastName", lastName }
                };
                IList<Record> records = repository.Select(clauses, parameters).ToList();
                if (records.Count == 1)
                {
                    return records[0].GlobalRecordId;
                }
                else
                {
                    // TODO
                    return workerId;
                }
            }
            catch (Exception ex)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine(ResX.HandledErrorLead);
                message.AppendLine();
                message.Append(ex.Message);
                MessageBox.Show(message.ToString(), ResX.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
