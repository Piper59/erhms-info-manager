using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ERHMS.Desktop.Controls
{
    public partial class Toaster : UserControl
    {
        private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(5.0);
        private static readonly TimeSpan UserExtendedDuration = TimeSpan.FromSeconds(1.0);

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

        public static readonly RoutedEvent ClosingEvent = EventManager.RegisterRoutedEvent(
            nameof(Closing),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(Toaster));

        private readonly DispatcherTimer timer = new DispatcherTimer();

        public Toaster()
        {
            InitializeComponent();
            timer.Tick += Timer_Tick;
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

        public event RoutedEventHandler Closing
        {
            add { AddHandler(ClosingEvent, value); }
            remove { RemoveHandler(ClosingEvent, value); }
        }

        private void OnClosing()
        {
            RaiseEvent(new RoutedEventArgs(ClosingEvent, this));
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            OnDeactivating();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            OnClosing();
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            timer.Stop();
            OnReactivating();
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            if (!IsMouseOver)
            {
                timer.Interval = UserExtendedDuration;
                timer.Start();
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            timer.Stop();
            OnReactivating();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!IsKeyboardFocusWithin)
            {
                timer.Interval = UserExtendedDuration;
                timer.Start();
            }
        }

        public void Show(string message)
        {
            MessageTextBlock.Text = message;
            OnActivating();
            if (!IsKeyboardFocusWithin && !IsMouseOver)
            {
                timer.Interval = DefaultDuration;
                timer.Start();
            }
        }
    }
}
