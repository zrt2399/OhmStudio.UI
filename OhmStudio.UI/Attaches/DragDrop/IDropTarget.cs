﻿using System.Windows;

namespace OhmStudio.UI.Attaches.DragDrop
{
    /// <summary>
    /// Interface implemented by Drop Handlers.
    /// </summary>
    public interface IDropTarget
    {
        /// <summary>
        /// Notifies the drop handler when a drag is initiated to display hint about potential drop targets.
        /// </summary>
        /// <param name="dropHintInfo">Object which contains several drop information.</param>
#if NETCOREAPP3_1_OR_GREATER
        void DropHint(IDropHintInfo dropHintInfo)
        {
            // nothing here
        }
#else
        void DropHint(IDropHintInfo dropHintInfo);
#endif

        /// <summary>
        /// Notifies the drop handler when dragging operation enters a potential drop target.
        /// </summary>
        /// <param name="dropInfo">Object which contains several drop information.</param>
#if NETCOREAPP3_1_OR_GREATER
        void DragEnter(IDropInfo dropInfo)
        {
            // nothing here
        }
#else
        void DragEnter(IDropInfo dropInfo);
#endif

        /// <summary>
        /// Notifies the drop handler about the current drag operation state.
        /// </summary>
        /// <param name="dropInfo">Object which contains several drop information.</param>
        /// <remarks>
        /// To allow a drop at the current drag position, the <see cref="IDropInfo.Effects"/> property on
        /// <paramref name="dropInfo"/> should be set to a value other than <see cref="DragDropEffects.None"/>
        /// and <see cref="IDropInfo.Data"/> should be set to a non-null value.
        /// </remarks>
        void DragOver(IDropInfo dropInfo);

        /// <summary>
        /// Notifies the drop handler when dragging operation leaves a potential drop target.
        /// </summary>
        /// <param name="dropInfo">Object which contains several drop information.</param>
#if NETCOREAPP3_1_OR_GREATER
        void DragLeave(IDropInfo dropInfo)
        {
            // nothing here
        }
#else
        void DragLeave(IDropInfo dropInfo);
#endif

        /// <summary>
        /// Performs a drop on the target.
        /// </summary>
        /// <param name="dropInfo">Object which contains several drop information.</param>
        void Drop(IDropInfo dropInfo);
    }
}