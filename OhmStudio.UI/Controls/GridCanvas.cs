using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OhmStudio.UI.Controls
{
    public class GridCanvas : Canvas
    {
        static GridCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridCanvas), new FrameworkPropertyMetadata(typeof(GridCanvas)));
        }

        public static readonly DependencyProperty LineBrushProperty =
           DependencyProperty.Register(nameof(LineBrush), typeof(Brush), typeof(GridCanvas),
               new FrameworkPropertyMetadata(Brushes.LightGray, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush LineBrush
        {
            get => (Brush)GetValue(LineBrushProperty);
            set => SetValue(LineBrushProperty, value);
        }

        public static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register(nameof(GridSize), typeof(double), typeof(GridCanvas),
                new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsRender));

        public double GridSize
        {
            get => (double)GetValue(GridSizeProperty);
            set => SetValue(GridSizeProperty, value);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            double width = ActualWidth;
            double height = ActualHeight;
            double gridSize = GridSize;

            Pen pen = new Pen(LineBrush, 0.4);

            for (double x = 0; x < width; x += gridSize)
            {
                dc.DrawLine(pen, new Point(x, 0), new Point(x, height));
            }

            for (double y = 0; y < height; y += gridSize)
            {
                dc.DrawLine(pen, new Point(0, y), new Point(width, y));
            }
        } 
    }
} 