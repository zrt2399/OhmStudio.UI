using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        private Cursor _cursor;
        //鼠标按下的位置
        private Point _mouseDownPosition;
        //鼠标按下控件的位置
        private Point _mouseDownControlPosition;

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
            DependencyProperty.RegisterAttached("IsDraggable", typeof(bool), typeof(GridCanvas), new PropertyMetadata(true));

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
            if (sender is FrameworkElement frameworkElement)
            {
                frameworkElement.Cursor = _cursor;
            }
            SetLeft(uIElement, Adsorb(GetLeft(uIElement)));
            SetTop(uIElement, Adsorb(GetTop(uIElement)));
            uIElement.ReleaseMouseCapture();
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

                //Debug.WriteLine($"left:{GetLeft(uIElement)},top:{GetTop(uIElement)}");
            }
        }

        public double Adsorb(double value)
        {
            int quotient = (int)(value / GridSize);
            var min = GridSize * quotient;
            var max = min + GridSize;

            if (value - min > GridSize / 2)
            {
                return max;
            }
            else
            {
                return min;
            }
        }

        private void UIElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement uIElement = sender as UIElement;
            if (!GetIsDraggable(uIElement))
            {
                return;
            }
            //uIElement.Focus();
            if (sender is FrameworkElement frameworkElement)
            {
                _cursor = frameworkElement.Cursor;
                frameworkElement.Cursor = Cursors.ScrollAll;
            }
            _mouseDownPosition = e.GetPosition(this);
            _mouseDownControlPosition = new Point(double.IsNaN(GetLeft(uIElement)) ? 0 : GetLeft(uIElement), double.IsNaN(GetTop(uIElement)) ? 0 : GetTop(uIElement));
            uIElement.CaptureMouse();
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