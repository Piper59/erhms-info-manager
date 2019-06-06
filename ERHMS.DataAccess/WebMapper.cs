using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using View = Epi.View;

namespace ERHMS.DataAccess
{
    public class WebMapper
    {
        public DataContext Context { get; private set; }
        public View View { get; private set; }
        private IDictionary<string, ViewEntity> EntitiesById { get; set; }
        private IDictionary<string, WebRecord> WebRecordsByRecordId { get; set; }
        private IDictionary<string, Responder> RespondersById { get; set; }
        private ILookup<string, Responder> RespondersByEmailAddress { get; set; }

        public WebMapper(DataContext context, View view)
        {
            Context = context;
            View = view;
            ViewEntityRepository<ViewEntity> entities = new ViewEntityRepository<ViewEntity>(context.Database, view);
            EntitiesById = entities.Select().ToDictionary(entity => entity.GlobalRecordId, StringComparer.OrdinalIgnoreCase);
            WebRecordsByRecordId = context.WebRecords.SelectByWebSurveyId(view.WebSurveyId)
                .ToDictionary(webRecord => webRecord.GlobalRecordId, StringComparer.OrdinalIgnoreCase);
            ICollection<Responder> responders = Context.Responders.Select().ToList();
            RespondersById = responders.ToDictionary(responder => responder.ResponderId, StringComparer.OrdinalIgnoreCase);
            RespondersByEmailAddress = responders.ToLookup(responder => responder.EmailAddress, StringComparer.OrdinalIgnoreCase);
        }

        public Tuple<ViewEntity, Responder> Map(Record record)
        {
            ViewEntity entity = null;
            Responder responder = null;
            if (View.Id == Context.Responders.View.Id)
            {
                RespondersById.TryGetValue(record.GlobalRecordId, out responder);
                if (responder == null && WebRecordsByRecordId.TryGetValue(record.GlobalRecordId, out WebRecord webRecord))
                {
                    RespondersById.TryGetValue(webRecord.ResponderId, out responder);
                }
                if (responder == null && record.TryGetValue(nameof(Responder.EmailAddress), out string emailAddress))
                {
                    responder = RespondersByEmailAddress[emailAddress].FirstOrDefault();
                }
                entity = responder;
            }
            else
            {
                EntitiesById.TryGetValue(record.GlobalRecordId, out entity);
                if (entity?.GetProperty(FieldNames.ResponderId) is string responderId)
                {
                    RespondersById.TryGetValue(responderId, out responder);
                }
                if (responder == null && WebRecordsByRecordId.TryGetValue(record.GlobalRecordId, out WebRecord webRecord))
                {
                    RespondersById.TryGetValue(webRecord.ResponderId, out responder);
                }
                if (responder == null && record.TryGetValue(FieldNames.ResponderEmailAddress, out string emailAddress))
                {
                    responder = RespondersByEmailAddress[emailAddress].FirstOrDefault();
                }
            }
            return Tuple.Create(entity, responder);
        }
    }
}
