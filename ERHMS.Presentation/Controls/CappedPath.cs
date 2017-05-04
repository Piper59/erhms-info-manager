using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ERHMS.Presentation.Controls
{
    public class CappedPath : FrameworkElement
    {
        public static readonly DependencyProperty StrokeProperty = Shape.StrokeProperty.AddOwner(typeof(CappedPath));
        public static readonly DependencyProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner(typeof(CappedPath));
        public static readonly DependencyProperty StrokeStartLineCapProperty = DependencyProperty.Register(
            "StrokeStartLineCap",
            typeof(Geometry),
            typeof(CappedPath),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty StrokeEndLineCapProperty = DependencyProperty.Register(
            "StrokeEndLineCap",
            typeof(Geometry), typeof(CappedPath),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data",
            typeof(PathGeometry),
            typeof(CappedPath),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public Geometry StrokeStartLineCap
        {
            get { return (Geometry)GetValue(StrokeStartLineCapProperty); }
            set { SetValue(StrokeStartLineCapProperty, value); }
        }

        public Geometry StrokeEndLineCap
        {
            get { return (Geometry)GetValue(StrokeEndLineCapProperty); }
            set { SetValue(StrokeEndLineCapProperty, value); }
        }

        public PathGeometry Data
        {
            get { return (PathGeometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private double GetRotation(Point tangent, bool start)
        {
            double angle = Math.Atan2(tangent.Y, tangent.X);
            if (start)
            {
                angle += Math.PI;
            }
            return angle * 180.0 / Math.PI;
        }

        private void RenderLineCap(DrawingContext drawingContext, Pen pen, bool start)
        {
            Geometry lineCap = start ? StrokeStartLineCap : StrokeEndLineCap;
            if (lineCap == null)
            {
                return;
            }
            Point point;
            Point tangent;
            Data.GetPointAtFractionLength(start ? 0.0 : 1.0, out point, out tangent);
            TransformGroup transforms = new TransformGroup();
            transforms.Children.Add(new RotateTransform(GetRotation(tangent, start)));
            transforms.Children.Add(new TranslateTransform(point.X, point.Y));
            drawingContext.PushTransform(transforms);
            drawingContext.DrawGeometry(Stroke, pen, lineCap);
            drawingContext.Pop();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pen pen = new Pen(Stroke, StrokeThickness);
            drawingContext.DrawGeometry(null, pen, Data);
            RenderLineCap(drawingContext, pen, true);
            RenderLineCap(drawingContext, pen, false);
        }
    }
}
