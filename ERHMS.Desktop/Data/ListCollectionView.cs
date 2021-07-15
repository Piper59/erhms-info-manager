using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Data;

namespace ERHMS.Desktop.Data
{
    public class ListCollectionView<TItem> : ListCollectionView, IEnumerable<TItem>
    {
        protected List<TItem> List { get; }
        public new TItem CurrentItem => (TItem)base.CurrentItem;

        public ListCollectionView(List<TItem> list)
            : base(list)
        {
            List = list;
        }

        public bool MoveCurrentTo(Predicate<TItem> predicate)
        {
            int position = -1;
            foreach (TItem item in (IEnumerable<TItem>)this)
            {
                position++;
                if (predicate(item))
                {
                    return MoveCurrentToPosition(position);
                }
            }
            return false;
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
