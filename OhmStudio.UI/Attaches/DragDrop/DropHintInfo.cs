using System;

namespace OhmStudio.UI.Attaches.DragDrop
{
    /// <summary>
    /// Implementation of the <see cref="IDropHintInfo"/> interface to hold DropHint information.
    /// </summary>
    public class DropHintInfo : IDropHintInfo
    {
        /// <inheritdoc />
        public IDragInfo DragInfo { get; }

        /// <inheritdoc />
        public Type DropTargetHintAdorner { get; set; }

        /// <inheritdoc />
        public string DropHintText { get; set; }

        /// <inheritdoc />
        public DropHintState DropTargetHintState { get; set; }

        public DropHintInfo(IDragInfo dragInfo)
        {
            DragInfo = dragInfo;
        }
    }
}