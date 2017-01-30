using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Web;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class SurveyViewModel : ViewModelBase
    {
        private static void RequestConfigurationInternal(string message)
        {
            NotifyMessage msg = new NotifyMessage(message);
            msg.Dismissed += (sender, e) =>
            {
                Locator.Main.OpenSettingsView();
            };
            Messenger.Default.Send(msg);
        }

        public static void RequestConfiguration()
        {
            RequestConfigurationInternal("Please configure web survey settings.");
        }

        public static void RequestConfiguration(string reason)
        {
            RequestConfigurationInternal(string.Format("{0} Please verify web survey settings.", reason));
        }

        public static void RequestConfiguration(ConfigurationError error)
        {
            RequestConfiguration(error.GetMessage());
        }

        public ICollection<ResponseType> ResponseTypes { get; private set; }

        private bool active;
        public bool Active
        {
            get { return active; }
            set { Set(() => Active, ref active, value); }
        }

        private View view;
        public View View
        {
            get { return view; }
            private set { Set(() => View, ref view, value); }
        }

        private Survey survey;
        public Survey Survey
        {
            get { return survey; }
            set { Set(() => Survey, ref survey, value); }
        }

        public RelayCommand PublishCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public SurveyViewModel()
        {
            ResponseTypes = EnumExtensions.GetValues<ResponseType>()
                .Where(responseType => responseType != ResponseType.Unspecified)
                .ToList();
            PublishCommand = new RelayCommand(Publish);
            CancelCommand = new RelayCommand(Cancel);
        }

        public SurveyViewModel(View view)
            : this()
        {
            View = view;
        }

        public void Activate(View view)
        {
            View = null;
            View = view;
            if (view.IsWebSurvey())
            {
                ConfigurationError error = ConfigurationError.None;
                BlockMessage msg = new BlockMessage("Retrieving web survey details \u2026");
                msg.Executing += (sender, e) =>
                {
                    Service service = new Service();
                    error = service.CheckConfiguration();
                    if (error == ConfigurationError.None)
                    {
                        Survey = service.GetSurvey(view);
                    }
                };
                msg.Executed += (sender, e) =>
                {
                    if (error != ConfigurationError.None)
                    {
                        RequestConfiguration(error);
                    }
                    else if (Survey == null)
                    {
                        RequestConfiguration("Failed to retrieve web survey details.");
                    }
                    else
                    {
                        Active = true;
                    }
                };
                Messenger.Default.Send(msg);
            }
            else
            {
                DateTime now = DateTime.Now;
                Survey = new Survey
                {
                    Title = view.Name,
                    StartDate = now,
                    EndDate = now.AddMonths(1),
                    ResponseType = ResponseType.Single,
                    Intro = null,
                    Outro = null,
                    Draft = false,
                    PublishKey = Guid.NewGuid()
                };
                Active = true;
            }
        }

        private bool Validate()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Survey.Title))
            {
                fields.Add("Title");
            }
            if (fields.Count > 0)
            {
                NotifyRequired(fields);
                return false;
            }
            else
            {
                return true;
            }
        }
        public void Publish()
        {
            if (!Validate())
            {
                return;
            }
            View.EnsureDataTablesExist();
            bool success = false;
            ConfigurationError error = ConfigurationError.None;
            BlockMessage msg = new BlockMessage("Publishing form to web \u2026");
            msg.Executing += (sender, e) =>
            {
                Service service = new Service();
                error = service.CheckConfiguration();
                if (error != ConfigurationError.None)
                {
                    return;
                }
                if (View.IsWebSurvey())
                {
                    success = service.Republish(View, Survey);
                }
                else
                {
                    success = service.Publish(View, Survey);
                    if (success)
                    {
                        WebSurvey webSurvey = DataContext.WebSurveys.Create();
                        webSurvey.WebSurveyId = Survey.SurveyId;
                        webSurvey.ViewId = View.Id;
                        webSurvey.PublishKey = Survey.PublishKey.ToString();
                        DataContext.WebSurveys.Save(webSurvey);
                    }
                }
            };
            msg.Executed += (sender, e) =>
            {
                if (error != ConfigurationError.None)
                {
                    RequestConfiguration(error);
                }
                else if (!success)
                {
                    RequestConfiguration("Failed to publish form to web.");
                }
                else
                {
                    Active = false;
                    Messenger.Default.Send(new ToastMessage("Form has been published to web."));
                }
            };
            Messenger.Default.Send(msg);
        }

        public void Cancel()
        {
            Active = false;
        }

        public bool Import()
        {
            if (!View.IsWebSurvey())
            {
                Messenger.Default.Send(new NotifyMessage("Form has not been published to web."));
                return false;
            }
            bool success = false;
            ConfigurationError error = ConfigurationError.None;
            Survey = null;
            BlockMessage msg = new BlockMessage("Importing data from web \u2026");
            msg.Executing += (sender, e) =>
            {
                Service service = new Service();
                error = service.CheckConfiguration();
                if (error != ConfigurationError.None)
                {
                    return;
                }
                Survey = service.GetSurvey(View);
                if (Survey == null)
                {
                    return;
                }
                ViewEntityRepository<ViewEntity> entities = new ViewEntityRepository<ViewEntity>(DataContext.Driver, View);
                try
                {
                    foreach (Record record in service.GetRecords(View, Survey))
                    {
                        ViewEntity entity = entities.SelectByGlobalRecordId(record.GlobalRecordId);
                        if (entity == null)
                        {
                            entity = entities.Create();
                            entity.GlobalRecordId = record.GlobalRecordId;
                        }
                        foreach (string key in record.Keys)
                        {
                            Type type = entities.GetDataType(key);
                            entity.SetProperty(key, record.GetValue(key, type));
                        }
                        entities.Save(entity);
                    }
                    success = true;
                }
                catch (Exception ex)
                {
                    Log.Current.Warn("Failed to import data from web", ex);
                }
            };
            msg.Executed += (sender, e) =>
            {
                if (error != ConfigurationError.None)
                {
                    RequestConfiguration(error);
                }
                else if (Survey == null)
                {
                    RequestConfiguration("Failed to retrieve web survey details.");
                }
                else if (!success)
                {
                    RequestConfiguration("Failed to import data from web.");
                }
                else
                {
                    Messenger.Default.Send(new ToastMessage("Data has been imported from web."));
                }
            };
            Messenger.Default.Send(msg);
            return success;
        }
    }
}
