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
                scrollViewer.PreviewMouseWheel += ScrollViewerPreviewMouseWheel;
            }
            else
            {
                scrollViewer.PreviewMouseWheel -= ScrollViewerPreviewMouseWheel;
            }

            static void ScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs args)
            {
                ScrollViewer scrollViewer2 = (ScrollViewer)sender;
                scrollViewer2.ScrollToHorizontalOffset(Math.Min(Math.Max(0.0, scrollViewer2.HorizontalOffset - args.Delta), scrollViewer2.ScrollableWidth));
                args.Handled = true;
            }
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

        static void ScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs args)
        {
            if (!args.Handled)
            {
                args.Handled = true;
                VisualHelper.GetParent<ScrollViewer>((UIElement)sender)?.RaiseEvent(new MouseWheelEventArgs(args.MouseDevice, args.Timestamp, args.Delta)
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