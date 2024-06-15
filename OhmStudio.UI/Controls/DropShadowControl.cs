using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace OhmStudio.UI.Controls
{
    public class DropShadowControl : ContentControl
    {
        static DropShadowControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropShadowControl), new FrameworkPropertyMetadata(typeof(DropShadowControl)));
        }

        public static readonly DependencyProperty IsShowShadowProperty =
           DependencyProperty.Register(nameof(IsShowShadow), typeof(bool), typeof(DropShadowControl), new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty ShadowColorProperty =
            DependencyProperty.Register(nameof(ShadowColor), typeof(Color), typeof(DropShadowControl), new FrameworkPropertyMetadata(Brushes.DarkGray.Color, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(DropShadowControl), new FrameworkPropertyMetadata(default(CornerRadius), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty ShadowDepthProperty =
            DependencyProperty.Register(nameof(ShadowDepth), typeof(double), typeof(DropShadowControl), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register(nameof(BlurRadius), typeof(double), typeof(DropShadowControl), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShadowDirectionProperty =
            DependencyProperty.Register(nameof(ShadowDirection), typeof(double), typeof(DropShadowControl), new FrameworkPropertyMetadata(315d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShadowOpacityProperty =
          DependencyProperty.Register(nameof(ShadowOpacity), typeof(double), typeof(DropShadowControl), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty RenderingBiasProperty =
            DependencyProperty.Register(nameof(RenderingBias), typeof(RenderingBias), typeof(DropShadowControl), new FrameworkPropertyMetadata(RenderingBias.Performance, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 是否显示阴影，默认值为true。
        /// </summary>
        public bool IsShowShadow
        {
            get => (bool)GetValue(IsShowShadowProperty);
            set => SetValue(IsShowShadowProperty, value);
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
    }
}