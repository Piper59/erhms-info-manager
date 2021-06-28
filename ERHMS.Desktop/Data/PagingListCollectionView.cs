using ERHMS.Common;
using ERHMS.Desktop.Commands;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace ERHMS.Desktop.Data
{
    public class PagingListCollectionView : ListCollectionView
    {
        private readonly IList sourceItems;
        private IList unpagedItems;
        private bool refreshing;
        private bool repaging;
        private object oldCurrentItem;

        public override IEnumerable SourceCollection => IsPaging ? new ArrayList(sourceItems) : sourceItems;

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

        private bool IsPaging => pageSize != null;

        private int currentPage;
        public int CurrentPage => currentPage;

        public int PageCount
        {
            get
            {
                if (IsPaging)
                {
                    int quotient = Math.DivRem(unpagedItems.Count, pageSize.Value, out int remainder);
                    return remainder == 0 ? quotient : quotient + 1;
                }
                else
                {
                    return unpagedItems.Count == 0 ? 0 : 1;
                }
            }
        }

        private int FirstPage => 1;
        private int PreviousPage => currentPage - 1;
        private int NextPage => currentPage + 1;
        private int LastPage => PageCount;

        public ICommand GoToPageCommand { get; }
        public ICommand GoToFirstPageCommand { get; }
        public ICommand GoToPreviousPageCommand { get; }
        public ICommand GoToNextPageCommand { get; }
        public ICommand GoToLastPageCommand { get; }

        public PagingListCollectionView(IList items)
            : base(items)
        {
            sourceItems = items;
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
            if (page != currentPage)
            {
                currentPage = page;
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

        public IEnumerable GetPage(int page)
        {
            if (!CanGoToPage(page))
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }
            int startIndex;
            int endIndex;
            if (IsPaging)
            {
                startIndex = pageSize.Value * (page - 1);
                endIndex = Math.Min(startIndex + pageSize.Value, unpagedItems.Count);
            }
            else
            {
                startIndex = 0;
                endIndex = unpagedItems.Count;
            }
            for (int index = startIndex; index < endIndex; index++)
            {
                yield return unpagedItems[index];
            }
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
            unpagedItems = IsPaging ? new ArrayList(InternalList) : InternalList;
            if (repaging)
            {
                currentPage = 1;
                repaging = false;
            }
            currentPage = currentPage.Clamp(FirstPage, LastPage);
            if (PageCount > 1)
            {
                InternalList.Clear();
                foreach (object item in GetPage(currentPage))
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
