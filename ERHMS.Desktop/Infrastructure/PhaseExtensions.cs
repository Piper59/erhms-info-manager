using ERHMS.Domain;
using System;
using System.Windows.Media;

namespace ERHMS.Desktop.Infrastructure
{
    public static class PhaseExtensions
    {
        public static Color ToColor(this Phase @this)
        {
            switch (@this)
            {
                case Phase.PreDeployment:
                    return Colors.ThemeBlue;
                case Phase.Deployment:
                    return Colors.ThemeRed;
                case Phase.PostDeployment:
                    return Colors.ThemeGreen;
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }
    }
}
