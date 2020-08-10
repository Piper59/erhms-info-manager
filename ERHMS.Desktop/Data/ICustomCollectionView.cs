using System.ComponentModel;

namespace ERHMS.Desktop.Data
{
    public interface ICustomCollectionView<TSelectable> : ISelectableCollectionView<TSelectable>, IPagingCollectionView, INotifyPropertyChanged
        where TSelectable : ISelectable
    { }
}
