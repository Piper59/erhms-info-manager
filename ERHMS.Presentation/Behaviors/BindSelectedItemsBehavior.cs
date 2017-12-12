using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ERHMS.Presentation.Behaviors
{
    public class BindSelectedItemsBehavior : BridgeBehavior<MultiSelector>
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems",
            typeof(IList),
            typeof(BindSelectedItemsBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedItems_PropertyChanged));

        private static void SelectedItems_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((BindSelectedItemsBehavior)sender).Push();
        }

        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pull();
        }

        protected override void PushCore()
        {
            AssociatedObject.UnselectAll();
            if (SelectedItems == null)
            {
                return;
            }
            foreach (object item in SelectedItems)
            {
                DependencyObject container = AssociatedObject.ItemContainerGenerator.ContainerFromItem(item);
                if (container != null)
                {
                    Selector.SetIsSelected(container, true);
                }
            }
            Dispatcher.BeginInvoke((Action)Pull);
        }

        protected override void PullCore()
        {
            SelectedItems = AssociatedObject.SelectedItems.Cast<object>().ToList();
        }
    }
}
