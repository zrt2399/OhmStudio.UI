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
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(SelectionControl), new PropertyMetadata((sender, e) =>
            {
                var selectionControl = (SelectionControl)sender;
                bool newValue = (bool)e.NewValue;
                var routedEventHandler = newValue ? selectionControl.Selected : selectionControl.Unselected;
                routedEventHandler?.Invoke(selectionControl, new RoutedEventArgs());
            }));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public event RoutedEventHandler Selected;

        public event RoutedEventHandler Unselected;
    }

    public class ShapeBorder : Border
    {
        public static readonly DependencyProperty ShapeTypeProperty =
            DependencyProperty.Register(nameof(ShapeType), typeof(ShapeType), typeof(ShapeBorder), new FrameworkPropertyMetadata(ShapeType.Rectangle, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IsDashedProperty =
            DependencyProperty.Register(nameof(IsDashed), typeof(bool), typeof(ShapeBorder), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShearProperty =
            DependencyProperty.Register(nameof(Shear), typeof(double), typeof(ShapeBorder), new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register(nameof(Geometry), typeof(Geometry), typeof(ShapeBorder));

        public ShapeType ShapeType
        {
            get => (ShapeType)GetValue(ShapeTypeProperty);
            set => SetValue(ShapeTypeProperty, value);
        }

        public bool IsDashed
        {
            get => (bool)GetValue(IsDashedProperty);
            set => SetValue(IsDashedProperty, value);
        }

        public double Shear
        {
            get => (double)GetValue(ShearProperty);
            set => SetValue(ShearProperty, value);
        }

        public Geometry Geometry
        {
            get => (Geometry)GetValue(GeometryProperty);
            set => SetValue(GeometryProperty, value);
        }

        private void DrawPolygon(DrawingContext dc, Brush brush, Pen pen, params Point[] points)
        {
            if (!points.Any())
            {
                return;
            }

            var geometry = new StreamGeometry();
            using (var geometryContext = geometry.Open())
            {
                geometryContext.BeginFigure(points[0], true, true);
                for (var i = 1; i < points.Length; i++)
                {
                    geometryContext.LineTo(points[i], true, false);
                }
            }
            geometry.Freeze();
            Geometry = geometry;
            dc.DrawGeometry(brush, pen, geometry);
        }

        private void DrawRoundedRectangle(DrawingContext dc, Brush brush, Pen pen, Rect rect, CornerRadius cornerRadius)
        {
            var geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                var minX = rect.Width / 2;
                var minY = rect.Height / 2;

                cornerRadius.TopLeft = Math.Min(Math.Min(cornerRadius.TopLeft, minX), minY);
                cornerRadius.TopRight = Math.Min(Math.Min(cornerRadius.TopRight, minX), minY);
                cornerRadius.BottomRight = Math.Min(Math.Min(cornerRadius.BottomRight, minX), minY);
                cornerRadius.BottomLeft = Math.Min(Math.Min(cornerRadius.BottomLeft, minX), minY);

                ctx.BeginFigure(new Point(rect.Left + cornerRadius.TopLeft, rect.Top), true, true);

                // Top line and top-right corner
                ctx.LineTo(new Point(rect.Right - cornerRadius.TopRight, rect.Top), true, false);
                ctx.ArcTo(new Point(rect.Right, rect.Top + cornerRadius.TopRight), new Size(cornerRadius.TopRight, cornerRadius.TopRight), 0, false, SweepDirection.Clockwise, true, false);

                // Right line and bottom-right corner
                ctx.LineTo(new Point(rect.Right, rect.Bottom - cornerRadius.BottomRight), true, false);
                ctx.ArcTo(new Point(rect.Right - cornerRadius.BottomRight, rect.Bottom), new Size(cornerRadius.BottomRight, cornerRadius.BottomRight), 0, false, SweepDirection.Clockwise, true, false);

                // Bottom line and bottom-left corner
                ctx.LineTo(new Point(rect.Left + cornerRadius.BottomLeft, rect.Bottom), true, false);
                ctx.ArcTo(new Point(rect.Left, rect.Bottom - cornerRadius.BottomLeft), new Size(cornerRadius.BottomLeft, cornerRadius.BottomLeft), 0, false, SweepDirection.Clockwise, true, false);

                // Left line and top-left corner
                ctx.LineTo(new Point(rect.Left, rect.Top + cornerRadius.TopLeft), true, false);
                ctx.ArcTo(new Point(rect.Left + cornerRadius.TopLeft, rect.Top), new Size(cornerRadius.TopLeft, cornerRadius.TopLeft), 0, false, SweepDirection.Clockwise, true, false);
            }
            geometry.Freeze();
            Geometry = geometry;
            dc.DrawGeometry(brush, pen, geometry);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var background = Background;
            var pen = new Pen(BorderBrush, new double[] { BorderThickness.Left, BorderThickness.Top, BorderThickness.Right, BorderThickness.Bottom }.Max());
            if (IsDashed)
            {
                pen.DashStyle = DashStyles.Dash;
            }
            double num = pen.Thickness * 0.5;
            switch (ShapeType)
            {
                case ShapeType.Diamond:
                    DrawPolygon(drawingContext, background, pen, new Point(ActualWidth / 2, num), new Point(ActualWidth - num, ActualHeight / 2), new Point(ActualWidth / 2, ActualHeight - num), new Point(num, ActualHeight / 2));
                    break;
                case ShapeType.Parallelogram:
                    double shear = Shear;
                    DrawPolygon(drawingContext, background, pen, new Point(Math.Min(shear, ActualWidth), num), new Point(ActualWidth - num, num), new Point(Math.Max(0, ActualWidth - shear), ActualHeight - num), new Point(num, ActualHeight - num));
                    break;
                default:
                    Rect rect = new Rect(new Point(num, num), new Point(ActualWidth - num, ActualHeight - num));
                    DrawRoundedRectangle(drawingContext, background, pen, rect, CornerRadius);

                    //double radius = new double[] { CornerRadius.TopLeft, CornerRadius.TopRight, CornerRadius.BottomRight, CornerRadius.BottomLeft }.Max();
                    //drawingContext.DrawRoundedRectangle(background, pen, rectangle, radius, radius); 
                    //base.OnRender(drawingContext);
                    break;
            }
        }
    }

    internal class PathItem : SelectionControl
    {
        internal PathItem()
        {
            DeleteCommand = new RelayCommand(Delete);
        }

        internal PathItem(WorkflowEditor editorParent) : this()
        {
            EditorParent = editorParent;
        }

        internal static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register(nameof(StartPoint), typeof(Point), typeof(PathItem), new PropertyMetadata((sender, e) =>
            {
                if (sender is PathItem pathItem && e.NewValue is Point point)
                {
                    Canvas.SetLeft(pathItem, point.X);
                    Canvas.SetTop(pathItem, point.Y);
                }
            }));

        internal static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register(nameof(EndPoint), typeof(Point), typeof(PathItem));

        internal static readonly DependencyProperty Point0Property =
            DependencyProperty.Register(nameof(Point0), typeof(Point), typeof(PathItem));

        internal static readonly DependencyProperty Point1Property =
            DependencyProperty.Register(nameof(Point1), typeof(Point), typeof(PathItem));

        internal static readonly DependencyProperty Point2Property =
            DependencyProperty.Register(nameof(Point2), typeof(Point), typeof(PathItem));

        internal static readonly DependencyProperty Point3Property =
            DependencyProperty.Register(nameof(Point3), typeof(Point), typeof(PathItem));

        internal static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(nameof(Points), typeof(PointCollection), typeof(PathItem));

        internal static readonly DependencyProperty StartEllipseItemProperty =
            DependencyProperty.Register(nameof(StartEllipseItem), typeof(EllipseItem), typeof(PathItem));

        internal static readonly DependencyProperty EndEllipseItemProperty =
            DependencyProperty.Register(nameof(EndEllipseItem), typeof(EllipseItem), typeof(PathItem));

        internal static readonly DependencyProperty IsCurveProperty =
            DependencyProperty.Register(nameof(IsCurve), typeof(bool), typeof(PathItem), new PropertyMetadata(true, (sender, e) =>
            {
                var pathItem = (PathItem)sender;
                if (pathItem.StartEllipseItem != null && pathItem.EndEllipseItem != null)
                {
                    var startPoint = pathItem.StartEllipseItem.GetPoint(pathItem.EditorParent);
                    var endPoint = pathItem.EndEllipseItem.GetPoint(pathItem.EditorParent);
                    pathItem.UpdateCurveAngle(startPoint, endPoint);
                }
            }));

        internal static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(nameof(StrokeDashArray), typeof(DoubleCollection), typeof(PathItem));

        internal Point StartPoint
        {
            get => (Point)GetValue(StartPointProperty);
            set => SetValue(StartPointProperty, value);
        }

        internal Point EndPoint
        {
            get => (Point)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        internal Point Point0
        {
            get => (Point)GetValue(Point0Property);
            set => SetValue(Point0Property, value);
        }

        internal Point Point1
        {
            get => (Point)GetValue(Point1Property);
            set => SetValue(Point1Property, value);
        }

        internal Point Point2
        {
            get => (Point)GetValue(Point2Property);
            set => SetValue(Point2Property, value);
        }

        internal Point Point3
        {
            get => (Point)GetValue(Point3Property);
            set => SetValue(Point3Property, value);
        }

        /// <summary>
        /// Gets or sets a collection that contains the vertex points of the polygon.
        /// </summary>
        internal PointCollection Points
        {
            get => (PointCollection)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        internal EllipseItem StartEllipseItem
        {
            get => (EllipseItem)GetValue(StartEllipseItemProperty);
            set => SetValue(StartEllipseItemProperty, value);
        }

        internal EllipseItem EndEllipseItem
        {
            get => (EllipseItem)GetValue(EndEllipseItemProperty);
            set => SetValue(EndEllipseItemProperty, value);
        }

        internal bool IsCurve
        {
            get => (bool)GetValue(IsCurveProperty);
            set => SetValue(IsCurveProperty, value);
        }

        internal DoubleCollection StrokeDashArray
        {
            get => (DoubleCollection)GetValue(StrokeDashArrayProperty);
            set => SetValue(StrokeDashArrayProperty, value);
        }

        internal WorkflowEditor EditorParent { get; set; }

        public ICommand DeleteCommand { get; }

        internal void UpdateBezierCurve(Point startPoint, Point endPoint)
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
                Point1 = new Point((startPoint.X + endPoint.X) / 2, endPoint.Y);
                Point2 = new Point((startPoint.X + endPoint.X) / 2, startPoint.Y);
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
            double arrowLength = 14;
            double arrowWidth = 10;

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
            StartEllipseItem.PathItem = null;
            StartEllipseItem = null;
            EndEllipseItem.PathItem = null;
            EndEllipseItem = null;
            ContextMenu = null;
            BindingOperations.ClearAllBindings(this);
            EditorParent.Children.Remove(this);
        }
    }

    internal class EllipseItem : Control
    {
        internal static readonly DependencyProperty DockProperty =
            DependencyProperty.Register(nameof(Dock), typeof(Dock), typeof(EllipseItem));

        internal static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(EllipseItem), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        internal Dock Dock
        {
            get => (Dock)GetValue(DockProperty);
            set => SetValue(DockProperty, value);
        }

        [TypeConverter(typeof(LengthConverter))]
        internal double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        internal WorkflowItem WorkflowParent { get; set; }

        internal PathItem PathItem { get; set; }

        internal Point GetPoint(UIElement parent)
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
                //var w = WorkflowParent.FirstOrDefault(WorkflowParent.NextStep);
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

        private Thumb PART_Thumb;
        private EllipseItem EllipseLeft;
        private EllipseItem EllipseTop;
        private EllipseItem EllipseRight;
        private EllipseItem EllipseBottom;

        internal Point MouseDownControlPoint { get; set; }

        internal Dictionary<Dock, EllipseItem> EllipseItems { get; private set; }

        public bool IsInit { get; private set; }

        public WorkflowEditor EditorParent { get; internal set; }

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
            EllipseItems = new Dictionary<Dock, EllipseItem>();
            PART_Thumb = GetTemplateChild("PART_Thumb") as Thumb;
            PART_Thumb.DragDelta += Thumb_DragDelta;
            PART_Thumb.DragCompleted += PART_Thumb_DragCompleted;

            EllipseItems.Add(EllipseLeft.Dock, EllipseLeft);
            EllipseItems.Add(EllipseTop.Dock, EllipseTop);
            EllipseItems.Add(EllipseRight.Dock, EllipseRight);
            EllipseItems.Add(EllipseBottom.Dock, EllipseBottom);
            foreach (var item in EllipseItems.Values)
            {
                item.WorkflowParent = this;
            }
            IsInit = true;
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
            var cellSize = EditorParent.GridSize;
            Width = Math.Max(cellSize, Width.Adsorb(cellSize));
            Height = Math.Max(cellSize, Height.Adsorb(cellSize));
            Dispatcher.InvokeAsync(() =>
            {
                UpdateCurve();
            }, DispatcherPriority.Render);
        }

        private void OnIsKeyboardFocusWithinChanged(object sender, EventArgs e)
        {
            if (IsKeyboardFocusWithin)
            {
                EditorParent.BeginUpdateSelectedItems();
                IsSelected = true;
                if (!EditorParent.IsCtrlKeyDown)
                {
                    foreach (var item in EditorParent.SelectableElements.Where(x => x != this))
                    {
                        item.IsSelected = false;
                    }
                }
                EditorParent.EndUpdateSelectedItems();
            }
        }

        internal void Delete()
        {
            if (IsInit)
            {
                foreach (var item in EllipseItems.Values)
                {
                    item.PathItem?.Delete();
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
            EditorParent.Children.Remove(this);
        }

        private static void OnWorkflowItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WorkflowItem)d).OnWorkflowItemChanged(e);
        }

        protected virtual void OnWorkflowItemChanged(DependencyPropertyChangedEventArgs e)
        {
            //if (e.NewValue == DataContext)
            //{
            //    SetValue(e.Property, e.OldValue);
            //    throw new InvalidOperationException("Cannot be set as oneself");
            //}
            //if (_updating)
            //{
            //    _updating = false;
            //    return;
            //}
            //if (e.Property == LastStepProperty)
            //{
            //    _updating = true;
            //    FirstOrDefault(LastStep).NextStep = LastStep;

            //}
            //else if (e.Property == NextStepProperty)
            //{
            //    _updating = true;
            //    FirstOrDefault(NextStep).LastStep = NextStep;
            //}
            //else if (e.Property == FromStepProperty)
            //{
            //    _updating = true;
            //    FirstOrDefault(FromStep).JumpStep = FromStep; 
            //}
            //else if (e.Property == JumpStepProperty)
            //{
            //    _updating = true;
            //    FirstOrDefault(JumpStep).FromStep = JumpStep; 
            //}
            UpdateCurve();
        }

        //private bool _updating;

        internal WorkflowItem FirstOrDefault(object item)
        {
            return EditorParent.FirstOrDefault(item);
        }

        public void UpdateCurve()
        {
            if (!IsInit)
            {
                Loaded += WorkflowItem_Loaded;
                return;
            }
            if (LastStep != null)
            {
                UpdateCurve(EllipseItems[Dock.Top].PathItem, FirstOrDefault(LastStep).EllipseItems[Dock.Bottom], EllipseItems[Dock.Top]);
            }
            if (NextStep != null)
            {
                UpdateCurve(EllipseItems[Dock.Bottom].PathItem, EllipseItems[Dock.Bottom], FirstOrDefault(NextStep).EllipseItems[Dock.Top]);
            }
            if (FromStep != null)
            {
                UpdateCurve(EllipseItems[Dock.Left].PathItem, FirstOrDefault(FromStep).EllipseItems[Dock.Right], EllipseItems[Dock.Left]);
            }
            if (JumpStep != null)
            {
                UpdateCurve(EllipseItems[Dock.Right].PathItem, EllipseItems[Dock.Right], FirstOrDefault(JumpStep).EllipseItems[Dock.Left]);
            }
        }

        private void WorkflowItem_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= WorkflowItem_Loaded;
            UpdateCurve();
        }

        private void UpdateCurve(PathItem pathItem, EllipseItem startEllipseItem, EllipseItem endEllipseItem)
        {
            if (!startEllipseItem.IsVisible || !endEllipseItem.IsVisible)
            {
                return;
            }

            if (pathItem == null)
            {
                pathItem = new PathItem(EditorParent);
                if (DataContext is not WorkflowItem)
                {
                    pathItem.Content = DataContext;
                    pathItem.ContentTemplate = EditorParent.PathTemplate;
                    pathItem.ContentTemplateSelector = EditorParent.PathTemplateSelector;
                }
                EditorParent.Children.Add(pathItem);
                startEllipseItem.PathItem = pathItem;
                endEllipseItem.PathItem = pathItem;
                pathItem.StartEllipseItem = startEllipseItem;
                pathItem.EndEllipseItem = endEllipseItem;
            }
            pathItem.UpdateBezierCurve(pathItem.StartEllipseItem.GetPoint(EditorParent), pathItem.EndEllipseItem.GetPoint(EditorParent));
        }
    }
}