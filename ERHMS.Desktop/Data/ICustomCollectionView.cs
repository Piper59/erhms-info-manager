namespace ERHMS.Desktop.Data
{
    public interface ICustomCollectionView<TSelectable> : ISelectableCollectionView<TSelectable>, IPagingCollectionView
        where TSelectable : ISelectable
    { }
}
