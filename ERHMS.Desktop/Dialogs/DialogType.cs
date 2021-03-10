using System;
using System.Drawing;
using System.Media;

namespace ERHMS.Desktop.Dialogs
{
    public enum DialogType
    {
        None,
        Information,
        Question,
        Warning,
        Error
    }

    public static class DialogTypeExtensions
    {
        public static Icon ToIcon(this DialogType @this)
        {
            switch (@this)
            {
                case DialogType.None:
                    return null;
                case DialogType.Information:
                    return SystemIcons.Information;
                case DialogType.Question:
                    return SystemIcons.Question;
                case DialogType.Warning:
                    return SystemIcons.Exclamation;
                case DialogType.Error:
                    return SystemIcons.Hand;
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }

        public static SystemSound ToSound(this DialogType @this)
        {
            switch (@this)
            {
                case DialogType.None:
                case DialogType.Information:
                case DialogType.Question:
                    return null;
                case DialogType.Warning:
                    return SystemSounds.Exclamation;
                case DialogType.Error:
                    return SystemSounds.Hand;
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }
    }
}
