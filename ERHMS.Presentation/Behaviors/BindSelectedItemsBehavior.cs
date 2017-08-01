using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ERHMS.Presentation.Behaviors
{
    public class BindSelectedItemsBehavior : BridgeBindingBehavior<MultiSelector>
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems",
            typeof(ObservableCollection<object>),
            typeof(BindSelectedItemsBehavior),
            new FrameworkPropertyMetadata(SelectedItemsProperty_PropertyChanged));

        private static void SelectedItemsProperty_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BindSelectedItemsBehavior @this = (BindSelectedItemsBehavior)sender;
            if (e.OldValue != null)
            {
                ((ObservableCollection<object>)e.OldValue).CollectionChanged -= @this.SelectedItems_CollectionChanged;
            }
            if (e.NewValue != null)
            {
                ((ObservableCollection<object>)e.NewValue).CollectionChanged += @this.SelectedItems_CollectionChanged;
            }
            @this.Push();
        }

        public ObservableCollection<object> SelectedItems
        {
            get { return (ObservableCollection<object>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
            Pull();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pull();
        }

        private void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Push();
        }

        private void Pull()
        {
            if (SelectedItems == null)
            {
                return;
            }
            Update(() =>
            {
                SelectedItems.Clear();
                if (AssociatedObject?.SelectedItems == null)
                {
                    return;
                }
                foreach (object item in AssociatedObject.SelectedItems)
                {
                    SelectedItems.Add(item);
                }
            });
        }

        private void Push()
        {
            if (AssociatedObject == null)
            {
                return;
            }
            Update(() =>
            {
                AssociatedObject.SelectedItems.Clear();
                if (SelectedItems == null)
                {
                    return;
                }
                foreach (object item in SelectedItems)
                {
                    AssociatedObject.SelectedItems.Add(item);
                }
            });
        }
    }
}
