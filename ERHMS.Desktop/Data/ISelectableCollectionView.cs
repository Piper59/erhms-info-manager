using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ERHMS.Desktop.Data
{
    public interface ISelectableCollectionView<TSelectable> : ICollectionView
        where TSelectable : ISelectable
    {
        Predicate<TSelectable> TypedFilter { set; }
        TSelectable SelectedItem { get; }
        IEnumerable<TSelectable> SelectedItems { get; }

        bool HasSelectedItem();
    }
}
