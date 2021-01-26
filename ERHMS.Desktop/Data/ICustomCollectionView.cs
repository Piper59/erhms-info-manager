using System;

namespace ERHMS.Desktop.Data
{
    public interface ICustomCollectionView<TItem> : ISelectableCollectionView<TItem>, IPagingCollectionView
        where TItem : ISelectable
    {
        Predicate<TItem> TypedFilter { set; }
    }
}
