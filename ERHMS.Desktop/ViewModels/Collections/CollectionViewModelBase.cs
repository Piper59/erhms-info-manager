using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Data;
using System.Collections;
using System.Collections.Generic;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public abstract class CollectionViewModelBase<TItem> : ObservableObject
    {
        protected readonly List<TItem> items;
        public PagingListCollectionView Items { get; }
        public TItem CurrentItem => (TItem)Items.CurrentItem;
        public IList SelectedItems { get; set; }

        protected CollectionViewModelBase()
        {
            items = new List<TItem>();
            Items = new PagingListCollectionView(items);
        }

        public bool HasCurrentItem()
        {
            return Items.HasCurrent();
        }

        public bool HasSelectedItems()
        {
            return SelectedItems != null && SelectedItems.Count > 0;
        }
    }
}
