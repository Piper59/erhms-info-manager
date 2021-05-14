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
    public class PagingListCollectionView : ListCollectionView, IPagingCollectionView
    {
        private readonly IList source;
        private bool refreshing;
        private bool repaging;
        private object oldCurrentItem;

        public override IEnumerable SourceCollection => new ArrayList(source);
        private new INotifyCollectionChanged SortDescriptions => base.SortDescriptions;

        public override Predicate<object> Filter
        {
            get
            {
                return base.Filter;
            }
            set
            {
                repaging = true;
                base.Filter = value;
            }
        }

        private int? pageSize;
        public int? PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                if (value != pageSize)
                {
                    pageSize = value;
                    repaging = true;
                    RefreshOrDefer();
                }
            }
        }

        public int PageCount { get; private set; }
        public int CurrentPage { get; private set; }
        private int NextPage => CurrentPage + 1;
        private int PreviousPage => CurrentPage - 1;

        public ICommand GoToPageCommand { get; }
        public ICommand GoToNextPageCommand { get; }
        public ICommand GoToPreviousPageCommand { get; }

        public PagingListCollectionView(IList source)
            : base(new ArrayList(source))
        {
            this.source = source;
            GroupDescriptions.CollectionChanged += Descriptions_CollectionChanged;
            SortDescriptions.CollectionChanged += Descriptions_CollectionChanged;
            GoToPageCommand = new SyncCommand<int>(GoToPageCore, CanGoToPage);
            GoToNextPageCommand = new SyncCommand(GoToNextPageCore, CanGoToNextPage);
            GoToPreviousPageCommand = new SyncCommand(GoToPreviousPageCore, CanGoToPreviousPage);
        }

        private void Descriptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            repaging = true;
            RefreshOrDefer();
        }

        private bool CanGoToPage(int page)
        {
            return page >= 1 && page <= PageCount;
        }

        private void GoToPageCore(int page)
        {
            if (page != CurrentPage)
            {
                CurrentPage = page;
                RefreshOrDefer();
            }
        }

        public bool GoToPage(int page)
        {
            if (CanGoToPage(page))
            {
                GoToPageCore(page);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CanGoToNextPage() => CanGoToPage(NextPage);
        private void GoToNextPageCore() => GoToPageCore(NextPage);
        public bool GoToNextPage() => GoToPage(NextPage);

        private bool CanGoToPreviousPage() => CanGoToPage(PreviousPage);
        private void GoToPreviousPageCore() => GoToPageCore(PreviousPage);
        public bool GoToPreviousPage() => GoToPage(PreviousPage);

        public override void Refresh()
        {
            repaging = true;
            base.Refresh();
        }

        protected override void RefreshOverride()
        {
            refreshing = true;
            try
            {
                oldCurrentItem = CurrentItem;
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
                OnRefresh();
            }
            base.OnCollectionChanged(args);
        }

        private void OnRefresh()
        {
            if (repaging)
            {
                CurrentPage = 1;
                repaging = false;
            }
            if (InternalList.Count == 0)
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
                if (CurrentPage == 0)
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
                IEnumerable<object> page = InternalList.Cast<object>()
                    .Skip(pageSize.Value * PreviousPage)
                    .Take(pageSize.Value)
                    .ToList();
                InternalList.Clear();
                foreach (object item in page)
                {
                    InternalList.Add(item);
                }
            }
            SetCurrent(oldCurrentItem);
        }

        private void SetCurrent(object item)
        {
            int position;
            if (item == null)
            {
                position = -1;
            }
            else
            {
                position = InternalList.IndexOf(item);
                if (position != -1)
                {
                    item = InternalList[position];
                }
            }
            SetCurrent(item, position);
        }
    }
}
