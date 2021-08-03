using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ERHMS.Desktop.Behaviors
{
    public class BindSelectedItems : BindProperty<MultiSelector>
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            nameof(SelectedItems),
            typeof(IList),
            typeof(BindSelectedItems),
            new FrameworkPropertyMetadata(OnPropertyChanged));

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
            Pull();
        }

        protected override void PullCore()
        {
            SelectedItems = AssociatedObject.SelectedItems;
        }
    }
}
