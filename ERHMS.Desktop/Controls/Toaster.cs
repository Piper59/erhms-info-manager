using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Controls
{
    public class Toaster : Control
    {
        private static readonly DependencyPropertyKey MessagePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Message),
            typeof(string),
            typeof(Toaster),
            new FrameworkPropertyMetadata());

        public static readonly DependencyProperty MessageProperty = MessagePropertyKey.DependencyProperty;

        public static readonly RoutedEvent ShownEvent = EventManager.RegisterRoutedEvent(
            "Shown",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(Toaster));

        static Toaster()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Toaster), new FrameworkPropertyMetadata(typeof(Toaster)));
            IsTabStopProperty.OverrideMetadata(typeof(Toaster), new FrameworkPropertyMetadata(false));
        }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            private set { SetValue(MessagePropertyKey, value); }
        }

        public event RoutedEventHandler Shown
        {
            add { AddHandler(ShownEvent, value); }
            remove { RemoveHandler(ShownEvent, value); }
        }

        public void Show(string message)
        {
            Message = message;
            RaiseEvent(new RoutedEventArgs(ShownEvent, this));
        }
    }
}
