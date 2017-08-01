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

        [ConstructorArgument("start")]
        public double Start { get; set; }

        [ConstructorArgument("end")]
        public double End { get; set; }

        [ConstructorArgument("direction")]
        public SweepDirection Direction { get; set; }

        public ArcExtension() { }

        public ArcExtension(Point center, double radius, double start, double end, SweepDirection direction)
        {
            Center = center;
            Radius = radius;
            Start = start;
            End = end;
            Direction = direction;
        }

        private Point GetPoint(double degrees)
        {
            double radians = degrees * Math.PI / 180.0;
            return new Point
            {
                X = Center.X + Radius * Math.Cos(radians),
                Y = Center.Y - Radius * Math.Sin(radians)
            };
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new PathFigureCollection
            {
                new PathFigure
                {
                    StartPoint = GetPoint(Start),
                    Segments = new PathSegmentCollection
                    {
                        new ArcSegment
                        {
                            Size = new Size(Radius, Radius),
                            Point = GetPoint(End),
                            SweepDirection = Direction
                        }
                    }
                }
            };
        }
    }
}
