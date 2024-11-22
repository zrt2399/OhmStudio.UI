using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OhmStudio.UI.Utilities;

namespace OhmStudio.UI.Controls
{
    public class ShapeBorder : Border
    {
        public static readonly DependencyProperty ShapeTypeProperty =
            DependencyProperty.Register(nameof(ShapeType), typeof(ShapeType), typeof(ShapeBorder), new FrameworkPropertyMetadata(ShapeType.Rectangle, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IsDashedProperty =
            DependencyProperty.Register(nameof(IsDashed), typeof(bool), typeof(ShapeBorder), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShearProperty =
            DependencyProperty.Register(nameof(Shear), typeof(double), typeof(ShapeBorder), new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register(nameof(Geometry), typeof(Geometry), typeof(ShapeBorder));

        public ShapeType ShapeType
        {
            get => (ShapeType)GetValue(ShapeTypeProperty);
            set => SetValue(ShapeTypeProperty, value);
        }

        public bool IsDashed
        {
            get => (bool)GetValue(IsDashedProperty);
            set => SetValue(IsDashedProperty, value);
        }

        public double Shear
        {
            get => (double)GetValue(ShearProperty);
            set => SetValue(ShearProperty, value);
        }

        public Geometry Geometry
        {
            get => (Geometry)GetValue(GeometryProperty);
            set => SetValue(GeometryProperty, value);
        }

        private Geometry DrawRoundedRectangle(DrawingContext dc, Brush brush, Pen pen, Rect rect, CornerRadius cornerRadius)
        {
            var geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                var minX = rect.Width / 2;
                var minY = rect.Height / 2;

                cornerRadius.TopLeft = Math.Min(Math.Min(cornerRadius.TopLeft, minX), minY);
                cornerRadius.TopRight = Math.Min(Math.Min(cornerRadius.TopRight, minX), minY);
                cornerRadius.BottomRight = Math.Min(Math.Min(cornerRadius.BottomRight, minX), minY);
                cornerRadius.BottomLeft = Math.Min(Math.Min(cornerRadius.BottomLeft, minX), minY);

                ctx.BeginFigure(new Point(rect.Left + cornerRadius.TopLeft, rect.Top), true, true);

                // Top line and top-right corner
                ctx.LineTo(new Point(rect.Right - cornerRadius.TopRight, rect.Top), true, false);
                ctx.ArcTo(new Point(rect.Right, rect.Top + cornerRadius.TopRight), new Size(cornerRadius.TopRight, cornerRadius.TopRight), 0, false, SweepDirection.Clockwise, true, false);

                // Right line and bottom-right corner
                ctx.LineTo(new Point(rect.Right, rect.Bottom - cornerRadius.BottomRight), true, false);
                ctx.ArcTo(new Point(rect.Right - cornerRadius.BottomRight, rect.Bottom), new Size(cornerRadius.BottomRight, cornerRadius.BottomRight), 0, false, SweepDirection.Clockwise, true, false);

                // Bottom line and bottom-left corner
                ctx.LineTo(new Point(rect.Left + cornerRadius.BottomLeft, rect.Bottom), true, false);
                ctx.ArcTo(new Point(rect.Left, rect.Bottom - cornerRadius.BottomLeft), new Size(cornerRadius.BottomLeft, cornerRadius.BottomLeft), 0, false, SweepDirection.Clockwise, true, false);

                // Left line and top-left corner
                ctx.LineTo(new Point(rect.Left, rect.Top + cornerRadius.TopLeft), true, false);
                ctx.ArcTo(new Point(rect.Left + cornerRadius.TopLeft, rect.Top), new Size(cornerRadius.TopLeft, cornerRadius.TopLeft), 0, false, SweepDirection.Clockwise, true, false);
            }
            geometry.Freeze();
            dc.DrawGeometry(brush, pen, geometry);
            return geometry;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pen pen = null;
            var background = Background;
            var borderBrush = BorderBrush;
            var thickness = new double[] { BorderThickness.Left, BorderThickness.Top, BorderThickness.Right, BorderThickness.Bottom }.Max();

            if (borderBrush != null)
            {
                pen = new Pen(borderBrush, thickness);
                if (IsDashed)
                {
                    pen.DashStyle = DashStyles.Dash;
                }
                if (borderBrush.IsFrozen)
                {
                    pen.Freeze();
                }
            }

            double num = thickness * 0.5;
            switch (ShapeType)
            {
                case ShapeType.Diamond:
                    Geometry = drawingContext.DrawPolygon(background, pen, new Point(ActualWidth / 2, num), new Point(ActualWidth - num, ActualHeight / 2), new Point(ActualWidth / 2, ActualHeight - num), new Point(num, ActualHeight / 2));
                    break;
                case ShapeType.Parallelogram:
                    double shear = Shear;
                    Geometry = drawingContext.DrawPolygon(background, pen, new Point(Math.Min(shear, ActualWidth), num), new Point(ActualWidth - num, num), new Point(Math.Max(0, ActualWidth - shear), ActualHeight - num), new Point(num, ActualHeight - num));
                    break;
                default:
                    Rect rect = new Rect(new Point(num, num), new Point(ActualWidth - num, ActualHeight - num));
                    Geometry = DrawRoundedRectangle(drawingContext, background, pen, rect, CornerRadius);

                    //double radius = new double[] { CornerRadius.TopLeft, CornerRadius.TopRight, CornerRadius.BottomRight, CornerRadius.BottomLeft }.Max();
                    //drawingContext.DrawRoundedRectangle(background, pen, rectangle, radius, radius); 
                    //base.OnRender(drawingContext);
                    break;
            }
        }
    }
}