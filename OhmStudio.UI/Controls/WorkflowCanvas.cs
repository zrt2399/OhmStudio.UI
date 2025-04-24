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
using OhmStudio.UI.Helpers;
using OhmStudio.UI.Messaging;
using OhmStudio.UI.Utilities;

namespace OhmStudio.UI.Controls
{
    public enum CanvasStatus
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
        //鼠标选中多个元素的Rectangle遮罩
        private Rectangle _multiSelectionMask;

        //鼠标按下的位置
        private Point _moveMouseDownPoint;
        //鼠标按下控件的位置
        private Point _moveMouseDownControlPoint;
        //多个选中时鼠标按下的位置
        private Point _multiMoveMouseDownPoint;

        private WorkflowItem _previousWorkflowItem;
        private EllipseItem _previousEllipseItem;

        private Point _lineStartPoint;
        private LineItem _currentLine;

        private bool _isUpdatingSelectedItems;

        static WorkflowCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WorkflowCanvas), new FrameworkPropertyMetadata(typeof(WorkflowCanvas)));
        }

        public WorkflowCanvas()
        {
            _multiSelectionMask = new Rectangle();
            _multiSelectionMask.Fill = "#44AACCEE".ToSolidColorBrush();
            _multiSelectionMask.Stroke = "#FF0F80D9".ToSolidColorBrush();
            _multiSelectionMask.StrokeDashArray = new DoubleCollection(new double[] { 2, 2 });
            _multiSelectionMask.MouseLeftButtonDown += MultiSelectionRectangle_MouseLeftButtonDown;
            SetZIndex(_multiSelectionMask, int.MaxValue - 1);
        }

        public static readonly DependencyProperty EditorParentProperty =
            DependencyProperty.Register(nameof(EditorParent), typeof(WorkflowEditor), typeof(WorkflowCanvas));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(WorkflowCanvas), new PropertyMetadata(OnItemsSourceChanged));

        public static readonly DependencyProperty CanvasStatusProperty =
            DependencyProperty.RegisterAttached(nameof(CanvasStatus), typeof(CanvasStatus), typeof(WorkflowCanvas), new FrameworkPropertyMetadata(default(CanvasStatus), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetCanvasStatus(DependencyObject element, CanvasStatus value) => element.SetValue(CanvasStatusProperty, value);

        public static CanvasStatus GetCanvasStatus(DependencyObject element) => (CanvasStatus)element.GetValue(CanvasStatusProperty);

        internal IEnumerable<CanvasItem> CanvasItems => Children.OfType<CanvasItem>();

        public IEnumerable<WorkflowItem> WorkflowItems => Children.OfType<WorkflowItem>();

        public WorkflowEditor EditorParent
        {
            get => (WorkflowEditor)GetValue(EditorParentProperty);
            internal set => SetValue(EditorParentProperty, value);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public IList SelectedItems
        {
            get => EditorParent.SelectedItems;
            internal set =>
                //if (EditorParent != null)
                //{
                EditorParent.SelectedItems = value;//}
        }

        public CanvasStatus CanvasStatus
        {
            get => (CanvasStatus)GetValue(CanvasStatusProperty);
            set => SetValue(CanvasStatusProperty, value);
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
            if (EditorParent == null)
            {
                throw new InvalidOperationException("The EditorParent is not loaded!");
            }
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
                    Content = item,
                    DataContext = item,
                    ContentTemplate = EditorParent.ItemTemplate,
                    ContentTemplateSelector = EditorParent.ItemTemplateSelector
                };
            }

            AttachWorkflowItems(result);
            return result;
        }

        private void AttachWorkflowItems(WorkflowItem workflowItem)
        {
            if (EditorParent.ItemContainerStyle != null)
            {
                workflowItem.Style = EditorParent.ItemContainerStyle;
            }
            else if (EditorParent.ItemContainerStyleSelector != null)
            {
                workflowItem.Style = EditorParent.ItemContainerStyleSelector.SelectStyle(workflowItem.DataContext, workflowItem);
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
                added.CanvasParent = this;
                added.MouseLeftButtonDown += WorkflowItem_MouseLeftButtonDown;
                added.Selected += WorkflowItem_SelectedChanged;
                added.Unselected += WorkflowItem_SelectedChanged;
            }
            if (visualRemoved is WorkflowItem removed)
            {
                removed.CanvasParent = null;
                removed.MouseLeftButtonDown -= WorkflowItem_MouseLeftButtonDown;
                removed.Selected -= WorkflowItem_SelectedChanged;
                removed.Unselected -= WorkflowItem_SelectedChanged;
            }
        }

        private void WorkflowItem_SelectedChanged(object sender, EventArgs e)
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
            CanvasStatus = CanvasStatus.MultiMoving;
            _multiSelectionMask.Cursor = Cursors.ScrollAll;
            _multiMoveMouseDownPoint = e.GetPosition(this);
            foreach (var item in WorkflowItems.Where(x => x.IsSelected))
            {
                item.PreviousMouseDownPoint = new Point(GetLeft(item), GetTop(item));
            }
            _multiSelectionMask.CaptureMouse();
        }

        private void WorkflowItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var workflowItem = sender as WorkflowItem;
            _previousWorkflowItem = workflowItem;
            Point point = e.GetPosition(this);
            var startEllipseItem = GetElementWithPoint<EllipseItem>(point);
            if (startEllipseItem == null)
            {
                CanvasStatus = CanvasStatus.Moving;
            }
            else
            {
                _previousWorkflowItem = startEllipseItem.WorkflowParent;
                _lineStartPoint = startEllipseItem.GetPoint(this);
                _previousEllipseItem = startEllipseItem;
                CanvasStatus = CanvasStatus.Drawing;

                if (_currentLine == null)
                {
                    _currentLine = new LineItem(this);
                    _currentLine.StartEllipseItem = startEllipseItem;
                    Children.Add(_currentLine);
                }
            }

            if (CanvasStatus == CanvasStatus.Moving && !workflowItem.IsDraggable /*|| CanvasStatus is CanvasStatus.Selecting or CanvasStatus.MultiMoving*/)
            {
                return;
            }

            _moveMouseDownPoint = point;
            _moveMouseDownControlPoint = new Point(GetLeft(workflowItem), GetTop(workflowItem));
            if (CanvasStatus == CanvasStatus.Moving)
            {
                Cursor = Cursors.ScrollAll;
                workflowItem.CaptureMouse();
            }
            else
            {
                Cursor = Cursors.Cross;
                _currentLine.CaptureMouse();
            }
        }

        internal void HandleMouseDown(MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || CanvasStatus is CanvasStatus.MultiMoving)
            {
                return;
            }
            Point point = Mouse.GetPosition(this);
            Children.Remove(_multiSelectionMask);

            //bool isLineItem = false;
            IEnumerable<CanvasItem> canvasItems;
            BeginUpdateSelectedItems();
            if (this.GetVisualHitOfType<CanvasItem>(point) is CanvasItem canvasItem)
            {
                //isLineItem = selectableElement is LineItem;
                canvasItem.IsSelected = true;
                canvasItems = CanvasItems.Where(x => x != canvasItem);
            }
            else
            {
                canvasItems = CanvasItems;
            }
            if (!KeyboardHelper.IsCtrlKeyDown)
            {
                foreach (var item in canvasItems)
                {
                    item.IsSelected = false;
                }
            }
            EndUpdateSelectedItems();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            Point point = e.GetPosition(this);
            if (CanvasStatus == CanvasStatus.MultiMoving)
            {
                Vector vector = point - _multiMoveMouseDownPoint;
                foreach (var item in WorkflowItems.Where(x => x.IsSelected && x.IsDraggable))
                {
                    SetLeft(item, item.PreviousMouseDownPoint.X + vector.X);
                    SetTop(item, item.PreviousMouseDownPoint.Y + vector.Y);

                    item.UpdateCurve();
                }
                UpdateMultiSelectionMask();
            }
            else if (CanvasStatus == CanvasStatus.Drawing)
            {
                _currentLine.UpdateBezierCurve(_lineStartPoint, point);
            }
            else if (CanvasStatus == CanvasStatus.Moving)
            {
                var workflowItem = _previousWorkflowItem;
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

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            try
            {
                if (CanvasStatus == CanvasStatus.Drawing)
                {
                    Point point = e.GetPosition(this);
                    var endWorkflowItem = GetElementWithPoint<WorkflowItem>(point);
                    if (endWorkflowItem != null)
                    {
                        SetStep(_previousWorkflowItem, endWorkflowItem, _previousEllipseItem);
                    }
                }
                else if (CanvasStatus == CanvasStatus.Moving)
                {
                    if (_previousWorkflowItem?.IsDraggable == true)
                    {
                        PositionWorkflowItem(_previousWorkflowItem);
                    }
                }
                else if (CanvasStatus == CanvasStatus.MultiMoving)
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
                Children.Remove(_currentLine);

                CanvasStatus = CanvasStatus.None;

                _multiSelectionMask.ReleaseMouseCapture();
                _previousWorkflowItem?.ReleaseMouseCapture();
                _currentLine?.ReleaseMouseCapture();

                _currentLine = null;
                Cursor = null;
                _multiSelectionMask.Cursor = null;
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

        private void PositionWorkflowItem(WorkflowItem workflowItem)
        {
            SetLeft(workflowItem, Adsorb(GetLeft(workflowItem)));
            SetTop(workflowItem, Adsorb(GetTop(workflowItem)));
            workflowItem.UpdateCurve();
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

            if (toEllipse.LineItem != null)
            {
                MessageTip.ShowWarning("该节点已经存在连接关系，无法创建连接曲线，请删除后再试");
            }
            else
            {
                if (fromEllipse.Dock == Dock.Right)
                {
                    fromStep.JumpToStep = toStep.DataContext;
                    toStep.FromStep = fromStep.DataContext;
                }
                else if (fromEllipse.Dock == Dock.Bottom)
                {
                    fromStep.NextStep = toStep.DataContext;
                    toStep.PreviousStep = fromStep.DataContext;
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
            return value.Adsorb(EditorParent.GridSpacing);
        }
    }
}