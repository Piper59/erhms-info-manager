using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Controls
{
    public partial class ItemsControlWatermark : UserControl
    {
        public static readonly DependencyProperty ItemsControlProperty = DependencyProperty.Register(
            nameof(ItemsControl),
            typeof(ItemsControl),
            typeof(ItemsControlWatermark));

        public ItemsControl ItemsControl
        {
            get { return (ItemsControl)GetValue(ItemsControlProperty); }
            set { SetValue(ItemsControlProperty, value); }
        }

        public ItemsControlWatermark()
        {
            InitializeComponent();
        }
    }
}
