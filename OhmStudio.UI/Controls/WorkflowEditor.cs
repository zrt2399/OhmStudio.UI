using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OhmStudio.UI.Commands;
using OhmStudio.UI.Helpers;
using OhmStudio.UI.Utilities;

namespace OhmStudio.UI.Controls
{
    public class WorkflowEditor : ItemsControl
    {
        public static readonly DependencyProperty ViewportZoomProperty;

        public static readonly DependencyProperty MinViewportZoomProperty;

        public static readonly DependencyProperty MaxViewportZoomProperty;

        public static readonly DependencyProperty ViewportLocationProperty;

        public static readonly DependencyProperty ViewportSizeProperty;

        public static readonly DependencyProperty ItemsExtentProperty;

        public static readonly DependencyProperty DecoratorsExtentProperty;

        public static readonly DependencyProperty DisableAutoPanningProperty;

        protected internal static readonly DependencyPropertyKey ViewportTransformPropertyKey;

        public static readonly DependencyProperty ViewportTransformProperty;

        protected static readonly DependencyPropertyKey IsSelectingPropertyKey;

        public static readonly DependencyProperty IsSelectingProperty;

        public static readonly DependencyPropertyKey IsPanningPropertyKey;

        public static readonly DependencyProperty IsPanningProperty;

        protected static readonly DependencyPropertyKey MouseLocationPropertyKey;

        public static readonly DependencyProperty MouseLocationProperty;

        protected static readonly DependencyPropertyKey SelectedAreaPropertyKey;

        public static readonly DependencyProperty SelectedAreaProperty;

        public static readonly DependencyProperty AutoPanSpeedProperty;

        public static readonly DependencyProperty AutoPanEdgeDistanceProperty;

        public static readonly DependencyProperty GridSpacingProperty;

        public static readonly DependencyProperty GridLineBrushProperty;

        public static readonly DependencyProperty SelectedItemsProperty;

        public static readonly DependencyProperty BringIntoViewSpeedProperty = DependencyProperty.Register(nameof(BringIntoViewSpeed), typeof(double), typeof(WorkflowEditor), new FrameworkPropertyMetadata(1000d));

        public static readonly DependencyProperty DisablePanningProperty = DependencyProperty.Register(nameof(DisablePanning), typeof(bool), typeof(WorkflowEditor), new FrameworkPropertyMetadata(false, OnDisablePanningChanged));

        public static readonly DependencyProperty DisableZoomingProperty = DependencyProperty.Register(nameof(DisableZooming), typeof(bool), typeof(WorkflowEditor), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty BringIntoViewMaxDurationProperty = DependencyProperty.Register(nameof(BringIntoViewMaxDuration), typeof(double), typeof(WorkflowEditor), new FrameworkPropertyMetadata(1d));
 
        public static readonly DependencyProperty LineTemplateProperty =
            DependencyProperty.Register(nameof(LineTemplate), typeof(DataTemplate), typeof(WorkflowEditor));

        public static readonly DependencyProperty LineTemplateSelectorProperty =
            DependencyProperty.Register(nameof(LineTemplateSelector), typeof(DataTemplateSelector), typeof(WorkflowEditor));

        public static readonly DependencyProperty LineContainerStyleProperty =
            DependencyProperty.Register(nameof(LineContainerStyle), typeof(Style), typeof(WorkflowEditor));

        public static readonly DependencyProperty LineContainerStyleSelectorProperty =
            DependencyProperty.Register(nameof(LineContainerStyleSelector), typeof(StyleSelector), typeof(WorkflowEditor));

        protected readonly TranslateTransform TranslateTransform = new TranslateTransform();

        protected readonly ScaleTransform ScaleTransform = new ScaleTransform();

        private DispatcherTimer _autoPanningTimer;

        private Point _previousMousePosition;

        private Point _startLocation;

        public uint GridSpacing
        {
            get => (uint)GetValue(GridSpacingProperty);
            set => SetValue(GridSpacingProperty, value);
        }

        public Brush GridLineBrush
        {
            get => (Brush)GetValue(GridLineBrushProperty);
            set => SetValue(GridLineBrushProperty, value);
        }

        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public Transform ViewportTransform => (Transform)GetValue(ViewportTransformProperty);

        public Size ViewportSize
        {
            get => (Size)GetValue(ViewportSizeProperty);
            set => SetValue(ViewportSizeProperty, value);
        }

        public Point ViewportLocation
        {
            get => (Point)GetValue(ViewportLocationProperty);
            set => SetValue(ViewportLocationProperty, value);
        }

        public double ViewportZoom
        {
            get => (double)GetValue(ViewportZoomProperty);
            set => SetValue(ViewportZoomProperty, value);
        }

        public double MinViewportZoom
        {
            get => (double)GetValue(MinViewportZoomProperty);
            set => SetValue(MinViewportZoomProperty, value);
        }

        public double MaxViewportZoom
        {
            get => (double)GetValue(MaxViewportZoomProperty);
            set => SetValue(MaxViewportZoomProperty, value);
        }

        public Rect ItemsExtent
        {
            get => (Rect)GetValue(ItemsExtentProperty);
            set => SetValue(ItemsExtentProperty, value);
        }

        public Rect DecoratorsExtent
        {
            get => (Rect)GetValue(DecoratorsExtentProperty);
            set => SetValue(DecoratorsExtentProperty, value);
        }

        public bool IsSelecting
        {
            get => (bool)GetValue(IsSelectingProperty);
            set => SetValue(IsSelectingPropertyKey, value);
        }

        public Rect SelectedArea
        {
            get => (Rect)GetValue(SelectedAreaProperty);
            set => SetValue(SelectedAreaPropertyKey, value);
        }

        public Point MouseLocation
        {
            get => (Point)GetValue(MouseLocationProperty);
            set => SetValue(MouseLocationPropertyKey, value);
        }

        public bool IsPanning
        {
            get => (bool)GetValue(IsPanningProperty);
            set => SetValue(IsPanningPropertyKey, value);
        }

        public bool DisableAutoPanning
        {
            get => (bool)GetValue(DisableAutoPanningProperty);
            set => SetValue(DisableAutoPanningProperty, value);
        }

        public double AutoPanSpeed
        {
            get => (double)GetValue(AutoPanSpeedProperty);
            set => SetValue(AutoPanSpeedProperty, value);
        }

        public double AutoPanEdgeDistance
        {
            get => (double)GetValue(AutoPanEdgeDistanceProperty);
            set => SetValue(AutoPanEdgeDistanceProperty, value);
        }

        public double BringIntoViewSpeed
        {
            get => (double)GetValue(BringIntoViewSpeedProperty);
            set => SetValue(BringIntoViewSpeedProperty, value);
        }

        public bool DisablePanning
        {
            get => (bool)GetValue(DisablePanningProperty);
            set => SetValue(DisablePanningProperty, value);
        }

        public bool DisableZooming
        {
            get => (bool)GetValue(DisableZoomingProperty);
            set => SetValue(DisableZoomingProperty, value);
        }

        public double BringIntoViewMaxDuration
        {
            get => (double)GetValue(BringIntoViewMaxDurationProperty);
            set => SetValue(BringIntoViewMaxDurationProperty, value);
        }

        public DataTemplate LineTemplate
        {
            get => (DataTemplate)GetValue(LineTemplateProperty);
            set => SetValue(LineTemplateProperty, value);
        }

        public DataTemplateSelector LineTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(LineTemplateSelectorProperty);
            set => SetValue(LineTemplateSelectorProperty, value);
        }

        public Style LineContainerStyle
        {
            get => (Style)GetValue(LineContainerStyleProperty);
            set => SetValue(LineContainerStyleProperty, value);
        }

        public StyleSelector LineContainerStyleSelector
        {
            get => (StyleSelector)GetValue(LineContainerStyleSelectorProperty);
            set => SetValue(LineContainerStyleSelectorProperty, value);
        }

        public ICommand SelectAllCommand { get; }

        public ICommand UnselectAllCommand { get; }

        public ICommand InvertSelectCommand { get; }

        protected internal WorkflowCanvas ItemsHost { get; private set; }

        public double AutoPanningTickRate { get; set; } = 1;

        static WorkflowEditor()
        {
            ViewportZoomProperty = DependencyProperty.Register(nameof(ViewportZoom), typeof(double), typeof(WorkflowEditor), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnViewportZoomChanged, ConstrainViewportZoomToRange));
            MinViewportZoomProperty = DependencyProperty.Register(nameof(MinViewportZoom), typeof(double), typeof(WorkflowEditor), new FrameworkPropertyMetadata(0.1, OnMinViewportZoomChanged, CoerceMinViewportZoom));
            MaxViewportZoomProperty = DependencyProperty.Register(nameof(MaxViewportZoom), typeof(double), typeof(WorkflowEditor), new FrameworkPropertyMetadata(2.0, OnMaxViewportZoomChanged, CoerceMaxViewportZoom));
            ViewportLocationProperty = DependencyProperty.Register(nameof(ViewportLocation), typeof(Point), typeof(WorkflowEditor), new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnViewportLocationChanged));
            ViewportSizeProperty = DependencyProperty.Register(nameof(ViewportSize), typeof(Size), typeof(WorkflowEditor), new FrameworkPropertyMetadata(default(Size)));
            ItemsExtentProperty = DependencyProperty.Register(nameof(ItemsExtent), typeof(Rect), typeof(WorkflowEditor), new FrameworkPropertyMetadata(default(Rect)));
            DecoratorsExtentProperty = DependencyProperty.Register(nameof(DecoratorsExtent), typeof(Rect), typeof(WorkflowEditor), new FrameworkPropertyMetadata(default(Rect)));
            DisableAutoPanningProperty = DependencyProperty.Register(nameof(DisableAutoPanning), typeof(bool), typeof(WorkflowEditor), new FrameworkPropertyMetadata(false, OnDisableAutoPanningChanged));
            ViewportTransformPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ViewportTransform), typeof(Transform), typeof(WorkflowEditor), new FrameworkPropertyMetadata(new TransformGroup()));
            ViewportTransformProperty = ViewportTransformPropertyKey.DependencyProperty;
            IsSelectingPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsSelecting), typeof(bool), typeof(WorkflowEditor), new FrameworkPropertyMetadata(false));
            IsSelectingProperty = IsSelectingPropertyKey.DependencyProperty;
            IsPanningPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsPanning), typeof(bool), typeof(WorkflowEditor), new FrameworkPropertyMetadata(false));
            IsPanningProperty = IsPanningPropertyKey.DependencyProperty;
            MouseLocationPropertyKey = DependencyProperty.RegisterReadOnly(nameof(MouseLocation), typeof(Point), typeof(WorkflowEditor), new FrameworkPropertyMetadata(default(Point)));
            MouseLocationProperty = MouseLocationPropertyKey.DependencyProperty;
            SelectedAreaPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedArea), typeof(Rect), typeof(WorkflowEditor), new FrameworkPropertyMetadata(default(Rect)));
            SelectedAreaProperty = SelectedAreaPropertyKey.DependencyProperty;
            AutoPanSpeedProperty = DependencyProperty.Register(nameof(AutoPanSpeed), typeof(double), typeof(WorkflowEditor), new FrameworkPropertyMetadata(15.0));
            AutoPanEdgeDistanceProperty = DependencyProperty.Register(nameof(AutoPanEdgeDistance), typeof(double), typeof(WorkflowEditor), new FrameworkPropertyMetadata(1.0));
            GridSpacingProperty = DependencyProperty.Register(nameof(GridSpacing), typeof(uint), typeof(WorkflowEditor), new FrameworkPropertyMetadata(20u));
            GridLineBrushProperty = DependencyProperty.Register(nameof(GridLineBrush), typeof(Brush), typeof(WorkflowEditor), new FrameworkPropertyMetadata(Brushes.LightGray));
            SelectedItemsProperty = DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(WorkflowEditor), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WorkflowEditor), new FrameworkPropertyMetadata(typeof(WorkflowEditor)));
        }

        public WorkflowEditor()
        {
            var transform = new TransformGroup();
            transform.Children.Add(ScaleTransform);
            transform.Children.Add(TranslateTransform);
            SetValue(ViewportTransformPropertyKey, transform);
            SelectAllCommand = new RelayCommand(SelectAll);
            UnselectAllCommand = new RelayCommand(UnselectAll);
            InvertSelectCommand = new RelayCommand(InvertSelect);
        }

        public void SelectAll()
        {
            SelectElement(true);
        }

        public void UnselectAll()
        {
            SelectElement(false);
        }

        public void InvertSelect()
        {
            SelectElement(true, true);
        }

        private void SelectElement(bool value, bool invert = false)
        {
            if (ItemsHost == null)
            {
                return;
            }
            try
            {
                ItemsHost.BeginUpdateSelectedItems();
                foreach (WorkflowItem item in ItemsHost.WorkflowItems)
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
            }
            finally
            {
                ItemsHost.EndUpdateSelectedItems();
            }
        }

        public Point GetDragPosition(DragEventArgs dragEventArgs)
        {
            return dragEventArgs.GetPosition(ItemsHost);
        }

        private static void OnDisableAutoPanningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WorkflowEditor)d).OnDisableAutoPanningChanged((bool)e.NewValue);
        }

        private static void OnViewportLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WorkflowEditor editor = (WorkflowEditor)d;
            Point translate = (Point)e.NewValue;
            editor.TranslateTransform.X = -translate.X * editor.ViewportZoom;
            editor.TranslateTransform.Y = -translate.Y * editor.ViewportZoom;
        }

        private static void OnViewportZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WorkflowEditor editor = (WorkflowEditor)d;
            double zoom = (double)e.NewValue;
            editor.ScaleTransform.ScaleX = zoom;
            editor.ScaleTransform.ScaleY = zoom;
            editor.ViewportSize = new Size(editor.ActualWidth / zoom, editor.ActualHeight / zoom);
        }

        private static void OnMinViewportZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WorkflowEditor zoom = (WorkflowEditor)d;
            zoom.CoerceValue(MaxViewportZoomProperty);
            zoom.CoerceValue(ViewportZoomProperty);
        }

        private static object CoerceMinViewportZoom(DependencyObject d, object value)
        {
            return ((double)value > 0.1) ? value : 0.1;
        }

        private static void OnMaxViewportZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WorkflowEditor zoom = (WorkflowEditor)d;
            zoom.CoerceValue(ViewportZoomProperty);
        }

        private static object CoerceMaxViewportZoom(DependencyObject d, object value)
        {
            WorkflowEditor editor = (WorkflowEditor)d;
            double min = editor.MinViewportZoom;
            return ((double)value < min) ? min : value;
        }

        private static object ConstrainViewportZoomToRange(DependencyObject d, object value)
        {
            WorkflowEditor editor = (WorkflowEditor)d;
            double num = (double)value;
            double minimum = editor.MinViewportZoom;
            if (num < minimum)
            {
                return minimum;
            }
            double maximum = editor.MaxViewportZoom;
            return (num > maximum) ? maximum : value;
        }

        private static void OnDisablePanningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (WorkflowEditor)d;
            editor.OnDisableAutoPanningChanged(editor.DisableAutoPanning || editor.DisablePanning);
        }

        public void BringIntoView(Point point, bool animated = true, Action onFinish = null)
        {
            Point newLocation = (Point)((Vector)point - (Vector)ViewportSize / 2);

            if (animated && newLocation != ViewportLocation)
            {
                IsPanning = true;
                DisablePanning = true;
                DisableZooming = true;

                double distance = (newLocation - ViewportLocation).Length;
                double duration = distance / (BringIntoViewSpeed + (distance / 10)) * ViewportZoom;
                duration = Math.Max(0.1, Math.Min(duration, BringIntoViewMaxDuration));

                this.StartAnimation(ViewportLocationProperty, newLocation, duration, (s, e) =>
                {
                    IsPanning = false;
                    DisablePanning = false;
                    DisableZooming = false;

                    onFinish?.Invoke();
                });
            }
            else
            {
                ViewportLocation = newLocation;
                onFinish?.Invoke();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemsHost = (GetTemplateChild("PART_ItemsHost") as WorkflowCanvas) ?? throw new InvalidOperationException("PART_ItemsHost is missing or is not of type Panel.");
            OnDisableAutoPanningChanged(DisableAutoPanning);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new WorkflowItem
            {
                CanvasParent = ItemsHost,
                RenderTransform = new TranslateTransform()
            };
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is WorkflowItem;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            ItemsHost.HandleMouseDown(e);
            MouseLocation = e.GetPosition(ItemsHost);
            Focus();
            _previousMousePosition = e.GetPosition(this);
            _startLocation = MouseLocation;
            if (ItemsHost.CanvasStatus == CanvasStatus.None)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    IsSelecting = true;
                    IsPanning = false;
                    SelectedArea = default;
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    IsPanning = true;
                    IsSelecting = false;
                }
            }
            if (!IsMouseCaptureWithin)
            {
                CaptureMouse();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            MouseLocation = e.GetPosition(ItemsHost);
            if (IsPanning)
            {
                Point currentMousePosition = e.GetPosition(this);
                ViewportLocation -= (currentMousePosition - _previousMousePosition) / ViewportZoom;
                _previousMousePosition = currentMousePosition;
            }
            else if (IsSelecting)
            {
                Point endLocation = MouseLocation;
                double left = Math.Min(endLocation.X, _startLocation.X);
                double top = Math.Min(endLocation.Y, _startLocation.Y);
                double width = Math.Abs(endLocation.X - _startLocation.X);
                double height = Math.Abs(endLocation.Y - _startLocation.Y);
                SelectedArea = new Rect(left, top, width, height);
                if (width >= 1.0 || height >= 1.0)
                {
                    RectangleGeometry rectangleGeometry = new RectangleGeometry(SelectedArea);
                    ItemsHost.BeginUpdateSelectedItems();
                    foreach (var item in ItemsHost.WorkflowItems)
                    {
                        item.IsSelected = item.CheckOverlap(rectangleGeometry);
                    }
                    ItemsHost.EndUpdateSelectedItems();
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            MouseLocation = e.GetPosition(ItemsHost);
            if (IsMouseCaptured && e.RightButton == MouseButtonState.Released && e.LeftButton == MouseButtonState.Released && e.MiddleButton == MouseButtonState.Released)
            {
                ReleaseMouseCapture();
            }
            if (IsSelecting)
            {
                e.Handled = true;
            }
            IsPanning = false;
            IsSelecting = false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (KeyboardHelper.IsCtrlKeyDown && Keyboard.IsKeyDown(Key.A))
            {
                SelectAll();
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            double zoom = Math.Pow(2.0, e.Delta / 3.0 / 120.0);
            ZoomAtPosition(zoom, e.GetPosition(ItemsHost));
            if (e.Source is WorkflowEditor)
            {
                e.Handled = true;
            }
        }

        private void HandleAutoPanning(object sender, EventArgs e)
        {
            if (!IsPanning && IsMouseCaptureWithin)
            {
                Point mousePosition = Mouse.GetPosition(this);
                double edgeDistance = AutoPanEdgeDistance;
                double autoPanSpeed = Math.Min(AutoPanSpeed, AutoPanSpeed * AutoPanningTickRate) / (ViewportZoom * 2.0);
                double x = ViewportLocation.X;
                double y = ViewportLocation.Y;
                if (mousePosition.X <= edgeDistance)
                {
                    x -= autoPanSpeed;
                }
                else if (mousePosition.X >= ActualWidth - edgeDistance)
                {
                    x += autoPanSpeed;
                }
                if (mousePosition.Y <= edgeDistance)
                {
                    y -= autoPanSpeed;
                }
                else if (mousePosition.Y >= ActualHeight - edgeDistance)
                {
                    y += autoPanSpeed;
                }
                ViewportLocation = new Point(x, y);
                MouseLocation = Mouse.GetPosition(ItemsHost);
                OnMouseMove(new MouseEventArgs(Mouse.PrimaryDevice, 0));
            }
        }

        protected virtual void OnDisableAutoPanningChanged(bool shouldDisable)
        {
            if (shouldDisable)
            {
                _autoPanningTimer?.Stop();
                return;
            }
            if (_autoPanningTimer == null)
            {
                _autoPanningTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(AutoPanningTickRate), DispatcherPriority.Render, HandleAutoPanning, Dispatcher);
                return;
            }
            _autoPanningTimer.Interval = TimeSpan.FromMilliseconds(AutoPanningTickRate);
            _autoPanningTimer.Start();
        }

        public void ZoomAtPosition(double zoom, Point location)
        {
            if (!DisableZooming)
            {
                double prevZoom = ViewportZoom;
                ViewportZoom *= zoom;
                if (Math.Abs(prevZoom - ViewportZoom) > 0.001)
                {
                    zoom = ViewportZoom / prevZoom;
                    Vector position = (Vector)location;
                    Vector dist = position - (Vector)ViewportLocation;
                    Vector zoomedDist = dist * zoom;
                    Vector diff = zoomedDist - dist;
                    ViewportLocation += diff / zoom;
                }
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            double zoom = ViewportZoom;
            ViewportSize = new Size(ActualWidth / zoom, ActualHeight / zoom);
        }
    }
}