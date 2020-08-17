using ERHMS.Desktop.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace ERHMS.Desktop.Data
{
    public class CustomCollectionView<TSelectable> : ListCollectionView, ICustomCollectionView<TSelectable>
        where TSelectable : ISelectable
    {
        private bool needsPageReset;
        private bool refreshing;
        private object currentItem;

        public List<TSelectable> Source { get; }
        public override IEnumerable SourceCollection => new ArrayList(Source);

        public override Predicate<object> Filter
        {
            get
            {
                return base.Filter;
            }
            set
            {
                needsPageReset = true;
                base.Filter = value;
            }
        }

        public Predicate<TSelectable> TypedFilter
        {
            set
            {
                if (value == null)
                {
                    Filter = null;
                }
                else
                {
                    Filter = item => value((TSelectable)item);
                }
            }
        }

        public TSelectable SelectedItem => (TSelectable)base.CurrentItem;
        public IEnumerable<TSelectable> SelectedItems => this.Cast<TSelectable>().Where(item => item.IsSelected);

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
                    RefreshOrDefer();
                }
            }
        }

        public int PageCount { get; private set; }
        public int CurrentPage { get; private set; }

        public ICommand GoToPageCommand { get; }
        public ICommand GoToNextPageCommand { get; }
        public ICommand GoToPreviousPageCommand { get; }

        public CustomCollectionView(List<TSelectable> source)
            : base(new ArrayList(source))
        {
            Source = source;
            GroupDescriptions.CollectionChanged += (sender, e) => ResetPageOrDefer();
            ((INotifyCollectionChanged)SortDescriptions).CollectionChanged += (sender, e) => ResetPageOrDefer();
            GoToPageCommand = new SyncCommand<int>(page => GoToPage(page), CanGoToPage);
            GoToNextPageCommand = new SyncCommand(() => GoToNextPage(), CanGoToNextPage);
            GoToPreviousPageCommand = new SyncCommand(() => GoToPreviousPage(), CanGoToPreviousPage);
        }

        public CustomCollectionView()
            : this(new List<TSelectable>()) { }

        public bool HasSelectedItem()
        {
            return CurrentPosition != -1;
        }

        private void ResetPageOrDefer()
        {
            if (IsRefreshDeferred)
            {
                needsPageReset = true;
            }
            else
            {
                GoToPage(1);
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
                    RefreshOrDefer();
                }
                return true;
            }
        }

        private bool CanGoToNextPage()
        {
            return CanGoToPage(CurrentPage + 1);
        }

        public bool GoToNextPage()
        {
            return GoToPage(CurrentPage + 1);
        }

        private bool CanGoToPreviousPage()
        {
            return CanGoToPage(CurrentPage - 1);
        }

        public bool GoToPreviousPage()
        {
            return GoToPage(CurrentPage - 1);
        }

        public override void Refresh()
        {
            needsPageReset = true;
            base.Refresh();
        }

        protected override void RefreshOverride()
        {
            currentItem = CurrentItem;
            refreshing = true;
            try
            {
                base.RefreshOverride();
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(PageCount)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CurrentPage)));
            }
            finally
            {
                refreshing = false;
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (refreshing)
            {
                RefreshCurrentPage();
            }
            base.OnCollectionChanged(args);
        }

        private void RefreshCurrentPage()
        {
            if (IsEmpty)
            {
                PageCount = 0;
                CurrentPage = 0;
            }
            else
            {
                if (pageSize == null)
                {
                    PageCount = 1;
                }
                else
                {
                    PageCount = Math.DivRem(InternalList.Count, pageSize.Value, out int remainder);
                    if (remainder > 0)
                    {
                        PageCount++;
                    }
                }
                if (needsPageReset)
                {
                    CurrentPage = 1;
                    needsPageReset = false;
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
            if (PageCount > 1)
            {
                IList<object> items = InternalList.Cast<object>().ToList();
                InternalList.Clear();
                foreach (object item in items.Skip((CurrentPage - 1) * pageSize.Value).Take(pageSize.Value))
                {
                    InternalList.Add(item);
                }
            }
            ResetCurrent(currentItem);
        }

        private void ResetCurrent(object item)
        {
            int position;
            if (item == null)
            {
                position = -1;
            }
            else
            {
                position = InternalList.IndexOf(item);
                if (position == -1)
                {
                    item = null;
                }
                else
                {
                    item = InternalList[position];
                }
            }
            SetCurrent(item, position);
        }
    }
}
