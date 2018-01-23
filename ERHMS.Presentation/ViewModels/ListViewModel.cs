using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class ListViewModel<T> : ViewModelBase
    {
        private static readonly TimeSpan Delay = TimeSpan.FromSeconds(0.5);
        private static readonly TimeSpan Infinity = new TimeSpan(0, 0, 0, 0, -1);

        private Timer timer;

        private ICollectionView objects;
        public ICollectionView Objects
        {
            get { return objects; }
            private set { SetProperty(nameof(Objects), ref objects, value); }
        }

        public IEnumerable<T> Items
        {
            get { return Objects?.Cast<T>(); }
        }

        private object selectedObject;
        public object SelectedObject
        {
            get
            {
                return selectedObject;
            }
            set
            {
                if (SetProperty(nameof(SelectedObject), ref selectedObject, value))
                {
                    OnSelectionChanged();
                }
            }
        }

        public T SelectedItem
        {
            get { return (T)SelectedObject; }
        }

        private IList selectedObjects;
        public IList SelectedObjects
        {
            get
            {
                return selectedObjects;
            }
            set
            {
                if (SetProperty(nameof(SelectedObjects), ref selectedObjects, value))
                {
                    OnSelectionChanged();
                }
            }
        }

        public IEnumerable<T> SelectedItems
        {
            get { return SelectedObjects.Cast<T>(); }
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
                if (SetProperty(nameof(Filter), ref filter, value))
                {
                    timer.Change(Delay, Infinity);
                }
            }
        }

        public ICommand RefreshCommand { get; private set; }

        protected ListViewModel(IServiceManager services)
            : base(services)
        {
            timer = new Timer(state =>
            {
                services.Dispatch.Post(Objects.Refresh);
            });
            RefreshCommand = new Command(Refresh);
            services.Data.Refreshing += Data_Refreshing;
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

        private void Data_Refreshing(object sender, RefreshingEventArgs e)
        {
            if (e.Type == typeof(T))
            {
                Refresh();
            }
        }

        protected abstract IEnumerable<T> GetItems();

        protected virtual IEnumerable<string> GetFilteredValues(T item)
        {
            yield break;
        }

        protected bool IsFilterMatch(object obj)
        {
            if (string.IsNullOrWhiteSpace(Filter))
            {
                return true;
            }
            foreach (string value in GetFilteredValues((T)obj))
            {
                if (value != null && value.Contains(Filter, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public void Refresh()
        {
            Objects = CollectionViewSource.GetDefaultView(GetItems());
            Objects.Filter = IsFilterMatch;
        }

        public bool HasSelectedItem()
        {
            return SelectedObject != null;
        }

        public bool HasOneSelectedItem()
        {
            return SelectedObjects?.Count == 1;
        }

        public bool HasAnySelectedItems()
        {
            return SelectedObjects?.Count > 0;
        }

        public void Select(T item)
        {
            foreach (object obj in Objects)
            {
                if (Equals(obj, item))
                {
                    SelectedObject = obj;
                    break;
                }
            }
        }

        public void Unselect()
        {
            SelectedObject = null;
        }

        public override void Dispose()
        {
            Services.Data.Refreshing -= Data_Refreshing;
            base.Dispose();
        }
    }
}
