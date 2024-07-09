using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            _multiSelectionRectangle.MouseLeftButtonUp += _multiSelectionRectangle_MouseLeftButtonUp;
        }

        private void _multiSelectionRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isMoving || _isSelecting)
            {
                return;
            }
            _isMultiMoving = true;
            var frameworkElement = sender as FrameworkElement;
            frameworkElement.Cursor = Cursors.ScrollAll;
            _multiMouseDownPoint = e.GetPosition(this);
            _multiMouseDownRectanglePoint = new Point(GetLeft(_multiSelectionRectangle), GetTop(_multiSelectionRectangle));
            foreach (var item in SelectedItems)
            {
                item.MouseDownControlPoint = new Point(GetLeft(item), GetTop(item));
            }
        }

        private void _multiSelectionRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isMoving || _isSelecting)
            {
                return;
            }
            _isMultiMoving = false;
            var frameworkElement = sender as FrameworkElement;
            frameworkElement.Cursor = null;
            var selected = SelectedItems.Where(x => GetIsDraggable(x));
            SetLeft(_multiSelectionRectangle, Adsorb(GetLeft(_multiSelectionRectangle)));
            SetTop(_multiSelectionRectangle, Adsorb(GetTop(_multiSelectionRectangle)));
            foreach (var item in selected)
            {
                //if (GetLeft(_multiSelectionRectangle) < 0)
                //{
                //    SetLeft(_multiSelectionRectangle, 0);
                //    continue;
                //}
                //if (GetTop(_multiSelectionRectangle) < 0)
                //{
                //    SetTop(_multiSelectionRectangle, 0);
                //    continue;
                //}

                SetLeft(item, Adsorb(GetLeft(item)));
                SetTop(item, Adsorb(GetTop(item)));

                //SetLeft(item, item.MouseDownControlPoint.X + vector.X);
                //SetTop(item, item.MouseDownControlPoint.Y + vector.Y);


                //if (GetLeft(item) > ActualWidth - item.ActualWidth)
                //{
                //    SetLeft(item, ActualWidth - item.ActualWidth);
                //}
                //if (GetTop(item) > ActualHeight - item.ActualHeight)
                //{
                //    SetTop(item, ActualHeight - item.ActualHeight);
                //}
            }
        }

        public IEnumerable<CustomBorder> Items => Children.OfType<CustomBorder>();

        public IEnumerable<CustomBorder> SelectedItems => Items.Where(x => x.IsSelected);

        //鼠标选中多个元素的Rectangle遮罩
        private Rectangle _multiSelectionRectangle;
        //跟随鼠标运移动绘制的多选Rectangle区域
        private Rectangle _selectionRectangle;
        //鼠标选择多个元素起始的位置
        private Point _selectionStartPoint;

        //正在拖动中
        private bool _isMoving;
        //正在选择中
        private bool _isSelecting;
        //多个选中正在拖动中
        private bool _isMultiMoving;

        //鼠标指针
        private Cursor _cursor;
        //鼠标按下的位置
        private Point _mouseDownPoint;
        //鼠标按下控件的位置
        private Point _mouseDownControlPoint;
        //多个选中时鼠标按下的位置
        private Point _multiMouseDownPoint;
        //多个选中时Rectangle遮罩按下的位置
        private Point _multiMouseDownRectanglePoint;

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
            if (_isMultiMoving)
            {
                return;
            }
            Point mousePosition = e.GetPosition(this);
            Children.Remove(_multiSelectionRectangle);
            DependencyObject hitObject = VisualTreeHelper.HitTest(this, mousePosition)?.VisualHit;
            foreach (var item in Items)
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
            _selectionStartPoint = e.GetPosition(this);
            _isSelecting = true;

            // Reset the selection rectangle
            _selectionRectangle.Width = 0;
            _selectionRectangle.Height = 0;
            SetLeft(_selectionRectangle, _selectionStartPoint.X);
            SetTop(_selectionRectangle, _selectionStartPoint.Y);
        }

        private void DragCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMoving)
            {
                return;
            }

            if (_isMultiMoving)
            {
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    return;
                }
                var selected = SelectedItems.Where(x => GetIsDraggable(x));
                Point point = e.GetPosition(this);
                Vector vector = point - _multiMouseDownPoint;

                SetLeft(_multiSelectionRectangle, _multiMouseDownRectanglePoint.X + vector.X);
                SetTop(_multiSelectionRectangle, _multiMouseDownRectanglePoint.Y + vector.Y);
                foreach (var item in selected)
                {
                    //if (GetLeft(_multiSelectionRectangle) < 0)
                    //{
                    //    SetLeft(_multiSelectionRectangle, 0);
                    //    continue;
                    //}
                    //if (GetTop(_multiSelectionRectangle) < 0)
                    //{
                    //    SetTop(_multiSelectionRectangle, 0);
                    //    continue;
                    //}
                    SetLeft(item, item.MouseDownControlPoint.X + vector.X);
                    SetTop(item, item.MouseDownControlPoint.Y + vector.Y);


                    //if (GetLeft(item) > ActualWidth - item.ActualWidth)
                    //{
                    //    SetLeft(item, ActualWidth - item.ActualWidth);
                    //}
                    //if (GetTop(item) > ActualHeight - item.ActualHeight)
                    //{
                    //    SetTop(item, ActualHeight - item.ActualHeight);
                    //}
                }
            }
            else
            {
                if (!_isSelecting)
                {
                    return;
                }
                Point point = e.GetPosition(this);

                double x = Math.Min(point.X, _selectionStartPoint.X);
                double y = Math.Min(point.Y, _selectionStartPoint.Y);

                double width = Math.Abs(point.X - _selectionStartPoint.X);
                double height = Math.Abs(point.Y - _selectionStartPoint.Y);

                SetLeft(_selectionRectangle, x);
                SetTop(_selectionRectangle, y);
                _selectionRectangle.Width = width;
                _selectionRectangle.Height = height;
                Rect selectedArea = new Rect(x, y, width, height);
                foreach (var item in Items)
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
                var selected = SelectedItems.ToArray();
                foreach (var item in selected)
                {
                    double left = GetLeft(item);
                    double top = GetTop(item);
                    double right = left + item.ActualWidth;
                    double bottom = top + item.ActualHeight;

                    minX = Math.Min(left, minX);
                    minY = Math.Min(top, minY);
                    maxX = Math.Max(right, maxX);
                    maxY = Math.Max(bottom, maxY);
                }

                if (selected.Length > 1)
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
                else if (selected.Length > 0)
                {
                    Children.Remove(_multiSelectionRectangle);
                }
            }
        }

        private void DragCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isMoving || _isMultiMoving)
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
            var frameworkElement = sender as FrameworkElement;
            if (!GetIsDraggable(frameworkElement) || _isSelecting || _isMultiMoving)
            {
                return;
            }

            _cursor = frameworkElement.Cursor;
            frameworkElement.Cursor = Cursors.ScrollAll;
            _mouseDownPoint = e.GetPosition(this);
            _mouseDownControlPoint = new Point(GetLeft(frameworkElement), GetTop(frameworkElement));
            frameworkElement.CaptureMouse();
        }

        private void UIElement_MouseMove(object sender, MouseEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (!GetIsDraggable(frameworkElement) || _isSelecting || _isMultiMoving)
            {
                return;
            }
            //Debug.WriteLine(e.LeftButton + GetLeft(frameworkElement).ToString() + GetTop(frameworkElement));
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(this);

                DependencyObject hitObject = VisualTreeHelper.HitTest(this, point)?.VisualHit;
                if (hitObject is Ellipse)
                {

                }

                Vector vector = point - _mouseDownPoint;
                SetLeft(frameworkElement, Math.Round(_mouseDownControlPoint.X + vector.X, 0));
                SetTop(frameworkElement, Math.Round(_mouseDownControlPoint.Y + vector.Y, 0));

                if (GetLeft(frameworkElement) < 0)
                {
                    SetLeft(frameworkElement, 0);
                }
                if (GetTop(frameworkElement) < 0)
                {
                    SetTop(frameworkElement, 0);
                }

                if (GetLeft(frameworkElement) > ActualWidth - frameworkElement.ActualWidth)
                {
                    SetLeft(frameworkElement, ActualWidth - frameworkElement.ActualWidth);
                }
                if (GetTop(frameworkElement) > ActualHeight - frameworkElement.ActualHeight)
                {
                    SetTop(frameworkElement, ActualHeight - frameworkElement.ActualHeight);
                }
                //Debug.WriteLine($"left:{GetLeft(uIElement)},top:{GetTop(uIElement)}");
            }
        }

        private void UIElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMoving = false;
            var frameworkElement = sender as FrameworkElement;
            if (!GetIsDraggable(frameworkElement) || _isSelecting || _isMultiMoving)
            {
                return;
            }
            frameworkElement.Cursor = _cursor;
            SetLeft(frameworkElement, Adsorb(GetLeft(frameworkElement)));
            SetTop(frameworkElement, Adsorb(GetTop(frameworkElement)));
            frameworkElement.ReleaseMouseCapture();
        }

        public double Adsorb(double value)
        {
            if (double.IsNaN(value))
            {
                return value;
            }
            int quotient = (int)(value / GridSize);
            var min = Math.Max(0, GridSize * quotient);
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