using ERHMS.Common.Linq;
using System.Collections.Generic;

namespace ERHMS.Desktop.ViewModels.Shared
{
    public class DetailsViewModel : List<KeyValuePair<string, object>>
    {
        public void Add(string key, object value)
        {
            Add(KeyValuePairExtensions.Create(key, value));
        }
    }
}
