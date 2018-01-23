using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Web;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using View = Epi.View;

namespace ERHMS.Presentation.ViewModels
{
    public class SurveyViewModel : DialogViewModel
    {
        private View view;
        public View View
        {
            get { return view; }
            private set { SetProperty(nameof(View), ref view, value); }
        }

        public ICollection<ResponseType> ResponseTypes { get; private set; }

        private Survey survey;
        public Survey Survey
        {
            get { return survey; }
            set { SetProperty(nameof(Survey), ref survey, value); }
        }

        public ICommand PublishCommand { get; private set; }

        public SurveyViewModel(IServiceManager services, View view)
            : base(services)
        {
            Title = "Publish to Web";
            View = view;
            ResponseTypes = EnumExtensions.GetValues<ResponseType>()
                .Where(responseType => responseType != ResponseType.Unspecified)
                .ToList();
            PublishCommand = new AsyncCommand(PublishAsync);
        }

        public async Task<bool> InitializeAsync()
        {
            if (View.IsWebSurvey())
            {
                ConfigurationError error = ConfigurationError.None;
                try
                {
                    await Services.Dialog.BlockAsync("Retrieving web survey details \u2026", () =>
                    {
                        if (Service.IsConfigured(out error))
                        {
                            Survey = Service.GetSurvey(View.WebSurveyId);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Log.Logger.Warn("Failed to retrieve web survey details", ex);
                }
                if (error != ConfigurationError.None)
                {
                    await Services.Dialog.AlertAsync(string.Format("{0} Please verify web survey settings.", error.GetErrorMessage()));
                    Services.Document.ShowByType(() => new SettingsViewModel(Services));
                    return false;
                }
                else if (Survey == null)
                {
                    await Services.Dialog.AlertAsync("Failed to retrieve web survey details. Please verify web survey settings.");
                    Services.Document.ShowByType(() => new SettingsViewModel(Services));
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                DateTime today = DateTime.Today;
                Survey = new Survey
                {
                    Title = View.Name,
                    StartDate = today,
                    EndDate = today.AddDays(10.0),
                    ResponseType = ResponseType.Single,
                    PublishKey = Guid.NewGuid()
                };
                return true;
            }
        }

        private async Task<bool> ValidateAsync()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Survey.Title))
            {
                fields.Add("Title");
            }
            if (fields.Count > 0)
            {
                await Services.Dialog.AlertAsync(ValidationError.Required, fields);
                return false;
            }
            if (!DateTimeExtensions.AreInOrder(Survey.StartDate, Survey.EndDate))
            {
                await Services.Dialog.AlertAsync("End date must be later than start date.");
                return false;
            }
            return true;
        }

        public async Task PublishAsync()
        {
            if (!await ValidateAsync())
            {
                return;
            }
            ConfigurationError error = ConfigurationError.None;
            bool success = false;
            try
            {
                await Services.Dialog.BlockAsync("Publishing form to web \u2026", () =>
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
                            Context.WebSurveys.Save(new WebSurvey(true)
                            {
                                WebSurveyId = Survey.SurveyId,
                                ViewId = View.Id,
                                PublishKey = Survey.PublishKey.ToString()
                            });
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to publish form to web", ex);
            }
            if (error != ConfigurationError.None)
            {
                await Services.Dialog.AlertAsync(string.Format("{0} Please verify web survey settings.", error.GetErrorMessage()));
                Services.Document.ShowByType(() => new SettingsViewModel(Services));
            }
            else if (!success)
            {
                await Services.Dialog.AlertAsync("Failed to publish form to web. Please verify web survey settings.");
                Services.Document.ShowByType(() => new SettingsViewModel(Services));
            }
            else
            {
                Services.Dialog.Notify("Form has been published to web.");
            }
            Close();
        }
    }
}
