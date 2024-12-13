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
using OhmStudio.UI.Helpers;
using OhmStudio.UI.Utilities;

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

    public abstract class CanvasItem : ContentControl
    {
        public event EventHandler Selected;

        public event EventHandler Unselected;

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(CanvasItem), new PropertyMetadata(OnIsSelectedChanged));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public WorkflowCanvas CanvasParent { get; internal set; }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var canvasItem = (CanvasItem)d;
            canvasItem.OnIsSelectedChanged(canvasItem.IsSelected);
        }

        protected virtual void OnIsSelectedChanged(bool isSelected)
        {
            var routedEventHandler = isSelected ? Selected : Unselected;
            routedEventHandler?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);
            if (CanvasParent != null && IsKeyboardFocusWithin)
            {
                CanvasParent.BeginUpdateSelectedItems();
                IsSelected = true;
                if (!KeyboardHelper.IsCtrlKeyDown)
                {
                    foreach (var item in CanvasParent.CanvasItems.Where(x => x != this))
                    {
                        item.IsSelected = false;
                    }
                }
                CanvasParent.EndUpdateSelectedItems();
            }
        }
    }

    public class LineItem : CanvasItem
    {
        private const double _baseOffset = 100d;
        private const double _offsetGrowthRate = 25d;

        private double Spacing { get; set; } = 20;
        private uint DirectionalArrowsCount { get; set; } = 2;

        private Point Source { get; set; }
        private Point Target { get; set; }

        private double DirectionalArrowsOffset { get; set; } = 0;

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

        public static readonly DependencyProperty ArrowSizeProperty =
           DependencyProperty.Register(nameof(ArrowSize), typeof(Size), typeof(LineItem), new PropertyMetadata(new Size(8, 8)));

        public static readonly DependencyProperty HighlightLineBrushProperty =
            DependencyProperty.Register(nameof(HighlightLineBrush), typeof(Brush), typeof(LineItem), new PropertyMetadata(Brushes.Orange));

        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.Register(nameof(LineBrush), typeof(Brush), typeof(LineItem), new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register(nameof(LineThickness), typeof(double), typeof(LineItem), new FrameworkPropertyMetadata(2d, FrameworkPropertyMetadataOptions.AffectsRender));

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

        public Size ArrowSize
        {
            get => (Size)GetValue(ArrowSizeProperty);
            set => SetValue(ArrowSizeProperty, value);
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

        public ICommand DeleteCommand { get; }

        public EllipseItem StartEllipseItem { get; internal set; }

        public EllipseItem EndEllipseItem { get; internal set; }

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
            Source = startPoint;
            Target = endPoint;

            var width = Math.Abs(startPoint.X - endPoint.X);
            var height = Math.Abs(startPoint.Y - endPoint.Y);
            if (this.FindChildOfType<UIElement>() is UIElement child)
            {
                child.Measure(new Size(double.MaxValue, double.MaxValue));
                Width = Math.Max(width, child.DesiredSize.Width);
                Height = Math.Max(height, child.DesiredSize.Height);
            }
            else
            {
                Width = width;
                Height = height;
            }

            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //base.OnRender(drawingContext);
            bool isForward = StartEllipseItem.Dock is Dock.Right or Dock.Bottom;
            double direction = isForward ? 1d : -1d;
            var spacing = new Vector(Spacing * direction, 0d);
            var spacingVertical = new Vector(spacing.Y, spacing.X);

            var source = Source;
            var target = Target;

            var sourceOrientation = StartEllipseItem.Dock is Dock.Bottom or Dock.Top ? Orientation.Vertical : Orientation.Horizontal;
            var targetOrientation = StartEllipseItem.Dock is Dock.Bottom or Dock.Top ? Orientation.Vertical : Orientation.Horizontal;

            Point startPoint = source + (sourceOrientation == Orientation.Vertical ? spacingVertical : spacing);
            Point endPoint = target - (targetOrientation == Orientation.Vertical ? spacingVertical : spacing);

            Vector delta = target - source;
            double height = Math.Abs(delta.Y);
            double width = Math.Abs(delta.X);

            // Smooth curve when distance is lower than base offset
            double smooth = Math.Min(_baseOffset, height);
            // Calculate offset based on distance
            double offset = Math.Max(smooth, width / 2d);
            // Grow slowly with distance
            offset = Math.Min(_baseOffset + Math.Sqrt(width * _offsetGrowthRate), offset);

            var controlPoint = new Vector(offset * direction, 0d);
            var controlPointVertical = new Vector(controlPoint.Y, controlPoint.X);

            // Avoid sharp bend if orientation different (when close to each other)
            if (targetOrientation != sourceOrientation)
            {
                controlPoint *= 0.5;
            }

            Point p0 = startPoint;
            Point p1 = startPoint + (sourceOrientation == Orientation.Vertical ? controlPointVertical : controlPoint);
            Point p2 = endPoint - (targetOrientation == Orientation.Vertical ? controlPointVertical : controlPoint);
            Point p3 = endPoint;

            var lineBrush = LineBrush;
            var pen = new Pen(lineBrush, LineThickness);
            if (lineBrush != null && pen.IsFrozen)
            {
                pen.Freeze();
            }

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(source, false, false);
                ctx.LineTo(p0, true, true);
                ctx.BezierTo(p1, p2, p3, true, true);
                ctx.LineTo(target, true, true);

                DrawDirectionalArrowsGeometry(ctx, p0, p1, p2, p3);
                DrawDefaultArrowhead(ctx, source, target, isForward, targetOrientation);
            }
            geometry.Freeze();

            drawingContext.DrawGeometry(lineBrush, pen, geometry);
        }

        private void DrawDefaultArrowhead(StreamGeometryContext context, Point source, Point target, bool isForward, Orientation orientation)
        {
            double direction = isForward ? 1d : -1d;
            Point from, to;
            if (orientation == Orientation.Horizontal)
            {
                double headWidth = ArrowSize.Width;
                double headHeight = ArrowSize.Height / 2;

                from = new Point(target.X - headWidth * direction, target.Y + headHeight);
                to = new Point(target.X - headWidth * direction, target.Y - headHeight);
            }
            else
            {
                double headWidth = ArrowSize.Width / 2;
                double headHeight = ArrowSize.Height;

                from = new Point(target.X - headWidth, target.Y - headHeight * direction);
                to = new Point(target.X + headWidth, target.Y - headHeight * direction);
            }
            context.BeginFigure(target, true, true);
            context.LineTo(from, true, true);
            context.LineTo(to, true, true);
        }

        private void DrawDirectionalArrowsGeometry(StreamGeometryContext context, Point p0, Point p1, Point p2, Point p3)
        {
            uint directionalArrowsCount = GetDirectionalArrowsCount();
            double spacing = 1d / (directionalArrowsCount + 1);
            for (int i = 1; i <= directionalArrowsCount; i++)
            {
                double t = (spacing * i + DirectionalArrowsOffset).WrapToRange(0d, 1d);
                var to = InterpolateCubicBezier(p0, p1, p2, p3, t);
                var direction = GetBezierTangent(p0, p1, p2, p3, t);

                DrawDirectionalArrowheadGeometry(context, direction, to);
            }
        }

        private uint GetDirectionalArrowsCount()
        {
            double offset = 5d, minDistance = 120d;
            if (Math.Abs(Source.X - Target.X) < offset && Math.Abs(Source.Y - Target.Y) < minDistance)
            {
                return 0;
            }
            if (Math.Abs(Source.Y - Target.Y) < offset && Math.Abs(Source.X - Target.X) < minDistance)
            {
                return 0;
            }
            return DirectionalArrowsCount;
        }

        private void DrawDirectionalArrowheadGeometry(StreamGeometryContext context, Vector direction, Point location)
        {
            double headWidth = ArrowSize.Width;
            double headHeight = ArrowSize.Height / 2;

            double angle = Math.Atan2(direction.Y, direction.X);
            double sinT = Math.Sin(angle);
            double cosT = Math.Cos(angle);

            var from = new Point(location.X + (headWidth * cosT - headHeight * sinT), location.Y + (headWidth * sinT + headHeight * cosT));
            var to = new Point(location.X + (headWidth * cosT + headHeight * sinT), location.Y - (headHeight * cosT - headWidth * sinT));

            context.BeginFigure(location, true, true);
            context.LineTo(from, true, true);
            context.LineTo(to, true, true);
        }

        private static Vector GetBezierTangent(Point P0, Point P1, Point P2, Point P3, double t)
        {
            // Calculate the derivatives of the Bezier curve equation and negate the result
            return -(-3 * (1 - t) * (1 - t) * (Vector)P0 +
                    (3 * (1 - t) * (1 - t) * (Vector)P1 - 6 * t * (1 - t) * (Vector)P1) +
                    (6 * t * (1 - t) * (Vector)P2 - 3 * t * t * (Vector)P2) +
                    3 * t * t * (Vector)P3);
        }

        private static Point InterpolateCubicBezier(Point P0, Point P1, Point P2, Point P3, double t)
        {
            // B = (1 − t)^3 * P0 + 3 * t * (1 − t)^2 * P1 + 3 * t^2 * (1 − t) * P2 + t^3 * P3
            return (Point)
                 ((Vector)P0 * (1 - t) * (1 - t) * (1 - t)
                + (Vector)P1 * 3 * t * (1 - t) * (1 - t)
                + (Vector)P2 * 3 * t * t * (1 - t)
                + (Vector)P3 * t * t * t);
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
                WorkflowParent.FirstOrDefault(WorkflowParent.JumpToStep).FromStep = null;
                WorkflowParent.JumpToStep = null;
            }
            else if (Dock == Dock.Bottom)
            {
                WorkflowParent.FirstOrDefault(WorkflowParent.NextStep).PreviousStep = null;
                WorkflowParent.NextStep = null;
            }
        }
    }

    /// <summary>
    /// 流程节点项。
    /// </summary>
    public class WorkflowItem : CanvasItem
    {
        private Thumb PART_Thumb;
        private EllipseItem EllipseLeft;
        private EllipseItem EllipseTop;
        private EllipseItem EllipseRight;
        private EllipseItem EllipseBottom;

        static WorkflowItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WorkflowItem), new FrameworkPropertyMetadata(typeof(WorkflowItem)));
        }

        public static readonly DependencyProperty IsDraggableProperty =
            DependencyProperty.Register(nameof(IsDraggable), typeof(bool), typeof(WorkflowItem), new PropertyMetadata(true));

        public static readonly DependencyProperty PreviousStepProperty =
            DependencyProperty.Register(nameof(PreviousStep), typeof(object), typeof(WorkflowItem), new PropertyMetadata(OnAnyStepChanged));

        public static readonly DependencyProperty FromStepProperty =
            DependencyProperty.Register(nameof(FromStep), typeof(object), typeof(WorkflowItem), new PropertyMetadata(OnAnyStepChanged));

        public static readonly DependencyProperty JumpToStepProperty =
            DependencyProperty.Register(nameof(JumpToStep), typeof(object), typeof(WorkflowItem), new PropertyMetadata(OnAnyStepChanged));

        public static readonly DependencyProperty NextStepProperty =
            DependencyProperty.Register(nameof(NextStep), typeof(object), typeof(WorkflowItem), new PropertyMetadata(OnAnyStepChanged));

        public static readonly DependencyProperty StepTypeProperty =
            DependencyProperty.Register(nameof(StepType), typeof(StepType), typeof(WorkflowItem), new PropertyMetadata(StepType.Nomal));

        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register(nameof(Geometry), typeof(Geometry), typeof(WorkflowItem));

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register(nameof(HighlightBrush), typeof(Brush), typeof(WorkflowItem), new PropertyMetadata(Brushes.Orange));

        internal Point PreviousMouseDownPoint { get; set; }

        internal Dictionary<Dock, EllipseItem> EllipseItems { get; private set; }

        public bool IsInit { get; private set; }

        public WorkflowEditor EditorParent => CanvasParent.EditorParent;

        public bool IsDraggable
        {
            get => (bool)GetValue(IsDraggableProperty);
            set => SetValue(IsDraggableProperty, value);
        }

        public object PreviousStep
        {
            get => GetValue(PreviousStepProperty);
            set => SetValue(PreviousStepProperty, value);
        }

        public object FromStep
        {
            get => GetValue(FromStepProperty);
            set => SetValue(FromStepProperty, value);
        }

        public object JumpToStep
        {
            get => GetValue(JumpToStepProperty);
            set => SetValue(JumpToStepProperty, value);
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
                if (PreviousStep != null)
                {
                    FirstOrDefault(PreviousStep).NextStep = null;
                    PreviousStep = null;
                }
                if (NextStep != null)
                {
                    FirstOrDefault(NextStep).PreviousStep = null;
                    NextStep = null;
                }
                if (FromStep != null)
                {
                    FirstOrDefault(FromStep).JumpToStep = null;
                    FromStep = null;
                }
                if (JumpToStep != null)
                {
                    FirstOrDefault(JumpToStep).FromStep = null;
                    JumpToStep = null;
                }
            }
            BindingOperations.ClearAllBindings(this);
            CanvasParent.Children.Remove(this);
        }

        private static void OnAnyStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WorkflowItem)d).OnAnyStepChanged(e);
        }

        protected virtual void OnAnyStepChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateCurve();
        }

        internal WorkflowItem FirstOrDefault(object item)
        {
            return CanvasParent.FirstOrDefault(item);
        }

        public void UpdateCurve()
        {
            if (!IsInit)
            {
                return;
            }
            Dispatcher.InvokeAsync(() =>
            {
                if (PreviousStep != null)
                {
                    UpdateCurve(EllipseItems[Dock.Top].LineItem, FirstOrDefault(PreviousStep).EllipseItems[Dock.Bottom], EllipseItems[Dock.Top]);
                }
                if (NextStep != null)
                {
                    UpdateCurve(EllipseItems[Dock.Bottom].LineItem, EllipseItems[Dock.Bottom], FirstOrDefault(NextStep).EllipseItems[Dock.Top]);
                }
                if (FromStep != null)
                {
                    UpdateCurve(EllipseItems[Dock.Left].LineItem, FirstOrDefault(FromStep).EllipseItems[Dock.Right], EllipseItems[Dock.Left]);
                }
                if (JumpToStep != null)
                {
                    UpdateCurve(EllipseItems[Dock.Right].LineItem, EllipseItems[Dock.Right], FirstOrDefault(JumpToStep).EllipseItems[Dock.Left]);
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