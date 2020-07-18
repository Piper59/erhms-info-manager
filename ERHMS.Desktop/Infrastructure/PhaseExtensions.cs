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
                    return Colors.PreDeployment;
                case Phase.Deployment:
                    return Colors.Deployment;
                case Phase.PostDeployment:
                    return Colors.PostDeployment;
                default:
                    throw new ArgumentException();
            }
        }

        public static string ToDisplayName(this Phase @this)
        {
            switch (@this)
            {
                case Phase.PreDeployment:
                    return "Pre-deployment";
                case Phase.Deployment:
                    return "Deployment";
                case Phase.PostDeployment:
                    return "Post-deployment";
                default:
                    throw new ArgumentException();
            }
        }
    }
}
