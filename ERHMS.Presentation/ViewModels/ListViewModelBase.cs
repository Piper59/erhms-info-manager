using ERHMS.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class ListViewModelBase<T> : ViewModelBase
    {
        private static readonly TimeSpan TimerInterval = TimeSpan.FromMilliseconds(500.0);

        private DispatcherTimer timer;

        private ICollectionView items;
        public ICollectionView Items
        {
            get { return items; }
            private set { Set(nameof(Items), ref items, value); }
        }

        public IEnumerable<T> TypedItems
        {
            get { return Items.Cast<T>(); }
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
                if (Set(nameof(SelectedItem), ref selectedItem, value))
                {
                    OnSelectedItemChanged();
                }
            }
        }

        private IList selectedItems;
        public IList SelectedItems
        {
            get { return selectedItems; }
            set { Set(nameof(SelectedItems), ref selectedItems, value); }
        }

        public IEnumerable<T> TypedSelectedItems
        {
            get { return SelectedItems.Cast<T>(); }
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
                if (Set(nameof(Filter), ref filter, value))
                {
                    if (timer.IsEnabled)
                    {
                        timer.Stop();
                    }
                    timer.Start();
                }
            }
        }

        protected ListViewModelBase()
        {
            timer = new DispatcherTimer
            {
                Interval = TimerInterval
            };
            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                Items.Refresh();
            };
        }

        public event EventHandler Refreshed;
        private void OnRefreshed(EventArgs e)
        {
            Refreshed?.Invoke(this, e);
        }
        private void OnRefreshed()
        {
            OnRefreshed(EventArgs.Empty);
        }

        public event EventHandler SelectedItemChanged;
        private void OnSelectedItemChanged(EventArgs e)
        {
            SelectedItemChanged?.Invoke(this, e);
        }
        private void OnSelectedItemChanged()
        {
            OnSelectedItemChanged(EventArgs.Empty);
        }

        public bool HasOneSelectedItem()
        {
            return SelectedItems == null ? SelectedItem != null : SelectedItems.Count == 1;
        }

        public bool HasAnySelectedItems()
        {
            return SelectedItems == null ? SelectedItem != null : SelectedItems.Count > 0;
        }

        protected abstract IEnumerable<T> GetItems();

        protected virtual IEnumerable<string> GetFilteredValues(T item)
        {
            return Enumerable.Empty<string>();
        }

        public virtual void Refresh()
        {
            Items = CollectionViewSource.GetDefaultView(GetItems());
            Items.Filter = MatchesFilter;
            OnRefreshed();
        }

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

        public void SelectItem(Predicate<T> predicate)
        {
            SelectedItem = TypedItems.FirstOrDefault(item => predicate(item));
        }
    }
}
