using System.Collections.Generic;

namespace ERHMS.Desktop.ViewModels
{
    public class DetailsViewModel : List<KeyValuePair<string, string>>
    {
        public void Add(string key, string value)
        {
            Add(new KeyValuePair<string, string>(key, value));
        }
    }
}
