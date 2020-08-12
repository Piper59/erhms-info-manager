using System.ComponentModel;
using System.Windows.Input;

namespace ERHMS.Desktop.Data
{
    public interface IPagingCollectionView : ICollectionView, INotifyPropertyChanged
    {
        int? PageSize { get; set; }
        int PageCount { get; }
        int CurrentPage { get; }

        ICommand GoToPageCommand { get; }
        ICommand GoToNextPageCommand { get; }
        ICommand GoToPreviousPageCommand { get; }

        bool GoToPage(int page);
        bool GoToNextPage();
        bool GoToPreviousPage();
    }
}
