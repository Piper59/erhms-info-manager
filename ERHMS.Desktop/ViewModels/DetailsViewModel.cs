using System.Collections.Generic;

namespace ERHMS.Desktop.ViewModels
{
    public class DetailsViewModel : List<KeyValuePair<string, object>>
    {
        public void Add(string key, object value)
        {
            Add(new KeyValuePair<string, object>(key, value));
        }

        public void Insert(int index, string key, object value)
        {
            Insert(index, new KeyValuePair<string, object>(key, value));
        }
    }
}
