using ERHMS.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class ListViewModelBase<T> : ViewModelBase
    {
        private ICollectionView items;
        public ICollectionView Items
        {
            get { return items; }
            set { Set(() => Items, ref items, value); }
        }

        private T selectedItem;
        public T SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (Set(() => SelectedItem, ref selectedItem, value))
                {
                    OnSelecting();
                }
            }
        }

        private IList selectedItems;
        public IList SelectedItems
        {
            get { return selectedItems; }
            set { Set(() => SelectedItems, ref selectedItems, value); }
        }

        private string filter;
        public string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                if (Set(() => Filter, ref filter, value))
                {
                    Items.Refresh();
                }
            }
        }

        public event EventHandler Selecting;
        private void OnSelecting(EventArgs e)
        {
            EventHandler handler = Selecting;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private void OnSelecting()
        {
            OnSelecting(EventArgs.Empty);
        }

        public bool HasSelectedItem()
        {
            return SelectedItem != null;
        }

        protected abstract ICollectionView GetItems();

        public void Refresh()
        {
            Items = GetItems();
            Items.Filter = MatchesFilter;
        }

        protected abstract IEnumerable<string> GetFilteredValues(T item);

        private bool MatchesFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(Filter))
            {
                return true;
            }
            foreach (string value in GetFilteredValues((T)item))
            {
                if (value != null && value.ContainsIgnoreCase(Filter))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
