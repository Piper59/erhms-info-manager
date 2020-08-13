using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Desktop.Controls
{
    public partial class ButtonTextBox : UserControl
    {
        public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.Register(
            nameof(ButtonContent),
            typeof(object),
            typeof(ButtonTextBox));

        public static readonly DependencyProperty ButtonNameProperty = DependencyProperty.Register(
            nameof(ButtonName),
            typeof(string),
            typeof(ButtonTextBox));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            nameof(Command),
            typeof(ICommand),
            typeof(ButtonTextBox));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            nameof(IsReadOnly),
            typeof(bool),
            typeof(ButtonTextBox));

        public new static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
            nameof(Padding),
            typeof(Thickness),
            typeof(ButtonTextBox),
            new PropertyMetadata(OnPaddingPropertyChanged));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(ButtonTextBox),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private static void OnPaddingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ButtonTextBox)sender).SetPadding();
        }

        public object ButtonContent
        {
            get { return GetValue(ButtonContentProperty); }
            set { SetValue(ButtonContentProperty, value); }
        }

        public string ButtonName
        {
            get { return (string)GetValue(ButtonNameProperty); }
            set { SetValue(ButtonNameProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public new Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ButtonTextBox()
        {
            InitializeComponent();
            Loaded += (sender, e) => SetPadding();
            Button.SizeChanged += (sender, e) => SetPadding();
            PreviewGotKeyboardFocus += OnPreviewGotKeyboardFocus;
        }

        private void SetPadding()
        {
            Thickness padding = Padding;
            padding.Right += Button.ActualWidth;
            TextBox.Padding = padding;
        }

        private void OnPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus == this)
            {
                TextBox.Focus();
                e.Handled = true;
            }
        }
    }
}
