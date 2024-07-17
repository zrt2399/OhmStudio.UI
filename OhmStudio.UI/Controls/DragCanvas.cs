using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
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
            MouseWheel += DragCanvas_MouseWheel;

            _multiSelectionMask = new Rectangle();
            _multiSelectionMask.Fill = "#44AACCEE".ToSolidColorBrush();
            _multiSelectionMask.Stroke = "#FF0F80D9".ToSolidColorBrush();
            _multiSelectionMask.StrokeDashArray = new DoubleCollection(new double[] { 2, 2 });
            _multiSelectionMask.MouseLeftButtonDown += MultiSelectionRectangle_MouseLeftButtonDown;

            _selectionArea = new Rectangle();
            _selectionArea.Fill = "#88AACCEE".ToSolidColorBrush();
            _selectionArea.Stroke = "#FF0F80D9".ToSolidColorBrush();
            SetZIndex(_selectionArea, int.MaxValue);
            ScaleTransform scaleTransform = new ScaleTransform();
            BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleXProperty, new Binding() { Source = this, Path = new PropertyPath(ScaleProperty) });
            BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleYProperty, new Binding() { Source = this, Path = new PropertyPath(ScaleProperty) });

            LayoutTransform = scaleTransform;
        }

        private void DragCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Point mousePosition = e.GetPosition(this); 
            //scaleTransform.CenterX = mousePosition.X;
            //scaleTransform.CenterY = mousePosition.Y;
            if (e.Delta > 0)
            {
                Scale += 0.2;
            }
            else
            {
                Scale -= 0.2;
            }
        }

        //鼠标选中多个元素的Rectangle遮罩
        private Rectangle _multiSelectionMask;
        //跟随鼠标运移动绘制的多选Rectangle区域
        private Rectangle _selectionArea;
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

        //鼠标按下的位置
        private Point _mouseDownPoint;
        //鼠标按下控件的位置
        private Point _mouseDownControlPoint;
        //多个选中时鼠标按下的位置
        private Point _multiMouseDownPoint;
        //多个选中时Rectangle遮罩按下的位置
        private Point _multiMouseDownRectanglePoint;

        private StepItem _lastStepItem;
        private EllipseItem _lastEllipseItem;

        private Point _pathStartPoint;
        private PathItem _currentPath;

        internal IEnumerable<ISelectableElement> SelectableElements => Children.OfType<ISelectableElement>();

        public IEnumerable<StepItem> StepItems => Children.OfType<StepItem>();

        public IEnumerable<StepItem> SelectedItems => StepItems.Where(x => x.IsSelected);

        public static readonly DependencyProperty LineBrushProperty =
           DependencyProperty.Register(nameof(LineBrush), typeof(Brush), typeof(DragCanvas),
               new FrameworkPropertyMetadata(Brushes.LightGray, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register(nameof(GridSize), typeof(double), typeof(DragCanvas),
                new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(nameof(Scale), typeof(double), typeof(DragCanvas), new PropertyMetadata(1d, (sender, e) =>
            {
                DragCanvas dragCanvas = (DragCanvas)sender;
                if (dragCanvas.Scale < 0.2)
                {
                    dragCanvas.Scale = 0.2;
                }
                if (dragCanvas.Scale > 3)
                {
                    dragCanvas.Scale = 3;
                }
            }));

        public static readonly DependencyProperty MousePositionProperty =
            DependencyProperty.Register(nameof(MousePosition), typeof(Point), typeof(DragCanvas));

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

        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public Point MousePosition
        {
            get => (Point)GetValue(MousePositionProperty);
            set => SetValue(MousePositionProperty, value);
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
                stepItem.CanvasParent = this;
                stepItem.MouseLeftButtonDown += StepItem_MouseLeftButtonDown;
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
            _multiMouseDownRectanglePoint = new Point(GetLeft(_multiSelectionMask), GetTop(_multiSelectionMask));
            foreach (var item in SelectedItems)
            {
                item.MouseDownControlPoint = new Point(GetLeft(item), GetTop(item));
            }
        }

        private void StepItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var stepItem = sender as StepItem;
            _lastStepItem = stepItem;
            Point point = e.GetPosition(this);
            var ellipseItem = this.GetFirstVisualHit<EllipseItem>(point);
            if (ellipseItem == null)
            {
                _isMoving = true;
            }
            else
            {
                _isDrawing = true;
                _pathStartPoint = ellipseItem.GetPoint(this);
                _lastEllipseItem = ellipseItem;
            }

            if ((!GetIsDraggable(stepItem) && _isMoving) || _isSelecting || _isMultiMoving)
            {
                return;
            }

            Cursor = _isDrawing ? Cursors.Cross : Cursors.ScrollAll;
            _mouseDownPoint = point;
            _mouseDownControlPoint = new Point(GetLeft(stepItem), GetTop(stepItem));
        }

        private void DragCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isMultiMoving)
            {
                return;
            }
            Point point = e.GetPosition(this);
            Children.Remove(_multiSelectionMask);

            bool isPathItem = false;
            IEnumerable<ISelectableElement> selectableElements;
            if (this.GetVisualHit<Control>(point) is ISelectableElement selectableElement)
            {
                isPathItem = true;
                selectableElement.IsSelected = true;
                selectableElements = SelectableElements.Where(x => x != selectableElement);
            }
            else
            {
                selectableElements = SelectableElements;
            }

            foreach (var item in selectableElements)
            {
                item.IsSelected = false;
            }

            if (_isMoving || _isDrawing || isPathItem)
            {
                return;
            }

            if (!Children.Contains(_selectionArea))
            {
                Children.Add(_selectionArea);
            }

            _selectionStartPoint = point;
            _isSelecting = true;

            // Reset the selection rectangle
            _selectionArea.Width = 0;
            _selectionArea.Height = 0;
            SetLeft(_selectionArea, _selectionStartPoint.X);
            SetTop(_selectionArea, _selectionStartPoint.Y);
        }

        private void DragCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(this);
            MousePosition = point;
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            if (_isMultiMoving)
            {
                Vector vector = point - _multiMouseDownPoint;

                SetLeft(_multiSelectionMask, _multiMouseDownRectanglePoint.X + vector.X);
                SetTop(_multiSelectionMask, _multiMouseDownRectanglePoint.Y + vector.Y);
                if (GetLeft(_multiSelectionMask) < 0)
                {
                    SetLeft(_multiSelectionMask, 0);
                }
                if (GetTop(_multiSelectionMask) < 0)
                {
                    SetTop(_multiSelectionMask, 0);
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
                    item.UpdateCurve();
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

                SetLeft(_selectionArea, x);
                SetTop(_selectionArea, y);
                _selectionArea.Width = width;
                _selectionArea.Height = height;
                Rect selectedArea = new Rect(x, y, width, height);
                foreach (var item in StepItems)
                {
                    item.IsSelected = selectedArea.IntersectsWith(new Rect(GetLeft(item), GetTop(item), item.ActualWidth, item.ActualHeight));
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
                    _multiSelectionMask.Width = maxX - minX;
                    _multiSelectionMask.Height = maxY - minY;
                    SetLeft(_multiSelectionMask, minX);
                    SetTop(_multiSelectionMask, minY);
                    if (!Children.Contains(_multiSelectionMask))
                    {
                        Children.Add(_multiSelectionMask);
                    }
                }
                else if (selected.Length > 0)
                {
                    Children.Remove(_multiSelectionMask);
                }
            }
            else if (_isDrawing)
            {
                if (_currentPath == null)
                {
                    _currentPath = new PathItem();
                    _currentPath.CanvasParent = this;
                    _currentPath.StartPoint = _pathStartPoint;
                    if (_lastEllipseItem.Orientation == EllipseOrientation.Right)
                    {
                        _currentPath.Text = "跳转";
                    }
                    var contextMenu = new ContextMenu();
                    var menuItem = new MenuItem() { Header = "删除连接", Icon = new PackIcon() { Kind = PackIconKind.TrashiOS } };
                    menuItem.Click += (sender, e) =>
                    {
                        var pathItem = ((ContextMenu)((MenuItem)sender).Parent).PlacementTarget as PathItem;
                        pathItem?.Delete();
                    };
                    contextMenu.Items.Add(menuItem);
                    _currentPath.ContextMenu = contextMenu;
                    Children.Add(_currentPath);
                }

                _currentPath.UpdateBezierCurve(_pathStartPoint, point);
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
                stepItem.UpdateCurve();
            }
        }

        private void DragCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_isSelecting)
                {
                    Children.Remove(_selectionArea);
                }
                else if (_isDrawing)
                {
                    bool drawingSuccess = false;
                    Point point = e.GetPosition(this);
                    var ellipseItem = this.GetFirstVisualHit<EllipseItem>(point);
                    if (ellipseItem != null)
                    {
                        drawingSuccess = ellipseItem.StepParent.SetStep(_lastStepItem, _lastEllipseItem, ellipseItem);
                    }
                    if (drawingSuccess)
                    {
                        _currentPath.Point3 = ellipseItem.GetPoint(this);
                        _lastEllipseItem.PathItem = _currentPath;
                        ellipseItem.PathItem = _currentPath;
                        ellipseItem.PathItem.StartEllipseItem = _lastEllipseItem;
                        ellipseItem.PathItem.EndEllipseItem = ellipseItem;

                        _lastStepItem.UpdateCurve();
                    }
                    else
                    {
                        if (_currentPath != null)
                        {
                            _currentPath.ContextMenu = null;
                        }
                        Children.Remove(_currentPath);
                    }
                    _currentPath = null;
                }
                else if (_isMoving)
                {
                    var stepItem = _lastStepItem;
                    if (stepItem == null || !GetIsDraggable(stepItem))
                    {
                        return;
                    }

                    PositionStepItem(stepItem);
                }
                else if (_isMultiMoving)
                {
                    SetLeft(_multiSelectionMask, Adsorb(GetLeft(_multiSelectionMask)));
                    SetTop(_multiSelectionMask, Adsorb(GetTop(_multiSelectionMask)));
                    foreach (var item in SelectedItems.Where(x => GetIsDraggable(x)))
                    {
                        PositionStepItem(item);
                    }
                    _multiSelectionMask.Cursor = null;
                }
            }
            finally
            {
                Cursor = null;
                _isMoving = false;
                _isDrawing = false;
                _isSelecting = false;
                _isMultiMoving = false;
            }
        }

        private void PositionStepItem(StepItem stepItem)
        {
            SetLeft(stepItem, Adsorb(GetLeft(stepItem)));
            SetTop(stepItem, Adsorb(GetTop(stepItem)));
            Dispatcher.InvokeAsync(() =>
            {
                stepItem.UpdateCurve();
            }, DispatcherPriority.Render);
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
            Pen sidePen = new Pen(LineBrush, 1);

            int index = 0;
            for (double x = 0; x < width; x += gridSize)
            {
                dc.DrawLine(IsSide(index) ? sidePen : pen, new Point(x, 0), new Point(x, height));
                index++;
            }
            index = 0;
            for (double y = 0; y < height; y += gridSize)
            {
                dc.DrawLine(IsSide(index) ? sidePen : pen, new Point(0, y), new Point(width, y));
                index++;
            }
        }

        private static bool IsSide(int value)
        {
            return value != 0 && value % 4 == 0;
        }
    }
}