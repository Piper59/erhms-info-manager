using Epi;
using Epi.Core.ServiceClient;
using Epi.SurveyManagerServiceV2;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Xml;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.EpiInfo.Web
{
    public static class Service
    {
        private static readonly Regex NamePattern = new Regex(@"^SurveyManagerService(?:V(?<version>\d+))?\.svc$", RegexOptions.IgnoreCase);

        private static Guid? OrganizationKey
        {
            get { return ConvertExtensions.ToNullableGuid(Settings.Default.OrganizationKey); }
        }

        public static bool IsConfigured(out ConfigurationError error, bool local = false)
        {
            Log.Logger.Debug("Checking web configuration");
            Configuration configuration = Configuration.GetNewInstance();
            Uri endpointUrl;
            try
            {
                endpointUrl = new Uri(configuration.Settings.WebServiceEndpointAddress);
            }
            catch
            {
                error = ConfigurationError.EndpointAddress;
                return false;
            }
            Match match = NamePattern.Match(endpointUrl.Segments[endpointUrl.Segments.Length - 1]);
            if (!match.Success)
            {
                error = ConfigurationError.EndpointAddress;
                return false;
            }
            Group version = match.Groups["version"];
            if (!version.Success || int.Parse(version.Value) < 2)
            {
                error = ConfigurationError.Version;
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
                using (ManagerServiceV2Client client = ServiceClient.GetClientV2())
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

        public static Survey GetSurvey(View view)
        {
            Log.Logger.DebugFormat("Getting web survey: {0}", view.Name);
            try
            {
                using (ManagerServiceV2Client client = ServiceClient.GetClientV2())
                {
                    SurveyInfoRequest request = new SurveyInfoRequest
                    {
                        Criteria = new SurveyInfoCriteria
                        {
                            OrganizationKey = OrganizationKey.Value,
                            SurveyType = ResponseType.Unspecified.ToEpiInfoValue(),
                            SurveyIdList = new string[]
                            {
                                view.WebSurveyId
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
                using (ManagerServiceV2Client client = ServiceClient.GetClientV2())
                {
                    PublishRequest request = new PublishRequest
                    {
                        SurveyInfo = survey.GetSurveyInfo(view)
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
                using (ManagerServiceV2Client client = ServiceClient.GetClientV2())
                {
                    PublishRequest request = new PublishRequest
                    {
                        Action = "Update",
                        SurveyInfo = survey.GetSurveyInfo(view)
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
            Log.Logger.DebugFormat("Getting web records: {0}", survey.SurveyId);
            using (ManagerServiceV2Client client = ServiceClient.GetClientV2())
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
                        Record record = new Record
                        {
                            GlobalRecordId = answer.ResponseId
                        };
                        using (XmlReader reader = XmlReader.Create(new StringReader(answer.XML)))
                        {
                            while (true)
                            {
                                XmlElement element = reader.ReadNextElement();
                                if (element == null)
                                {
                                    break;
                                }
                                if (element.Name == "ResponseDetail")
                                {
                                    record[element.GetAttribute("QuestionName")] = element.InnerText;
                                }
                            }
                        }
                        yield return record;
                    }
                }
            }
        }

        public static bool TryAddRecord(View view, Survey survey, Record record)
        {
            Log.Logger.DebugFormat("Adding web record: {0}", view.Name);
            try
            {
                using (ManagerServiceV2Client client = ServiceClient.GetClientV2())
                {
                    PreFilledAnswerRequest request = new PreFilledAnswerRequest
                    {
                        AnswerInfo = new PreFilledAnswerDTO
                        {
                            OrganizationKey = OrganizationKey.Value,
                            SurveyId = new Guid(survey.SurveyId),
                            UserPublishKey = survey.PublishKey,
                            SurveyQuestionAnswerList = record
                        }
                    };
                    PreFilledAnswerResponse response = client.SetSurveyAnswer(request);
                    switch (response.Status)
                    {
                        case "Success":
                            record.GlobalRecordId = response.SurveyResponseID;
                            record.Passcode = response.SurveyResponsePassCode;
                            return true;
                        case "Failed":
                            return false;
                        default:
                            Log.Logger.WarnFormat("Unrecognized status: {0}", response.Status);
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to add web record", ex);
                return false;
            }
        }
    }
}
