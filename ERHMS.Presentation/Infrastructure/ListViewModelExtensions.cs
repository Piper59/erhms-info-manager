using ERHMS.Domain;
using System.Collections.Generic;

namespace ERHMS.Presentation
{
    public static class ListViewModelExtensions
    {
        public static IEnumerable<string> GetFilteredValues(Responder item)
        {
            yield return item.LastName;
            yield return item.FirstName;
            yield return item.EmailAddress;
            yield return item.City;
            yield return item.State;
            yield return item.OrganizationName;
            yield return item.Occupation;
        }
    }
}
