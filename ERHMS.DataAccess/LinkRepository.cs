using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public abstract class LinkRepository<TLink, TItem> : TableEntityRepository<TLink> where TLink : Link<TItem>, new()
    {
        protected DataContext DataContext { get; private set; }

        public LinkRepository(IDataDriver driver, string tableName, DataContext dataContext)
            : base(driver, tableName)
        {
            DataContext = dataContext;
        }

        public abstract IEnumerable<TItem> SelectItems();

        public IEnumerable<DeepLink<TItem>> SelectDeepLinks()
        {
            ICollection<TLink> links = Select().ToList();
            ICollection<Incident> incidents = DataContext.Incidents.Select().ToList();
            foreach (TItem item in SelectItems())
            {
                TLink itemLink = links.SingleOrDefault(link => link.IsEqual(item));
                Incident itemIncident = null;
                if (itemLink != null)
                {
                    itemIncident = incidents.Single(incident => incident.IncidentId.EqualsIgnoreCase(itemLink.IncidentId));
                    if (itemIncident.Deleted)
                    {
                        continue;
                    }
                }
                yield return new DeepLink<TItem>(item, itemIncident);
            }
        }

        public IEnumerable<TItem> SelectItems(string incidentId)
        {
            return SelectDeepLinks()
                .Where(deepLink => StringExtensions.EqualsIgnoreCase(deepLink.Incident?.IncidentId, incidentId))
                .Select(deepLink => deepLink.Item);
        }
    }
}
