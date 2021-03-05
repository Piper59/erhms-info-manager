using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Reflection;

namespace ERHMS.Desktop.Dialogs
{
    public enum DialogSeverity
    {
        None,
        Question,
        Exclamation,
        Hand
    }

    public static class DialogSeverityExtensions
    {
        private static readonly IReadOnlyDictionary<string, Icon> SystemIconsByName = typeof(SystemIcons)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(property => property.PropertyType == typeof(Icon))
            .ToDictionary(property => property.Name, property => (Icon)property.GetValue(null));

        private static readonly IReadOnlyDictionary<string, SystemSound> SystemSoundsByName = typeof(SystemSounds)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(property => property.PropertyType == typeof(SystemSound))
            .ToDictionary(property => property.Name, property => (SystemSound)property.GetValue(null));

        public static Icon ToSystemIcon(this DialogSeverity @this)
        {
            return @this == DialogSeverity.None ? null : SystemIconsByName[@this.ToString()];
        }

        public static SystemSound ToSystemSound(this DialogSeverity @this)
        {
            return @this == DialogSeverity.None ? null : SystemSoundsByName[@this.ToString()];
        }
    }
}
