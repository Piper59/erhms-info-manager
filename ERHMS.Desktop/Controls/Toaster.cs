using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ERHMS.Desktop.Controls
{
    public class Toaster : Control
    {
        private static readonly TimeSpan DefaultDeactivationDelay = TimeSpan.FromSeconds(5.0);
        private static readonly TimeSpan MouseLeaveDeactivationDelay = TimeSpan.FromSeconds(1.0);

        private static readonly DependencyPropertyKey MessagePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Message),
            typeof(string),
            typeof(Toaster),
            new FrameworkPropertyMetadata());

        public static readonly DependencyProperty MessageProperty = MessagePropertyKey.DependencyProperty;

        public static readonly RoutedEvent ActivatingEvent = EventManager.RegisterRoutedEvent(
            nameof(Activating),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(Toaster));

        public static readonly RoutedEvent ReactivatingEvent = EventManager.RegisterRoutedEvent(
            nameof(Reactivating),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(Toaster));

        public static readonly RoutedEvent DeactivatingEvent = EventManager.RegisterRoutedEvent(
            nameof(Deactivating),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(Toaster));

        static Toaster()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Toaster), new FrameworkPropertyMetadata(typeof(Toaster)));
            IsTabStopProperty.OverrideMetadata(typeof(Toaster), new FrameworkPropertyMetadata(false));
        }

        private readonly DispatcherTimer deactivationTimer = new DispatcherTimer();

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            private set { SetValue(MessagePropertyKey, value); }
        }

        public Toaster()
        {
            deactivationTimer.Tick += DeactivationTimer_Tick;
        }

        public event RoutedEventHandler Activating
        {
            add { AddHandler(ActivatingEvent, value); }
            remove { RemoveHandler(ActivatingEvent, value); }
        }

        private void OnActivating()
        {
            RaiseEvent(new RoutedEventArgs(ActivatingEvent, this));
        }

        public event RoutedEventHandler Reactivating
        {
            add { AddHandler(ReactivatingEvent, value); }
            remove { RemoveHandler(ReactivatingEvent, value); }
        }

        private void OnReactivating()
        {
            RaiseEvent(new RoutedEventArgs(ReactivatingEvent, this));
        }

        public event RoutedEventHandler Deactivating
        {
            add { AddHandler(DeactivatingEvent, value); }
            remove { RemoveHandler(DeactivatingEvent, value); }
        }

        private void OnDeactivating()
        {
            RaiseEvent(new RoutedEventArgs(DeactivatingEvent, this));
        }

        private void DeactivationTimer_Tick(object sender, EventArgs e)
        {
            deactivationTimer.Stop();
            OnDeactivating();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            deactivationTimer.Stop();
            OnReactivating();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            deactivationTimer.Interval = MouseLeaveDeactivationDelay;
            deactivationTimer.Start();
        }

        public void Show(string message)
        {
            Message = message;
            OnActivating();
            if (!IsMouseOver)
            {
                deactivationTimer.Interval = DefaultDeactivationDelay;
                deactivationTimer.Start();
            }
        }
    }
}
