using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace OhmStudio.UI.Attaches
{
    public class DropShadowAttach
    {
        public static readonly DependencyProperty ShadowColorProperty =
            DependencyProperty.RegisterAttached("ShadowColor", typeof(Color), typeof(DropShadowAttach), new FrameworkPropertyMetadata(Brushes.Transparent.Color, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(DropShadowAttach), new FrameworkPropertyMetadata(default(CornerRadius), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty ShadowDepthProperty =
            DependencyProperty.RegisterAttached("ShadowDepth", typeof(double), typeof(DropShadowAttach), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.RegisterAttached("BlurRadius", typeof(double), typeof(DropShadowAttach), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShadowDirectionProperty =
            DependencyProperty.RegisterAttached("ShadowDirection", typeof(double), typeof(DropShadowAttach), new FrameworkPropertyMetadata(315d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShadowOpacityProperty =
          DependencyProperty.RegisterAttached("ShadowOpacity", typeof(double), typeof(DropShadowAttach), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty RenderingBiasProperty =
            DependencyProperty.RegisterAttached("RenderingBias", typeof(RenderingBias), typeof(DropShadowAttach), new FrameworkPropertyMetadata(RenderingBias.Performance, FrameworkPropertyMetadataOptions.AffectsRender));

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