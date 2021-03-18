using Microsoft.Xaml.Behaviors;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ERHMS.Desktop.Behaviors
{
    public class RemoveEmptyRuns : Behavior<TextBlock>
    {
        public static readonly DependencyProperty PreserveProperty = DependencyProperty.RegisterAttached(
            "Preserve",
            typeof(bool),
            typeof(RemoveEmptyRuns));

        public static bool GetPreserve(Run run)
        {
            return (bool)run.GetValue(PreserveProperty);
        }

        public static void SetPreserve(Run run, bool value)
        {
            run.SetValue(PreserveProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            IReadOnlyCollection<Run> emptyRuns = AssociatedObject.Inlines.OfType<Run>()
                .Where(run => run.Text == " " && !GetPreserve(run))
                .ToList();
            foreach (Run emptyRun in emptyRuns)
            {
                AssociatedObject.Inlines.Remove(emptyRun);
            }
        }
    }
}
