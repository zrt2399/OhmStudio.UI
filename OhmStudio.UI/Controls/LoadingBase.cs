using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace OhmStudio.UI.Controls
{
    public abstract class LoadingBase : ContentControl
    {
        protected Storyboard Storyboard;

        public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register(
            nameof(IsRunning), typeof(bool), typeof(LoadingBase), new PropertyMetadata(true, (sender, args) =>
            {
                var loadingBase = (LoadingBase)sender;
                if ((bool)args.NewValue)
                {
                    loadingBase.Storyboard?.Begin();
                }
                else
                {
                    loadingBase.Storyboard?.Stop();
                }
            }));

        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        public static readonly DependencyProperty DotCountProperty = DependencyProperty.Register(
            nameof(DotCount), typeof(int), typeof(LoadingBase),
            new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty DotIntervalProperty = DependencyProperty.Register(
            nameof(DotInterval), typeof(double), typeof(LoadingBase),
            new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty DotBorderBrushProperty = DependencyProperty.Register(
            nameof(DotBorderBrush), typeof(Brush), typeof(LoadingBase),
            new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty DotBorderThicknessProperty = DependencyProperty.Register(
            nameof(DotBorderThickness), typeof(double), typeof(LoadingBase),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty DotDiameterProperty = DependencyProperty.Register(
            nameof(DotDiameter), typeof(double), typeof(LoadingBase),
            new FrameworkPropertyMetadata(6.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty DotSpeedProperty = DependencyProperty.Register(
            nameof(DotSpeed), typeof(double), typeof(LoadingBase),
            new FrameworkPropertyMetadata(4.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty DotDelayTimeProperty = DependencyProperty.Register(
            nameof(DotDelayTime), typeof(double), typeof(LoadingBase),
            new FrameworkPropertyMetadata(80.0, FrameworkPropertyMetadataOptions.AffectsRender));

        protected readonly Canvas PrivateCanvas = new()
        {
            ClipToBounds = true
        };

        protected LoadingBase()
        {
            Content = PrivateCanvas;
        }

        public int DotCount
        {
            get => (int)GetValue(DotCountProperty);
            set => SetValue(DotCountProperty, value);
        }

        public double DotInterval
        {
            get => (double)GetValue(DotIntervalProperty);
            set => SetValue(DotIntervalProperty, value);
        }

        public Brush DotBorderBrush
        {
            get => (Brush)GetValue(DotBorderBrushProperty);
            set => SetValue(DotBorderBrushProperty, value);
        }

        public double DotBorderThickness
        {
            get => (double)GetValue(DotBorderThicknessProperty);
            set => SetValue(DotBorderThicknessProperty, value);
        }

        public double DotDiameter
        {
            get => (double)GetValue(DotDiameterProperty);
            set => SetValue(DotDiameterProperty, value);
        }

        public double DotSpeed
        {
            get => (double)GetValue(DotSpeedProperty);
            set => SetValue(DotSpeedProperty, value);
        }

        public double DotDelayTime
        {
            get => (double)GetValue(DotDelayTimeProperty);
            set => SetValue(DotDelayTimeProperty, value);
        }

        protected abstract void UpdateDots();

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            UpdateDots();
        }

        protected virtual Ellipse CreateEllipse(int index)
        {
            var ellipse = new Ellipse();
            ellipse.SetBinding(WidthProperty, new Binding(DotDiameterProperty.Name) { Source = this });
            ellipse.SetBinding(HeightProperty, new Binding(DotDiameterProperty.Name) { Source = this });
            ellipse.SetBinding(Shape.FillProperty, new Binding(ForegroundProperty.Name) { Source = this });
            ellipse.SetBinding(Shape.StrokeThicknessProperty, new Binding(DotBorderThicknessProperty.Name) { Source = this });
            ellipse.SetBinding(Shape.StrokeProperty, new Binding(DotBorderBrushProperty.Name) { Source = this });
            return ellipse;
        }
    }

    public class LoadingCircle : LoadingBase
    {
        public static readonly DependencyProperty DotOffSetProperty = DependencyProperty.Register(
            nameof(DotOffSet), typeof(double), typeof(LoadingCircle),
            new FrameworkPropertyMetadata(20.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double DotOffSet
        {
            get => (double)GetValue(DotOffSetProperty);
            set => SetValue(DotOffSetProperty, value);
        }

        public static readonly DependencyProperty NeedHiddenProperty = DependencyProperty.Register(
            nameof(NeedHidden), typeof(bool), typeof(LoadingCircle),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool NeedHidden
        {
            get => (bool)GetValue(NeedHiddenProperty);
            set => SetValue(NeedHiddenProperty, value);
        }

        static LoadingCircle()
        { 
            DotSpeedProperty.OverrideMetadata(typeof(LoadingCircle),
                new FrameworkPropertyMetadata(6.0, FrameworkPropertyMetadataOptions.AffectsRender));
            DotDelayTimeProperty.OverrideMetadata(typeof(LoadingCircle),
                new FrameworkPropertyMetadata(220.0, FrameworkPropertyMetadataOptions.AffectsRender));
        }

        protected sealed override void UpdateDots()
        {
            var dotCount = DotCount;
            var dotInterval = DotInterval;
            var dotSpeed = DotSpeed;
            var dotDelayTime = DotDelayTime;
            var needHidden = NeedHidden;

            if (dotCount < 1)
            {
                return;
            }

            PrivateCanvas.Children.Clear();

            //定义动画
            Storyboard = new Storyboard
            {
                RepeatBehavior = RepeatBehavior.Forever
            };

            //创建圆点
            for (var i = 0; i < dotCount; i++)
            {
                var border = CreateBorder(i, dotInterval, needHidden);

                var framesMove = new DoubleAnimationUsingKeyFrames
                {
                    BeginTime = TimeSpan.FromMilliseconds(dotDelayTime * i)
                };

                var subAngle = -dotInterval * i;

                //开始位置
                var frame0 = new LinearDoubleKeyFrame
                {
                    Value = 0 + subAngle,
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero)
                };

                //开始位置到第一次匀速开始
                var frame1 = new EasingDoubleKeyFrame
                {
                    EasingFunction = new PowerEase
                    {
                        EasingMode = EasingMode.EaseOut
                    },
                    Value = 180 + subAngle,
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(dotSpeed * (0.75 / 7)))
                };

                //第一次匀速开始到第一次匀速结束
                var frame2 = new LinearDoubleKeyFrame
                {
                    Value = 180 + DotOffSet + subAngle,
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(dotSpeed * (2.75 / 7)))
                };

                //第一次匀速结束到匀加速结束
                var frame3 = new EasingDoubleKeyFrame
                {
                    EasingFunction = new PowerEase
                    {
                        EasingMode = EasingMode.EaseIn
                    },
                    Value = 360 + subAngle,
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(dotSpeed * (3.5 / 7)))
                };

                //匀加速结束到匀减速结束
                var frame4 = new EasingDoubleKeyFrame
                {
                    EasingFunction = new PowerEase
                    {
                        EasingMode = EasingMode.EaseOut
                    },
                    Value = 540 + subAngle,
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(dotSpeed * (4.25 / 7)))
                };

                //匀减速结束到第二次匀速结束
                var frame5 = new LinearDoubleKeyFrame
                {
                    Value = 540 + DotOffSet + subAngle,
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(dotSpeed * (6.25 / 7)))
                };

                //第二次匀速结束到匀加速结束
                var frame6 = new EasingDoubleKeyFrame
                {
                    EasingFunction = new PowerEase
                    {
                        EasingMode = EasingMode.EaseIn
                    },
                    Value = 720 + subAngle,
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(dotSpeed))
                };

                framesMove.KeyFrames.Add(frame0);
                framesMove.KeyFrames.Add(frame1);
                framesMove.KeyFrames.Add(frame2);
                framesMove.KeyFrames.Add(frame3);
                framesMove.KeyFrames.Add(frame4);
                framesMove.KeyFrames.Add(frame5);
                framesMove.KeyFrames.Add(frame6);

                Storyboard.SetTarget(framesMove, border);
                Storyboard.SetTargetProperty(framesMove, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)"));
                Storyboard.Children.Add(framesMove);

                if (NeedHidden)
                {
                    //隐藏一段时间
                    var frame7 = new DiscreteObjectKeyFrame
                    {
                        Value = Visibility.Collapsed,
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(dotSpeed))
                    };

                    var frame8 = new DiscreteObjectKeyFrame
                    {
                        Value = Visibility.Collapsed,
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(dotSpeed + 0.4))
                    };

                    var frame9 = new DiscreteObjectKeyFrame
                    {
                        Value = Visibility.Visible,
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero)
                    };
                    var framesVisibility = new ObjectAnimationUsingKeyFrames
                    {
                        BeginTime = TimeSpan.FromMilliseconds(dotDelayTime * i)
                    };
                    framesVisibility.KeyFrames.Add(frame9);
                    framesVisibility.KeyFrames.Add(frame7);
                    framesVisibility.KeyFrames.Add(frame8);
                    Storyboard.SetTarget(framesVisibility, border);
                    Storyboard.SetTargetProperty(framesVisibility, new PropertyPath("(UIElement.Visibility)"));
                    Storyboard.Children.Add(framesVisibility);
                }
                PrivateCanvas.Children.Add(border);
            }

            Storyboard.Begin();
            if (!IsRunning)
            {
                Storyboard.Stop();
            }
        }

        private Border CreateBorder(int index, double dotInterval, bool needHidden)
        {
            var ellipse = CreateEllipse(index);
            ellipse.HorizontalAlignment = HorizontalAlignment.Center;
            ellipse.VerticalAlignment = VerticalAlignment.Bottom;
            var rt = new RotateTransform
            {
                Angle = -dotInterval * index
            };
            var myTransGroup = new TransformGroup();
            myTransGroup.Children.Add(rt);
            var border = new Border
            {
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = myTransGroup,
                Child = ellipse,
                Visibility = needHidden ? Visibility.Collapsed : Visibility.Visible
            };
            border.SetBinding(WidthProperty, new Binding("Width") { Source = this });
            border.SetBinding(HeightProperty, new Binding("Height") { Source = this });

            return border;
        }
    }

    public class LoadingLine : LoadingBase
    {
        private const double MoveLength = 80;

        private const double UniformScale = .6;

        public LoadingLine()
        {
            SetBinding(HeightProperty, new Binding(nameof(DotDiameter)) { Source = this });
        }
 
        protected sealed override void UpdateDots()
        {
            var dotCount = DotCount;
            var dotInterval = DotInterval;
            var dotDiameter = DotDiameter;
            var dotSpeed = DotSpeed;
            var dotDelayTime = DotDelayTime;

            if (dotCount < 1)
            {
                return;
            }

            PrivateCanvas.Children.Clear();

            //计算相关尺寸
            var centerWidth = dotDiameter * dotCount + dotInterval * (dotCount - 1) + MoveLength;
            var speedDownLength = (ActualWidth - MoveLength) / 2;
            var speedUniformLength = centerWidth / 2;

            //定义动画
            Storyboard = new Storyboard
            {
                RepeatBehavior = RepeatBehavior.Forever
            };

            //创建圆点
            for (var i = 0; i < dotCount; i++)
            {
                var ellipse = CreateEllipse(i, dotInterval, dotDiameter);

                var frames = new ThicknessAnimationUsingKeyFrames
                {
                    BeginTime = TimeSpan.FromMilliseconds(dotDelayTime * i)
                };
                //开始位置
                var frame0 = new LinearThicknessKeyFrame
                {
                    Value = new Thickness(ellipse.Margin.Left, 0, 0, 0),
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero)
                };

                //开始位置到匀速开始
                var frame1 = new EasingThicknessKeyFrame
                {
                    EasingFunction = new PowerEase
                    {
                        EasingMode = EasingMode.EaseOut
                    },
                    Value = new Thickness(speedDownLength + ellipse.Margin.Left, 0, 0, 0),
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(dotSpeed * (1 - UniformScale) / 2))
                };

                //匀速开始到匀速结束
                var frame2 = new LinearThicknessKeyFrame
                {
                    Value = new Thickness(speedDownLength + speedUniformLength + ellipse.Margin.Left, 0, 0, 0),
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(dotSpeed * (1 + UniformScale) / 2))
                };

                //匀速结束到匀加速结束
                var frame3 = new EasingThicknessKeyFrame
                {
                    EasingFunction = new PowerEase
                    {
                        EasingMode = EasingMode.EaseIn
                    },
                    Value = new Thickness(ActualWidth + ellipse.Margin.Left + speedUniformLength, 0, 0, 0),
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(dotSpeed))
                };

                frames.KeyFrames.Add(frame0);
                frames.KeyFrames.Add(frame1);
                frames.KeyFrames.Add(frame2);
                frames.KeyFrames.Add(frame3);

                Storyboard.SetTarget(frames, ellipse);
                Storyboard.SetTargetProperty(frames, new PropertyPath(MarginProperty));
                Storyboard.Children.Add(frames);

                PrivateCanvas.Children.Add(ellipse);
            }

            Storyboard.Begin();
            if (!IsRunning)
            {
                Storyboard.Stop();
            }
        }

        private Ellipse CreateEllipse(int index, double dotInterval, double dotDiameter)
        {
            var ellipse = base.CreateEllipse(index);
            ellipse.HorizontalAlignment = HorizontalAlignment.Left;
            ellipse.VerticalAlignment = VerticalAlignment.Top;
            ellipse.Margin = new Thickness(-(dotInterval + dotDiameter) * index, 0, 0, 0);
            return ellipse;
        }
    }
}