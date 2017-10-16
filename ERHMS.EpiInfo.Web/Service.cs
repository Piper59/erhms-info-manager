using Epi;
using Epi.Core.ServiceClient;
using Epi.SurveyManagerService;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text.RegularExpressions;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.EpiInfo.Web
{
    public static class Service
    {
        private static readonly Regex NamePattern = new Regex(@"^SurveyManagerService(?:V\d+)?\.svc$", RegexOptions.IgnoreCase);

        private static Guid? OrganizationKey
        {
            get { return ConvertExtensions.ToNullableGuid(Settings.Default.OrganizationKey); }
        }

        public static bool IsConfigured(out ConfigurationError error, bool local = false)
        {
            Log.Logger.Debug("Checking web configuration");
            Uri endpoint;
            if (!Uri.TryCreate(Configuration.GetNewInstance().Settings.WebServiceEndpointAddress, UriKind.Absolute, out endpoint))
            {
                error = ConfigurationError.Address;
                return false;
            }
            Match nameMatch = NamePattern.Match(endpoint.Segments[endpoint.Segments.Length - 1]);
            if (!nameMatch.Success)
            {
                error = ConfigurationError.Address;
                return false;
            }
            if (!OrganizationKey.HasValue)
            {
                error = ConfigurationError.OrganizationKey;
                return false;
            }
            if (local)
            {
                error = ConfigurationError.None;
                return true;
            }
            try
            {
                using (ManagerServiceClient client = ServiceClient.GetClient())
                {
                    SurveyInfoRequest request = new SurveyInfoRequest
                    {
                        Criteria = new SurveyInfoCriteria
                        {
                            OrganizationKey = OrganizationKey.Value,
                            SurveyIdList = new string[] { }
                        }
                    };
                    if (!client.IsValidOrgKey(request))
                    {
                        error = ConfigurationError.OrganizationKey;
                        return false;
                    }
                    error = ConfigurationError.None;
                    return true;
                }
            }
            catch (EndpointNotFoundException)
            {
                error = ConfigurationError.Connection;
                return false;
            }
            catch
            {
                error = ConfigurationError.Unknown;
                return false;
            }
        }

        public static Survey GetSurvey(string surveyId)
        {
            Log.Logger.DebugFormat("Getting web survey: {0}", surveyId);
            try
            {
                using (ManagerServiceClient client = ServiceClient.GetClient())
                {
                    SurveyInfoRequest request = new SurveyInfoRequest
                    {
                        Criteria = new SurveyInfoCriteria
                        {
                            OrganizationKey = OrganizationKey.Value,
                            SurveyType = ResponseTypeExtensions.EpiInfoValues.Forward(ResponseType.Unspecified),
                            SurveyIdList = new string[]
                            {
                                surveyId
                            }
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
                            Log.Logger.Warn("Multiple web surveys found");
                        }
                        Survey survey = new Survey(response.SurveyInfoList[0]);
                        Log.Logger.DebugFormat("Found web survey: {0}", survey.SurveyId);
                        return survey;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to get web survey", ex);
                return null;
            }
        }

        public static bool Publish(View view, Survey survey)
        {
            Log.Logger.DebugFormat("Publishing to web: {0}", view.Name);
            try
            {
                using (ManagerServiceClient client = ServiceClient.GetClient())
                {
                    PublishRequest request = new PublishRequest
                    {
                        SurveyInfo = survey.GetInfo(view)
                    };
                    PublishResponse response = client.PublishSurvey(request);
                    if (response.PublishInfo.IsPulished)
                    {
                        Uri url = new Uri(response.PublishInfo.URL);
                        survey.SurveyId = url.Segments[url.Segments.Length - 1];
                        view.WebSurveyId = survey.SurveyId;
                        view.SaveToDb();
                        Log.Logger.DebugFormat("Published to web: {0}", survey.SurveyId);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to publish to web", ex);
                return false;
            }
        }

        public static bool Republish(View view, Survey survey)
        {
            Log.Logger.DebugFormat("Republishing to web: {0}, {1}", view.Name, survey.SurveyId);
            try
            {
                using (ManagerServiceClient client = ServiceClient.GetClient())
                {
                    PublishRequest request = new PublishRequest
                    {
                        Action = "Update",
                        SurveyInfo = survey.GetInfo(view)
                    };
                    PublishResponse response = client.RePublishSurvey(request);
                    return response.PublishInfo.IsPulished;
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to republish to web", ex);
                return false;
            }
        }

        public static IEnumerable<Record> GetRecords(Survey survey)
        {
            Log.Logger.DebugFormat("Getting web survey records: {0}", survey.SurveyId);
            using (ManagerServiceClient client = ServiceClient.GetClient())
            {
                SurveyAnswerRequest request = new SurveyAnswerRequest
                {
                    Criteria = new SurveyAnswerCriteria
                    {
                        OrganizationKey = OrganizationKey.Value,
                        SurveyId = survey.SurveyId,
                        IsDraftMode = survey.Draft,
                        UserPublishKey = survey.PublishKey,
                        StatusId = -1,
                        ReturnSizeInfoOnly = true,
                        SurveyAnswerIdList = new List<string>()
                    },
                    SurveyAnswerList = new List<SurveyAnswerDTO>()
                };
                int pageCount;
                {
                    SurveyAnswerResponse response = client.GetSurveyAnswer(request);
                    request.Criteria.PageSize = response.PageSize;
                    pageCount = response.NumberOfPages;
                }
                request.Criteria.ReturnSizeInfoOnly = false;
                for (int page = 1; page <= pageCount; page++)
                {
                    request.Criteria.PageNumber = page;
                    SurveyAnswerResponse response = client.GetSurveyAnswer(request);
                    foreach (SurveyAnswerDTO answer in response.SurveyResponseList)
                    {
                        Record record = new Record(answer.ResponseId);
                        record.SetValues(answer.XML);
                        yield return record;
                    }
                }
            }
        }
    }
}
