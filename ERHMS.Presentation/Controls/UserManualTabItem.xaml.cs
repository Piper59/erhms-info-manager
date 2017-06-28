using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.Controls
{
    public partial class UserManualTabItem : TabItem
    {
        public static readonly DependencyProperty NavigateUriProperty = DependencyProperty.Register(
            "NavigateUri",
            typeof(Uri),
            typeof(UserManualTabItem));

        public Uri NavigateUri
        {
            get { return (Uri)GetValue(NavigateUriProperty); }
            set { SetValue(NavigateUriProperty, value); }
        }

        public UserManualTabItem()
        {
            SetResourceReference(StyleProperty, typeof(TabItem));
            InitializeComponent();
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            Process.Start(NavigateUri.AbsoluteUri);
        }
    }
}
