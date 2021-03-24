using ERHMS.Desktop.Properties;
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

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(ItemsControlWatermark),
            new FrameworkPropertyMetadata(ResXResources.Lead_EmptyItemsControl));

        public ItemsControl ItemsControl
        {
            get { return (ItemsControl)GetValue(ItemsControlProperty); }
            set { SetValue(ItemsControlProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ItemsControlWatermark()
        {
            InitializeComponent();
        }
    }
}
