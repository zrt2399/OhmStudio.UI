using System.Windows;
using ControlzEx.Behaviors;

namespace OhmStudio.UI.Attaches
{
    public sealed class WindowChromeAttach
    {
        public static readonly DependencyProperty CornerPreferenceProperty;

        public static readonly DependencyProperty EnableMaxRestoreProperty;

        public static readonly DependencyProperty EnableMinimizeProperty;

        public static readonly DependencyProperty IgnoreTaskbarOnMaximizeProperty;

        public static readonly DependencyProperty KeepBorderOnMaximizeProperty;

        public static readonly DependencyProperty ResizeBorderThicknessProperty;

        static WindowChromeAttach()
        {
            CornerPreferenceProperty =
                DependencyProperty.RegisterAttached("CornerPreference", typeof(WindowCornerPreference),
                    typeof(WindowChromeAttach), new FrameworkPropertyMetadata(WindowCornerPreference.Default,
                    FrameworkPropertyMetadataOptions.AffectsRender, OnCornerPreferenceChangedCallback));

            EnableMaxRestoreProperty =
                DependencyProperty.RegisterAttached("EnableMaxRestore", typeof(bool),
                    typeof(WindowChromeAttach), new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.AffectsRender, OnEnableMaxRestoreChangedCallback));

            EnableMinimizeProperty =
                DependencyProperty.RegisterAttached("EnableMinimize", typeof(bool),
                    typeof(WindowChromeAttach), new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.AffectsRender, OnEnableMinimizeChangedCallback));

            IgnoreTaskbarOnMaximizeProperty =
                DependencyProperty.RegisterAttached("IgnoreTaskbarOnMaximize", typeof(bool),
                    typeof(WindowChromeAttach), new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.AffectsRender, OnIgnoreTaskbarOnMaximizeChangedCallback));

            KeepBorderOnMaximizeProperty =
                DependencyProperty.RegisterAttached("KeepBorderOnMaximize", typeof(bool),
                    typeof(WindowChromeAttach), new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.AffectsRender, OnKeepBorderOnMaximizeChangedCallback));

            ResizeBorderThicknessProperty =
                DependencyProperty.RegisterAttached("ResizeBorderThickness", typeof(Thickness),
                    typeof(WindowChromeAttach), new FrameworkPropertyMetadata(WindowChromeBehavior.GetDefaultResizeBorderThickness(),
                    FrameworkPropertyMetadataOptions.AffectsRender, OnResizeBorderThicknessChangedCallback));
        }

        private static void OnCornerPreferenceChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is Window && e.NewValue is WindowCornerPreference cornerPreference)
            {
                obj.GetOrAddBehavior(BehaviorFactory.CreateWindowChromeBehavior).CornerPreference = cornerPreference;
            }
        }

        private static void OnEnableMaxRestoreChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is Window && e.NewValue is bool b)
            {
                obj.GetOrAddBehavior(BehaviorFactory.CreateWindowChromeBehavior).EnableMaxRestore = b;
            }
        }

        private static void OnEnableMinimizeChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is Window && e.NewValue is bool b)
            {
                obj.GetOrAddBehavior(BehaviorFactory.CreateWindowChromeBehavior).EnableMinimize = b;
            }
        }

        private static void OnIgnoreTaskbarOnMaximizeChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is Window && e.NewValue is bool b)
            {
                obj.GetOrAddBehavior(BehaviorFactory.CreateWindowChromeBehavior).IgnoreTaskbarOnMaximize = b;
            }
        }

        private static void OnKeepBorderOnMaximizeChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is Window && e.NewValue is bool b)
            {
                obj.GetOrAddBehavior(BehaviorFactory.CreateWindowChromeBehavior).KeepBorderOnMaximize = b;
            }
        }

        private static void OnResizeBorderThicknessChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is Window && e.NewValue is Thickness thickness)
            {
                obj.GetOrAddBehavior(BehaviorFactory.CreateWindowChromeBehavior).ResizeBorderThickness = thickness;
            }
        }

        public static WindowCornerPreference GetCornerPreference(DependencyObject obj)
        => (WindowCornerPreference)obj.GetValue(CornerPreferenceProperty);

        public static void SetCornerPreference(DependencyObject obj, WindowCornerPreference value)
        => obj.SetValue(CornerPreferenceProperty, value);

        public static bool GetEnableMaxRestore(DependencyObject obj)
        => (bool)obj.GetValue(EnableMaxRestoreProperty);

        public static void SetEnableMaxRestore(DependencyObject obj, bool value)
        => obj.SetValue(EnableMaxRestoreProperty, value);

        public static bool GetEnableMinimize(DependencyObject obj)
        => (bool)obj.GetValue(EnableMinimizeProperty);

        public static void SetEnableMinimize(DependencyObject obj, bool value)
        => obj.SetValue(EnableMinimizeProperty, value);

        public static bool GetIgnoreTaskbarOnMaximize(DependencyObject obj)
        => (bool)obj.GetValue(IgnoreTaskbarOnMaximizeProperty);

        public static void SetIgnoreTaskbarOnMaximize(DependencyObject obj, bool value)
        => obj.SetValue(IgnoreTaskbarOnMaximizeProperty, value);

        public static bool GetKeepBorderOnMaximize(DependencyObject obj)
        => (bool)obj.GetValue(KeepBorderOnMaximizeProperty);

        public static void SetKeepBorderOnMaximize(DependencyObject obj, bool value)
        => obj.SetValue(KeepBorderOnMaximizeProperty, value);

        public static Thickness GetResizeBorderThickness(DependencyObject obj)
        => (Thickness)obj.GetValue(ResizeBorderThicknessProperty);

        public static void SetResizeBorderThickness(DependencyObject obj, Thickness value)
        => obj.SetValue(ResizeBorderThicknessProperty, value);
    }
} 