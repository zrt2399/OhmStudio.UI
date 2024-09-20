using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OhmStudio.UI.Commands;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Controls
{
    public enum ShapeType
    {
        Rectangle,
        Diamond,
        Parallelogram
    }

    //[TypeConverter(typeof(EnumDescriptionConverter))]
    public enum StepType
    {
        [Description("开始")]
        Begin,
        [Description("中间节点")]
        Nomal,
        [Description("条件节点")]
        Condition,
        [Description("标记节点")]
        Reference,
        [Description("结束")]
        End
    }

    public abstract class SelectionControl : ContentControl
    {
        public event RoutedEventHandler Selected;

        public event RoutedEventHandler Unselected;

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(SelectionControl), new PropertyMetadata(OnIsSelectedChanged));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selectionControl = (SelectionControl)d;
            selectionControl.OnIsSelectedChanged();
        }

        public virtual void OnIsSelectedChanged()
        {
            var routedEventHandler = IsSelected ? Selected : Unselected;
            routedEventHandler?.Invoke(this, new RoutedEventArgs());
        }
    }

    public class LineItem : SelectionControl
    {
        static LineItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineItem), new FrameworkPropertyMetadata(typeof(LineItem)));
        }

        public LineItem()
        {
            DeleteCommand = new RelayCommand(Delete);
        }

        public LineItem(WorkflowCanvas canvasParent) : this()
        {
            CanvasParent = canvasParent;
        }

        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register(nameof(StartPoint), typeof(Point), typeof(LineItem), new PropertyMetadata((sender, e) =>
            {
                if (sender is LineItem lineItem && e.NewValue is Point point)
                {
                    Canvas.SetLeft(lineItem, point.X);
                    Canvas.SetTop(lineItem, point.Y);
                }
            }));

        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register(nameof(EndPoint), typeof(Point), typeof(LineItem));

        public static readonly DependencyProperty Point0Property =
            DependencyProperty.Register(nameof(Point0), typeof(Point), typeof(LineItem));

        public static readonly DependencyProperty Point1Property =
            DependencyProperty.Register(nameof(Point1), typeof(Point), typeof(LineItem));

        public static readonly DependencyProperty Point2Property =
            DependencyProperty.Register(nameof(Point2), typeof(Point), typeof(LineItem));

        public static readonly DependencyProperty Point3Property =
            DependencyProperty.Register(nameof(Point3), typeof(Point), typeof(LineItem));

        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(nameof(Points), typeof(PointCollection), typeof(LineItem));

        public static readonly DependencyProperty StartEllipseItemProperty =
            DependencyProperty.Register(nameof(StartEllipseItem), typeof(EllipseItem), typeof(LineItem));

        public static readonly DependencyProperty EndEllipseItemProperty =
            DependencyProperty.Register(nameof(EndEllipseItem), typeof(EllipseItem), typeof(LineItem));

        public static readonly DependencyProperty IsCurveProperty =
            DependencyProperty.Register(nameof(IsCurve), typeof(bool), typeof(LineItem), new PropertyMetadata(true, (sender, e) =>
            {
                var lineItem = (LineItem)sender;
                if (lineItem.StartEllipseItem != null && lineItem.EndEllipseItem != null)
                {
                    var startPoint = lineItem.StartEllipseItem.GetPoint(lineItem.CanvasParent);
                    var endPoint = lineItem.EndEllipseItem.GetPoint(lineItem.CanvasParent);
                    lineItem.UpdateCurveAngle(startPoint, endPoint);
                }
            }));

        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(nameof(StrokeDashArray), typeof(DoubleCollection), typeof(LineItem));

        public static readonly DependencyProperty HighlightLineBrushProperty =
            DependencyProperty.Register(nameof(HighlightLineBrush), typeof(Brush), typeof(LineItem), new PropertyMetadata(Brushes.Orange));

        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.Register(nameof(LineBrush), typeof(Brush), typeof(LineItem));

        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register(nameof(LineThickness), typeof(double), typeof(LineItem), new PropertyMetadata(2d));

        public Point StartPoint
        {
            get => (Point)GetValue(StartPointProperty);
            set => SetValue(StartPointProperty, value);
        }

        public Point EndPoint
        {
            get => (Point)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public Point Point0
        {
            get => (Point)GetValue(Point0Property);
            set => SetValue(Point0Property, value);
        }

        public Point Point1
        {
            get => (Point)GetValue(Point1Property);
            set => SetValue(Point1Property, value);
        }

        public Point Point2
        {
            get => (Point)GetValue(Point2Property);
            set => SetValue(Point2Property, value);
        }

        public Point Point3
        {
            get => (Point)GetValue(Point3Property);
            set => SetValue(Point3Property, value);
        }

        /// <summary>
        /// Gets or sets a collection that contains the vertex points of the polygon.
        /// </summary>
        public PointCollection Points
        {
            get => (PointCollection)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        public EllipseItem StartEllipseItem
        {
            get => (EllipseItem)GetValue(StartEllipseItemProperty);
            set => SetValue(StartEllipseItemProperty, value);
        }

        public EllipseItem EndEllipseItem
        {
            get => (EllipseItem)GetValue(EndEllipseItemProperty);
            set => SetValue(EndEllipseItemProperty, value);
        }

        public bool IsCurve
        {
            get => (bool)GetValue(IsCurveProperty);
            set => SetValue(IsCurveProperty, value);
        }

        public DoubleCollection StrokeDashArray
        {
            get => (DoubleCollection)GetValue(StrokeDashArrayProperty);
            set => SetValue(StrokeDashArrayProperty, value);
        }

        public Brush LineBrush
        {
            get => (Brush)GetValue(LineBrushProperty);
            set => SetValue(LineBrushProperty, value);
        }

        public Brush HighlightLineBrush
        {
            get => (Brush)GetValue(HighlightLineBrushProperty);
            set => SetValue(HighlightLineBrushProperty, value);
        }

        [TypeConverter(typeof(LengthConverter))]
        public double LineThickness
        {
            get => (double)GetValue(LineThicknessProperty);
            set => SetValue(LineThicknessProperty, value);
        }

        public WorkflowCanvas CanvasParent { get; private set; }

        public ICommand DeleteCommand { get; }

        public void UpdateBezierCurve(Point startPoint, Point endPoint)
        {
            Point startPointTemp = new Point();
            Point endPointTemp = new Point();

            startPointTemp.X = Math.Min(startPoint.X, endPoint.X);
            startPointTemp.Y = Math.Min(startPoint.Y, endPoint.Y);
            endPointTemp.X = Math.Max(startPoint.X, endPoint.X);
            endPointTemp.Y = Math.Max(startPoint.Y, endPoint.Y);

            StartPoint = startPointTemp;
            EndPoint = endPointTemp;

            UpdateCurveAngle(startPoint, endPoint);
        }

        private void UpdateCurveAngle(Point startPoint, Point endPoint)
        {
            startPoint.X -= StartPoint.X;
            startPoint.Y -= StartPoint.Y;
            endPoint.X -= StartPoint.X;
            endPoint.Y -= StartPoint.Y;
            Point0 = startPoint;
            if (IsCurve)
            {
                Point controlPoint1 = new Point((startPoint.X + endPoint.X) / 2, endPoint.Y);
                Point controlPoint2 = new Point((startPoint.X + endPoint.X) / 2, startPoint.Y);
                if (StartEllipseItem != null)
                {
                    double offset = 200;
                    if (StartEllipseItem.Dock == Dock.Right)
                    {
                        controlPoint1 = new Point(startPoint.X + offset, startPoint.Y);
                        controlPoint2 = new Point(endPoint.X - offset, endPoint.Y);
                    }
                    else if (StartEllipseItem.Dock == Dock.Bottom)
                    {
                        controlPoint1 = new Point(startPoint.X, startPoint.Y + offset);
                        controlPoint2 = new Point(endPoint.X, endPoint.Y - offset);
                    }
                }

                Point1 = controlPoint1;
                Point2 = controlPoint2;
            }
            else
            {
                Vector vector = endPoint - startPoint;
                Point1 = startPoint + vector;
                Point2 = endPoint - vector;
            }
            Point3 = endPoint;
            UpdateArrow(Point2, endPoint);
        }

        private void UpdateArrow(Point point2, Point endPoint)
        {
            double arrowLength = 16;
            double arrowWidth = 12;

            // 计算箭头的方向
            Vector direction = endPoint - point2;
            direction.Normalize();

            // 算出箭头的两个角点
            Point arrowPoint1 = endPoint - direction * arrowLength + new Vector(-direction.Y, direction.X) * arrowWidth / 2;
            Point arrowPoint2 = endPoint - direction * arrowLength - new Vector(-direction.Y, direction.X) * arrowWidth / 2;

            // 更新箭头形状的点
            Points = new PointCollection(new Point[] { endPoint, arrowPoint1, arrowPoint2 });
        }

        internal void Delete()
        {
            StartEllipseItem.RemoveStep();
            StartEllipseItem.LineItem = null;
            StartEllipseItem = null;
            EndEllipseItem.LineItem = null;
            EndEllipseItem = null;
            ContextMenu = null;
            BindingOperations.ClearAllBindings(this);
            CanvasParent.Children.Remove(this);
        }
    }

    public class EllipseItem : Control
    {
        static EllipseItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EllipseItem), new FrameworkPropertyMetadata(typeof(EllipseItem)));
        }

        public static readonly DependencyProperty DockProperty =
            DependencyProperty.Register(nameof(Dock), typeof(Dock), typeof(EllipseItem));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(EllipseItem), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public Dock Dock
        {
            get => (Dock)GetValue(DockProperty);
            set => SetValue(DockProperty, value);
        }

        [TypeConverter(typeof(LengthConverter))]
        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public WorkflowItem WorkflowParent { get; internal set; }

        public LineItem LineItem { get; internal set; }

        public Point GetPoint(UIElement parent)
        {
            return TranslatePoint(new Point(ActualWidth / 2, ActualHeight / 2), parent);
        }

        internal void RemoveStep()
        {
            if (Dock == Dock.Right)
            {
                WorkflowParent.FirstOrDefault(WorkflowParent.JumpStep).FromStep = null;
                WorkflowParent.JumpStep = null;
            }
            else if (Dock == Dock.Bottom)
            {
                WorkflowParent.FirstOrDefault(WorkflowParent.NextStep).LastStep = null;
                WorkflowParent.NextStep = null;
            }
        }
    }

    /// <summary>
    /// 流程节点项。
    /// </summary>
    public class WorkflowItem : SelectionControl
    {
        static WorkflowItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WorkflowItem), new FrameworkPropertyMetadata(typeof(WorkflowItem)));
        }

        public WorkflowItem()
        {
            DependencyPropertyDescriptor property = DependencyPropertyDescriptor.FromProperty(IsKeyboardFocusWithinProperty, typeof(WorkflowItem));
            //property?.RemoveValueChanged(this, OnIsKeyboardFocusWithinChanged);
            property?.AddValueChanged(this, OnIsKeyboardFocusWithinChanged);
        }

        public static readonly DependencyProperty IsDraggableProperty =
            DependencyProperty.Register(nameof(IsDraggable), typeof(bool), typeof(WorkflowItem), new PropertyMetadata(true));

        public static readonly DependencyProperty LastStepProperty =
            DependencyProperty.Register(nameof(LastStep), typeof(object), typeof(WorkflowItem), new PropertyMetadata(OnWorkflowItemChanged));

        public static readonly DependencyProperty FromStepProperty =
            DependencyProperty.Register(nameof(FromStep), typeof(object), typeof(WorkflowItem), new PropertyMetadata(OnWorkflowItemChanged));

        public static readonly DependencyProperty JumpStepProperty =
            DependencyProperty.Register(nameof(JumpStep), typeof(object), typeof(WorkflowItem), new PropertyMetadata(OnWorkflowItemChanged));

        public static readonly DependencyProperty NextStepProperty =
            DependencyProperty.Register(nameof(NextStep), typeof(object), typeof(WorkflowItem), new PropertyMetadata(OnWorkflowItemChanged));

        public static readonly DependencyProperty StepTypeProperty =
            DependencyProperty.Register(nameof(StepType), typeof(StepType), typeof(WorkflowItem), new PropertyMetadata(StepType.Nomal));

        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register(nameof(Geometry), typeof(Geometry), typeof(WorkflowItem));

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register(nameof(HighlightBrush), typeof(Brush), typeof(WorkflowItem), new PropertyMetadata(Brushes.Orange));

        private Thumb PART_Thumb;
        private EllipseItem EllipseLeft;
        private EllipseItem EllipseTop;
        private EllipseItem EllipseRight;
        private EllipseItem EllipseBottom;

        internal Point LastMouseDownPoint { get; set; }

        internal Dictionary<Dock, EllipseItem> EllipseItems { get; private set; }

        public bool IsInit { get; private set; }

        public WorkflowCanvas CanvasParent { get; internal set; }

        public WorkflowEditor EditorParent => CanvasParent.EditorParent;

        public bool IsDraggable
        {
            get => (bool)GetValue(IsDraggableProperty);
            set => SetValue(IsDraggableProperty, value);
        }

        public object LastStep
        {
            get => GetValue(LastStepProperty);
            set => SetValue(LastStepProperty, value);
        }

        public object FromStep
        {
            get => GetValue(FromStepProperty);
            set => SetValue(FromStepProperty, value);
        }

        public object JumpStep
        {
            get => GetValue(JumpStepProperty);
            set => SetValue(JumpStepProperty, value);
        }

        public object NextStep
        {
            get => GetValue(NextStepProperty);
            set => SetValue(NextStepProperty, value);
        }

        public StepType StepType
        {
            get => (StepType)GetValue(StepTypeProperty);
            set => SetValue(StepTypeProperty, value);
        }

        public Geometry Geometry
        {
            get => (Geometry)GetValue(GeometryProperty);
            set => SetValue(GeometryProperty, value);
        }

        public Brush HighlightBrush
        {
            get => (Brush)GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }

        public override void OnApplyTemplate()
        {
            if (PART_Thumb != null)
            {
                PART_Thumb.DragDelta -= Thumb_DragDelta;
                PART_Thumb.DragCompleted -= PART_Thumb_DragCompleted;
            }
            base.OnApplyTemplate();
            EllipseLeft = GetTemplateChild("EllipseLeft") as EllipseItem;
            EllipseTop = GetTemplateChild("EllipseTop") as EllipseItem;
            EllipseRight = GetTemplateChild("EllipseRight") as EllipseItem;
            EllipseBottom = GetTemplateChild("EllipseBottom") as EllipseItem;

            PART_Thumb = GetTemplateChild("PART_Thumb") as Thumb;
            PART_Thumb.DragDelta += Thumb_DragDelta;
            PART_Thumb.DragCompleted += PART_Thumb_DragCompleted;

            if (EllipseItems == null)
            {
                EllipseItems = new Dictionary<Dock, EllipseItem>();
                EllipseItems.Add(EllipseLeft.Dock, EllipseLeft);
                EllipseItems.Add(EllipseTop.Dock, EllipseTop);
                EllipseItems.Add(EllipseRight.Dock, EllipseRight);
                EllipseItems.Add(EllipseBottom.Dock, EllipseBottom);
            }

            foreach (var item in EllipseItems.Values)
            {
                item.WorkflowParent = this;
            }
            IsInit = true;
            UpdateCurve();
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange > 0)
            {
                Width += e.HorizontalChange;
            }
            if (Height + e.VerticalChange > 0)
            {
                Height += e.VerticalChange;
            }
            UpdateCurve();
        }

        private void PART_Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var cellSize = EditorParent.GridSpacing;
            Width = Math.Max(cellSize, Width.Adsorb(cellSize));
            Height = Math.Max(cellSize, Height.Adsorb(cellSize));
            UpdateCurve();
        }

        private void OnIsKeyboardFocusWithinChanged(object sender, EventArgs e)
        {
            if (IsKeyboardFocusWithin)
            {
                CanvasParent.BeginUpdateSelectedItems();
                IsSelected = true;
                if (!CanvasParent.IsCtrlKeyDown)
                {
                    foreach (var item in CanvasParent.SelectableElements.Where(x => x != this))
                    {
                        item.IsSelected = false;
                    }
                }
                CanvasParent.EndUpdateSelectedItems();
            }
        }

        internal void Delete()
        {
            if (IsInit)
            {
                foreach (var item in EllipseItems.Values)
                {
                    item.LineItem?.Delete();
                }
            }
            else
            {
                if (LastStep != null)
                {
                    FirstOrDefault(LastStep).NextStep = null;
                    LastStep = null;
                }
                if (NextStep != null)
                {
                    FirstOrDefault(NextStep).LastStep = null;
                    NextStep = null;
                }
                if (FromStep != null)
                {
                    FirstOrDefault(FromStep).JumpStep = null;
                    FromStep = null;
                }
                if (JumpStep != null)
                {
                    FirstOrDefault(JumpStep).FromStep = null;
                    JumpStep = null;
                }
            }
            BindingOperations.ClearAllBindings(this);
            CanvasParent.Children.Remove(this);
        }

        private static void OnWorkflowItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WorkflowItem)d).OnWorkflowItemChanged(e);
        }

        protected virtual void OnWorkflowItemChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateCurve();
        }

        internal WorkflowItem FirstOrDefault(object item)
        {
            return CanvasParent.FirstOrDefault(item);
        }

        public void UpdateCurve()
        {
            //if (!IsInit)
            //{
            //    Loaded += WorkflowItem_Loaded;
            //    return;
            //}
            Dispatcher.InvokeAsync(() =>
            {
                if (LastStep != null)
                {
                    UpdateCurve(EllipseItems[Dock.Top].LineItem, FirstOrDefault(LastStep).EllipseItems[Dock.Bottom], EllipseItems[Dock.Top]);
                }
                if (NextStep != null)
                {
                    UpdateCurve(EllipseItems[Dock.Bottom].LineItem, EllipseItems[Dock.Bottom], FirstOrDefault(NextStep).EllipseItems[Dock.Top]);
                }
                if (FromStep != null)
                {
                    UpdateCurve(EllipseItems[Dock.Left].LineItem, FirstOrDefault(FromStep).EllipseItems[Dock.Right], EllipseItems[Dock.Left]);
                }
                if (JumpStep != null)
                {
                    UpdateCurve(EllipseItems[Dock.Right].LineItem, EllipseItems[Dock.Right], FirstOrDefault(JumpStep).EllipseItems[Dock.Left]);
                }
            }, DispatcherPriority.Render);
        }

        internal bool CheckOverlap(RectangleGeometry rectangleGeometry)
        {
            GeneralTransform transform = TransformToVisual(CanvasParent);
            Geometry geometry = Geometry?.Clone();
            if (geometry != null)
            {
                geometry.Transform = (Transform)transform;
            }
            return rectangleGeometry.FillContainsWithDetail(geometry) != IntersectionDetail.Empty;
        }

        private void WorkflowItem_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= WorkflowItem_Loaded;
            UpdateCurve();
        }

        private void UpdateCurve(LineItem lineItem, EllipseItem startEllipseItem, EllipseItem endEllipseItem)
        {
            if (!startEllipseItem.IsVisible || !endEllipseItem.IsVisible)
            {
                return;
            }

            if (lineItem == null)
            {
                lineItem = new LineItem(CanvasParent);
                if (DataContext is not WorkflowItem)
                {
                    lineItem.Content = DataContext;
                    lineItem.DataContext = DataContext;
                    lineItem.ContentTemplate = EditorParent.LineTemplate;
                    lineItem.ContentTemplateSelector = EditorParent.LineTemplateSelector;
                    if (EditorParent.LineContainerStyle != null)
                    {
                        lineItem.Style = EditorParent.LineContainerStyle;
                    }
                    else if (EditorParent.LineContainerStyleSelector != null)
                    {
                        lineItem.Style = EditorParent.LineContainerStyleSelector.SelectStyle(DataContext, lineItem);
                    }
                }
                CanvasParent.Children.Add(lineItem);
                startEllipseItem.LineItem = lineItem;
                endEllipseItem.LineItem = lineItem;
                lineItem.StartEllipseItem = startEllipseItem;
                lineItem.EndEllipseItem = endEllipseItem;
            }
            lineItem.UpdateBezierCurve(lineItem.StartEllipseItem.GetPoint(CanvasParent), lineItem.EndEllipseItem.GetPoint(CanvasParent));
        }
    }
}