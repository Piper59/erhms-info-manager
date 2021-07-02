using System.Collections;
using System.Collections.Generic;
using System.Windows.Data;

namespace ERHMS.Desktop.Data
{
    public abstract class ListCollectionView<TItem> : ListCollectionView, IEnumerable<TItem>
    {
        protected List<TItem> List { get; }
        public new TItem CurrentItem => (TItem)base.CurrentItem;

        protected ListCollectionView(List<TItem> list)
            : base(list)
        {
            List = list;
        }

        IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
        {
            IEnumerator enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return (TItem)enumerator.Current;
            }
        }
    }
}
