using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Data;
using System.Collections;
using System.Collections.Generic;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public abstract class CollectionViewModel<TItem> : ObservableObject
    {
        protected List<TItem> List { get; }
        public PagingListCollectionView<TItem> Items { get; }
        public TItem CurrentItem => Items.CurrentItem;
        public IList SelectedItems { get; set; }

        protected CollectionViewModel()
        {
            List = new List<TItem>();
            Items = new PagingListCollectionView<TItem>(List);
        }

        public bool HasCurrentItem()
        {
            return Items.HasCurrentItem();
        }

        public bool HasSelectedItems()
        {
            return SelectedItems != null && SelectedItems.Count > 0;
        }
    }
}
