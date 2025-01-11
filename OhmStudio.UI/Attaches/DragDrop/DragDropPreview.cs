using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using OhmStudio.UI.Attaches.DragDrop.Utilities;

namespace OhmStudio.UI.Attaches.DragDrop
{
    internal class DragDropPreview : Popup
    {
        protected DragDropPreview(UIElement rootElement, UIElement previewElement, Point translation, Point anchorPoint)
        {
            PlacementTarget = rootElement;
            Placement = PlacementMode.Relative;
            AllowsTransparency = true;
            Focusable = false;
            PopupAnimation = PopupAnimation.Fade;
            StaysOpen = true;
            HorizontalOffset = -9999;
            VerticalOffset = -9999;
            IsHitTestVisible = false;
            AllowDrop = false;

            Child = previewElement;
            Translation = translation;
            AnchorPoint = anchorPoint;
        }

        public DragDropPreview(UIElement rootElement, IDragInfo dragInfo, UIElement visualTarget, UIElement sender)
        {
            PlacementTarget = rootElement;
            Placement = PlacementMode.Relative;
            AllowsTransparency = true;
            Focusable = false;
            PopupAnimation = PopupAnimation.Fade;
            StaysOpen = true;
            HorizontalOffset = -9999;
            VerticalOffset = -9999;
            IsHitTestVisible = false;
            AllowDrop = false;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;

            _dragInfo = dragInfo;
            Child = CreatePreviewPresenter(dragInfo, visualTarget, sender);
            Translation = DragDropAttach.GetDragAdornerTranslation(dragInfo.VisualSource);
            AnchorPoint = DragDropAttach.GetDragMouseAnchorPoint(dragInfo.VisualSource);
        }

        private IDragInfo _dragInfo = null;
        private UIElement _visualTarget = null;
        private Rect _visualSourceItemBounds = Rect.Empty;

        public Point Translation { get; }

        public Point AnchorPoint { get; }

        public bool UseDefaultDragAdorner { get; private set; }

        /// <summary>Identifies the <see cref="ItemTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty ItemTemplateProperty
            = DependencyProperty.Register(nameof(ItemTemplate),
                                          typeof(DataTemplate),
                                          typeof(DragDropPreview),
                                          new FrameworkPropertyMetadata((DataTemplate)null));

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="ItemTemplateSelector"/> dependency property.</summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty
            = DependencyProperty.Register(nameof(ItemTemplateSelector),
                                          typeof(DataTemplateSelector),
                                          typeof(DragDropPreview),
                                          new FrameworkPropertyMetadata((DataTemplateSelector)null));

        public DataTemplateSelector ItemTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty);
            set => SetValue(ItemTemplateSelectorProperty, value);
        }

        /// <summary>Identifies the <see cref="ItemsPanel"/> dependency property.</summary>
        public static readonly DependencyProperty ItemsPanelProperty
            = DependencyProperty.Register(nameof(ItemsPanel),
                                          typeof(ItemsPanelTemplate),
                                          typeof(DragDropPreview),
                                          new PropertyMetadata(default(ItemsPanelTemplate)));

        public ItemsPanelTemplate ItemsPanel
        {
            get => (ItemsPanelTemplate)GetValue(ItemsPanelProperty);
            set => SetValue(ItemsPanelProperty, value);
        }

        public void Move(Point point)
        {
            var translation = Translation;
            var translationX = point.X + translation.X;
            var translationY = point.Y + translation.Y;

            if (Child is not null)
            {
                var renderSize = Child.RenderSize;

                var renderSizeWidth = renderSize.Width;
                var renderSizeHeight = renderSize.Height;

                // Only set if the template contains a Canvas.
                if (!_visualSourceItemBounds.IsEmpty)
                {
                    renderSizeWidth = Math.Min(renderSizeWidth, _visualSourceItemBounds.Width);
                    renderSizeHeight = Math.Min(renderSizeHeight, _visualSourceItemBounds.Height);
                }

                if (renderSizeWidth > 0 && renderSizeHeight > 0)
                {
                    var offsetX = renderSizeWidth * -AnchorPoint.X;
                    var offsetY = renderSizeHeight * -AnchorPoint.Y;

                    translationX += offsetX;
                    translationY += offsetY;
                }
            }

            SetCurrentValue(HorizontalOffsetProperty, translationX);
            SetCurrentValue(VerticalOffsetProperty, translationY);
        }

        protected override void OnOpened(EventArgs e)

        {
            base.OnOpened(e);

            if (PresentationSource.FromVisual(Child) is HwndSource hwndSource)
            {
                var windowHandle = hwndSource.Handle;
                var wsEx = WindowStyleHelper.GetWindowStyleEx(windowHandle);

                wsEx |= WindowStyleHelper.WS_EX.NOACTIVATE; // We don't want our this window to be activated
                wsEx |= WindowStyleHelper.WS_EX.TRANSPARENT;

                WindowStyleHelper.SetWindowStyleEx(windowHandle, wsEx);
            }
        }

        private static bool IsMultiSelection(IDragInfo dragInfo)
        {
            return dragInfo?.Data is IEnumerable and not string;
        }

        public static bool HasDragDropPreview(IDragInfo dragInfo, UIElement visualTarget, UIElement sender)
        {
            var visualSource = dragInfo?.VisualSource;
            if (visualSource is null)
            {
                return false;
            }

            var isMultiSelection = IsMultiSelection(dragInfo);

            // Check for target template or template selector
            DataTemplate template = isMultiSelection
                ? DragDropAttach.TryGetDropAdornerMultiItemTemplate(visualTarget, sender) ?? DragDropAttach.TryGetDropAdornerTemplate(visualTarget, sender)
                : DragDropAttach.TryGetDropAdornerTemplate(visualTarget, sender);
            DataTemplateSelector templateSelector = isMultiSelection
                ? DragDropAttach.TryGetDropAdornerMultiItemTemplateSelector(visualTarget, sender) ?? DragDropAttach.TryGetDropAdornerTemplateSelector(visualTarget, sender)
                : DragDropAttach.TryGetDropAdornerTemplateSelector(visualTarget, sender);

            if (template is not null)
            {
                templateSelector = null;
            }

            // Check for source template or template selector if there is no target one
            if (template is null && templateSelector is null)
            {
                var sourceContext = DragDropAttach.GetDragDropContext(dragInfo.VisualSource);
                var targetContext = visualTarget != null ? DragDropAttach.GetDragDropContext(visualTarget) : null;
                var isSameContext = string.Equals(sourceContext, targetContext) || string.IsNullOrEmpty(targetContext);

                if (isSameContext)
                {
                    template = isMultiSelection
                        ? DragDropAttach.TryGetDragAdornerMultiItemTemplate(visualSource, sender) ?? DragDropAttach.TryGetDragAdornerTemplate(visualSource, sender)
                        : DragDropAttach.TryGetDragAdornerTemplate(visualSource, sender);
                    templateSelector = isMultiSelection
                        ? DragDropAttach.TryGetDragAdornerMultiItemTemplateSelector(visualSource, sender) ?? DragDropAttach.TryGetDragAdornerTemplateSelector(visualSource, sender)
                        : DragDropAttach.TryGetDragAdornerTemplateSelector(visualSource, sender);

                    var useDefaultDragAdorner = template is null && templateSelector is null && DragDropAttach.GetUseDefaultDragAdorner(visualSource);
                    if (useDefaultDragAdorner)
                    {
                        template = dragInfo.VisualSourceItem.GetCaptureScreenDataTemplate(dragInfo.VisualSourceFlowDirection);
                    }

                    if (template is not null)
                    {
                        templateSelector = null;
                    }
                }
            }

            return template is not null || templateSelector is not null;
        }

        public void UpdatePreviewPresenter(IDragInfo dragInfo, UIElement visualTarget, UIElement sender)
        {
            var visualSource = dragInfo?.VisualSource;
            if (visualSource is null)
            {
                return;
            }

            if (_visualTarget != null && visualTarget != null && ReferenceEquals(_visualTarget, visualTarget))
            {
                return;
            }

            _visualTarget = visualTarget;

            var isMultiSelection = IsMultiSelection(dragInfo);

            // Get target template or template selector
            DataTemplate template = isMultiSelection
                ? DragDropAttach.TryGetDropAdornerMultiItemTemplate(visualTarget, sender) ?? DragDropAttach.TryGetDropAdornerTemplate(visualTarget, sender)
                : DragDropAttach.TryGetDropAdornerTemplate(visualTarget, sender);
            DataTemplateSelector templateSelector = isMultiSelection
                ? DragDropAttach.TryGetDropAdornerMultiItemTemplateSelector(visualTarget, sender) ?? DragDropAttach.TryGetDropAdornerTemplateSelector(visualTarget, sender)
                : DragDropAttach.TryGetDropAdornerTemplateSelector(visualTarget, sender);
            ItemsPanelTemplate itemsPanel = DragDropAttach.TryGetDropAdornerItemsPanel(visualTarget, sender);

            if (template is not null)
            {
                templateSelector = null;
            }

            // Get source template or template selector if there is no target one
            if (template is null && templateSelector is null)
            {
                var sourceContext = DragDropAttach.GetDragDropContext(dragInfo.VisualSource);
                var targetContext = visualTarget != null ? DragDropAttach.GetDragDropContext(visualTarget) : null;
                var isSameContext = string.Equals(sourceContext, targetContext) || string.IsNullOrEmpty(targetContext);

                if (isSameContext)
                {
                    template = isMultiSelection
                        ? DragDropAttach.TryGetDragAdornerMultiItemTemplate(visualSource, sender) ?? DragDropAttach.TryGetDragAdornerTemplate(visualSource, sender)
                        : DragDropAttach.TryGetDragAdornerTemplate(visualSource, sender);
                    templateSelector = isMultiSelection
                        ? DragDropAttach.TryGetDragAdornerMultiItemTemplateSelector(visualSource, sender) ?? DragDropAttach.TryGetDragAdornerTemplateSelector(visualSource, sender)
                        : DragDropAttach.TryGetDragAdornerTemplateSelector(visualSource, sender);
                    itemsPanel = DragDropAttach.TryGetDragAdornerItemsPanel(visualTarget, sender);

                    UseDefaultDragAdorner = template is null && templateSelector is null && DragDropAttach.GetUseDefaultDragAdorner(visualSource);
                    if (UseDefaultDragAdorner)
                    {
                        template = dragInfo.VisualSourceItem.GetCaptureScreenDataTemplate(dragInfo.VisualSourceFlowDirection);
                        UseDefaultDragAdorner = template is not null;
                    }

                    if (template is not null)
                    {
                        templateSelector = null;
                    }
                }
            }

            SetCurrentValue(ItemTemplateSelectorProperty, templateSelector);
            SetCurrentValue(ItemTemplateProperty, template);
            SetCurrentValue(ItemsPanelProperty, itemsPanel);
        }

        private UIElement CreatePreviewPresenter(IDragInfo dragInfo, UIElement visualTarget, UIElement sender)
        {
            var visualSource = dragInfo?.VisualSource;
            if (visualSource is null)
            {
                return null;
            }

            var useVisualSourceItemSizeForDragAdorner = dragInfo.VisualSourceItem != null && DragDropAttach.GetUseVisualSourceItemSizeForDragAdorner(visualSource);

            UpdatePreviewPresenter(dragInfo, visualTarget, sender);

            UIElement adornment = null;

            if (ItemTemplate != null || ItemTemplateSelector != null)
            {
                if (dragInfo.Data is IEnumerable enumerable and not string)
                {
                    var items = enumerable.Cast<object>().ToList();
                    var itemsCount = items.Count;
                    var maxItemsCount = DragDropAttach.TryGetDragPreviewMaxItemsCount(dragInfo, sender);
                    if (!UseDefaultDragAdorner && itemsCount <= maxItemsCount)
                    {
                        // sort items if necessary before creating the preview
                        var sorter = DragDropAttach.TryGetDragPreviewItemsSorter(dragInfo, sender);

                        var itemsControl = new ItemsControl
                        {
                            ItemsSource = sorter?.SortDragPreviewItems(items) ?? items,
                            Tag = dragInfo
                        };

                        itemsControl.SetBinding(ItemsControl.ItemTemplateProperty, new Binding(nameof(ItemTemplate)) { Source = this });
                        itemsControl.SetBinding(ItemsControl.ItemTemplateSelectorProperty, new Binding(nameof(ItemTemplateSelector)) { Source = this });
                        itemsControl.SetBinding(ItemsControl.ItemsPanelProperty, new Binding(nameof(ItemsPanel)) { Source = this });

                        if (useVisualSourceItemSizeForDragAdorner)
                        {
                            var bounds = VisualTreeExtensions.GetVisibleDescendantBounds(dragInfo.VisualSourceItem);
                            itemsControl.SetCurrentValue(MinWidthProperty, bounds.Width);
                        }

                        // The ItemsControl doesn't display unless we create a grid to contain it.
                        var grid = new Grid();
                        grid.Children.Add(itemsControl);
                        adornment = grid;
                    }
                }
                else
                {
                    var contentPresenter = new ContentPresenter
                    {
                        Content = dragInfo.Data,
                        Tag = dragInfo
                    };

                    contentPresenter.SetBinding(ContentPresenter.ContentTemplateProperty, new Binding(nameof(ItemTemplate)) { Source = this });
                    contentPresenter.SetBinding(ContentPresenter.ContentTemplateSelectorProperty, new Binding(nameof(ItemTemplateSelector)) { Source = this });

                    if (useVisualSourceItemSizeForDragAdorner)
                    {
                        var bounds = VisualTreeExtensions.GetVisibleDescendantBounds(dragInfo.VisualSourceItem);
                        contentPresenter.SetCurrentValue(MinWidthProperty, bounds.Width);
                        contentPresenter.SetCurrentValue(MinHeightProperty, bounds.Height);
                    }

                    contentPresenter.Loaded += ContentPresenter_OnLoaded;

                    adornment = contentPresenter;
                }
            }

            if (adornment != null && UseDefaultDragAdorner)
            {
                adornment.Opacity = DragDropAttach.GetDefaultDragAdornerOpacity(visualSource);
            }

            return adornment;
        }

        private void ContentPresenter_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ContentPresenter contentPresenter)
            {
                contentPresenter.Loaded -= ContentPresenter_OnLoaded;

                // If the template contains a Canvas then we get a strange size.
                if (UseDefaultDragAdorner && _dragInfo?.VisualSourceItem.GetVisualDescendent<Canvas>() is not null)
                {
                    _visualSourceItemBounds = _dragInfo?.VisualSourceItem != null ? VisualTreeHelper.GetDescendantBounds(_dragInfo.VisualSourceItem) : Rect.Empty;

                    contentPresenter.SetCurrentValue(MaxWidthProperty, _visualSourceItemBounds.Width);
                    contentPresenter.SetCurrentValue(MaxHeightProperty, _visualSourceItemBounds.Height);
                    SetCurrentValue(MaxWidthProperty, _visualSourceItemBounds.Width);
                    SetCurrentValue(MaxHeightProperty, _visualSourceItemBounds.Height);
                }
                else
                {
                    contentPresenter.ApplyTemplate();
                    if (contentPresenter.GetVisualDescendent<Canvas>() is not null)
                    {
                        // Get the first element and set it's vertical alignment to top.
                        if (contentPresenter.GetVisualDescendent<DependencyObject>() is FrameworkElement fe)
                        {
                            fe.SetCurrentValue(VerticalAlignmentProperty, VerticalAlignment.Top);
                        }

                        _visualSourceItemBounds = _dragInfo?.VisualSourceItem != null ? VisualTreeHelper.GetDescendantBounds(_dragInfo.VisualSourceItem) : Rect.Empty;

                        contentPresenter.SetCurrentValue(MaxWidthProperty, _visualSourceItemBounds.Width);
                        contentPresenter.SetCurrentValue(MaxHeightProperty, _visualSourceItemBounds.Height);
                        SetCurrentValue(MaxWidthProperty, _visualSourceItemBounds.Width);
                        SetCurrentValue(MaxHeightProperty, _visualSourceItemBounds.Height);
                    }
                }
            }
        }
    }
}