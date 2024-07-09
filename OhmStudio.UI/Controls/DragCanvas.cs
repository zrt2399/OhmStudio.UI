using System;
using System.Collections.Generic;
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
            _multiSelectionRectangle.MouseLeftButtonDown += MultiSelectionRectangle_MouseLeftButtonDown;
            _multiSelectionRectangle.MouseLeftButtonUp += MultiSelectionRectangle_MouseLeftButtonUp;

            _selectionRectangle = new Rectangle();
            _selectionRectangle.Fill = "#88AACCEE".ToSolidColorBrush();
            _selectionRectangle.Stroke = "#FF0F80D9".ToSolidColorBrush();
            SetZIndex(_selectionRectangle, int.MaxValue);
        }

        //鼠标选中多个元素的Rectangle遮罩
        private Rectangle _multiSelectionRectangle;
        //跟随鼠标运移动绘制的多选Rectangle区域
        private Rectangle _selectionRectangle;
        //鼠标选择多个元素起始的位置
        private Point _selectionStartPoint;

        //正在拖动中
        private bool _isMoving;
        //正在绘制曲线中
        private bool _isDrawing;
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

        private StepItem _lastStepItem;

        private Point _pathStartPoint;
        private Path _currentPath;
        private PathFigure _pathFigure;
        private BezierSegment _bezierSegment;

        public IEnumerable<StepItem> Items => Children.OfType<StepItem>();

        public IEnumerable<StepItem> SelectedItems => Items.Where(x => x.IsSelected);

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
            if (visualAdded is StepItem stepItem)
            {
                if (double.IsNaN(GetLeft(stepItem)))
                {
                    SetLeft(stepItem, 0);
                }
                if (double.IsNaN(GetTop(stepItem)))
                {
                    SetTop(stepItem, 0);
                }
                stepItem.MouseLeftButtonDown += StepItem_MouseLeftButtonDown;
                stepItem.MouseLeftButtonUp += StepItem_MouseLeftButtonUp;
            }
        }

        private void MultiSelectionRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isMoving || _isDrawing || _isSelecting)
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

        private void MultiSelectionRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_isMoving || _isDrawing || _isSelecting)
                {
                    return;
                }
                var frameworkElement = sender as FrameworkElement;
                frameworkElement.Cursor = null;

                SetLeft(_multiSelectionRectangle, Adsorb(GetLeft(_multiSelectionRectangle)));
                SetTop(_multiSelectionRectangle, Adsorb(GetTop(_multiSelectionRectangle)));
                foreach (var item in SelectedItems.Where(x => GetIsDraggable(x)))
                {
                    SetLeft(item, Adsorb(GetLeft(item)));
                    SetTop(item, Adsorb(GetTop(item)));
                }
            }
            finally
            {
                _isMultiMoving = false;
            }
        }

        private void DragCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isMultiMoving)
            {
                return;
            }
            Point point = e.GetPosition(this);
            Children.Remove(_multiSelectionRectangle);
            foreach (var item in Items)
            {
                item.IsSelected = false;
            }
            DependencyObject hitObject = VisualTreeHelper.HitTest(this, point)?.VisualHit;
            if (hitObject?.FindParentObject<StepItem>() is StepItem stepItem)
            {
                stepItem.IsSelected = true;
            }

            if (_isMoving || _isDrawing)
            {
                return;
            }

            if (!Children.Contains(_selectionRectangle))
            {
                Children.Add(_selectionRectangle);
            }

            _selectionStartPoint = point;
            _isSelecting = true;

            // Reset the selection rectangle
            _selectionRectangle.Width = 0;
            _selectionRectangle.Height = 0;
            SetLeft(_selectionRectangle, _selectionStartPoint.X);
            SetTop(_selectionRectangle, _selectionStartPoint.Y);
        }

        private void DragCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            Point point = e.GetPosition(this);
            if (_isMultiMoving)
            {
                Vector vector = point - _multiMouseDownPoint;

                SetLeft(_multiSelectionRectangle, _multiMouseDownRectanglePoint.X + vector.X);
                SetTop(_multiSelectionRectangle, _multiMouseDownRectanglePoint.Y + vector.Y);
                if (GetLeft(_multiSelectionRectangle) < 0)
                {
                    SetLeft(_multiSelectionRectangle, 0);
                }
                if (GetTop(_multiSelectionRectangle) < 0)
                {
                    SetTop(_multiSelectionRectangle, 0);
                }
                foreach (var item in SelectedItems.Where(x => GetIsDraggable(x)))
                {
                    var minX = item.MouseDownControlPoint.X - _multiMouseDownRectanglePoint.X;
                    var minY = item.MouseDownControlPoint.Y - _multiMouseDownRectanglePoint.Y;
                    SetLeft(item, item.MouseDownControlPoint.X + vector.X);
                    SetTop(item, item.MouseDownControlPoint.Y + vector.Y);
                    if (GetLeft(item) < minX)
                    {
                        SetLeft(item, minX);
                    }
                    if (GetTop(item) < minY)
                    {
                        SetTop(item, minY);
                    }

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
            else if (_isSelecting)
            {
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
                    if (selectedArea.IntersectsWith(new Rect(GetLeft(item), GetTop(item), item.ActualWidth, item.ActualHeight)))
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
            else if (_isDrawing)
            {
                if (_currentPath == null)
                {
                    // Initialize the path and the Bezier curve only when mouse moves after the initial click
                    _currentPath = new Path
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };

                    _pathFigure = new PathFigure { StartPoint = _pathStartPoint };
                    _bezierSegment = new BezierSegment();
                    _pathFigure.Segments.Add(_bezierSegment);
                    PathGeometry pathGeometry = new PathGeometry();
                    pathGeometry.Figures.Add(_pathFigure);
                    _currentPath.Data = pathGeometry;

                    Children.Add(_currentPath);
                }

                _bezierSegment.Point1 = new Point((_pathStartPoint.X + point.X) / 2, _pathStartPoint.Y);
                _bezierSegment.Point2 = new Point((_pathStartPoint.X + point.X) / 2, point.Y);
                _bezierSegment.Point3 = point;
                _currentPath.Data = new PathGeometry(new PathFigure[] { _pathFigure });
            }
            else if (_isMoving)
            {
                var stepItem = _lastStepItem;
                if (!GetIsDraggable(stepItem))
                {
                    return;
                }
                Vector vector = point - _mouseDownPoint;
                SetLeft(stepItem, Math.Round(_mouseDownControlPoint.X + vector.X, 0));
                SetTop(stepItem, Math.Round(_mouseDownControlPoint.Y + vector.Y, 0));

                if (GetLeft(stepItem) < 0)
                {
                    SetLeft(stepItem, 0);
                }
                if (GetTop(stepItem) < 0)
                {
                    SetTop(stepItem, 0);
                }

                if (GetLeft(stepItem) > ActualWidth - stepItem.ActualWidth)
                {
                    SetLeft(stepItem, ActualWidth - stepItem.ActualWidth);
                }
                if (GetTop(stepItem) > ActualHeight - stepItem.ActualHeight)
                {
                    SetTop(stepItem, ActualHeight - stepItem.ActualHeight);
                }
            }
        }

        private void DragCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_isMoving || _isDrawing || _isMultiMoving)
                {
                    return;
                }

                Children.Remove(_selectionRectangle);
            }
            finally
            {
                _isSelecting = false;
            }
        }

        public bool IsPoint(Point point)
        {
            List<UIElement> hitElements = new List<UIElement>();

            // 使用HitTest方法和回调函数获取所有命中的控件
            VisualTreeHelper.HitTest(
                this,
                null,
                new HitTestResultCallback(
                    result =>
                    {
                        HitTestResultBehavior behavior = HitTestResultBehavior.Continue;
                        if (result.VisualHit is UIElement hitElement)
                        {
                            hitElements.Add(hitElement);
                        }
                        return behavior;
                    }),
                new PointHitTestParameters(point));

            return hitElements.Any(x => x is Ellipse);
        }

        private void StepItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var stepItem = sender as StepItem;
            _lastStepItem = stepItem;
            Point point = e.GetPosition(this);
            var ellipseItem = stepItem.IsPoint(point);
            if (ellipseItem != null)
            {
                _isDrawing = true;
                _pathStartPoint = ellipseItem.GetEllipsePoint(this);
            }
            else
            {
                _isMoving = true;
            }

            if ((!GetIsDraggable(stepItem) && !_isDrawing) || _isSelecting || _isMultiMoving)
            {
                return;
            }

            _cursor = stepItem.Cursor;
            stepItem.Cursor = _isDrawing ? Cursors.Cross : Cursors.ScrollAll;
            _mouseDownPoint = point;
            _mouseDownControlPoint = new Point(GetLeft(stepItem), GetTop(stepItem));
        }

        private void StepItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var stepItem = sender as StepItem;
                if (_isDrawing)
                {
                    Point point = e.GetPosition(this);
                    if (!IsPoint(point))
                    {
                        Children.Remove(_currentPath);
                    }

                    _currentPath = null;
                }

                if ((!GetIsDraggable(stepItem) && !_isDrawing) || _isSelecting || _isMultiMoving)
                {
                    return;
                }
                stepItem.Cursor = _cursor;
                SetLeft(stepItem, Adsorb(GetLeft(stepItem)));
                SetTop(stepItem, Adsorb(GetTop(stepItem)));
            }
            finally
            {
                _isMoving = false;
                _isDrawing = false;
            }
        }

        private double Adsorb(double value)
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