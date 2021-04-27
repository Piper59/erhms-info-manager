using ERHMS.Desktop.ViewModels.Collections;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views.Collections
{
    public partial class FileInfoCollectionView : UserControl
    {
        public new FileInfoCollectionViewModel DataContext
        {
            get { return (FileInfoCollectionViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public FileInfoCollectionView()
        {
            InitializeComponent();
        }
    }
}
