using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public abstract class LinkRepository<TLink, TItem> : TableEntityRepository<TLink> where TLink : Link<TItem>, new()
    {
        public Project Project { get; private set; }
        public IncidentRepository Incidents { get; private set; }

        protected LinkRepository(Project project, IDataDriver driver, IncidentRepository incidents, string tableName)
            : base(driver, tableName)
        {
            Project = project;
            Incidents = incidents;
        }

        public abstract IEnumerable<TItem> SelectItems();

        public IEnumerable<DeepLink<TItem>> SelectDeepLinks()
        {
            ICollection<TLink> links = Select().ToList();
            ICollection<Incident> incidents = Incidents.Select().ToList();
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

        public IEnumerable<DeepLink<TItem>> SelectDeepLinksByIncidentId(string incidentId)
        {
            ICollection<TLink> links = Select("[IncidentId] = {@}", incidentId).ToList();
            Incident incident = Incidents.SelectByGuid(incidentId);
            foreach (TItem item in SelectItems())
            {
                if (links.Any(link => link.IsEqual(item)))
                {
                    yield return new DeepLink<TItem>(item, incident);
                }
            }
        }

        public IEnumerable<TItem> SelectItemsByIncidentId(string incidentId)
        {
            return SelectDeepLinksByIncidentId(incidentId).Select(deepLink => deepLink.Item);
        }
    }
}
