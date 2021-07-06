using ERHMS.Common;
using ERHMS.Desktop.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace ERHMS.Desktop.Data
{
    public class PagingListCollectionView<TItem> : ListCollectionView<TItem>
    {
        private bool refreshing;
        private bool repaging;
        private object oldCurrentItem;

        public override IEnumerable SourceCollection => IsPaging ? new List<TItem>(List) : List;
        protected IList UnpagedInternalList { get; private set; }

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

        private new INotifyCollectionChanged SortDescriptions => base.SortDescriptions;

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

        private bool IsPaging => PageSize != null;

        public int PageCount
        {
            get
            {
                if (IsPaging)
                {
                    int quotient = Math.DivRem(UnpagedInternalList.Count, PageSize.Value, out int remainder);
                    return remainder == 0 ? quotient : quotient + 1;
                }
                else
                {
                    return UnpagedInternalList.Count == 0 ? 0 : 1;
                }
            }
        }

        public int CurrentPage { get; private set; }
        private int FirstPage => 1;
        private int PreviousPage => CurrentPage - 1;
        private int NextPage => CurrentPage + 1;
        private int LastPage => PageCount;

        public ICommand GoToPageCommand { get; }
        public ICommand GoToFirstPageCommand { get; }
        public ICommand GoToPreviousPageCommand { get; }
        public ICommand GoToNextPageCommand { get; }
        public ICommand GoToLastPageCommand { get; }

        public PagingListCollectionView(List<TItem> list)
            : base(list)
        {
            GroupDescriptions.CollectionChanged += Descriptions_CollectionChanged;
            SortDescriptions.CollectionChanged += Descriptions_CollectionChanged;
            GoToPageCommand = new SyncCommand<int>(GoToPageCore, CanGoToPage);
            GoToFirstPageCommand = new SyncCommand(GoToFirstPageCore, CanGoToFirstPage);
            GoToPreviousPageCommand = new SyncCommand(GoToPreviousPageCore, CanGoToPreviousPage);
            GoToNextPageCommand = new SyncCommand(GoToNextPageCore, CanGoToNextPage);
            GoToLastPageCommand = new SyncCommand(GoToLastPageCore, CanGoToLastPage);
        }

        private void Descriptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            repaging = true;
            RefreshOrDefer();
        }

        private bool CanGoToPage(int page)
        {
            return page >= FirstPage && page <= LastPage;
        }

        private bool CanGoToFirstPage() => CanGoToPage(FirstPage);
        private bool CanGoToPreviousPage() => CanGoToPage(PreviousPage);
        private bool CanGoToNextPage() => CanGoToPage(NextPage);
        private bool CanGoToLastPage() => CanGoToPage(LastPage);

        private void GoToPageCore(int page)
        {
            if (page != CurrentPage)
            {
                CurrentPage = page;
                RefreshOrDefer();
            }
        }

        private void GoToFirstPageCore() => GoToPageCore(FirstPage);
        private void GoToPreviousPageCore() => GoToPageCore(PreviousPage);
        private void GoToNextPageCore() => GoToPageCore(NextPage);
        private void GoToLastPageCore() => GoToPageCore(LastPage);

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

        public bool GoToFirstPage() => GoToPage(FirstPage);
        public bool GoToPreviousPage() => GoToPage(PreviousPage);
        public bool GoToNextPage() => GoToPage(NextPage);
        public bool GoToLastPage() => GoToPage(LastPage);

        public IEnumerable<TItem> GetPage(int page)
        {
            if (!CanGoToPage(page))
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }
            int startIndex;
            int endIndex;
            if (IsPaging)
            {
                startIndex = PageSize.Value * (page - 1);
                endIndex = Math.Min(startIndex + PageSize.Value, UnpagedInternalList.Count);
            }
            else
            {
                startIndex = 0;
                endIndex = UnpagedInternalList.Count;
            }
            for (int index = startIndex; index < endIndex; index++)
            {
                yield return (TItem)UnpagedInternalList[index];
            }
        }

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
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CurrentPage)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(PageCount)));
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
                OnRefreshed();
                RestoreCurrent();
            }
            base.OnCollectionChanged(args);
        }

        private void OnRefreshed()
        {
            UnpagedInternalList = IsPaging ? new ArrayList(InternalList) : InternalList;
            if (repaging)
            {
                CurrentPage = FirstPage;
                repaging = false;
            }
            CurrentPage = CurrentPage.Clamp(FirstPage, LastPage);
            if (PageCount > 1)
            {
                InternalList.Clear();
                foreach (object item in GetPage(CurrentPage))
                {
                    InternalList.Add(item);
                }
            }
        }

        private void RestoreCurrent()
        {
            int position = InternalList.IndexOf(oldCurrentItem);
            object item = position == -1 ? null : InternalList[position];
            SetCurrent(item, position);
        }
    }
}
