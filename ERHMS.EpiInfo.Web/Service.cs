using Epi;
using Epi.Core.ServiceClient;
using Epi.SurveyManagerServiceV2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Xml;
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
            if (string.IsNullOrWhiteSpace(Address))
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
            Log.Current.DebugFormat("Getting survey: {0}", view.Name);
            try
            {
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
            catch (Exception ex)
            {
                Log.Current.Warn(string.Format("Failed to get survey: {0}", view.Name), ex);
                return null;
            }
        }

        public bool Publish(View view, Survey survey)
        {
            Log.Current.DebugFormat("Publishing to web: {0}", view.Name);
            try
            {
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
            catch (Exception ex)
            {
                Log.Current.Warn(string.Format("Failed to publish to web: {0}", view.Name), ex);
                return false;
            }
        }

        public bool Republish(View view, Survey survey)
        {
            try
            {
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
            catch (Exception ex)
            {
                Log.Current.Warn(string.Format("Failed to republish to web: {0}", view.Name), ex);
                return false;
            }
        }

        public IEnumerable<Record> GetRecords(View view, Survey survey)
        {
            Log.Current.DebugFormat("Importing from web: {0}", view.Name);
            ManagerServiceV2Client client = ServiceClient.GetClientV2();
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
                    Record record = new Record();
                    record.GlobalRecordId = answer.ResponseId;
                    using (XmlReader reader = XmlReader.Create(new StringReader(answer.XML)))
                    {
                        while (reader.Read())
                        {
                            while (reader.NodeType == XmlNodeType.Element && reader.Name == "ResponseDetail")
                            {
                                record[reader.GetAttribute("QuestionName")] = reader.ReadElementContentAsString();
                            }
                        }
                    }
                    yield return record;
                }
            }
        }

        public Record AddRecord(View view, Survey survey, object record)
        {
            Log.Current.DebugFormat("Adding record: {0}", view.Name);
            try
            {
                Record _record = new Record();
                foreach (PropertyInfo property in record.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    _record[view.Fields[property.Name].Name] = Convert.ToString(property.GetValue(record, null));
                }
                ManagerServiceV2Client client = ServiceClient.GetClientV2();
                PreFilledAnswerRequest request = new PreFilledAnswerRequest
                {
                    AnswerInfo = new PreFilledAnswerDTO
                    {
                        OrganizationKey = OrganizationKey.Value,
                        SurveyId = new Guid(survey.SurveyId),
                        UserPublishKey = survey.PublishKey,
                        SurveyQuestionAnswerList = _record
                    }
                };
                PreFilledAnswerResponse response = client.SetSurveyAnswer(request);
                switch (response.Status)
                {
                    case "Failed":
                        return null;
                    case "Success":
                        _record.GlobalRecordId = response.SurveyResponseID;
                        _record.Passcode = response.SurveyResponsePassCode;
                        return _record;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception ex)
            {
                Log.Current.Warn(string.Format("Failed to add record: {0}", view.Name), ex);
                return null;
            }
        }
    }
}
