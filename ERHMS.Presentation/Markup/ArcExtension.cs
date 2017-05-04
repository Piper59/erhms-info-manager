using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace ERHMS.Presentation.Markup
{
    [MarkupExtensionReturnType(typeof(PathFigureCollection))]
    public class ArcExtension : MarkupExtension
    {
        [ConstructorArgument("center")]
        public Point Center { get; set; }

        [ConstructorArgument("radius")]
        public double Radius { get; set; }

        [ConstructorArgument("startAngle")]
        public double StartAngle { get; set; }

        [ConstructorArgument("endAngle")]
        public double EndAngle { get; set; }

        [ConstructorArgument("sweepDirection")]
        public SweepDirection SweepDirection { get; set; }

        public ArcExtension() { }

        public ArcExtension(Point center, double radius, double startAngle, double endAngle, SweepDirection sweepDirection)
        {
            Center = center;
            Radius = radius;
            StartAngle = startAngle;
            EndAngle = endAngle;
            SweepDirection = sweepDirection;
        }

        private Point GetPoint(double angle)
        {
            double angleInRadians = angle * Math.PI / 180.0;
            return new Point
            {
                X = Center.X + Radius * Math.Cos(angleInRadians),
                Y = Center.Y - Radius * Math.Sin(angleInRadians)
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
                            SweepDirection = SweepDirection
                        }
                    }
                }
            };
        }
    }
}
