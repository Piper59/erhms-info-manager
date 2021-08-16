using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ERHMS.Desktop.Behaviors
{
    public class ResetSelectedIndexOnDataContextChanged : Behavior<Selector>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.DataContextChanged -= AssociatedObject_DataContextChanged;
        }

        private void AssociatedObject_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (AssociatedObject.HasItems)
            {
                AssociatedObject.SelectedIndex = 0;
            }
        }
    }
}
