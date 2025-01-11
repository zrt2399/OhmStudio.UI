using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace OhmStudio.UI.Attaches.DragDrop
{
    /// <summary>
    /// Base class for drop target Adorner.
    /// </summary>
    public abstract class DropTargetAdorner : Adorner
    { 
        private readonly AdornerLayer adornerLayer;

        /// <summary>
        /// Gets or Sets the pen which can be used for the render process.
        /// </summary>
        public Pen Pen { get; set; } = new Pen(Brushes.Gray, 2);

        public IDropInfo DropInfo { get; set; }

        public DropTargetAdorner(UIElement adornedElement, IDropInfo dropInfo)
            : base(adornedElement)
        {
            DropInfo = dropInfo;
            IsHitTestVisible = false;
            AllowDrop = false;
            SnapsToDevicePixels = true;
            adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            // can be null but should normally not be null
            adornerLayer?.Add(this);
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

        internal static DropTargetAdorner Create(Type type, UIElement adornedElement, IDropInfo dropInfo)
        {
            if (!typeof(DropTargetAdorner).IsAssignableFrom(type))
            {
                throw new InvalidOperationException("The requested adorner class does not derive from DropTargetAdorner.");
            }

            var ctor = type.GetConstructor(new[] { typeof(UIElement), typeof(IDropInfo) });
            if (ctor is null && dropInfo is DropInfo)
            {
                ctor = type.GetConstructor(new[] { typeof(UIElement), typeof(DropInfo) });
            }

            return ctor?.Invoke(new object[] { adornedElement, dropInfo }) as DropTargetAdorner;
        }
    }
}