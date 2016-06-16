using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace ERHMS.Presentation.Markup
{
    [MarkupExtensionReturnType(typeof(PathFigureCollection))]
    public class ArcExtension : MarkupExtension
    {
        [ConstructorArgument("x")]
        public double X { get; set; }

        [ConstructorArgument("y")]
        public double Y { get; set; }

        [ConstructorArgument("radius")]
        public double Radius { get; set; }

        [ConstructorArgument("startAngle")]
        public double StartAngle { get; set; }

        [ConstructorArgument("endAngle")]
        public double EndAngle { get; set; }

        [ConstructorArgument("clockwise")]
        public bool Clockwise { get; set; }

        public ArcExtension() { }

        public ArcExtension(double x, double y, double radius, double startAngle, double endAngle, bool clockwise)
        {
            X = x;
            Y = y;
            Radius = radius;
            StartAngle = startAngle;
            EndAngle = endAngle;
            Clockwise = clockwise;
        }

        private Point GetPoint(double angle)
        {
            double radians = angle * Math.PI / 180.0;
            return new Point(X + Radius * Math.Cos(radians), Y - Radius * Math.Sin(radians));
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
                            SweepDirection = Clockwise ? SweepDirection.Clockwise : SweepDirection.Counterclockwise
                        }
                    }
                }
            };
        }
    }
}
