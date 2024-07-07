using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Controls
{
    public class DragCanvas : Canvas
    {
        static DragCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragCanvas), new FrameworkPropertyMetadata(typeof(DragCanvas)));
        }

        public DragCanvas()
        {
            MouseLeftButtonDown += DragCanvas_MouseLeftButtonDown;
            MouseMove += DragCanvas_MouseMove;
            MouseLeftButtonUp += DragCanvas_MouseLeftButtonUp;

            _multiSelectionRectangle = new Rectangle();
            _multiSelectionRectangle.Fill = "#44AACCEE".ToSolidColorBrush();
            _multiSelectionRectangle.Stroke = "#FF0F80D9".ToSolidColorBrush();
            _multiSelectionRectangle.StrokeDashArray = new DoubleCollection(new double[] { 2, 2 });
            _multiSelectionRectangle.MouseLeftButtonDown += _multiSelectionRectangle_MouseLeftButtonDown;  
            _multiSelectionRectangle.MouseMove += _multiSelectionRectangle_MouseMove;
            _multiSelectionRectangle.MouseLeftButtonUp += _multiSelectionRectangle_MouseLeftButtonUp;
        }

        private void _multiSelectionRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _multiSelectionRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void _multiSelectionRectangle_MouseMove(object sender, MouseEventArgs e)
        {
            var list = Children.OfType<CustomBorder>().Where(x => GetIsDraggable(x) && x.IsSelected).ToArray();
        }

        private void DragCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Point mousePosition = e.GetPosition(this);
            //Children.Remove(_multiSelectionRectangle);
            //DependencyObject hitObject = VisualTreeHelper.HitTest(this, mousePosition).VisualHit;
            //foreach (var item in Children.OfType<CustomBorder>())
            //{
            //    item.IsSelected = false;
            //}
            //if (hitObject != null && hitObject.FindParentObject<CustomBorder>() is CustomBorder customBorder)
            //{
            //    customBorder.IsSelected = true;
            //}
        }

        private Rectangle _multiSelectionRectangle;
        private Rectangle _selectionRectangle;
        private Point _startPoint;
        private bool _isSelecting;
        private bool _isMoving;
        private bool _lastInputIsDraggable;
        private Cursor _cursor;
        //鼠标按下的位置
        private Point _mouseDownPosition;
        //鼠标按下控件的位置
        private Point _mouseDownControlPosition;

        public static readonly DependencyProperty LineBrushProperty =
           DependencyProperty.Register(nameof(LineBrush), typeof(Brush), typeof(DragCanvas),
               new FrameworkPropertyMetadata(Brushes.LightGray, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register(nameof(GridSize), typeof(double), typeof(DragCanvas),
                new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush LineBrush
        {
            get => (Brush)GetValue(LineBrushProperty);
            set => SetValue(LineBrushProperty, value);
        }

        public double GridSize
        {
            get => (double)GetValue(GridSizeProperty);
            set => SetValue(GridSizeProperty, value);
        }

        public static readonly DependencyProperty IsDraggableProperty =
            DependencyProperty.RegisterAttached("IsDraggable", typeof(bool), typeof(DragCanvas), new PropertyMetadata(true));

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
            if (visualAdded is CustomBorder customBorder)
            {
                if (double.IsNaN(GetLeft(customBorder)))
                {
                    SetLeft(customBorder, 0);
                }
                if (double.IsNaN(GetTop(customBorder)))
                {
                    SetTop(customBorder, 0);
                }
                customBorder.MouseLeftButtonDown += UIElement_MouseLeftButtonDown;
                customBorder.MouseMove += UIElement_MouseMove;
                customBorder.MouseLeftButtonUp += UIElement_MouseLeftButtonUp;
            }
        }

        private void DragCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);
            Children.Remove(_multiSelectionRectangle);
            DependencyObject hitObject = VisualTreeHelper.HitTest(this, mousePosition)?.VisualHit;
            foreach (var item in Children.OfType<CustomBorder>())
            {
                item.IsSelected = false;
            }
            if (hitObject?.FindParentObject<CustomBorder>() is CustomBorder customBorder)
            {
                customBorder.IsSelected = true;
            }

            if (_isMoving)
            {
                return;
            }

            _selectionRectangle = new Rectangle();
            _selectionRectangle.Fill = "#88AACCEE".ToSolidColorBrush();
            _selectionRectangle.Stroke = "#FF0F80D9".ToSolidColorBrush();

            SetZIndex(_selectionRectangle, int.MaxValue);
            Children.Add(_selectionRectangle);
            _startPoint = e.GetPosition(this);
            _isSelecting = true;

            // Reset the selection rectangle
            _selectionRectangle.Width = 0;
            _selectionRectangle.Height = 0;
            SetLeft(_selectionRectangle, _startPoint.X);
            SetTop(_selectionRectangle, _startPoint.Y);
        }

        private void DragCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMoving)
            {
                return;
            }
            if (_isSelecting)
            {
                Point currentPoint = e.GetPosition(this);

                double x = Math.Min(currentPoint.X, _startPoint.X);
                double y = Math.Min(currentPoint.Y, _startPoint.Y);

                double width = Math.Abs(currentPoint.X - _startPoint.X);
                double height = Math.Abs(currentPoint.Y - _startPoint.Y);

                SetLeft(_selectionRectangle, x);
                SetTop(_selectionRectangle, y);
                _selectionRectangle.Width = width;
                _selectionRectangle.Height = height;
                Rect selectedArea = new Rect(x, y, width, height);
                foreach (var item in Children.OfType<CustomBorder>())
                {
                    item.IsSelected = false;
                    if (IsInside(selectedArea, new Rect(GetLeft(item), GetTop(item), item.ActualWidth, item.ActualHeight)))
                    {
                        item.IsSelected = true;
                    }
                }

                double minX = double.MaxValue;
                double minY = double.MaxValue;
                double maxX = double.MinValue;
                double maxY = double.MinValue;
                var list = Children.OfType<CustomBorder>().Where(x => x.IsSelected).ToArray();
                foreach (var item in list)
                {
                    double left = GetLeft(item);
                    double top = GetTop(item);
                    double right = left + item.Width;
                    double bottom = top + item.Height;

                    minX = Math.Min(left, minX);
                    minY = Math.Min(top, minY);
                    maxX = Math.Max(right, maxX);
                    maxY = Math.Max(bottom, maxY);
                }

                if (list.Length > 1)
                {
                    _multiSelectionRectangle.Width = maxX - minX;
                    _multiSelectionRectangle.Height = maxY - minY;
                    SetLeft(_multiSelectionRectangle, minX);
                    SetTop(_multiSelectionRectangle, minY);
                    //SetZIndex(multiSelectionRectangle, int.MinValue);
                    if (!Children.Contains(_multiSelectionRectangle))
                    {
                        Children.Add(_multiSelectionRectangle);
                    }
                }
                else if (list.Length > 0)
                {
                    Children.Remove(_multiSelectionRectangle);
                }
            }
        }

        private void DragCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isMoving)
            {
                return;
            }
            _isSelecting = false;
            Children.Remove(_selectionRectangle);
            _selectionRectangle = null;
        }

        public static bool IsInside(Rect outerRect, Rect innerRect)
        {
            // 获取内部矩形的四个顶点
            Point topLeft = new Point(innerRect.Left, innerRect.Top);
            Point topRight = new Point(innerRect.Right, innerRect.Top);
            Point bottomLeft = new Point(innerRect.Left, innerRect.Bottom);
            Point bottomRight = new Point(innerRect.Right, innerRect.Bottom);

            return outerRect.Contains(topLeft) || outerRect.Contains(topRight) || outerRect.Contains(bottomLeft) || outerRect.Contains(bottomRight);
        }

        private void UIElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMoving = true;
            UIElement uIElement = sender as UIElement;
            if (!GetIsDraggable(uIElement) || _isSelecting)
            {
                _lastInputIsDraggable = false;
                return;
            }
            _lastInputIsDraggable = true;
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

        private void UIElement_MouseMove(object sender, MouseEventArgs e)
        {
            UIElement uIElement = sender as UIElement;
            if (!GetIsDraggable(uIElement) || !_lastInputIsDraggable || _isSelecting)
            {
                return;
            }
            //Debug.WriteLine(e.LeftButton + GetLeft(uIElement).ToString() + GetTop(uIElement));
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(this);

                DependencyObject hitObject = VisualTreeHelper.HitTest(this, point)?.VisualHit;
                if (hitObject is Ellipse)
                {

                }

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

                if (GetLeft(uIElement) > ActualWidth - uIElement.RenderSize.Width)
                {
                    SetLeft(uIElement, ActualWidth - uIElement.RenderSize.Width);
                }
                if (GetTop(uIElement) > ActualHeight - uIElement.RenderSize.Height)
                {
                    SetTop(uIElement, ActualHeight - uIElement.RenderSize.Height);
                }
                //Debug.WriteLine($"left:{GetLeft(uIElement)},top:{GetTop(uIElement)}");
            }
        }

        private void UIElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMoving = false;
            UIElement uIElement = sender as UIElement;
            if (!GetIsDraggable(uIElement) || _isSelecting)
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

        public double Adsorb(double value)
        {
            if (double.IsNaN(value))
            {
                return value;
            }
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