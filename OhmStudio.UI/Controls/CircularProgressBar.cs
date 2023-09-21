using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace OhmStudio.UI.Controls
{
    /// <summary>
    /// 圆形进度条。
    /// </summary>
    public partial class CircularProgressBar : ProgressBar
    {
        public CircularProgressBar()
        {
            ValueChanged += CircularProgressBar_ValueChanged;
        }

        void CircularProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CircularProgressBar bar = sender as CircularProgressBar;
            double currentAngle = bar.Angle;
            double targetAngle = e.NewValue / bar.Maximum * 359.999;
            DoubleAnimation anim = new DoubleAnimation(currentAngle, targetAngle, TimeSpan.FromMilliseconds(400));
            bar.BeginAnimation(AngleProperty, anim, HandoffBehavior.SnapshotAndReplace);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CircularProgressBar), new PropertyMetadata(string.Empty));

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(CircularProgressBar), new PropertyMetadata(0.0));

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(CircularProgressBar), new PropertyMetadata(10.0));

        public double BrushStrokeThickness
        {
            get => (double)GetValue(BrushStrokeThicknessProperty);
            set => SetValue(BrushStrokeThicknessProperty, value);
        }

        public static readonly DependencyProperty BrushStrokeThicknessProperty =
            DependencyProperty.Register("BrushStrokeThickness", typeof(double), typeof(CircularProgressBar), new PropertyMetadata(1.0));
    }
}