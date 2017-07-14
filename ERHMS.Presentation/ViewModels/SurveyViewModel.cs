using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
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
        private static void RequestConfigurationImpl(string message)
        {
            AlertMessage msg = new AlertMessage
            {
                Message = message
            };
            msg.Dismissed += (sender, e) =>
            {
                MainViewModel.Instance.OpenSettingsView();
            };
            Messenger.Default.Send(msg);
        }

        public static void RequestConfiguration()
        {
            RequestConfigurationImpl("Please configure web survey settings.");
        }

        public static void RequestConfiguration(string reason)
        {
            RequestConfigurationImpl(string.Format("{0} Please verify web survey settings.", reason));
        }

        private static string GetErrorMessage(ConfigurationError error)
        {
            switch (error)
            {
                case ConfigurationError.EndpointAddress:
                    return "Invalid endpoint address.";
                case ConfigurationError.Version:
                    return "Endpoint address must be version 2 (SurveyManagerServiceV2.svc) or later.";
                case ConfigurationError.OrganizationKey:
                    return "Invalid organization key.";
                case ConfigurationError.Connection:
                    return "Failed to connect to service.";
                case ConfigurationError.Unknown:
                    return "Configuration error.";
                default:
                    throw new InvalidEnumValueException(error);
            }
        }
        public static void RequestConfiguration(ConfigurationError error)
        {
            RequestConfiguration(GetErrorMessage(error));
        }

        public ICollection<ResponseType> ResponseTypes { get; private set; }

        private bool active;
        public bool Active
        {
            get { return active; }
            set { Set(nameof(Active), ref active, value); }
        }

        private View view;
        public View View
        {
            get { return view; }
            private set { Set(nameof(View), ref view, value); }
        }

        private Survey survey;
        public Survey Survey
        {
            get { return survey; }
            set { Set(nameof(Survey), ref survey, value); }
        }

        public RelayCommand PublishCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public SurveyViewModel(View view)
        {
            ResponseTypes = EnumExtensions.GetValues<ResponseType>()
                .Where(responseType => responseType != ResponseType.Unspecified)
                .ToList();
            View = view;
            PublishCommand = new RelayCommand(Publish);
            CancelCommand = new RelayCommand(Cancel);
        }

        public void Activate()
        {
            if (View.IsWebSurvey())
            {
                ConfigurationError error = ConfigurationError.None;
                BlockMessage msg = new BlockMessage
                {
                    Message = "Retrieving web survey details \u2026"
                };
                msg.Executing += (sender, e) =>
                {
                    if (Service.IsConfigured(out error))
                    {
                        Survey = Service.GetSurvey(View);
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
                    Title = View.Name,
                    StartDate = now,
                    EndDate = now.AddDays(10.0),
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
                ShowRequiredMessage(fields);
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
            ConfigurationError error = ConfigurationError.None;
            bool success = false;
            BlockMessage msg = new BlockMessage
            {
                Message = "Publishing form to web \u2026"
            };
            msg.Executing += (sender, e) =>
            {
                if (!Service.IsConfigured(out error))
                {
                    return;
                }
                if (View.IsWebSurvey())
                {
                    success = Service.Republish(View, Survey);
                }
                else
                {
                    success = Service.Publish(View, Survey);
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
                    Messenger.Default.Send(new ToastMessage
                    {
                        Message = "Form has been published to web."
                    });
                }
                Active = false;
            };
            Messenger.Default.Send(msg);
        }

        public void Cancel()
        {
            Active = false;
        }
    }
}
