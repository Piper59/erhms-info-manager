using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ERHMS.Desktop.Controls
{
    [TemplatePart(Name = TextBoxPartName, Type = typeof(TextBoxBase))]
    [TemplatePart(Name = ButtonPartName, Type = typeof(ButtonBase))]
    public class ActionTextBox : TextBox
    {
        public const string TextBoxPartName = "PART_TextBox";
        public const string ButtonPartName = "PART_Button";

        public static readonly DependencyProperty ActionAutomationNameProperty = DependencyProperty.Register(
            nameof(ActionAutomationName),
            typeof(string),
            typeof(ActionTextBox),
            new FrameworkPropertyMetadata(""));

        public static readonly DependencyProperty ActionContentProperty = DependencyProperty.Register(
            nameof(ActionContent),
            typeof(object),
            typeof(ActionTextBox));

        public static readonly DependencyProperty ActionCommandProperty = DependencyProperty.Register(
            nameof(ActionCommand),
            typeof(ICommand),
            typeof(ActionTextBox));

        static ActionTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ActionTextBox), new FrameworkPropertyMetadata(typeof(ActionTextBox)));
            IsTabStopProperty.OverrideMetadata(typeof(ActionTextBox), new FrameworkPropertyMetadata(false));
        }

        public string ActionAutomationName
        {
            get { return (string)GetValue(ActionAutomationNameProperty); }
            set { SetValue(ActionAutomationNameProperty, value); }
        }

        public object ActionContent
        {
            get { return GetValue(ActionContentProperty); }
            set { SetValue(ActionContentProperty, value); }
        }

        public ICommand ActionCommand
        {
            get { return (ICommand)GetValue(ActionCommandProperty); }
            set { SetValue(ActionCommandProperty, value); }
        }

        private TextBoxBase textBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            textBox = (TextBoxBase)Template.FindName(TextBoxPartName, this);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus == this)
            {
                textBox.Focus();
                e.Handled = true;
            }
            base.OnGotKeyboardFocus(e);
        }
    }
}
