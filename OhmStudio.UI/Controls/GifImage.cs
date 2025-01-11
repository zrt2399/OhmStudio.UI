using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OhmStudio.UI.Helpers;

namespace OhmStudio.UI.Controls
{
    public class GifImage : Control, IDisposable
    {
        static GifImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GifImage), new FrameworkPropertyMetadata(typeof(GifImage)));
        }

        private System.Windows.Controls.Image PART_Image;
        /// <summary>
        /// gif动画的<see cref="Bitmap"/>。
        /// </summary>
        private Bitmap gifBitmap;
        /// <summary>
        /// 用于显示每一帧的<see cref="BitmapSource"/>。
        /// </summary>
        private BitmapSource bitmapSource;

        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(GifImage), new PropertyMetadata(OnSourceChanged));

        private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GifImage image = sender as GifImage;
            if (!image.IsLoaded)
            {
                return;
            }

            if (e.NewValue == e.OldValue)
            {
                return;
            }

            image.Refresh();
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(GifImage));

        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        // Using a DependencyProperty as the backing store for Stretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(GifImage), new PropertyMetadata(Stretch.Uniform));

        private void Refresh()
        {
            if (PART_Image != null)
            {
                Dispatcher.Invoke(async () =>
                {
                    //防呆置空
                    if (Source == null)
                    {
                        StopAnimate();
                        gifBitmap?.Dispose();
                        if (gifBitmap != null)
                        {
                            gifBitmap = null;
                        }

                        PART_Image.Source = null;
                        return;
                    }
                    if (gifBitmap != null)
                    {
                        StopAnimate();
                        gifBitmap?.Dispose();
                    }
                    gifBitmap = await ImageHelper.GetBitmapAsync(Source);
                    bitmapSource = GetBitmapSource();
                    PART_Image.Source = bitmapSource;
                    StartAnimate();
                });
            }
        }

        /// <summary>
        /// 从<see cref="Bitmap"/>中获得用于显示的那一帧图像的<see cref="BitmapSource"/>。
        /// </summary>
        /// <returns></returns>
        private BitmapSource GetBitmapSource()
        {
            IntPtr handle = IntPtr.Zero;
            try
            {
                if (gifBitmap != null)
                {
                    handle = gifBitmap.GetHbitmap();
                    bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    ImageHelper.DeleteObject(handle);
                }
            }
            return bitmapSource;
        }

        /// <summary>
        /// Start.
        /// </summary>
        public void StartAnimate()
        {
            ImageAnimator.Animate(gifBitmap, OnFrameChanged);
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public void StopAnimate()
        {
            ImageAnimator.StopAnimate(gifBitmap, OnFrameChanged);
        }

        /// <summary>
        /// 帧处理。
        /// </summary>
        private void OnFrameChanged(object sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                ImageAnimator.UpdateFrames(); // 更新到下一帧
                bitmapSource?.Freeze();
                bitmapSource = GetBitmapSource();
                PART_Image.Source = bitmapSource;
                InvalidateVisual();
            }, DispatcherPriority.Render);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Image = GetTemplateChild("PART_Image") as System.Windows.Controls.Image;
            Loaded -= GifImage_Loaded;
            Loaded += GifImage_Loaded;
            Unloaded -= GifImage_Unloaded;
            Unloaded += GifImage_Unloaded;
        }

        private void GifImage_Unloaded(object sender, RoutedEventArgs e)
        {
            StopAnimate();
        }

        private void GifImage_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)
            }
            StopAnimate();
            gifBitmap?.Dispose();
            // TODO: 释放未托管的资源(未托管的对象)并重写终结器
            // TODO: 将大型字段设置为 null 
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~GifImage()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}