using System;
using System.ComponentModel;

namespace ERHMS.Desktop.Data
{
    public static class ICollectionViewExtensions
    {
        public static bool HasCurrentItem(this ICollectionView @this)
        {
            return @this.CurrentPosition != -1;
        }

        public static bool MoveCurrentTo(this ICollectionView @this, Predicate<object> predicate)
        {
            int position = -1;
            foreach (object item in @this)
            {
                position++;
                if (predicate(item))
                {
                    return @this.MoveCurrentToPosition(position);
                }
            }
            return false;
        }
    }
}
