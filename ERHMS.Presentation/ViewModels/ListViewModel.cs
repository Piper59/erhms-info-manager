using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class ListViewModel<T> : ViewModelBase
    {
        private static readonly TimeSpan TimerInterval = TimeSpan.FromSeconds(0.5);

        private DispatcherTimer timer;

        private ICollectionView items;
        public ICollectionView Items
        {
            get { return items; }
            private set { Set(nameof(Items), ref items, value); }
        }

        public IEnumerable<T> TypedItems
        {
            get { return Items?.Cast<T>(); }
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
                    OnSelectionChanged();
                }
            }
        }

        private IList selectedItems;
        public IList SelectedItems
        {
            get
            {
                return selectedItems;
            }
            set
            {
                if (Set(nameof(SelectedItems), ref selectedItems, value))
                {
                    OnSelectionChanged();
                }
            }
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
                    timer.Stop();
                    timer.Start();
                }
            }
        }

        protected virtual IEnumerable<Type> RefreshTypes
        {
            get { yield return typeof(T); }
        }

        public RelayCommand RefreshCommand { get; private set; }

        protected ListViewModel(IServiceManager services)
            : base(services)
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
            Items = CollectionViewSource.GetDefaultView(Enumerable.Empty<T>());
            SelectedItems = new List<T>();
            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
            MessengerInstance.Register<RefreshMessage>(this, msg =>
            {
                if (RefreshTypes.Contains(msg.Type))
                {
                    Refresh();
                }
            });
        }

        public event EventHandler SelectionChanged;
        private void OnSelectionChanged(EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
        private void OnSelectionChanged()
        {
            OnSelectionChanged(EventArgs.Empty);
        }

        public virtual bool CanRefresh()
        {
            return true;
        }

        public bool HasSelectedItem()
        {
            return SelectedItem != null || (SelectedItems != null && SelectedItems.Count > 0);
        }

        public bool HasSingleSelectedItem()
        {
            return (SelectedItem != null && SelectedItems == null) || SelectedItems.Count == 1;
        }

        protected abstract IEnumerable<T> GetItems();

        protected virtual IEnumerable<string> GetFilteredValues(T item)
        {
            yield break;
        }

        protected virtual bool IsMatch(object item)
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

        public virtual void Refresh()
        {
            Items = CollectionViewSource.GetDefaultView(GetItems());
            Items.Filter = IsMatch;
        }
    }
}
