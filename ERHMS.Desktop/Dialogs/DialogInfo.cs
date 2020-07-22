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
        public DialogButtonCollection Buttons { get; set; } = DialogButtonCollection.OK;

        public DialogInfo(DialogInfoPreset preset)
        {
            switch (preset)
            {
                case DialogInfoPreset.Normal:
                    break;
                case DialogInfoPreset.Warning:
                    Sound = SystemSounds.Asterisk;
                    Icon = SystemIcons.Warning;
                    break;
                case DialogInfoPreset.Error:
                    Sound = SystemSounds.Asterisk;
                    Icon = SystemIcons.Error;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
