using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Controls
{
    public partial class Watermark : UserControl
    {
        public static readonly DependencyProperty ItemsControlProperty = DependencyProperty.Register(
            nameof(ItemsControl),
            typeof(ItemsControl),
            typeof(Watermark));

        public ItemsControl ItemsControl
        {
            get { return (ItemsControl)GetValue(ItemsControlProperty); }
            set { SetValue(ItemsControlProperty, value); }
        }

        public Watermark()
        {
            InitializeComponent();
        }
    }
}
