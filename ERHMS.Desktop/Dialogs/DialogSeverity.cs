using System;
using System.Drawing;
using System.Media;

namespace ERHMS.Desktop.Dialogs
{
    public enum DialogSeverity
    {
        None,
        Information,
        Question,
        Warning,
        Error
    }

    public static class DialogSeverityExtensions
    {
        public static Icon ToSystemIcon(this DialogSeverity @this)
        {
            switch (@this)
            {
                case DialogSeverity.None:
                    return null;
                case DialogSeverity.Information:
                    return SystemIcons.Information;
                case DialogSeverity.Question:
                    return SystemIcons.Question;
                case DialogSeverity.Warning:
                    return SystemIcons.Exclamation;
                case DialogSeverity.Error:
                    return SystemIcons.Hand;
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }

        public static SystemSound ToSystemSound(this DialogSeverity @this)
        {
            switch (@this)
            {
                case DialogSeverity.None:
                case DialogSeverity.Information:
                case DialogSeverity.Question:
                    return null;
                case DialogSeverity.Warning:
                    return SystemSounds.Exclamation;
                case DialogSeverity.Error:
                    return SystemSounds.Hand;
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }
    }
}
