using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Desktop.Data
{
    public abstract class TypedListCollectionView<TItem> : ListCollectionView
    {
        protected new List<TItem> SourceCollection => (List<TItem>)base.SourceCollection;
        public new TItem CurrentItem => (TItem)base.CurrentItem;

        protected TypedListCollectionView(List<TItem> items)
            : base(items) { }

        protected TypedListCollectionView()
            : this(new List<TItem>()) { }

        protected TypedListCollectionView(IEnumerable<TItem> items)
            : this(items.ToList()) { }
    }
}
