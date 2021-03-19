using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace ERHMS.Desktop.Controls
{
    [TemplatePart(Name = CloseButtonPartName, Type = typeof(ButtonBase))]
    public class Toaster : Control
    {
        public const string CloseButtonPartName = "PART_CloseButton";

        private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(5.0);
        private static readonly TimeSpan UserExtendedDuration = TimeSpan.FromSeconds(1.0);

        private static readonly DependencyPropertyKey MessagePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Message),
            typeof(string),
            typeof(Toaster),
            new FrameworkPropertyMetadata());

        public static readonly DependencyProperty MessageProperty = MessagePropertyKey.DependencyProperty;

        public static readonly DependencyProperty MessageStyleProperty = DependencyProperty.Register(
            nameof(MessageStyle),
            typeof(Style),
            typeof(Toaster));

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

        static Toaster()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Toaster), new FrameworkPropertyMetadata(typeof(Toaster)));
        }

        private readonly DispatcherTimer timer = new DispatcherTimer();

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            private set { SetValue(MessagePropertyKey, value); }
        }

        public Style MessageStyle
        {
            get { return (Style)GetValue(MessageStyleProperty); }
            set { SetValue(MessageStyleProperty, value); }
        }

        private ButtonBase closeButton;

        public Toaster()
        {
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            OnDeactivating();
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (closeButton != null)
            {
                closeButton.Click -= CloseButton_Click;
            }
            closeButton = (ButtonBase)Template.FindName(CloseButtonPartName, this);
            if (closeButton != null)
            {
                closeButton.Click += CloseButton_Click;
            }
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
            Message = message;
            OnActivating();
            if (!IsKeyboardFocusWithin && !IsMouseOver)
            {
                timer.Interval = DefaultDuration;
                timer.Start();
            }
        }
    }
}
