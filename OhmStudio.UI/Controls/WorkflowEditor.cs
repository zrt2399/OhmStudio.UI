using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    public enum EditorStatus
    {
        None,
        /// <summary>正在拖动中。</summary>
        Moving,
        /// <summary>正在绘制曲线中。</summary>
        Drawing,
        /// <summary>正在选择中。</summary>
        Selecting,
        /// <summary>多个选中正在拖动中。</summary>
        MultiMoving
    }

    public class WorkflowEditor : Canvas
    {
        static WorkflowEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WorkflowEditor), new FrameworkPropertyMetadata(typeof(WorkflowEditor)));
        }

        public WorkflowEditor()
        {
            MouseLeftButtonDown += WorkflowEditor_MouseLeftButtonDown;
            MouseMove += WorkflowEditor_MouseMove;
            PreviewMouseLeftButtonUp += WorkflowEditor_MouseLeftButtonUp;
            MouseWheel += WorkflowEditor_MouseWheel;
            KeyDown += WorkflowEditor_KeyDown;

            _multiSelectionMask = new Rectangle();
            _multiSelectionMask.Fill = "#44AACCEE".ToSolidColorBrush();
            _multiSelectionMask.Stroke = "#FF0F80D9".ToSolidColorBrush();
            _multiSelectionMask.StrokeDashArray = new DoubleCollection(new double[] { 2, 2 });
            _multiSelectionMask.MouseLeftButtonDown += MultiSelectionRectangle_MouseLeftButtonDown;
            SetZIndex(_multiSelectionMask, int.MaxValue - 1);

            _selectionArea = new Rectangle();
            _selectionArea.Fill = "#88AACCEE".ToSolidColorBrush();
            _selectionArea.Stroke = "#FF0F80D9".ToSolidColorBrush();
            SetZIndex(_selectionArea, int.MaxValue);

            ScaleTransform scaleTransform = new ScaleTransform();
            BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleXProperty, new Binding() { Source = this, Path = new PropertyPath(ScaleProperty) });
            BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleYProperty, new Binding() { Source = this, Path = new PropertyPath(ScaleProperty) });
            LayoutTransform = scaleTransform;
        }

        private void WorkflowEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsCtrlKeyDown && Keyboard.IsKeyDown(Key.A))
            {
                SelectAll();
            }
        }

        //鼠标选中多个元素的Rectangle遮罩
        private Rectangle _multiSelectionMask;
        //跟随鼠标运移动绘制的多选Rectangle区域
        private Rectangle _selectionArea;
        //鼠标选择多个元素起始的位置
        private Point _selectionStartPoint;

        //鼠标按下的位置
        private Point _mouseDownPoint;
        //鼠标按下控件的位置
        private Point _mouseDownControlPoint;
        //多个选中时鼠标按下的位置
        private Point _multiMouseDownPoint;
        //多个选中时Rectangle遮罩按下的位置
        private Point _multiMouseDownRectanglePoint;

        private WorkflowItem _lastWorkflowItem;
        private EllipseItem _lastEllipseItem;

        private Point _pathStartPoint;
        private PathItem _currentPath;

        private bool _isUpdatingSelectedItems;

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(WorkflowEditor), new PropertyMetadata(OnItemsSourceChanged));

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(WorkflowEditor), new FrameworkPropertyMetadata(default(IList), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty PathTemplateProperty =
            DependencyProperty.Register(nameof(PathTemplate), typeof(DataTemplate), typeof(WorkflowEditor));

        public static readonly DependencyProperty PathTemplateSelectorProperty =
            DependencyProperty.Register(nameof(PathTemplateSelector), typeof(DataTemplateSelector), typeof(WorkflowEditor));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(WorkflowEditor));

        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(WorkflowEditor));

        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register(nameof(ItemContainerStyle), typeof(Style), typeof(WorkflowEditor), new PropertyMetadata(OnContainerStyleChanged));

        public static readonly DependencyProperty ItemContainerStyleSelectorProperty =
            DependencyProperty.Register(nameof(ItemContainerStyleSelector), typeof(StyleSelector), typeof(WorkflowEditor), new PropertyMetadata(OnContainerStyleSelectorChanged));

        public static readonly DependencyProperty LineBrushProperty =
           DependencyProperty.Register(nameof(LineBrush), typeof(Brush), typeof(WorkflowEditor),
               new FrameworkPropertyMetadata(Brushes.LightGray, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register(nameof(GridSize), typeof(double), typeof(WorkflowEditor),
                new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(nameof(Scale), typeof(double), typeof(WorkflowEditor), new PropertyMetadata(1d, (sender, e) =>
            {
                WorkflowEditor workflowEditor = (WorkflowEditor)sender;
                if (workflowEditor.Scale < 0.2)
                {
                    workflowEditor.Scale = 0.2;
                }
                if (workflowEditor.Scale > 3)
                {
                    workflowEditor.Scale = 3;
                }
            }));

        public static readonly DependencyProperty MousePositionProperty =
            DependencyProperty.Register(nameof(MousePosition), typeof(Point), typeof(WorkflowEditor));

        internal bool IsCtrlKeyDown => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

        internal IEnumerable<SelectionControl> SelectableElements => Children.OfType<SelectionControl>();

        public IEnumerable<WorkflowItem> WorkflowItems => Children.OfType<WorkflowItem>();

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public Style ItemContainerStyle
        {
            get => (Style)GetValue(ItemContainerStyleProperty);
            set => SetValue(ItemContainerStyleProperty, value);
        }

        public StyleSelector ItemContainerStyleSelector
        {
            get => (StyleSelector)GetValue(ItemContainerStyleSelectorProperty);
            set => SetValue(ItemContainerStyleSelectorProperty, value);
        }

        public DataTemplate PathTemplate
        {
            get => (DataTemplate)GetValue(PathTemplateProperty);
            set => SetValue(PathTemplateProperty, value);
        }

        public DataTemplateSelector PathTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(PathTemplateSelectorProperty);
            set => SetValue(PathTemplateSelectorProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public DataTemplateSelector ItemTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty);
            set => SetValue(ItemTemplateSelectorProperty, value);
        }

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

        private EditorStatus _editorStatus;
        internal EditorStatus EditorStatus
        {
            get => _editorStatus;
            set
            {
                if (_editorStatus == value)
                {
                    return;
                }
                SetEditorStatus(this, value);
                _editorStatus = value;
            }
        }

        public static readonly DependencyProperty EditorStatusProperty =
            DependencyProperty.RegisterAttached(nameof(EditorStatus), typeof(EditorStatus), typeof(WorkflowEditor), new FrameworkPropertyMetadata(EditorStatus.None, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetEditorStatus(DependencyObject element, EditorStatus value)
        {
            element.SetValue(EditorStatusProperty, value);
        }

        public static EditorStatus GetEditorStatus(DependencyObject element)
        {
            return (EditorStatus)element.GetValue(EditorStatusProperty);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var workflowEditor = (WorkflowEditor)d;
            IEnumerable oldItemsSource = (IEnumerable)e.OldValue;
            IEnumerable newItemsSource = (IEnumerable)e.NewValue;
            workflowEditor.OnItemsSourceChanged(oldItemsSource, newItemsSource);
        }

        public virtual void OnItemsSourceChanged(IEnumerable oldItemsSource, IEnumerable newItemsSource)
        {
            Children.Clear();
            if (oldItemsSource != null)
            {
                if (oldItemsSource is INotifyCollectionChanged notifyCollectionChanged)
                {
                    notifyCollectionChanged.CollectionChanged -= CollectionChanged;
                }
            }
            if (newItemsSource != null)
            {
                foreach (var item in newItemsSource.OfType<object>())
                {
                    Children.Add(CreateWorkflowItem(item));
                }
                if (newItemsSource is INotifyCollectionChanged notifyCollectionChanged)
                {
                    notifyCollectionChanged.CollectionChanged += CollectionChanged;
                }
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    Children.Add(CreateWorkflowItem(item));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var workflowItem = WorkflowItems.FirstOrDefault(x => x.DataContext == oldItem);
                    workflowItem.Delete();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var newItem = WorkflowItems.FirstOrDefault(x => x.DataContext == e.NewItems[0]);
                var oldItem = WorkflowItems.FirstOrDefault(x => x.DataContext == e.OldItems[0]);
                Children.Add(CreateWorkflowItem(newItem));
                oldItem.Delete();
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Children.Clear();
            }
        }

        private WorkflowItem CreateWorkflowItem(object item)
        {
            WorkflowItem result;
            if (item is WorkflowItem workflowItem)
            {
                result = workflowItem;
                result.DataContext = workflowItem;
            }
            else
            {
                result = new WorkflowItem()
                {
                    DataContext = item,
                    Content = item,
                    ContentTemplate = ItemTemplate,
                    ContentTemplateSelector = ItemTemplateSelector
                };
            }
            result.EditorParent = this;
            AttachWorkflowItems(result);
            return result;
        }

        private static void OnContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WorkflowEditor)d).OnContainerStyleChanged(e);
        }

        protected virtual void OnContainerStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            AttachWorkflowItems();
        }

        private static void OnContainerStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WorkflowEditor)d).OnContainerStyleSelectorChanged(e);
        }

        protected virtual void OnContainerStyleSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            AttachWorkflowItems();
        }

        private void AttachWorkflowItems()
        {
            foreach (var item in WorkflowItems)
            {
                AttachWorkflowItems(item);
            }
        }

        private void AttachWorkflowItems(WorkflowItem workflowItem)
        {
            if (ItemContainerStyle != null)
            {
                workflowItem.Style = ItemContainerStyle;
            }
            else if (ItemContainerStyleSelector != null)
            {
                workflowItem.Style = ItemContainerStyleSelector.SelectStyle(workflowItem.DataContext, workflowItem);
            }
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            UpdateSelectedItems();
            if (visualAdded is WorkflowItem added)
            {
                if (double.IsNaN(GetLeft(added)))
                {
                    SetLeft(added, 0);
                }
                if (double.IsNaN(GetTop(added)))
                {
                    SetTop(added, 0);
                }
                added.EditorParent = this;
                added.MouseLeftButtonDown += WorkflowItem_MouseLeftButtonDown;
                added.Selected += WorkflowItem_SelectedChanged;
                added.Unselected += WorkflowItem_SelectedChanged;
            }
            if (visualRemoved is WorkflowItem removed)
            {
                removed.EditorParent = null;
                removed.MouseLeftButtonDown -= WorkflowItem_MouseLeftButtonDown;
                removed.Selected -= WorkflowItem_SelectedChanged;
                removed.Unselected -= WorkflowItem_SelectedChanged;
            }
        }

        private void WorkflowItem_SelectedChanged(object sender, RoutedEventArgs e)
        {
            if (!_isUpdatingSelectedItems)
            {
                UpdateSelectedItems();
            }
        }

        public void BeginUpdateSelectedItems()
        {
            _isUpdatingSelectedItems = true;
        }

        public void EndUpdateSelectedItems()
        {
            _isUpdatingSelectedItems = false;
            UpdateSelectedItems();
        }

        internal void UpdateSelectedItems()
        {
            List<object> list = new List<object>();
            foreach (var item in WorkflowItems.Where(x => x.IsSelected))
            {
                list.Add(item.DataContext);
            }

            if (SelectedItems == null)
            {
                SelectedItems = list;
            }
            else
            {
                if (list.Count != 0 || SelectedItems.Count != 0)
                {
                    SelectedItems = list;
                }
            }

            UpdateMultiSelectionMask();
        }

        public void SelectAll()
        {
            SelectElement(true);
        }

        public void UnselectAll()
        {
            SelectElement(false);
        }

        public void Invert()
        {
            SelectElement(true, true);
        }

        private void SelectElement(bool value, bool invert = false)
        {
            BeginUpdateSelectedItems();
            foreach (var item in WorkflowItems)
            {
                if (invert)
                {
                    item.IsSelected = !item.IsSelected;
                }
                else
                {
                    item.IsSelected = value;
                }
            }
            EndUpdateSelectedItems();
        }

        private void MultiSelectionRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (CanvasStatus is CanvasStatus.Moving or CanvasStatus.Drawing or CanvasStatus.Selecting)
            //{
            //    return;
            //}

            EditorStatus = EditorStatus.MultiMoving;
            _multiSelectionMask.Cursor = Cursors.ScrollAll;
            _multiMouseDownPoint = e.GetPosition(this);
            _multiMouseDownRectanglePoint = new Point(GetLeft(_multiSelectionMask), GetTop(_multiSelectionMask));
            foreach (var item in WorkflowItems.Where(x => x.IsSelected))
            {
                item.MouseDownControlPoint = new Point(GetLeft(item), GetTop(item));
            }
        }

        private void WorkflowItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var workflowItem = sender as WorkflowItem;
            _lastWorkflowItem = workflowItem;
            Point point = e.GetPosition(this);
            var startEllipseItem = GetEllipseWithPoint(point);
            if (startEllipseItem == null)
            {
                EditorStatus = EditorStatus.Moving;
            }
            else
            {
                _lastWorkflowItem = startEllipseItem.WorkflowParent;
                _pathStartPoint = startEllipseItem.GetPoint(this);
                _lastEllipseItem = startEllipseItem;
                EditorStatus = EditorStatus.Drawing;

                if (_currentPath == null)
                {
                    _currentPath = new PathItem(this);
                    Children.Add(_currentPath);
                }
            }

            if ((!workflowItem.IsDraggable && startEllipseItem == null) /*|| CanvasStatus is CanvasStatus.Selecting or CanvasStatus.MultiMoving*/)
            {
                return;
            }

            Cursor = EditorStatus == EditorStatus.Drawing ? Cursors.Cross : Cursors.ScrollAll;
            _mouseDownPoint = point;
            _mouseDownControlPoint = new Point(GetLeft(workflowItem), GetTop(workflowItem));
        }

        private void WorkflowEditor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (EditorStatus is EditorStatus.MultiMoving)
            {
                return;
            }
            Point point = e.GetPosition(this);
            Children.Remove(_multiSelectionMask);

            //bool isPathItem = false;
            IEnumerable<SelectionControl> selectableElements;
            BeginUpdateSelectedItems();
            if (this.GetVisualHit<SelectionControl>(point) is SelectionControl selectableElement)
            {
                //isPathItem = selectableElement is PathItem;
                selectableElement.IsSelected = true;
                selectableElements = SelectableElements.Where(x => x != selectableElement);
            }
            else
            {
                selectableElements = SelectableElements;
            }
            if (!IsCtrlKeyDown)
            {
                foreach (var item in selectableElements)
                {
                    item.IsSelected = false;
                }
            }
            EndUpdateSelectedItems();

            Focus();
            if (EditorStatus is EditorStatus.Moving or EditorStatus.Drawing /*|| isPathItem*/)
            {
                return;
            }

            if (!Children.Contains(_selectionArea))
            {
                Children.Add(_selectionArea);
            }

            _selectionStartPoint = point;
            EditorStatus = EditorStatus.Selecting;

            // Reset the selection rectangle
            _selectionArea.Width = 0;
            _selectionArea.Height = 0;
            SetLeft(_selectionArea, _selectionStartPoint.X);
            SetTop(_selectionArea, _selectionStartPoint.Y);
        }

        private void WorkflowEditor_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(this);
            MousePosition = point;
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (EditorStatus == EditorStatus.MultiMoving)
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
                foreach (var item in WorkflowItems.Where(x => x.IsSelected && x.IsDraggable))
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
            else if (EditorStatus == EditorStatus.Selecting)
            {
                double x = Math.Min(point.X, _selectionStartPoint.X);
                double y = Math.Min(point.Y, _selectionStartPoint.Y);

                double width = Math.Abs(point.X - _selectionStartPoint.X);
                double height = Math.Abs(point.Y - _selectionStartPoint.Y);

                SetLeft(_selectionArea, x);
                SetTop(_selectionArea, y);
                _selectionArea.Width = width;
                _selectionArea.Height = height;

                if (width >= 1 && height >= 1)
                {
                    Rect selectedArea = new Rect(x, y, width, height);
                    RectangleGeometry rectangleGeometry = new RectangleGeometry(selectedArea);
                    BeginUpdateSelectedItems();
                    foreach (var item in WorkflowItems)
                    {
                        item.IsSelected = CheckOverlap(rectangleGeometry, item);
                        //item.IsSelected = selectedArea.IntersectsWith(new Rect(GetLeft(item), GetTop(item), item.ActualWidth, item.ActualHeight));
                    }
                    EndUpdateSelectedItems();
                }
            }
            else if (EditorStatus == EditorStatus.Drawing)
            {
                _currentPath.UpdateBezierCurve(_pathStartPoint, point);
            }
            else if (EditorStatus == EditorStatus.Moving)
            {
                var workflowItem = _lastWorkflowItem;
                if (!workflowItem.IsDraggable)
                {
                    return;
                }
                Vector vector = point - _mouseDownPoint;
                SetLeft(workflowItem, Math.Round(_mouseDownControlPoint.X + vector.X, 0));
                SetTop(workflowItem, Math.Round(_mouseDownControlPoint.Y + vector.Y, 0));

                if (GetLeft(workflowItem) < 0)
                {
                    SetLeft(workflowItem, 0);
                }
                if (GetTop(workflowItem) < 0)
                {
                    SetTop(workflowItem, 0);
                }

                if (GetLeft(workflowItem) > ActualWidth - workflowItem.ActualWidth)
                {
                    SetLeft(workflowItem, ActualWidth - workflowItem.ActualWidth);
                }
                if (GetTop(workflowItem) > ActualHeight - workflowItem.ActualHeight)
                {
                    SetTop(workflowItem, ActualHeight - workflowItem.ActualHeight);
                }
                workflowItem.UpdateCurve();
            }
        }

        private bool CheckOverlap(RectangleGeometry rectangleGeometry, WorkflowItem workflowItem)
        {
            GeneralTransform transform = workflowItem.TransformToVisual(this);
            Geometry geometry = workflowItem.Geometry?.Clone();
            if (geometry != null)
            {
                geometry.Transform = (Transform)transform;
            }
            return rectangleGeometry.FillContainsWithDetail(geometry) != IntersectionDetail.Empty;
        }

        private EllipseItem GetEllipseWithPoint(Point point)
        {
            var ellipseItem = this.GetVisualHit<EllipseItem>(point);
            if (ellipseItem != null && !ellipseItem.IsVisible)
            {
                return null;
            }
            return ellipseItem;
        }

        private void WorkflowEditor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (EditorStatus == EditorStatus.Drawing)
                {
                    Point point = e.GetPosition(this);
                    var endEllipseItem = GetEllipseWithPoint(point);
                    if (endEllipseItem != null)
                    {
                        SetStep(_lastWorkflowItem.DataContext, endEllipseItem.WorkflowParent.DataContext, _lastEllipseItem, endEllipseItem);
                    }
                }
                else if (EditorStatus == EditorStatus.Moving)
                {
                    var workflowItem = _lastWorkflowItem;
                    if (workflowItem == null || !workflowItem.IsDraggable)
                    {
                        return;
                    }

                    PositionWorkflowItem(workflowItem);
                }
                else if (EditorStatus == EditorStatus.MultiMoving)
                {
                    SetLeft(_multiSelectionMask, Adsorb(GetLeft(_multiSelectionMask)));
                    SetTop(_multiSelectionMask, Adsorb(GetTop(_multiSelectionMask)));
                    foreach (var item in WorkflowItems.Where(x => x.IsSelected && x.IsDraggable))
                    {
                        PositionWorkflowItem(item);
                    }
                }
            }
            finally
            {
                Cursor = null;
                _multiSelectionMask.Cursor = null;
                Children.Remove(_selectionArea);
                Children.Remove(_currentPath);
                _currentPath = null;
                EditorStatus = EditorStatus.None;
            }
        }

        private void PositionWorkflowItem(WorkflowItem workflowItem)
        {
            SetLeft(workflowItem, Adsorb(GetLeft(workflowItem)));
            SetTop(workflowItem, Adsorb(GetTop(workflowItem)));
            Dispatcher.InvokeAsync(() =>
            {
                workflowItem.UpdateCurve();
            }, DispatcherPriority.Render);
        }

        private void WorkflowEditor_MouseWheel(object sender, MouseWheelEventArgs e)
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

        public void UpdateMultiSelectionMask()
        {
            if (SelectedItems != null && SelectedItems.Count > 1)
            {
                double minX = double.MaxValue;
                double minY = double.MaxValue;
                double maxX = double.MinValue;
                double maxY = double.MinValue;

                foreach (var item in WorkflowItems.Where(x => x.IsSelected))
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
                _multiSelectionMask.Width = maxX - minX;
                _multiSelectionMask.Height = maxY - minY;
                SetLeft(_multiSelectionMask, minX);
                SetTop(_multiSelectionMask, minY);
                if (!Children.Contains(_multiSelectionMask))
                {
                    Children.Add(_multiSelectionMask);
                }
            }
            else
            {
                Children.Remove(_multiSelectionMask);
            }
        }

        internal void SetStep(object fromStep, object toStep, EllipseItem fromEllipse, EllipseItem toEllipse)
        {
            if (fromEllipse == toEllipse)
            {
                return;
            }
            if (fromStep == toStep)
            {
                UIMessageTip.ShowWarning("无法设置下一节点为自己");
            }
            else if (fromEllipse.PathItem != null || toEllipse.PathItem != null)
            {
                UIMessageTip.ShowWarning("该节点已经存在连接关系，无法创建连接曲线，请删除后再试");
            }
            else if (fromEllipse.Dock == Dock.Left || toEllipse.Dock == Dock.Right)
            {
                UIMessageTip.ShowWarning("被跳转节点无法直接设置下一节点");
            }
            else if (fromEllipse.Dock == Dock.Top || toEllipse.Dock == Dock.Bottom)
            {
                UIMessageTip.ShowWarning("上一步下一步节点设置错误");
            }
            else if (fromEllipse.Dock == Dock.Right && toEllipse.Dock == Dock.Top)
            {
                UIMessageTip.ShowWarning("跳转节点只能设置为下一节点的被跳转节点");
            }
            else if (fromEllipse.Dock == Dock.Bottom && toEllipse.Dock == Dock.Left)
            {
                UIMessageTip.ShowWarning("下一步节点只能设置为下一节点的上一步节点");
            }
            else
            {
                var fromWorkflow = FirstOrDefault(fromStep);
                var toWorkflow = FirstOrDefault(toStep);
                if (fromEllipse.Dock == Dock.Right)
                {
                    fromWorkflow.JumpStep = toStep;
                    toWorkflow.FromStep = fromStep;
                }
                else if (fromEllipse.Dock == Dock.Bottom)
                {
                    fromWorkflow.NextStep = toStep;
                    toWorkflow.LastStep = fromStep;
                }
            }
        }

        public WorkflowItem FirstOrDefault(object item)
        {
            var workflowItem = WorkflowItems.FirstOrDefault(x => x.DataContext == item);
            workflowItem ??= new WorkflowItem();
            return workflowItem;
        }

        private double Adsorb(double value)
        {
            return value.Adsorb(GridSize);
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