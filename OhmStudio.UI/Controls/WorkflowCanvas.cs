using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using OhmStudio.UI.Messaging;
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
        //Selecting,
        /// <summary>多个选中正在拖动中。</summary>
        MultiMoving
    }

    public class WorkflowCanvas : Canvas
    {
        static WorkflowCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WorkflowCanvas), new FrameworkPropertyMetadata(typeof(WorkflowCanvas)));
        }

        public WorkflowCanvas()
        {
            MouseLeftButtonDown += WorkflowCanvas_MouseLeftButtonDown;
            MouseMove += WorkflowCanvas_MouseMove;
            MouseLeftButtonUp += WorkflowCanvas_MouseLeftButtonUp;
             
            _multiSelectionMask = new Rectangle();
            _multiSelectionMask.Fill = "#44AACCEE".ToSolidColorBrush();
            _multiSelectionMask.Stroke = "#FF0F80D9".ToSolidColorBrush();
            _multiSelectionMask.StrokeDashArray = new DoubleCollection(new double[] { 2, 2 });
            _multiSelectionMask.MouseLeftButtonDown += MultiSelectionRectangle_MouseLeftButtonDown;
            SetZIndex(_multiSelectionMask, int.MaxValue - 1);
        }

        //鼠标选中多个元素的Rectangle遮罩
        private Rectangle _multiSelectionMask;
 
        //鼠标按下的位置
        private Point _moveMouseDownPoint;
        //鼠标按下控件的位置
        private Point _moveMouseDownControlPoint;
        //多个选中时鼠标按下的位置
        private Point _multiMoveMouseDownPoint;

        private WorkflowItem _lastWorkflowItem;
        private EllipseItem _lastEllipseItem;

        private Point _pathStartPoint;
        private PathItem _currentPath;

        private bool _isUpdatingSelectedItems;

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(WorkflowCanvas), new PropertyMetadata(OnItemsSourceChanged));

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(WorkflowCanvas), new FrameworkPropertyMetadata(default(IList), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsChanged));

        public static readonly DependencyProperty PathTemplateProperty =
            DependencyProperty.Register(nameof(PathTemplate), typeof(DataTemplate), typeof(WorkflowCanvas));

        public static readonly DependencyProperty PathTemplateSelectorProperty =
            DependencyProperty.Register(nameof(PathTemplateSelector), typeof(DataTemplateSelector), typeof(WorkflowCanvas));

        public static readonly DependencyProperty PathContainerStyleProperty =
            DependencyProperty.Register(nameof(PathContainerStyle), typeof(Style), typeof(WorkflowCanvas));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(WorkflowCanvas));

        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(WorkflowCanvas));

        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register(nameof(ItemContainerStyle), typeof(Style), typeof(WorkflowCanvas), new PropertyMetadata(OnContainerStyleChanged));

        public static readonly DependencyProperty ItemContainerStyleSelectorProperty =
            DependencyProperty.Register(nameof(ItemContainerStyleSelector), typeof(StyleSelector), typeof(WorkflowCanvas), new PropertyMetadata(OnContainerStyleSelectorChanged));

        public static readonly DependencyProperty GridSpacingProperty =
            DependencyProperty.Register(nameof(GridSpacing), typeof(double), typeof(WorkflowCanvas),
                new FrameworkPropertyMetadata(20d));

        public static readonly DependencyProperty MousePositionProperty =
            DependencyProperty.Register(nameof(MousePosition), typeof(Point), typeof(WorkflowCanvas));

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

        public Style PathContainerStyle
        {
            get => (Style)GetValue(PathContainerStyleProperty);
            set => SetValue(PathContainerStyleProperty, value);
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

        public double GridSpacing
        {
            get => (double)GetValue(GridSpacingProperty);
            set => SetValue(GridSpacingProperty, value);
        }

        public Point MousePosition
        {
            get => (Point)GetValue(MousePositionProperty);
            set => SetValue(MousePositionProperty, value);
        }
         
        internal EditorStatus EditorStatus { get; set; }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var workflowCanvas = (WorkflowCanvas)d;

        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var workflowCanvas = (WorkflowCanvas)d;
            IEnumerable oldItemsSource = (IEnumerable)e.OldValue;
            IEnumerable newItemsSource = (IEnumerable)e.NewValue;
            workflowCanvas.OnItemsSourceChanged(oldItemsSource, newItemsSource);
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
                    workflowItem?.Delete();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var newItem = WorkflowItems.FirstOrDefault(x => x.DataContext == e.NewItems[0]);
                var oldItem = WorkflowItems.FirstOrDefault(x => x.DataContext == e.OldItems[0]);
                Children.Add(CreateWorkflowItem(newItem));
                oldItem?.Delete();
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
            ((WorkflowCanvas)d).OnContainerStyleChanged(e);
        }

        protected virtual void OnContainerStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            AttachWorkflowItems();
        }

        private static void OnContainerStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WorkflowCanvas)d).OnContainerStyleSelectorChanged(e);
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
                SetLeft(added, Adsorb(GetLeft(added)));
                SetTop(added, Adsorb(GetTop(added)));
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

        private void MultiSelectionRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EditorStatus = EditorStatus.MultiMoving;
            _multiSelectionMask.Cursor = Cursors.ScrollAll;
            _multiMoveMouseDownPoint = e.GetPosition(this);
            foreach (var item in WorkflowItems.Where(x => x.IsSelected))
            {
                item.LastMouseDownPoint = new Point(GetLeft(item), GetTop(item));
            }
            _multiSelectionMask.CaptureMouse();
        }

        private void WorkflowItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var workflowItem = sender as WorkflowItem;
            _lastWorkflowItem = workflowItem;
            Point point = e.GetPosition(this);
            var startEllipseItem = GetElementWithPoint<EllipseItem>(point);
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

            if (EditorStatus == EditorStatus.Moving && !workflowItem.IsDraggable /*|| CanvasStatus is CanvasStatus.Selecting or CanvasStatus.MultiMoving*/)
            {
                return;
            }

            _moveMouseDownPoint = point;
            _moveMouseDownControlPoint = new Point(GetLeft(workflowItem), GetTop(workflowItem));
            if (EditorStatus == EditorStatus.Moving)
            {
                Cursor = Cursors.ScrollAll;
                workflowItem.CaptureMouse();
            }
            else
            {
                Cursor = Cursors.Cross;
                _currentPath.CaptureMouse();
            }
        }

        internal void HandleLMouseDown(MouseButtonEventArgs e)
        {
            if (EditorStatus is EditorStatus.MultiMoving || e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            Point point = Mouse.GetPosition(this);
            Children.Remove(_multiSelectionMask);

            //bool isPathItem = false;
            IEnumerable<SelectionControl> selectableElements;
            BeginUpdateSelectedItems();
            if (this.GetVisualHitOfType<SelectionControl>(point) is SelectionControl selectableElement)
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
        }

        private void WorkflowCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HandleLMouseDown(e);
        }

        private void WorkflowCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(this);
            MousePosition = point;
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (EditorStatus == EditorStatus.MultiMoving)
            {
                Vector vector = point - _multiMoveMouseDownPoint;
                foreach (var item in WorkflowItems.Where(x => x.IsSelected && x.IsDraggable))
                {
                    SetLeft(item, item.LastMouseDownPoint.X + vector.X);
                    SetTop(item, item.LastMouseDownPoint.Y + vector.Y);

                    item.UpdateCurve();
                }
                UpdateMultiSelectionMask();
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
                Vector vector = point - _moveMouseDownPoint;

                SetLeft(workflowItem, _moveMouseDownControlPoint.X + vector.X);
                SetTop(workflowItem, _moveMouseDownControlPoint.Y + vector.Y);
                workflowItem.UpdateCurve();
            }
        }

        private T GetElementWithPoint<T>(Point point) where T : FrameworkElement
        {
            var frameworkElement = this.GetVisualHitOfType<T>(point);
            if (frameworkElement != null && !frameworkElement.IsVisible)
            {
                return null;
            }
            return frameworkElement;
        }

        private void WorkflowCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (EditorStatus == EditorStatus.Drawing)
                {
                    Point point = e.GetPosition(this);
                    var endWorkflowItem = GetElementWithPoint<WorkflowItem>(point);
                    if (endWorkflowItem != null)
                    {
                        SetStep(_lastWorkflowItem, endWorkflowItem, _lastEllipseItem);
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
                    foreach (var item in WorkflowItems.Where(x => x.IsSelected && x.IsDraggable))
                    {
                        PositionWorkflowItem(item);
                    }
                    UpdateMultiSelectionMask();
                }
            }
            finally
            {
                //Children.Remove(_selectionArea);
                Children.Remove(_currentPath);

                EditorStatus = EditorStatus.None;

                //_selectionArea.ReleaseMouseCapture();
                _multiSelectionMask.ReleaseMouseCapture();
                _lastWorkflowItem?.ReleaseMouseCapture();
                _currentPath?.ReleaseMouseCapture();

                _currentPath = null;
                Cursor = null;
                _multiSelectionMask.Cursor = null;
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

        internal void SetStep(WorkflowItem fromStep, WorkflowItem toStep, EllipseItem fromEllipse)
        {
            if (fromStep == toStep)
            {
                return;
            }
            EllipseItem toEllipse;

            if (fromEllipse.Dock == Dock.Right)
            {
                toEllipse = toStep.EllipseItems[Dock.Left];
            }
            else if (fromEllipse.Dock == Dock.Bottom)
            {
                toEllipse = toStep.EllipseItems[Dock.Top];
            }
            else
            {
                return;
            }

            if (fromEllipse.PathItem != null || toEllipse.PathItem != null)
            {
                UIMessageTip.ShowWarning("该节点已经存在连接关系，无法创建连接曲线，请删除后再试");
            }
            else
            {
                if (fromEllipse.Dock == Dock.Right)
                {
                    fromStep.JumpStep = toStep.DataContext;
                    toStep.FromStep = fromStep.DataContext;
                }
                else if (fromEllipse.Dock == Dock.Bottom)
                {
                    fromStep.NextStep = toStep.DataContext;
                    toStep.LastStep = fromStep.DataContext;
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
            return value.Adsorb(GridSpacing);
        }
    }
}