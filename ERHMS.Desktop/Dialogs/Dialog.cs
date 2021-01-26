using System;
using System.Drawing;
using System.Media;

namespace ERHMS.Desktop.Dialogs
{
    public class Dialog
    {
        public SystemSound Sound { get; set; }
        public Icon Icon { get; set; }
        public string Lead { get; set; }
        public string Body { get; set; }
        public string Details { get; set; }
        public DialogButtonCollection Buttons { get; set; }

        public Dialog(DialogPreset preset)
        {
            switch (preset)
            {
                case DialogPreset.Default:
                    Buttons = new DialogButtonCollection
                    {
                        { "_OK", null, true, true }
                    };
                    break;
                case DialogPreset.Warning:
                    Sound = SystemSounds.Asterisk;
                    Icon = SystemIcons.Warning;
                    break;
                case DialogPreset.Error:
                    Sound = SystemSounds.Asterisk;
                    Icon = SystemIcons.Error;
                    Buttons = new DialogButtonCollection
                    {
                        { "_Close", null, true, true }
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(preset));
            }
        }
    }
}
