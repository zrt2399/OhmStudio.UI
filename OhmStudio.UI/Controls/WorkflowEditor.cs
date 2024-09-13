using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OhmStudio.UI.Commands;

namespace OhmStudio.UI.Controls
{
    public class WorkflowEditor : ItemsControl
    {
        public static readonly DependencyProperty ViewportZoomProperty = DependencyProperty.Register(nameof(ViewportZoom), typeof(double), typeof(WorkflowEditor), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnViewportZoomChanged, ConstrainViewportZoomToRange));
        public static readonly DependencyProperty MinViewportZoomProperty = DependencyProperty.Register(nameof(MinViewportZoom), typeof(double), typeof(WorkflowEditor), new FrameworkPropertyMetadata(0.1d, OnMinViewportZoomChanged, CoerceMinViewportZoom));
        public static readonly DependencyProperty MaxViewportZoomProperty = DependencyProperty.Register(nameof(MaxViewportZoom), typeof(double), typeof(WorkflowEditor), new FrameworkPropertyMetadata(2d, OnMaxViewportZoomChanged, CoerceMaxViewportZoom));
        public static readonly DependencyProperty ViewportLocationProperty = DependencyProperty.Register(nameof(ViewportLocation), typeof(Point), typeof(WorkflowEditor), new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnViewportLocationChanged));
        public static readonly DependencyProperty ViewportSizeProperty = DependencyProperty.Register(nameof(ViewportSize), typeof(Size), typeof(WorkflowEditor), new FrameworkPropertyMetadata(default(Size)));
        public static readonly DependencyProperty ItemsExtentProperty = DependencyProperty.Register(nameof(ItemsExtent), typeof(Rect), typeof(WorkflowEditor), new FrameworkPropertyMetadata(default(Rect)));
        public static readonly DependencyProperty DecoratorsExtentProperty = DependencyProperty.Register(nameof(DecoratorsExtent), typeof(Rect), typeof(WorkflowEditor), new FrameworkPropertyMetadata(default(Rect)));
        protected internal static readonly DependencyPropertyKey ViewportTransformPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ViewportTransform), typeof(Transform), typeof(WorkflowEditor), new FrameworkPropertyMetadata(new TransformGroup()));
        public static readonly DependencyProperty ViewportTransformProperty = ViewportTransformPropertyKey.DependencyProperty;

        protected static readonly DependencyPropertyKey IsSelectingPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsSelecting), typeof(bool), typeof(WorkflowEditor), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty IsSelectingProperty = IsSelectingPropertyKey.DependencyProperty;

        public static readonly DependencyPropertyKey IsPanningPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsPanning), typeof(bool), typeof(WorkflowEditor), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty IsPanningProperty = IsPanningPropertyKey.DependencyProperty;

        protected static readonly DependencyPropertyKey MouseLocationPropertyKey = DependencyProperty.RegisterReadOnly(nameof(MouseLocation), typeof(Point), typeof(WorkflowEditor), new FrameworkPropertyMetadata(default(Point)));
        public static readonly DependencyProperty MouseLocationProperty = MouseLocationPropertyKey.DependencyProperty;

        protected static readonly DependencyPropertyKey SelectedAreaPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedArea), typeof(Rect), typeof(WorkflowEditor), new FrameworkPropertyMetadata(default(Rect)));
        public static readonly DependencyProperty SelectedAreaProperty = SelectedAreaPropertyKey.DependencyProperty;

        public static readonly DependencyProperty AutoPanSpeedProperty = DependencyProperty.Register(nameof(AutoPanSpeed), typeof(double), typeof(WorkflowEditor), new FrameworkPropertyMetadata(10d));
        public static readonly DependencyProperty AutoPanEdgeDistanceProperty = DependencyProperty.Register(nameof(AutoPanEdgeDistance), typeof(double), typeof(WorkflowEditor), new FrameworkPropertyMetadata(1d));

        public static readonly DependencyProperty GridSpacingProperty =
            DependencyProperty.Register(nameof(GridSpacing), typeof(double), typeof(WorkflowEditor),
                new FrameworkPropertyMetadata(20d));

        public static readonly DependencyProperty GridLineBrushProperty =
            DependencyProperty.Register(nameof(GridLineBrush), typeof(Brush), typeof(WorkflowEditor),
                new FrameworkPropertyMetadata(Brushes.LightGray));

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(WorkflowEditor), new FrameworkPropertyMetadata(default(IList), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double GridSpacing
        {
            get => (double)GetValue(GridSpacingProperty);
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

        /// <summary>
        /// Gets the transform used to offset the viewport.
        /// </summary>
        protected readonly TranslateTransform TranslateTransform = new TranslateTransform();

        /// <summary>
        /// Gets the transform used to zoom on the viewport.
        /// </summary>
        protected readonly ScaleTransform ScaleTransform = new ScaleTransform();

        static WorkflowEditor()
        {
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

        /// <summary>
        /// Gets the transform that is applied to all child controls.
        /// </summary>
        public Transform ViewportTransform => (Transform)GetValue(ViewportTransformProperty);

        public Size ViewportSize
        {
            get => (Size)GetValue(ViewportSizeProperty);
            set => SetValue(ViewportSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the viewport's top-left coordinates in graph space coordinates.
        /// </summary>
        public Point ViewportLocation
        {
            get => (Point)GetValue(ViewportLocationProperty);
            set => SetValue(ViewportLocationProperty, value);
        }


        /// <summary>
        /// Gets or sets the zoom factor of the viewport.
        /// </summary>
        public double ViewportZoom
        {
            get => (double)GetValue(ViewportZoomProperty);
            set => SetValue(ViewportZoomProperty, value);
        }

        /// <summary>
        /// Gets or sets the minimum zoom factor of the viewport
        /// </summary>
        public double MinViewportZoom
        {
            get => (double)GetValue(MinViewportZoomProperty);
            set => SetValue(MinViewportZoomProperty, value);
        }

        /// <summary>
        /// Gets or sets the maximum zoom factor of the viewport
        /// </summary>
        public double MaxViewportZoom
        {
            get => (double)GetValue(MaxViewportZoomProperty);
            set => SetValue(MaxViewportZoomProperty, value);
        }

        /// <summary>
        /// The area covered by the <see cref="ItemContainer"/>s.
        /// </summary>
        public Rect ItemsExtent
        {
            get => (Rect)GetValue(ItemsExtentProperty);
            set => SetValue(ItemsExtentProperty, value);
        }

        /// <summary>
        /// The area covered by the <see cref="ItemContainer"/>s.
        /// </summary>
        public Rect DecoratorsExtent
        {
            get => (Rect)GetValue(DecoratorsExtentProperty);
            set => SetValue(DecoratorsExtentProperty, value);
        }

        public bool IsSelecting
        {
            get => (bool)GetValue(IsSelectingProperty);
            internal set => SetValue(IsSelectingPropertyKey, value);
        }

        public Rect SelectedArea
        {
            get => (Rect)GetValue(SelectedAreaProperty);
            internal set => SetValue(SelectedAreaPropertyKey, value);
        }

        /// <summary>
        /// Gets the current mouse location in graph space coordinates (relative to the <see cref="ItemsHost" />).
        /// </summary>
        public Point MouseLocation
        {
            get => (Point)GetValue(MouseLocationProperty);
            protected set => SetValue(MouseLocationPropertyKey, value);
        }

        public bool IsPanning
        {
            get => (bool)GetValue(IsPanningProperty);
            protected internal set => SetValue(IsPanningPropertyKey, value);
        }
        /// <summary>
        /// Gets or sets the speed used when auto-panning scaled by <see cref="AutoPanningTickRate"/>
        /// </summary>
        public double AutoPanSpeed
        {
            get => (double)GetValue(AutoPanSpeedProperty);
            set => SetValue(AutoPanSpeedProperty, value);
        }

        /// <summary>
        /// Gets or sets the maximum distance in pixels from the edge of the editor that will trigger auto-panning.
        /// </summary>
        public double AutoPanEdgeDistance
        {
            get => (double)GetValue(AutoPanEdgeDistanceProperty);
            set => SetValue(AutoPanEdgeDistanceProperty, value);
        }

        public ICommand SelectAllCommand { get; }

        public ICommand UnselectAllCommand { get; }

        public ICommand InvertSelectCommand { get; }

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
            try
            {
                ItemsHost.BeginUpdateSelectedItems();
                foreach (var item in ItemsHost.WorkflowItems)
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

        private static void OnViewportLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (WorkflowEditor)d;
            var translate = (Point)e.NewValue;

            editor.TranslateTransform.X = -translate.X * editor.ViewportZoom;
            editor.TranslateTransform.Y = -translate.Y * editor.ViewportZoom;

            //editor.OnViewportUpdated();
        }

        private static void OnViewportZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (WorkflowEditor)d;
            double zoom = (double)e.NewValue;

            editor.ScaleTransform.ScaleX = zoom;
            editor.ScaleTransform.ScaleY = zoom;

            editor.ViewportSize = new Size(editor.ActualWidth / zoom, editor.ActualHeight / zoom);

            //editor.ApplyRenderingOptimizations();
            //editor.OnViewportUpdated();
        }

        private static void OnMinViewportZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zoom = (WorkflowEditor)d;
            zoom.CoerceValue(MaxViewportZoomProperty);
            zoom.CoerceValue(ViewportZoomProperty);
        }

        private static object CoerceMinViewportZoom(DependencyObject d, object value)
            => (double)value > 0.1d ? value : 0.1d;

        private static void OnMaxViewportZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zoom = (WorkflowEditor)d;
            zoom.CoerceValue(ViewportZoomProperty);
        }

        private static object CoerceMaxViewportZoom(DependencyObject d, object value)
        {
            var editor = (WorkflowEditor)d;
            double min = editor.MinViewportZoom;

            return (double)value < min ? min : value;
        }

        private static object ConstrainViewportZoomToRange(DependencyObject d, object value)
        {
            var editor = (WorkflowEditor)d;

            var num = (double)value;
            double minimum = editor.MinViewportZoom;
            if (num < minimum)
            {
                return minimum;
            }

            double maximum = editor.MaxViewportZoom;
            return num > maximum ? maximum : value;
        }

        protected internal WorkflowCanvas ItemsHost { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ItemsHost = GetTemplateChild("PART_ItemsHost") as WorkflowCanvas ?? throw new InvalidOperationException("PART_ItemsHost is missing or is not of type Panel.");

            OnDisableAutoPanningChanged(false);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new WorkflowItem()
            {
                EditorParent = ItemsHost,
                RenderTransform = new TranslateTransform()
            };
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is WorkflowItem;
        }

        private DispatcherTimer _autoPanningTimer;
        public static double AutoPanningTickRate { get; set; } = 1;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            ItemsHost.HandleLMouseDown(e);
            MouseLocation = e.GetPosition(ItemsHost);
            Focus();

            _previousMousePosition = e.GetPosition(this);

            _startLocation = MouseLocation;

            if (ItemsHost.EditorStatus == EditorStatus.None)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    IsSelecting = true;
                    IsPanning = false;
                    SelectedArea = new Rect();
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

        private Point _previousMousePosition;

        private Point _startLocation;

        /// <inheritdoc />
        protected override void OnMouseMove(MouseEventArgs e)
        {
            MouseLocation = Mouse.GetPosition(ItemsHost);

            if (IsPanning)
            {
                var currentMousePosition = Mouse.GetPosition(this);
                ViewportLocation -= (currentMousePosition - _previousMousePosition) / ViewportZoom;
                _previousMousePosition = currentMousePosition;
                //Debug.WriteLine(ViewportLocation);
            }
            if (IsSelecting)
            {
                var endLocation = MouseLocation;
                double left = endLocation.X < _startLocation.X ? endLocation.X : _startLocation.X;
                double top = endLocation.Y < _startLocation.Y ? endLocation.Y : _startLocation.Y;
                double width = Math.Abs(endLocation.X - _startLocation.X);
                double height = Math.Abs(endLocation.Y - _startLocation.Y);

                SelectedArea = new Rect(left, top, width, height);

                if (width >= 1 || height >= 1)
                {
                    RectangleGeometry rectangleGeometry = new RectangleGeometry(SelectedArea);
                    ItemsHost.BeginUpdateSelectedItems();
                    foreach (var item in ItemsHost.WorkflowItems)
                    {
                        item.IsSelected = CheckOverlap(rectangleGeometry, item);
                        //item.IsSelected = selectedArea.IntersectsWith(new Rect(GetLeft(item), GetTop(item), item.ActualWidth, item.ActualHeight));
                    }
                    ItemsHost.EndUpdateSelectedItems();
                }
            }
        }

        private bool CheckOverlap(RectangleGeometry rectangleGeometry, WorkflowItem workflowItem)
        {
            GeneralTransform transform = workflowItem.TransformToVisual(ItemsHost);
            Geometry geometry = workflowItem.Geometry?.Clone();
            if (geometry != null)
            {
                geometry.Transform = (Transform)transform;
            }
            return rectangleGeometry.FillContainsWithDetail(geometry) != IntersectionDetail.Empty;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            MouseLocation = e.GetPosition(ItemsHost);

            // Release the mouse capture if all the mouse buttons are released
            if (IsMouseCaptured && e.RightButton == MouseButtonState.Released && e.LeftButton == MouseButtonState.Released && e.MiddleButton == MouseButtonState.Released)
            {
                ReleaseMouseCapture();
            }

            // Disable context menu if selecting
            if (IsSelecting)
            {
                e.Handled = true;
            }
            IsPanning = false;
            IsSelecting = false;
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        { }

        internal bool IsCtrlKeyDown => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (IsCtrlKeyDown && Keyboard.IsKeyDown(Key.A))
            {
                SelectAll();
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            double zoom = Math.Pow(2.0, e.Delta / 3.0 / Mouse.MouseWheelDeltaForOneLine);
            ZoomAtPosition(zoom, e.GetPosition(ItemsHost));

            // Handle it for nested editors
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
                double autoPanSpeed = Math.Min(AutoPanSpeed, AutoPanSpeed * AutoPanningTickRate) / (ViewportZoom * 2);
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
            }
            else if (_autoPanningTimer == null)
            {
                _autoPanningTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(AutoPanningTickRate),
                    DispatcherPriority.Render, HandleAutoPanning, Dispatcher);
            }
            else
            {
                _autoPanningTimer.Interval = TimeSpan.FromMilliseconds(AutoPanningTickRate);
                _autoPanningTimer.Start();
            }
        }

        public void ZoomAtPosition(double zoom, Point location)
        {
            //if (!DisableZooming)
            //{
            double prevZoom = ViewportZoom;
            ViewportZoom *= zoom;

            if (Math.Abs(prevZoom - ViewportZoom) > 0.001)
            {
                // get the actual zoom value because Zoom might have been coerced
                zoom = ViewportZoom / prevZoom;
                Vector position = (Vector)location;

                var dist = position - (Vector)ViewportLocation;
                var zoomedDist = dist * zoom;
                var diff = zoomedDist - dist;
                ViewportLocation += diff / zoom;
            }
            //}
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            double zoom = ViewportZoom;
            ViewportSize = new Size(ActualWidth / zoom, ActualHeight / zoom);

            //OnViewportUpdated();
        }
    }
}