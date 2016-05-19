using Epi;
using Epi.Core.ServiceClient;
using Epi.SurveyManagerServiceV2;
using System;
using System.ServiceModel;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.EpiInfo.Web
{
    public class Service
    {
        private Configuration configuration;

        public string Address
        {
            get { return configuration.Settings.WebServiceEndpointAddress; }
        }

        public Guid? OrganizationKey
        {
            get { return Settings.Default.WebSurveyKey; }
        }

        public Service()
        {
            configuration = Configuration.GetNewInstance();
        }

        public bool IsConfigured()
        {
            if (string.IsNullOrEmpty(Address))
            {
                return false;
            }
            else if (!OrganizationKey.HasValue)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public ConfigurationError CheckConfiguration()
        {
            Log.Current.Debug("Checking web configuration");
            if (Address == null || !Address.EndsWith("V2.svc", StringComparison.OrdinalIgnoreCase))
            {
                return ConfigurationError.Version;
            }
            else if (!OrganizationKey.HasValue)
            {
                return ConfigurationError.OrganizationKey;
            }
            else
            {
                try
                {
                    ManagerServiceV2Client client = ServiceClient.GetClientV2();
                    SurveyInfoRequest request = new SurveyInfoRequest
                    {
                        Criteria = new SurveyInfoCriteria
                        {
                            OrganizationKey = OrganizationKey.Value,
                            SurveyIdList = new string[] { }
                        }
                    };
                    if (client.IsValidOrgKey(request))
                    {
                        return ConfigurationError.None;
                    }
                    else
                    {
                        return ConfigurationError.OrganizationKey;
                    }
                }
                catch (EndpointNotFoundException)
                {
                    return ConfigurationError.Connection;
                }
                catch
                {
                    return ConfigurationError.Unknown;
                }
            }
        }

        public Survey GetSurvey(View view)
        {
            // TODO: Handle errors
            Log.Current.DebugFormat("Getting survey: {0}", view.Name);
            ManagerServiceV2Client client = ServiceClient.GetClientV2();
            SurveyInfoRequest request = new SurveyInfoRequest
            {
                Criteria = new SurveyInfoCriteria
                {
                    OrganizationKey = OrganizationKey.Value,
                    SurveyType = ResponseType.Unspecified.ToEpiInfoValue(),
                    SurveyIdList = new string[] { view.WebSurveyId }
                }
            };
            SurveyInfoResponse response = client.GetSurveyInfo(request);
            if (response.SurveyInfoList.Length == 0)
            {
                return null;
            }
            else
            {
                if (response.SurveyInfoList.Length > 1)
                {
                    Log.Current.WarnFormat("Multiple surveys found: {0}", view.Name);
                }
                return Survey.FromServiceObject(response.SurveyInfoList[0]);
            }
        }

        public bool Publish(View view, Survey survey)
        {
            // TODO: Handle errors
            Log.Current.DebugFormat("Publishing to web: {0}", view.Name);
            ManagerServiceV2Client client = ServiceClient.GetClientV2();
            PublishRequest request = new PublishRequest
            {
                SurveyInfo = survey.ToServiceObject(view)
            };
            PublishResponse response = client.PublishSurvey(request);
            if (response.PublishInfo.IsPulished)
            {
                string url = response.PublishInfo.URL;
                survey.SurveyId = url.Substring(url.LastIndexOf('/') + 1);
                view.WebSurveyId = survey.SurveyId;
                view.SaveToDb();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Republish(View view, Survey survey)
        {
            // TODO: Handle errors
            Log.Current.DebugFormat("Republishing to web: {0}", view.Name);
            ManagerServiceV2Client client = ServiceClient.GetClientV2();
            PublishRequest request = new PublishRequest
            {
                Action = "Update",
                SurveyInfo = survey.ToServiceObject(view)
            };
            PublishResponse response = client.RePublishSurvey(request);
            return response.PublishInfo.IsPulished;
        }
    }
}
