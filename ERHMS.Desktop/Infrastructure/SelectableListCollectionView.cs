using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Desktop.Infrastructure
{
    public class SelectableListCollectionView<T> : ListCollectionView
        where T : ISelectable
    {
        public List<T> Source { get; }
        public T SelectedItem => (T)CurrentItem;
        public IEnumerable<T> SelectedItems => this.Cast<T>().Where(item => item.Selected);

        public SelectableListCollectionView(List<T> source)
            : base(source)
        {
            Source = source;
        }

        public bool HasSelectedItem()
        {
            return CurrentPosition != -1;
        }
    }
}
