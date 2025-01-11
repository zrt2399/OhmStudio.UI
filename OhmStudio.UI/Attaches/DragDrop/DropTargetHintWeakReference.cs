using System;
using System.Windows;

namespace OhmStudio.UI.Attaches.DragDrop
{
    /// <summary>
    /// Wrapper of the <see cref="UIElement"/> so we only have weak references to the drop targets
    /// to avoid memory leaks.
    /// </summary>
    internal sealed class DropTargetHintWeakReference : IDisposable
    {
        private readonly WeakReference<UIElement> _dropTarget;
        private DropTargetHintAdorner dropTargetHintAdorner;

        public DropTargetHintWeakReference(UIElement dropTarget)
        {
            _dropTarget = new WeakReference<UIElement>(dropTarget);
        }

        public UIElement Target => _dropTarget.TryGetTarget(out var target) ? target : null;

        /// <summary>
        /// Property indicating if the weak reference is still alive, or should be disposed of.
        /// </summary>
        public bool IsAlive => _dropTarget.TryGetTarget(out _);

        /// <summary>
        /// The current adorner for the drop target.
        /// </summary>
        public DropTargetHintAdorner DropTargetHintAdorner
        {
            get => dropTargetHintAdorner;
            set
            {
                dropTargetHintAdorner?.Detach();
                dropTargetHintAdorner = value;
            }
        }

        public void Dispose()
        {
            DropTargetHintAdorner = null;
        }
    }
}