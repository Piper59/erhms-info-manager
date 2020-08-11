using ERHMS.Desktop.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace ERHMS.Desktop.Data
{
    public class CustomCollectionView<TSelectable> : List<TSelectable>, ICustomCollectionView<TSelectable>
        where TSelectable : ISelectable
    {
        private class InternalCollectionView : ListCollectionView
        {
            public CustomCollectionView<TSelectable> Parent { get; }

            public override Predicate<object> Filter
            {
                get
                {
                    return base.Filter;
                }
                set
                {
                    Parent.NeedsPageReset = true;
                    base.Filter = value;
                }
            }

            public InternalCollectionView(CustomCollectionView<TSelectable> parent, IList source)
                : base(source)
            {
                Parent = parent;
                GroupDescriptions.CollectionChanged += (sender, e) => ResetPageOrDefer();
                ((INotifyCollectionChanged)SortDescriptions).CollectionChanged += (sender, e) => ResetPageOrDefer();
            }

            private void ResetPageOrDefer()
            {
                if (IsRefreshDeferred)
                {
                    Parent.NeedsPageReset = true;
                }
                else
                {
                    Parent.GoToPage(1);
                }
            }

            protected override void RefreshOverride()
            {
                object currentItem = Parent.CurrentItem;
                Parent.OnCurrentChanging();
                Parent.CurrentPosition = -1;
                base.RefreshOverride();
                if (currentItem != null)
                {
                    int index = Parent.IndexOf(currentItem);
                    if (index != -1)
                    {
                        Parent.CurrentPosition = index;
                    }
                }
                Parent.OnCurrentChanged();
            }

            public new void RefreshOrDefer()
            {
                base.RefreshOrDefer();
            }

            public new IDisposable DeferRefresh()
            {
                return base.DeferRefresh();
            }
        }

        private readonly InternalCollectionView @base;

        private bool NeedsPageReset { get; set; }
        public CultureInfo Culture { get => @base.Culture; set => @base.Culture = value; }
        public List<TSelectable> Source { get; }
        public IEnumerable SourceCollection => Source;
        public bool IsEmpty => Count == 0;
        public int CurrentPosition { get; private set; } = -1;
        public TSelectable SelectedItem => CurrentPosition == -1 ? default(TSelectable) : this[CurrentPosition];
        public IEnumerable<TSelectable> SelectedItems => this.Where(item => item.Selected);
        public object CurrentItem => SelectedItem;
        public bool IsCurrentBeforeFirst => CurrentPosition == -1;
        public bool IsCurrentAfterLast => IsEmpty;
        public bool CanFilter => @base.CanFilter;
        public Predicate<object> Filter { get => @base.Filter; set => @base.Filter = value; }

        public Predicate<TSelectable> TypedFilter
        {
            set
            {
                if (value == null)
                {
                    @base.Filter = null;
                }
                else
                {
                    @base.Filter = item => value((TSelectable)item);
                }
            }
        }

        public bool CanGroup => @base.CanGroup;
        public ObservableCollection<GroupDescription> GroupDescriptions => @base.GroupDescriptions;
        public ReadOnlyObservableCollection<object> Groups => @base.Groups;
        public bool CanSort => @base.CanSort;
        public SortDescriptionCollection SortDescriptions => @base.SortDescriptions;

        private int? pageSize;
        public int? PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                if (value != null && value.Value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                if (value != pageSize)
                {
                    pageSize = value;
                    @base.RefreshOrDefer();
                }
            }
        }

        public int PageCount { get; private set; }
        public int CurrentPage { get; private set; }

        public ICommand GoToPageCommand { get; }
        public ICommand GoToNextPageCommand { get; }
        public ICommand GoToPreviousPageCommand { get; }

        public CustomCollectionView(List<TSelectable> source)
            : base(source)
        {
            Source = source;
            @base = new InternalCollectionView(this, source);
            ((INotifyCollectionChanged)@base).CollectionChanged += Base_CollectionChanged;
            GoToPageCommand = new SyncCommand<int>(page => GoToPage(page), CanGoToPage, ErrorBehavior.Raise);
            GoToNextPageCommand = new SyncCommand(() => GoToNextPage(), CanGoToNextPage, ErrorBehavior.Raise);
            GoToPreviousPageCommand = new SyncCommand(() => GoToPreviousPage(), CanGoToPreviousPage, ErrorBehavior.Raise);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
        private void OnPropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        public event CurrentChangingEventHandler CurrentChanging;
        private void OnCurrentChanging() => CurrentChanging?.Invoke(this, new CurrentChangingEventArgs(false));

        public event EventHandler CurrentChanged;

        private void OnCurrentChanged()
        {
            CurrentChanged?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(CurrentPosition));
            OnPropertyChanged(nameof(SelectedItem));
            OnPropertyChanged(nameof(CurrentItem));
            OnPropertyChanged(nameof(IsCurrentBeforeFirst));
            OnPropertyChanged(nameof(IsCurrentAfterLast));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private void OnCollectionChanged() => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        private void Base_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (@base.IsEmpty)
            {
                PageCount = 0;
                CurrentPage = 0;
            }
            else
            {
                if (PageSize == null)
                {
                    PageCount = 1;
                }
                else
                {
                    PageCount = Math.DivRem(@base.Count, PageSize.Value, out int remainder);
                    if (remainder > 0)
                    {
                        PageCount++;
                    }
                }
                if (NeedsPageReset)
                {
                    CurrentPage = 1;
                    NeedsPageReset = false;
                }
                else if (CurrentPage < 1)
                {
                    CurrentPage = 1;
                }
                else if (CurrentPage > PageCount)
                {
                    CurrentPage = PageCount;
                }
            }
            Clear();
            IEnumerable<TSelectable> items = @base.Cast<TSelectable>();
            if (PageSize != null)
            {
                items = items.Skip((CurrentPage - 1) * PageSize.Value).Take(PageSize.Value);
            }
            AddRange(items);
            OnCollectionChanged();
            OnPropertyChanged(nameof(PageCount));
            OnPropertyChanged(nameof(CurrentPage));
        }

        public bool Contains(object item) => base.Contains((TSelectable)item);
        private int IndexOf(object item) => base.IndexOf((TSelectable)item);
        public bool HasSelectedItem() => CurrentPosition != -1;

        public bool MoveCurrentToPosition(int position)
        {
            if (position == -1 || (position >= 0 && position < Count))
            {
                OnCurrentChanging();
                CurrentPosition = position;
                OnCurrentChanged();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool MoveCurrentToNext() => MoveCurrentToPosition(CurrentPosition + 1);
        public bool MoveCurrentToPrevious() => MoveCurrentToPosition(CurrentPosition - 1);
        public bool MoveCurrentToFirst() => MoveCurrentToPosition(0);
        public bool MoveCurrentToLast() => MoveCurrentToPosition(Count - 1);

        public bool MoveCurrentTo(object item)
        {
            if (item == null)
            {
                MoveCurrentToPosition(-1);
                return true;
            }
            else
            {
                int position = IndexOf((TSelectable)item);
                if (position == -1)
                {
                    return false;
                }
                else
                {
                    MoveCurrentToPosition(position);
                    return true;
                }
            }
        }

        private bool CanGoToPage(int page)
        {
            return page >= 1 && page <= PageCount;
        }

        public bool GoToPage(int page)
        {
            if (page < 1 || page > PageCount)
            {
                return false;
            }
            else
            {
                if (page != CurrentPage)
                {
                    CurrentPage = page;
                    @base.RefreshOrDefer();
                }
                return true;
            }
        }

        private bool CanGoToNextPage() => CanGoToPage(CurrentPage + 1);
        public bool GoToNextPage() => GoToPage(CurrentPage + 1);
        private bool CanGoToPreviousPage() => CanGoToPage(CurrentPage - 1);
        public bool GoToPreviousPage() => GoToPage(CurrentPage - 1);

        public void Refresh()
        {
            NeedsPageReset = true;
            @base.Refresh();
        }

        public IDisposable DeferRefresh() => @base.DeferRefresh();
    }
}
