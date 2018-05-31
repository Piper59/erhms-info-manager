using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class ListViewModel<T> : ViewModelBase, IWeakEventListener
    {
        private DispatcherTimer timer;

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
                    timer.Stop();
                    timer.Start();
                }
            }
        }

        protected virtual IEnumerable<Type> RefreshTypes
        {
            get { yield return typeof(T); }
        }

        public ICommand RefreshCommand { get; private set; }

        protected ListViewModel()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.5)
            };
            timer.Tick += (sender, e) =>
            {
                Objects.Refresh();
            };
            RefreshCommand = new Command(Refresh);
            RefreshingWeakEventManager.AddListener(ServiceLocator.Data, this);
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

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(RefreshingWeakEventManager))
            {
                if (RefreshTypes.Contains(((RefreshingEventArgs)e).Type))
                {
                    Refresh();
                }
                return true;
            }
            else
            {
                return false;
            }
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
    }
}
