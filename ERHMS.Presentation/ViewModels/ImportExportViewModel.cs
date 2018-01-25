using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Web;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System;
using System.Linq;
using System.Threading.Tasks;
using Record = ERHMS.EpiInfo.Web.Record;
using View = Epi.View;

namespace ERHMS.Presentation.ViewModels
{
    public class ImportExportViewModel : ViewModelBase
    {
        public ImportExportViewModel(IServiceManager services)
            : base(services) { }

        public void ImportFromProject(int viewId)
        {
            Context.Project.CollectedData.EnsureDataTablesExist(viewId);
            ImportExport.ImportFromView(Services.Dialog.GetOwner(), Context.Project.GetViewById(viewId));
            if (viewId == Context.Responders.View.Id)
            {
                Services.Data.Refresh(typeof(Responder));
            }
        }

        public void ImportFromPackage(int viewId)
        {
            Context.Project.CollectedData.EnsureDataTablesExist(viewId);
            ImportExport.ImportFromPackage(Services.Dialog.GetOwner(), Context.Project.GetViewById(viewId));
            if (viewId == Context.Responders.View.Id)
            {
                Services.Data.Refresh(typeof(Responder));
            }
        }

        public async Task ImportFromFileAsync(int viewId)
        {
            Context.Project.CollectedData.EnsureDataTablesExist(viewId);
            await Services.Wrapper.InvokeAsync(Analysis.Import.Create(Context.Project.FilePath, Context.Project.GetViewById(viewId).Name));
            if (viewId == Context.Responders.View.Id)
            {
                Services.Data.Refresh(typeof(Responder));
            }
        }

        public async Task ImportFromWebAsync(int viewId)
        {
            View view = Context.Project.GetViewById(viewId);
            if (!view.IsWebSurvey())
            {
                await Services.Dialog.AlertAsync("Form has not been published to web.");
                return;
            }
            ConfigurationError error = ConfigurationError.None;
            if (!Service.IsConfigured(out error, true))
            {
                await Services.Dialog.AlertAsync(string.Format("{0} Please verify web survey settings.", error.GetErrorMessage()));
                Services.Document.ShowByType(() => new SettingsViewModel(Services));
                return;
            }
            Survey survey = null;
            bool unlinked = false;
            try
            {
                await Services.Dialog.BlockAsync("Importing data from web \u2026", () =>
                {
                    if (!Service.IsConfigured(out error))
                    {
                        return;
                    }
                    survey = Service.GetSurvey(view.WebSurveyId);
                    if (survey == null)
                    {
                        return;
                    }
                    Context.Project.CollectedData.EnsureDataTablesExist(viewId);
                    ILookup<string, Responder> responders = Context.Responders.Select()
                        .ToLookup(responder => responder.EmailAddress, StringComparer.OrdinalIgnoreCase);
                    ViewEntityRepository<ViewEntity> entities = new ViewEntityRepository<ViewEntity>(Context.Database, view);
                    foreach (Record record in Service.GetRecords(survey))
                    {
                        if (record.ContainsKey("ResponderID") && record.ContainsKey("ResponderEmailAddress"))
                        {
                            ViewEntity entity = entities.SelectById(record.GlobalRecordId);
                            if (entity != null && !string.IsNullOrEmpty(entity.GetProperty("ResponderID") as string))
                            {
                                continue;
                            }
                            Responder responder = responders[record["ResponderEmailAddress"]].FirstOrDefault();
                            if (responder == null)
                            {
                                unlinked = true;
                            }
                            else
                            {
                                record["ResponderID"] = responder.ResponderId;
                            }
                        }
                        entities.Save(record);
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to import data from web", ex);
                await Services.Dialog.AlertAsync("Failed to import data from web. Please verify web survey settings.", ex);
                Services.Document.ShowByType(() => new SettingsViewModel(Services));
                return;
            }
            if (error != ConfigurationError.None)
            {
                await Services.Dialog.AlertAsync(string.Format("{0} Please verify web survey settings.", error.GetErrorMessage()));
                Services.Document.ShowByType(() => new SettingsViewModel(Services));
            }
            else if (survey == null)
            {
                await Services.Dialog.AlertAsync("Failed to retrieve web survey details. Please verify web survey settings.");
                Services.Document.ShowByType(() => new SettingsViewModel(Services));
            }
            else
            {
                Services.Dialog.Notify("Data has been imported from web.");
                if (view.Id == Context.Responders.View.Id)
                {
                    Services.Data.Refresh(typeof(Responder));
                }
                if (unlinked)
                {
                    string message = string.Join(" ", new string[]
                    {
                        "One or more records could not be linked to a responder.",
                        "Review these records?"
                    });
                    if (await Services.Dialog.ConfirmAsync(message, "Review"))
                    {
                        Services.Document.Show(
                            model => model.View.Id == view.Id,
                            () => new RecordListViewModel(Services, view));
                    }
                }
            }
        }

        public void ImportFromMobile(int viewId)
        {
            Context.Project.CollectedData.EnsureDataTablesExist(viewId);
            ImportExport.ImportFromMobile(Services.Dialog.GetOwner(), Context.Project.GetViewById(viewId));
            if (viewId == Context.Responders.View.Id)
            {
                Services.Data.Refresh(typeof(Responder));
            }
        }

        public void ExportToPackage(int viewId)
        {
            Context.Project.CollectedData.EnsureDataTablesExist(viewId);
            ImportExport.ExportToPackage(Services.Dialog.GetOwner(), Context.Project.GetViewById(viewId));
        }

        public async Task ExportToFileAsync(int viewId)
        {
            Context.Project.CollectedData.EnsureDataTablesExist(viewId);
            await Services.Wrapper.InvokeAsync(Analysis.Export.Create(Context.Project.FilePath, Context.Project.GetViewById(viewId).Name));
        }
    }
}
