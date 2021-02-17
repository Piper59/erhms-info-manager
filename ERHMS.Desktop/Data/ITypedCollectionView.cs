using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ERHMS.Desktop.Data
{
    public interface ITypedCollectionView<TItem> : ICollectionView, IEnumerable<TItem>
    {
        Predicate<TItem> TypedFilter { set; }
    }
}
