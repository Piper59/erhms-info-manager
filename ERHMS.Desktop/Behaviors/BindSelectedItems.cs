using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ERHMS.Desktop.Behaviors
{
    public class BindSelectedItems : Behavior<MultiSelector>
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            nameof(SelectedItems),
            typeof(IList),
            typeof(BindSelectedItems),
            new FrameworkPropertyMetadata(SelectedItems_PropertyChanged));

        private static void SelectedItems_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((BindSelectedItems)sender).VerifyUpdating();
        }

        private bool updating;

        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updating = true;
            try
            {
                SelectedItems = AssociatedObject.SelectedItems;
            }
            finally
            {
                updating = false;
            }
        }

        private void VerifyUpdating()
        {
            if (!updating)
            {
                throw new NotSupportedException("Cannot update selected items programmatically.");
            }
        }
    }
}
