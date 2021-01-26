using System.Collections.Generic;
using System.ComponentModel;

namespace ERHMS.Desktop.Data
{
    public interface ISelectableCollectionView<TItem> : ICollectionView
        where TItem : ISelectable
    {
        TItem SelectedItem { get; }
        IEnumerable<TItem> SelectedItems { get; }

        bool HasSelection();
    }
}
