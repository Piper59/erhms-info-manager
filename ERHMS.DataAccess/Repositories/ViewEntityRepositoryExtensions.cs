using ERHMS.Domain;
using ERHMS.EpiInfo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public static class ViewEntityRepositoryExtensions
    {
        public static IEnumerable<ResponderEntity> WithResponders(this IEnumerable<ViewEntity> @this, DataContext context)
        {
            IDictionary<string, Responder> responders = context.Responders.Select().ToDictionary(
                responder => responder.ResponderId,
                StringComparer.OrdinalIgnoreCase);
            foreach (ViewEntity @base in @this)
            {
                ResponderEntity entity = new ResponderEntity(@base);
                string responderId;
                if (@base.TryGetProperty(FieldNames.ResponderId, out responderId))
                {
                    entity.ResponderId = responderId;
                    Responder responder;
                    if (responderId != null && responders.TryGetValue(responderId, out responder))
                    {
                        entity.Responder = responder;
                    }
                }
                yield return entity;
            }
        }
    }
}
