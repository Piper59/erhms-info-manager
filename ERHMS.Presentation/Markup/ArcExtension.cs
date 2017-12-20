using ERHMS.Utility;
using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace ERHMS.Presentation.Markup
{
    [MarkupExtensionReturnType(typeof(PathFigureCollection))]
    public class ArcExtension : MarkupExtension
    {
        [ConstructorArgument("centerX")]
        public double CenterX { get; set; }

        [ConstructorArgument("centerY")]
        public double CenterY { get; set; }

        [ConstructorArgument("radius")]
        public double Radius { get; set; }

        [ConstructorArgument("startAngle")]
        public double StartAngle { get; set; }

        [ConstructorArgument("endAngle")]
        public double EndAngle { get; set; }

        [ConstructorArgument("direction")]
        public SweepDirection Direction { get; set; }

        public ArcExtension() { }

        public ArcExtension(double centerX, double centerY, double radius, double startAngle, double endAngle, SweepDirection direction)
        {
            CenterX = centerX;
            CenterY = centerY;
            Radius = radius;
            StartAngle = startAngle;
            EndAngle = endAngle;
            Direction = direction;
        }

        private Point GetPoint(double degrees)
        {
            double radians = ConvertExtensions.ToRadians(degrees);
            return new Point
            {
                X = CenterX + Radius * Math.Cos(radians),
                Y = CenterY - Radius * Math.Sin(radians)
            };
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new PathFigureCollection
            {
                new PathFigure
                {
                    StartPoint = GetPoint(StartAngle),
                    Segments = new PathSegmentCollection
                    {
                        new ArcSegment
                        {
                            Size = new Size(Radius, Radius),
                            Point = GetPoint(EndAngle),
                            SweepDirection = Direction
                        }
                    }
                }
            };
        }
    }
}
