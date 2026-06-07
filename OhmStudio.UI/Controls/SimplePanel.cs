using System;
using System.Windows;
using System.Windows.Controls;

namespace OhmStudio.UI.Controls
{
    /// <summary>
    /// Provides a lightweight alternative to the <see cref="Grid"/> control.
    /// </summary>
    /// <remarks>
    /// It is recommended to use this class when the row and column division features of a <see cref="Grid"/> are not required.
    /// </remarks>
    public class SimplePanel : Panel
    {
        protected override Size MeasureOverride(Size constraint)
        {
            var maxSize = new Size();

            foreach (UIElement child in InternalChildren)
            {
                if (child != null)
                {
                    child.Measure(constraint);
                    maxSize.Width = Math.Max(maxSize.Width, child.DesiredSize.Width);
                    maxSize.Height = Math.Max(maxSize.Height, child.DesiredSize.Height);
                }
            }

            return maxSize;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                child?.Arrange(new Rect(arrangeSize));
            }

            return arrangeSize;
        }
    }
} 