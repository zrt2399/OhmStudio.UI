using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        public static readonly DependencyProperty IsDraggableProperty =
            DependencyProperty.RegisterAttached("IsDraggable", typeof(bool), typeof(GridCanvas), new PropertyMetadata(true, (sender, e) =>
            {
                if (sender is FrameworkElement frameworkElement)
                {
                    if ((bool)e.NewValue)
                    {
                        frameworkElement.Cursor = Cursors.ScrollAll;
                    }
                    else
                    {
                        frameworkElement.Cursor = Cursors.Arrow;
                    }
                }
            }));

        public static void SetIsDraggable(DependencyObject element, bool value)
        {
            element.SetValue(IsDraggableProperty, value);
        }

        public static bool GetIsDraggable(DependencyObject element)
        {
            return (bool)element.GetValue(IsDraggableProperty);
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            if (visualAdded is UIElement uIElement)
            {
                //uIElement.Focusable = true;
                if (uIElement is FrameworkElement frameworkElement)
                {
                    frameworkElement.Cursor = Cursors.ScrollAll;
                }
                uIElement.MouseLeftButtonDown += UIElement_MouseLeftButtonDown;
                uIElement.MouseMove += UIElement_MouseMove;
                uIElement.MouseLeftButtonUp += UIElement_MouseLeftButtonUp;
            }
        }

        private void UIElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIElement uIElement = sender as UIElement;
            if (!GetIsDraggable(uIElement))
            {
                return;
            }
            uIElement.ReleaseMouseCapture();
            if (GetLeft(uIElement) < 0)
            {
                SetLeft(uIElement, 0);
            }
            if (GetTop(uIElement) < 0)
            {
                SetTop(uIElement, 0);
            }
            Debug.WriteLine($"left:{GetLeft(uIElement)},top:{GetTop(uIElement)}");
        }

        private void UIElement_MouseMove(object sender, MouseEventArgs e)
        {
            UIElement uIElement = sender as UIElement;
            if (!GetIsDraggable(uIElement))
            {
                return;
            }
            //Debug.WriteLine(e.LeftButton + GetLeft(uIElement).ToString() + GetTop(uIElement));
            if (e.LeftButton == MouseButtonState.Pressed /*&& GetLeft(uIElement) >= 0 && GetTop(uIElement) >= 0*/)
            {
                Point point = e.GetPosition(this);
                Vector vector = point - _mouseDownPosition;
                SetLeft(uIElement, Math.Round(_mouseDownControlPosition.X + vector.X, 0));
                SetTop(uIElement, Math.Round(_mouseDownControlPosition.Y + vector.Y, 0));

                if (GetLeft(uIElement) < 0)
                {
                    SetLeft(uIElement, 0);
                }
                if (GetTop(uIElement) < 0)
                {
                    SetTop(uIElement, 0);
                }
                Debug.WriteLine($"left:{GetLeft(uIElement)},top:{GetTop(uIElement)}");
                //txtx.Text = Canvas.GetLeft(c).ToString();
                //txty.Text = Canvas.SetTop(c).ToString();
            }
        }

        //鼠标按下的位置
        private Point _mouseDownPosition;
        //鼠标按下控件的位置
        private Point _mouseDownControlPosition;

        private void UIElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement uIElement = sender as UIElement;
            if (!GetIsDraggable(uIElement))
            {
                return;
            }
            //uIElement.Focus();
            _mouseDownPosition = e.GetPosition(this);
            _mouseDownControlPosition = new Point(double.IsNaN(GetLeft(uIElement)) ? 0 : GetLeft(uIElement), double.IsNaN(GetTop(uIElement)) ? 0 : GetTop(uIElement));
            Debug.WriteLine(uIElement.CaptureMouse());
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