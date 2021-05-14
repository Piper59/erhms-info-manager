using System.ComponentModel;

namespace ERHMS.Desktop.Data
{
    public static class ICollectionViewExtensions
    {
        public static bool HasCurrent(this ICollectionView @this)
        {
            return @this.CurrentPosition != -1;
        }
    }
}
