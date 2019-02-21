using ERHMS.Presentation.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace ERHMS.Presentation.Dialogs
{
    public partial class TraceDialog : Window
    {
        public class Listener : TraceListener
        {
            public TextBox Log { get; private set; }

            public Listener(TextBox log)
            {
                Log = log;
            }

            public override void Write(string message)
            {
                Log.AppendText(message);
            }

            public override void WriteLine(string message)
            {
                Write(message);
                Write(Environment.NewLine);
            }
        }

        private IWin32Window win32Window;

        public TraceDialog(TraceSource source)
        {
            InitializeComponent();
            source.Listeners.Add(new Listener(Log));
            win32Window = new Win32Window(this);
            Closing += (sender, e) =>
            {
                e.Cancel = true;
            };
        }

        private void Log_TextChanged(object sender, TextChangedEventArgs e)
        {
            Log.ScrollToEnd();
            try
            {
                User32.FlashWindowEx(win32Window.Handle, 5);
            }
            catch { }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Log.Clear();
        }
    }
}
