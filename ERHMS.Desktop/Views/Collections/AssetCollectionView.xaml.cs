using ERHMS.Desktop.ViewModels.Collections;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views.Collections
{
    public partial class AssetCollectionView : UserControl
    {
        public new AssetCollectionViewModel DataContext
        {
            get { return (AssetCollectionViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public AssetCollectionView()
        {
            InitializeComponent();
        }
    }
}
