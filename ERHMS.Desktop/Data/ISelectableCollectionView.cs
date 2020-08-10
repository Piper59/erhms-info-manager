using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ERHMS.Desktop.Data
{
    public interface ISelectableCollectionView<TSelectable> : ICollectionView, IEnumerable<TSelectable>
        where TSelectable : ISelectable
    {
        TSelectable SelectedItem { get; }
        IEnumerable<TSelectable> SelectedItems { get; }
        Predicate<TSelectable> TypedFilter { set; }

        bool HasSelectedItem();
    }
}
