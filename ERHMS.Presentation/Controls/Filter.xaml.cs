using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.Controls
{
    public partial class Filter : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(string),
            typeof(Filter),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public Filter()
        {
            InitializeComponent();
        }
    }
}
