using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Web;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using ERHMS.Presentation.ViewModels;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Base = ERHMS.EpiInfo.ImportExport;
using View = Epi.View;

namespace ERHMS.Presentation
{
    public static class ImportExport
    {
        private static DataContext Context
        {
            get { return ServiceLocator.Data.Context; }
        }

        public static void ImportFromProject(int viewId)
        {
            Context.Project.CollectedData.EnsureDataTablesExist(viewId);
            Base.ImportFromView(ServiceLocator.Dialog.GetOwner(), Context.Project.GetViewById(viewId));
            if (viewId == Context.Responders.View.Id)
            {
                ServiceLocator.Data.Refresh(typeof(Responder));
            }
        }

        public static void ImportFromPackage(int viewId)
        {
            Context.Project.CollectedData.EnsureDataTablesExist(viewId);
            Base.ImportFromPackage(ServiceLocator.Dialog.GetOwner(), Context.Project.GetViewById(viewId));
            if (viewId == Context.Responders.View.Id)
            {
                ServiceLocator.Data.Refresh(typeof(Responder));
            }
        }

        public static async Task ImportFromFileAsync(int viewId)
        {
            Context.Project.CollectedData.EnsureDataTablesExist(viewId);
            Wrapper wrapper = Analysis.Import.Create(Context.Project.FilePath, Context.Project.GetViewById(viewId).Name);
            await ServiceLocator.Wrapper.InvokeAsync(wrapper);
            if (viewId == Context.Responders.View.Id)
            {
                ServiceLocator.Data.Refresh(typeof(Responder));
            }
        }

        public static async Task ImportFromWebAsync(int viewId)
        {
            Context.Project.CollectedData.EnsureDataTablesExist(viewId);
            View view = Context.Project.GetViewById(viewId);
            if (!view.IsWebSurvey())
            {
                await ServiceLocator.Dialog.AlertAsync(Resources.WebViewNotPublished);
                return;
            }
            ConfigurationError error = ConfigurationError.None;
            if (!Service.IsConfigured(out error, local: true))
            {
                await ServiceLocator.Dialog.AlertAsync(string.Format(Resources.WebConfigure, error.GetErrorMessage()));
                ServiceLocator.Document.ShowSettings();
                return;
            }
            Survey survey = null;
            ICollection<RecordViewModel> records = new List<RecordViewModel>();
            try
            {
                await ServiceLocator.Dialog.BlockAsync(Resources.WebImporting, () =>
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
                    WebMapper mapper = new WebMapper(Context, view);
                    foreach (Record record in Service.GetRecords(survey).OrderBy(record => record.ModifiedOn))
                    {
                        Tuple<ViewEntity, Responder> tuple = mapper.Map(record);
                        ViewEntity entity = tuple.Item1;
                        Responder responder = tuple.Item2;
                        if (entity != null)
                        {
                            record.EntityId = entity.GlobalRecordId;
                        }
                        if (responder != null && record.ContainsKey(FieldNames.ResponderId))
                        {
                            record[FieldNames.ResponderId] = responder.ResponderId;
                        }
                        records.Add(new RecordViewModel
                        {
                            Import = entity == null || record.ModifiedOn > entity.ModifiedOn,
                            Record = record,
                            Entity = entity,
                            Responder = responder
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to import data from web", ex);
                await ServiceLocator.Dialog.ShowErrorAsync(Resources.WebImportFailed, ex);
                ServiceLocator.Document.ShowSettings();
                return;
            }
            if (error != ConfigurationError.None)
            {
                await ServiceLocator.Dialog.AlertAsync(string.Format(Resources.WebConfigure, error.GetErrorMessage()));
                ServiceLocator.Document.ShowSettings();
            }
            else if (survey == null)
            {
                await ServiceLocator.Dialog.AlertAsync(Resources.WebGetSurveyFailed);
                ServiceLocator.Document.ShowSettings();
            }
            else
            {
                ServiceLocator.Document.Show(() => new RecordListViewModel(view, records));
            }
        }

        public static void ImportFromMobile(int viewId)
        {
            Context.Project.CollectedData.EnsureDataTablesExist(viewId);
            Base.ImportFromMobile(ServiceLocator.Dialog.GetOwner(), Context.Project.GetViewById(viewId));
            if (viewId == Context.Responders.View.Id)
            {
                ServiceLocator.Data.Refresh(typeof(Responder));
            }
        }

        public static void ExportToPackage(int viewId)
        {
            Context.Project.CollectedData.EnsureDataTablesExist(viewId);
            Base.ExportToPackage(ServiceLocator.Dialog.GetOwner(), Context.Project.GetViewById(viewId));
        }

        public static async Task ExportToFileAsync(int viewId)
        {
            Context.Project.CollectedData.EnsureDataTablesExist(viewId);
            Wrapper wrapper = Analysis.Export.Create(Context.Project.FilePath, Context.Project.GetViewById(viewId).Name);
            await ServiceLocator.Wrapper.InvokeAsync(wrapper);
        }
    }
}
