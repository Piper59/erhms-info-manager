using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.Controls
{
    public partial class UserGuideTabItem : TabItem
    {
        public static readonly DependencyProperty NavigateUriProperty = DependencyProperty.Register(
            "NavigateUri",
            typeof(Uri),
            typeof(UserGuideTabItem));

        public Uri NavigateUri
        {
            get { return (Uri)GetValue(NavigateUriProperty); }
            set { SetValue(NavigateUriProperty, value); }
        }

        public UserGuideTabItem()
        {
            InitializeComponent();
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            Process.Start(NavigateUri.AbsoluteUri);
        }
    }
}
