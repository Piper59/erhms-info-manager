using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ERHMS.Presentation.Controls
{
    public class CappedPath : FrameworkElement
    {
        public static readonly DependencyProperty StrokeProperty = Shape.StrokeProperty.AddOwner(typeof(CappedPath));
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner(typeof(CappedPath));
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeStartLineCapProperty = DependencyProperty.Register(
            "StrokeStartLineCap",
            typeof(Geometry),
            typeof(CappedPath),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public Geometry StrokeStartLineCap
        {
            get { return (Geometry)GetValue(StrokeStartLineCapProperty); }
            set { SetValue(StrokeStartLineCapProperty, value); }
        }

        public static readonly DependencyProperty StrokeEndLineCapProperty = DependencyProperty.Register(
            "StrokeEndLineCap",
            typeof(Geometry),
            typeof(CappedPath),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public Geometry StrokeEndLineCap
        {
            get { return (Geometry)GetValue(StrokeEndLineCapProperty); }
            set { SetValue(StrokeEndLineCapProperty, value); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data",
            typeof(PathGeometry),
            typeof(CappedPath),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public PathGeometry Data
        {
            get { return (PathGeometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private void DrawLineCap(DrawingContext drawingContext, Pen pen, bool start)
        {
            Point point;
            Point tangent;
            double progress = start ? 0.0 : 1.0;
            Data.GetPointAtFractionLength(progress, out point, out tangent);
            double angle = Math.Atan2(tangent.Y, tangent.X) * 180.0 / Math.PI;
            if (start)
            {
                angle += 180.0;
            }
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new RotateTransform(angle));
            transformGroup.Children.Add(new TranslateTransform(point.X, point.Y));
            drawingContext.PushTransform(transformGroup);
            drawingContext.DrawGeometry(Stroke, pen, start ? StrokeStartLineCap : StrokeEndLineCap);
            drawingContext.Pop();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pen pen = new Pen(Stroke, StrokeThickness);
            drawingContext.DrawGeometry(null, pen, Data);
            if (StrokeStartLineCap != null)
            {
                DrawLineCap(drawingContext, pen, true);
            }
            if (StrokeEndLineCap != null)
            {
                DrawLineCap(drawingContext, pen, false);
            }
        }
    }
}
