using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using OhmStudio.UI.Utilities;

namespace OhmStudio.UI.Controls
{
    public class DropShadowChrome : ContentControl
    {
        public static readonly DependencyProperty ShowShadowProperty =
           DependencyProperty.RegisterAttached(nameof(ShowShadow), typeof(bool), typeof(DropShadowChrome), new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty ShadowBrushProperty =
            DependencyProperty.RegisterAttached(nameof(ShadowBrush), typeof(SolidColorBrush), typeof(DropShadowChrome), new FrameworkPropertyMetadata(Brushes.DarkGray, (sender, e) =>
            {
                if (sender is DropShadowChrome dropShadowChrome)
                {
                    dropShadowChrome.ShadowColor = e.NewValue is SolidColorBrush brush ? brush.Color : Brushes.Transparent.Color;
                }
            }));

        public static readonly DependencyProperty ShadowColorProperty =
            DependencyProperty.RegisterAttached(nameof(ShadowColor), typeof(Color), typeof(DropShadowChrome), new FrameworkPropertyMetadata(Brushes.DarkGray.Color, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached(nameof(CornerRadius), typeof(CornerRadius), typeof(DropShadowChrome), new FrameworkPropertyMetadata(default(CornerRadius), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShadowDepthProperty =
            DependencyProperty.RegisterAttached(nameof(ShadowDepth), typeof(double), typeof(DropShadowChrome), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.RegisterAttached(nameof(BlurRadius), typeof(double), typeof(DropShadowChrome), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShadowDirectionProperty =
            DependencyProperty.RegisterAttached(nameof(ShadowDirection), typeof(double), typeof(DropShadowChrome), new FrameworkPropertyMetadata(315d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShadowOpacityProperty =
          DependencyProperty.RegisterAttached(nameof(ShadowOpacity), typeof(double), typeof(DropShadowChrome), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty RenderingBiasProperty =
            DependencyProperty.RegisterAttached(nameof(RenderingBias), typeof(RenderingBias), typeof(DropShadowChrome), new FrameworkPropertyMetadata(RenderingBias.Performance, FrameworkPropertyMetadataOptions.AffectsRender));

        static DropShadowChrome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropShadowChrome), new FrameworkPropertyMetadata(typeof(DropShadowChrome)));
        }

        public DropShadowChrome()
        {
            GotFocus += DropShadowChrome_GotFocus;
        }

        private void DropShadowChrome_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!e.Handled && Equals(e.OriginalSource, this))
            {
                if (Content.GetFirstFocusable() is UIElement uIElement)
                {
                    uIElement.Focus();
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// 是否显示阴影，默认值为true。
        /// </summary>
        public bool ShowShadow
        {
            get => (bool)GetValue(ShowShadowProperty);
            set => SetValue(ShowShadowProperty, value);
        }

        /// <summary>
        /// 阴影颜色，默认值为<see cref="Brushes.DarkGray"/>(#FFA9A9A9)。
        /// </summary>
        public SolidColorBrush ShadowBrush
        {
            get => (SolidColorBrush)GetValue(ShadowBrushProperty);
            set => SetValue(ShadowBrushProperty, value);
        }

        /// <summary>
        /// 阴影颜色，默认值为<see cref="Brushes.DarkGray"/>(#FFA9A9A9)。
        /// </summary>
        public Color ShadowColor
        {
            get => (Color)GetValue(ShadowColorProperty);
            set => SetValue(ShadowColorProperty, value);
        }

        /// <summary>
        /// 圆角，默认值为0。
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// 阴影偏移，默认值为5。
        /// </summary>
        public double ShadowDepth
        {
            get => (double)GetValue(ShadowDepthProperty);
            set => SetValue(ShadowDepthProperty, value);
        }

        /// <summary>
        /// 阴影半径，默认值为5。
        /// </summary>
        public double BlurRadius
        {
            get => (double)GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }

        /// <summary>
        /// 投影的方向，默认值为315。
        /// </summary>
        public double ShadowDirection
        {
            get => (double)GetValue(ShadowDirectionProperty);
            set => SetValue(ShadowDirectionProperty, value);
        }

        /// <summary>
        /// 阴影透明度，默认值为1。
        /// </summary>
        public double ShadowOpacity
        {
            get => (double)GetValue(ShadowOpacityProperty);
            set => SetValue(ShadowOpacityProperty, value);
        }

        /// <summary>
        /// 阴影呈现质量，默认值为<see cref="RenderingBias.Performance"/>。
        /// </summary>
        public RenderingBias RenderingBias
        {
            get => (RenderingBias)GetValue(RenderingBiasProperty);
            set => SetValue(RenderingBiasProperty, value);
        }

        public static void SetShowShadow(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowShadowProperty, value);
        }

        public static bool GetShowShadow(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowShadowProperty);
        }

        public static void SetShadowBrush(DependencyObject obj, SolidColorBrush value)
        {
            obj.SetValue(ShadowBrushProperty, value);
        }

        public static SolidColorBrush GetShadowBrush(DependencyObject obj)
        {
            return (SolidColorBrush)obj.GetValue(ShadowBrushProperty);
        }

        public static void SetShadowColor(DependencyObject obj, Color value)
        {
            obj.SetValue(ShadowColorProperty, value);
        }

        public static Color GetShadowColor(DependencyObject obj)
        {
            return (Color)obj.GetValue(ShadowColorProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetShadowDepth(DependencyObject obj, double value)
        {
            obj.SetValue(ShadowDepthProperty, value);
        }

        public static double GetShadowDepth(DependencyObject obj)
        {
            return (double)obj.GetValue(ShadowDepthProperty);
        }

        public static void SetBlurRadius(DependencyObject obj, double value)
        {
            obj.SetValue(BlurRadiusProperty, value);
        }

        public static double GetBlurRadius(DependencyObject obj)
        {
            return (double)obj.GetValue(BlurRadiusProperty);
        }

        public static void SetShadowDirection(DependencyObject obj, double value)
        {
            obj.SetValue(ShadowDirectionProperty, value);
        }

        public static double GetShadowDirection(DependencyObject obj)
        {
            return (double)obj.GetValue(ShadowDirectionProperty);
        }

        public static void SetShadowOpacity(DependencyObject obj, double value)
        {
            obj.SetValue(ShadowOpacityProperty, value);
        }

        public static double GetShadowOpacity(DependencyObject obj)
        {
            return (double)obj.GetValue(ShadowOpacityProperty);
        }

        public static void SetRenderingBias(DependencyObject obj, RenderingBias value)
        {
            obj.SetValue(RenderingBiasProperty, value);
        }

        public static RenderingBias GetRenderingBias(DependencyObject obj)
        {
            return (RenderingBias)obj.GetValue(RenderingBiasProperty);
        }
    }
}