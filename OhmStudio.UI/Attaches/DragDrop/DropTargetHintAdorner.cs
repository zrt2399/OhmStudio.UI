using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace OhmStudio.UI.Attaches.DragDrop
{
    /// <summary>
    /// This adorner is used to display hints for where items can be dropped.
    /// </summary>
    public class DropTargetHintAdorner : Adorner
    {
        private readonly ContentPresenter presenter;

        private readonly AdornerLayer adornerLayer;

        public static readonly DependencyProperty DropHintDataProperty
            = DependencyProperty.Register(nameof(DropHintData),
                                          typeof(DropHintData),
                                          typeof(DropTargetHintAdorner),
                                          new PropertyMetadata(default(DropHintData)));

        public DropHintData DropHintData
        {
            get => (DropHintData)GetValue(DropHintDataProperty);
            set => SetValue(DropHintDataProperty, value);
        }

        public DropTargetHintAdorner(UIElement adornedElement, DataTemplate dataTemplate, DropHintData dropHintData)
            : base(adornedElement)
        {
            SetCurrentValue(DropHintDataProperty, dropHintData);
            IsHitTestVisible = false;
            AllowDrop = false;
            SnapsToDevicePixels = true;
            adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            adornerLayer?.Add(this);

            presenter = new ContentPresenter()
            {
                IsHitTestVisible = false,
                ContentTemplate = dataTemplate
            };
            var binding = new Binding(nameof(DropHintData))
            {
                Source = this,
                Mode = BindingMode.OneWay
            };
            presenter.SetBinding(ContentPresenter.ContentProperty, binding);
        }

        /// <summary>
        /// Detach the adorner from its adorner layer.
        /// </summary>
        public void Detach()
        {
            if (adornerLayer is null)
            {
                return;
            }

            if (!adornerLayer.Dispatcher.CheckAccess())
            {
                adornerLayer.Dispatcher.Invoke(Detach);
                return;
            }

            adornerLayer.Remove(this);
        }

        private static Rect GetBounds(FrameworkElement element, UIElement visual)
        {
            return new Rect(
                element.TranslatePoint(new Point(0, 0), visual),
                element.TranslatePoint(new Point(element.ActualWidth, element.ActualHeight), visual));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            presenter.Measure(constraint);
            return presenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var bounds = GetBounds(AdornedElement as FrameworkElement, AdornedElement);
            presenter.Arrange(bounds);
            return bounds.Size;
        }

        protected override Visual GetVisualChild(int index)
        {
            return presenter;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Update hint text and state for the adorner.
        /// </summary>
        /// <param name="hintData"></param>
        public void Update(DropHintData hintData)
        {
            var currentData = DropHintData;
            bool requiresUpdate = (hintData?.HintState != currentData?.HintState || hintData?.HintText != currentData?.HintText);
            SetCurrentValue(DropHintDataProperty, hintData);
            if (requiresUpdate)
            {
                adornerLayer?.Update();
            }
        }

        /// <summary>
        /// Construct a new drop hint target adorner.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="adornedElement"></param>
        /// <param name="dataTemplate"></param>
        /// <param name="hintData"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        internal static DropTargetHintAdorner CreateHintAdorner(Type type, UIElement adornedElement, DataTemplate dataTemplate, DropHintData hintData)
        {
            if (!typeof(DropTargetHintAdorner).IsAssignableFrom(type))
            {
                throw new InvalidOperationException("The requested adorner class does not derive from DropTargetHintAdorner.");
            }

            return type.GetConstructor(new[]
                                       {
                                           typeof(UIElement),
                                           typeof(DataTemplate),
                                           typeof(DropHintData)
                                       })
                       ?.Invoke(new object[]
                       {
                                    adornedElement,
                                    dataTemplate,
                                    hintData
                       }) 
                       as DropTargetHintAdorner;
        }
    }
}