using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using ControlzEx;
using ControlzEx.Behaviors;
using ControlzEx.Native;
using OhmStudio.UI.Helpers;

namespace OhmStudio.UI.Controls
{
    public class CustomizeTitleBar : Border
    {
        public static readonly DependencyProperty ShowSystemMenuProperty =
            DependencyProperty.Register("ShowSystemMenu", typeof(bool),
                typeof(CustomizeTitleBar), new PropertyMetadata(true));

        public static readonly DependencyProperty IsWindowTitleBarProperty =
            DependencyProperty.Register("IsWindowTitleBar", typeof(bool),
                typeof(CustomizeTitleBar), new PropertyMetadata(true));

        public bool ShowSystemMenu
        {
            get => (bool)GetValue(ShowSystemMenuProperty);
            set => SetValue(ShowSystemMenuProperty, value);
        }

        public bool IsWindowTitleBar
        {
            get => (bool)GetValue(IsWindowTitleBarProperty);
            set => SetValue(IsWindowTitleBarProperty, value);
        }

        public CustomizeTitleBar()
        {
            PresentationSource.AddSourceChangedHandler(this, OnSourceChanged);

            WindowChrome.SetIsHitTestVisibleInChrome(this, true);
            CoreceUpdateIsWindowTitleBar();
        }

        private void OnSourceChanged(object sender, SourceChangedEventArgs args)
        {
            var newHwndSource = (HwndSource)args.NewSource;
            newHwndSource?.AddHook(WndProc);

            var oldHwndSource = (HwndSource)args.OldSource;
            oldHwndSource?.RemoveHook(WndProc);
        }

        protected virtual IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 165:
                    handled = true;
                    CoreceShowContextMenu();
                    break;
            }
            return IntPtr.Zero;
        }

        private void CoreceUpdateIsWindowTitleBar()
        {
            if (IsWindowTitleBar)
            {
                NonClientControlProperties.SetHitTestResult(this, HT.CAPTION);
            }
            else
            {
                NonClientControlProperties.SetHitTestResult(this, HT.TOP);
            }
        }

        private void CoreceShowContextMenu()
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }

            if (Window.GetWindow(this) is Window window)
            {
                if (ShowSystemMenu)
                {
#pragma warning disable CS0618
                    ControlzEx.SystemCommands.ShowSystemMenu(window, PointFromScreen(MouseHelper.GetMousePosition()));
#pragma warning restore CS0618
                }
                else
                {
                    if (ContextMenu != null)
                    {
                        ContextMenu.PlacementTarget = this;
                        ContextMenu.Placement = PlacementMode.MousePoint;
                        ContextMenu.IsOpen = true;
                    }
                }
            }
        }

        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                CoreceShowContextMenu();
                e.Handled = true;
            }
        }
    }
}