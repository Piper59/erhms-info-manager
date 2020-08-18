using ERHMS.Data;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using ERHMS.EpiInfo.Data;
using ERHMS.EpiInfo.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ERHMS.Desktop
{
    public static class Integration
    {
        private class DialogService : IDialogService
        {
            public bool? Show(DialogInfo info)
            {
                Window window = new DialogView
                {
                    DataContext = new DialogViewModel(info),
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                info.Sound?.Play();
                return window.ShowDialog();
            }
        }

        static Integration()
        {
            App.ConfigureLog();
            ConfigureServices();
        }

        private static void ConfigureServices()
        {
            ServiceProvider.Install<IDialogService>(() => new DialogService());
        }

        private static void Run(Action action, [CallerMemberName] string methodName = null)
        {
            string title = $"{nameof(ERHMS)}.{nameof(Integration)}.{methodName}";
            Log.Default.Debug($"Executing: {title}");
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Log.Default.Warn(ex);
                ServiceProvider.Resolve<IDialogService>().Show(new DialogInfo(DialogInfoPreset.Error)
                {
                    Lead = ResX.HandledErrorLead,
                    Body = ex.Message,
                    Details = ex.ToString()
                });
            }
            Log.Default.Debug($"Executed: {title}");
        }

        public static string LookUpWorkerId(string firstName, string lastName, string workerId)
        {
            Run(() =>
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
                    workerId = records[0].GlobalRecordId;
                }
                else
                {
                    // TODO
                }
            });
            return workerId;
        }
    }
}
