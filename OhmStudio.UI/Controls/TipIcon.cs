using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OhmStudio.UI.Messaging;
using OhmStudio.UI.Utilities;

namespace OhmStudio.UI.Controls
{
    public class IconBase : Control
    {
        public static readonly DependencyProperty IconTypeProperty =
            DependencyProperty.Register(nameof(IconType), typeof(IconType), typeof(IconBase), new FrameworkPropertyMetadata(IconType.Information, FrameworkPropertyMetadataOptions.AffectsRender));

        public IconType IconType
        {
            get => (IconType)GetValue(IconTypeProperty);
            set => SetValue(IconTypeProperty, value);
        }
    }

    public class TipIcon : IconBase
    {
        static TipIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TipIcon), new FrameworkPropertyMetadata(typeof(TipIcon)));
        }
    }

    internal class Icon : IconBase
    {
        public Icon()
        {
            Height = 20;
            Width = 20;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (IconType == IconType.Information)
            {
                var background = new SolidColorBrush(Color.FromRgb(103, 148, 186));
                var pen = new Pen(background, 2);
                drawingContext.DrawEllipse(background, pen, new Point(ActualHeight / 2, ActualWidth / 2), ActualHeight / 2 - 1, ActualWidth / 2 - 1);

                var exclamationPen = new Pen(new SolidColorBrush(Color.FromRgb(245, 245, 245)), 2);
                drawingContext.DrawLine(exclamationPen, new Point(10, 4), new Point(10, 6));
                drawingContext.DrawLine(exclamationPen, new Point(10, 8), new Point(10, 16));
            }
            else if (IconType == IconType.Success)
            {
                StreamGeometry geometry = new StreamGeometry();
                using (StreamGeometryContext ctx = geometry.Open())
                {
                    ctx.BeginFigure(new Point(1.4, 10), false, false);
                    ctx.LineTo(new Point(8, 16), true, false);
                    ctx.LineTo(new Point(18.4, 2), true, false);
                }
                var background = new SolidColorBrush(Color.FromRgb(110, 190, 40));
                var pen = new Pen(background, 4);
                drawingContext.DrawGeometry(background, pen, geometry);
            }
            else if (IconType == IconType.Warning)
            {
                var points = new[] { new Point(10, 1), new Point(19.2, 18.6), new Point(0.8, 18.6) };
                var background = new SolidColorBrush(Color.FromRgb(220, 155, 40));
                var pen = new Pen(background, 2) { LineJoin = PenLineJoin.Bevel };
                drawingContext.DrawPolygon(background, pen, points);
                var exclamationPen = new Pen(new SolidColorBrush(Color.FromRgb(251, 245, 233)), 2);
                drawingContext.DrawLine(exclamationPen, new Point(10, 6), new Point(10, 14));
                drawingContext.DrawLine(exclamationPen, new Point(10, 16), new Point(10, 18));
            }
            else if (IconType == IconType.Error)
            {
                var pen = new Pen(new SolidColorBrush(Color.FromRgb(230, 80, 80)), 4);
                drawingContext.DrawLine(pen, new Point(2, 2), new Point(18, 18));
                drawingContext.DrawLine(pen, new Point(18, 2), new Point(2, 18));
            }
        }
    }
}