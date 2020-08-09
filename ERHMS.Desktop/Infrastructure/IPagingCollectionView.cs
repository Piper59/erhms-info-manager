using System.ComponentModel;

namespace ERHMS.Desktop.Infrastructure
{
    public interface IPagingCollectionView : ICollectionView
    {
        int? PageSize { get; set; }
        int PageCount { get; }
        int CurrentPage { get; }

        bool GoToPage(int page);
        bool GoToNextPage();
        bool GoToPreviousPage();
    }
}
