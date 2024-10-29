using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OhmStudio.UI.Helpers;

namespace OhmStudio.UI.Attaches
{
    public class ScrollViewerAttach
    {
        public static readonly DependencyProperty IsDisabledProperty =
            DependencyProperty.RegisterAttached("IsDisabled", typeof(bool), typeof(ScrollViewerAttach), new PropertyMetadata(false, OnIsDisabledChanged));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.RegisterAttached("Orientation", typeof(Orientation), typeof(ScrollViewerAttach), new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.Inherits, OnOrientationChanged));

        private static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer)
            {
                return;
            }

            if ((Orientation)e.NewValue == Orientation.Horizontal)
            {
                scrollViewer.PreviewMouseWheel += PreviewMouseWheel;
            }
            else
            {
                scrollViewer.PreviewMouseWheel -= PreviewMouseWheel;
            }
        }

        private static void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            scrollViewer.ScrollToHorizontalOffset(Math.Min(Math.Max(0.0, scrollViewer.HorizontalOffset - e.Delta), scrollViewer.ScrollableWidth));
            e.Handled = true;
        }

        public static void SetOrientation(DependencyObject element, Orientation value)
        {
            element.SetValue(OrientationProperty, value);
        }

        public static Orientation GetOrientation(DependencyObject element)
        {
            return (Orientation)element.GetValue(OrientationProperty);
        }

        private static void OnIsDisabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is UIElement uIElement)
            {
                if ((bool)e.NewValue)
                {
                    uIElement.PreviewMouseWheel += ScrollViewerPreviewMouseWheel;
                }
                else
                {
                    uIElement.PreviewMouseWheel -= ScrollViewerPreviewMouseWheel;
                }
            }
        }

        private static void ScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                VisualHelper.GetParent<ScrollViewer>((UIElement)sender)?.RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = UIElement.MouseWheelEvent,
                    Source = sender
                });
            }
        }

        public static void SetIsDisabled(DependencyObject element, bool value)
        {
            element.SetValue(IsDisabledProperty, value);
        }

        public static bool GetIsDisabled(DependencyObject element)
        {
            return (bool)element.GetValue(IsDisabledProperty);
        }
    }
}