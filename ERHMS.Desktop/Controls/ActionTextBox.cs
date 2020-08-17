using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ERHMS.Desktop.Controls
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBoxBase))]
    [TemplatePart(Name = "PART_Button", Type = typeof(ButtonBase))]
    public class ActionTextBox : TextBox
    {
        public static readonly DependencyProperty ActionNameProperty = DependencyProperty.Register(
            nameof(ActionName),
            typeof(string),
            typeof(ActionTextBox),
            new PropertyMetadata(""));

        public static readonly DependencyProperty ActionTextProperty = DependencyProperty.Register(
            nameof(ActionText),
            typeof(object),
            typeof(ActionTextBox));

        public static readonly DependencyProperty CommandProperty = ButtonBase.CommandProperty.AddOwner(typeof(ActionTextBox));

        static ActionTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ActionTextBox), new FrameworkPropertyMetadata(typeof(ActionTextBox)));
            IsTabStopProperty.OverrideMetadata(typeof(ActionTextBox), new FrameworkPropertyMetadata(false));
        }

        public string ActionName
        {
            get { return (string)GetValue(ActionNameProperty); }
            set { SetValue(ActionNameProperty, value); }
        }

        public object ActionText
        {
            get { return GetValue(ActionTextProperty); }
            set { SetValue(ActionTextProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private TextBox TextBox { get; set; }
        private Button Button { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TextBox = (TextBox)Template.FindName("PART_TextBox", this);
            Button = (Button)Template.FindName("PART_Button", this);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus == this)
            {
                TextBox.Focus();
                e.Handled = true;
            }
            base.OnGotKeyboardFocus(e);
        }
    }
}
