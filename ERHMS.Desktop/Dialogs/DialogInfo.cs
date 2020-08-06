using System;
using System.Drawing;
using System.Media;

namespace ERHMS.Desktop.Dialogs
{
    public class DialogInfo
    {
        public SystemSound Sound { get; set; }
        public Icon Icon { get; set; }
        public string Lead { get; set; }
        public string Body { get; set; }
        public string Details { get; set; }
        public DialogButtonCollection Buttons { get; set; }

        public DialogInfo(DialogInfoPreset preset)
        {
            switch (preset)
            {
                case DialogInfoPreset.Default:
                    Buttons = new DialogButtonCollection
                    {
                        new DialogButton(null, "_OK", true, true)
                    };
                    break;
                case DialogInfoPreset.Warning:
                    Sound = SystemSounds.Asterisk;
                    Icon = SystemIcons.Warning;
                    Buttons = new DialogButtonCollection();
                    break;
                case DialogInfoPreset.Error:
                    Sound = SystemSounds.Asterisk;
                    Icon = SystemIcons.Error;
                    Buttons = new DialogButtonCollection
                    {
                        new DialogButton(null, "_Close", true, true)
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(preset));
            }
        }
    }
}
