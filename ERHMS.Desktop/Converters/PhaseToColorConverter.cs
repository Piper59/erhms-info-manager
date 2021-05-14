using ERHMS.Domain;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ERHMS.Desktop.Converters
{
    public class PhaseToColorConverter : IValueConverter
    {
        private static Color GetColor(Phase phase)
        {
            switch (phase)
            {
                case Phase.PreDeployment:
                    return Colors.PreDeployment;
                case Phase.Deployment:
                    return Colors.Deployment;
                case Phase.PostDeployment:
                    return Colors.PostDeployment;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase));
            }
        }

        public byte? Alpha { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Phase phase = (Phase)value;
            Color color = GetColor(phase);
            byte? alpha = (byte?)parameter ?? Alpha;
            if (alpha != null)
            {
                color.A = alpha.Value;
            }
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
