using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Web;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class SurveyViewModel : DialogViewModel
    {
        private static string GetErrorMessageInternal(string reason)
        {
            return string.Format("{0} Please verify web survey settings.", reason);
        }

        public static string GetErrorMessage(string reason)
        {
            return GetErrorMessageInternal(reason);
        }

        public static string GetErrorMessage(ConfigurationError error)
        {
            return GetErrorMessageInternal(error.GetErrorMessage());
        }

        private Epi.View view;
        public Epi.View View
        {
            get { return view; }
            private set { Set(nameof(View), ref view, value); }
        }

        public ICollection<ResponseType> ResponseTypes { get; private set; }

        private Survey survey;
        public Survey Survey
        {
            get { return survey; }
            set { Set(nameof(Survey), ref survey, value); }
        }

        private RelayCommand publishCommand;
        public ICommand PublishCommand
        {
            get { return publishCommand ?? (publishCommand = new RelayCommand(Publish)); }
        }

        public SurveyViewModel(IServiceManager services, Epi.View view)
            : base(services)
        {
            Title = "Publish to Web";
            View = view;
            ResponseTypes = EnumExtensions.GetValues<ResponseType>()
                .Where(responseType => responseType != ResponseType.Unspecified)
                .ToList();
        }

        public void Open()
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
                        Survey = Service.GetSurvey(View.WebSurveyId);
                    }
                };
                msg.Executed += (sender, e) =>
                {
                    if (error != ConfigurationError.None)
                    {
                        Documents.ShowSettings(GetErrorMessage(error));
                    }
                    else if (Survey == null)
                    {
                        Documents.ShowSettings(GetErrorMessage("Failed to retrieve web survey details."));
                    }
                    else
                    {
                        Dialogs.ShowAsync(this);
                    }
                };
                MessengerInstance.Send(msg);
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
                    PublishKey = Guid.NewGuid()
                };
                Dialogs.ShowAsync(this);
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
                ShowValidationMessage(ValidationError.Required, fields);
                return false;
            }
            return true;
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
                        Context.WebSurveys.Save(new WebSurvey
                        {
                            WebSurveyId = Survey.SurveyId,
                            ViewId = View.Id,
                            PublishKey = Survey.PublishKey.ToString()
                        });
                    }
                }
            };
            msg.Executed += (sender, e) =>
            {
                if (error != ConfigurationError.None)
                {
                    Documents.ShowSettings(GetErrorMessage(error));
                }
                else if (!success)
                {
                    Documents.ShowSettings(GetErrorMessage("Failed to publish form to web."));
                }
                else
                {
                    MessengerInstance.Send(new ToastMessage
                    {
                        Message = "Form has been published to web."
                    });
                }
                Close();
            };
            MessengerInstance.Send(msg);
        }
    }
}
