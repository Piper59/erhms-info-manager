using Epi.Core.ServiceClient;
using Epi.SurveyManagerServiceV2;
using ERHMS.Utility;
using System;

namespace ERHMS.EpiInfo.Web
{
    public static partial class Service
    {
        public static bool TryAddRecord(Survey survey, Record record)
        {
            Log.Logger.DebugFormat("Adding web survey record: {0}", survey.SurveyId);
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
                            record.PassCode = response.SurveyResponsePassCode;
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
                Log.Logger.Warn("Failed to add web survey record", ex);
                return false;
            }
        }
    }
}
