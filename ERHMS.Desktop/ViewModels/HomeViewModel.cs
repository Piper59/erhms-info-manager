using ERHMS.Desktop.CollectionViews;

namespace ERHMS.Desktop.ViewModels
{
    public class HomeViewModel
    {
        public PhaseCollectionView Phases { get; } = new PhaseCollectionView();
    }
}
