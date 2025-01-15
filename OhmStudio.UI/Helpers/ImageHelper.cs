using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OhmStudio.UI.Utilities;

namespace OhmStudio.UI.Helpers
{
    public enum ImageType
    {
        Bmp, Png, Jpeg
    }

    public static class ImageHelper
    {
        /// <summary>
        /// 图片转化。
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static BitmapImage GetBitmapImage(this Uri uri)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.UriSource = uri;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }

        /// <summary>
        /// 删除本地 bitmap resource。
        /// </summary>
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// ImageSource转Bitmap。
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <returns></returns>
        public static async Task<Bitmap> GetBitmapAsync(this ImageSource bitmapSource)
        {
            var uriString = bitmapSource.ToString();
            Uri uri = new Uri(uriString);
            if (uriString.IsContained("pack://siteoforigin:"))
            {
                var localUri = Path.Combine(Environment.CurrentDirectory, uri.LocalPath);
                return new Bitmap(localUri);
            }
            if (uriString.IsContained("pack://application:"))
            {
                Stream stream = Application.GetResourceStream(uri).Stream;
                return new Bitmap(stream);
            }
            if (uriString.IsContained("http://") || uriString.IsContained("https://"))
            {
                using var httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode(); // 确保请求成功
                using Stream stream = await response.Content.ReadAsStreamAsync();

                //using Stream stream = (await WebRequest.Create(uri).GetResponseAsync()).GetResponseStream();
                return new Bitmap(stream);
            }
            string file = uriString;
            return new Bitmap(file);
        }

        public static Bitmap ToBitmap(this BitmapImage bitmapImage, BitmapEncoder bitmapEncoder)
        {
            return BitmapImageToBitmap(bitmapImage, bitmapEncoder);
        }

        public static BitmapImage ToBitmapImage(this Bitmap bitmap, ImageFormat imageFormat = null, bool isDisposeBitmap = true)
        {
            return BitmapToBitmapImage(bitmap, imageFormat, isDisposeBitmap);
        }

        public static BitmapSource ToBitmapSource(this Stream stream, bool isDisposeStream = true)
        {
            return StreamToBitmapSource(stream, isDisposeStream);
        }

        /// <summary>
        ///  BitmapImage转Bitmap。
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <param name="bitmapEncoder">编码器。</param>
        /// <returns></returns>
        public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage, BitmapEncoder bitmapEncoder)
        {
            using MemoryStream memoryStream = new MemoryStream();
            BitmapFrame bitmapFrame = BitmapFrame.Create(bitmapImage);
            bitmapEncoder.Frames.Add(bitmapFrame);
            bitmapEncoder.Save(memoryStream);
            return new Bitmap(memoryStream);
        }

        public static BitmapSource StreamToBitmapSource(Stream stream, bool isDisposeStream = true)
        {
            IntPtr handle = IntPtr.Zero;
            try
            {
                using var bitmap = new Bitmap(stream);
                handle = bitmap.GetHbitmap();
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    DeleteObject(handle);
                }
                if (isDisposeStream)
                {
                    stream.Dispose();
                }
            }
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap, ImageFormat imageFormat = default, bool isDisposeBitmap = true)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }
            try
            {
                using MemoryStream memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, imageFormat ?? ImageFormat.Bmp);
                memoryStream.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
            finally
            {
                if (isDisposeBitmap)
                {
                    bitmap.Dispose();
                }
            }
        }

        /// <summary>
        /// 保存控件为png。
        /// </summary>
        /// <param name="uIElement"></param>
        /// <param name="filePath"></param>
        /// <param name="imageType"></param>
        /// <param name="dpi"></param>
        public static void SaveAsImage(this UIElement uIElement, string filePath, ImageType imageType, int dpi = 300)
        {
            // Calculate the render size based on the desired DPI
            var renderWidth = (int)(uIElement.RenderSize.Width * dpi / 96);
            var renderHeight = (int)(uIElement.RenderSize.Height * dpi / 96);

            // Create a render target bitmap and render the control on it
            var renderTargetBitmap = new RenderTargetBitmap(renderWidth, renderHeight, dpi, dpi, PixelFormats.Default);
            renderTargetBitmap.Render(uIElement);
            BitmapEncoder encoder = imageType switch
            {
                ImageType.Bmp => new BmpBitmapEncoder(),
                ImageType.Jpeg => new JpegBitmapEncoder(),
                _ => new PngBitmapEncoder()
            };
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            encoder.Save(fileStream);
        }

        public static Bitmap ToBitmap(this BitmapSource bitmapSource)
        {
            return BitmapSourceToBitmap(bitmapSource);
        }

        public static Bitmap ToBitmap(this BitmapSource bitmapSource, int width, int height)
        {
            return BitmapSourceToBitmap(bitmapSource, width, height);
        }

        /// <summary>
        /// BitmapSource转Bitmap。
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <returns></returns>
        public static Bitmap BitmapSourceToBitmap(BitmapSource bitmapSource)
        {
            return BitmapSourceToBitmap(bitmapSource, bitmapSource.PixelWidth, bitmapSource.PixelHeight);
        }

        /// <summary>
        /// Convert BitmapSource to Bitmap.
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap BitmapSourceToBitmap(BitmapSource bitmapSource, int width, int height)
        {
            Bitmap bmp = null;
            try
            {
                System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                /*set the translate type according to the in param(source)*/
                switch (bitmapSource.Format.ToString())
                {
                    case "Rgb24":
                    case "Bgr24":
                        format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                        break;
                    case "Bgra32":
                        format = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
                        break;
                    case "Bgr32":
                        format = System.Drawing.Imaging.PixelFormat.Format32bppRgb;
                        break;
                    case "Pbgra32":
                        format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                        break;
                }
                bmp = new Bitmap(width, height, format);
                BitmapData data = bmp.LockBits(new Rectangle(System.Drawing.Point.Empty, bmp.Size),
                    ImageLockMode.WriteOnly,
                    format);
                bitmapSource.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
                bmp.UnlockBits(data);
            }
            catch
            {
                if (bmp != null)
                {
                    bmp.Dispose();
                    bmp = null;
                }
            }

            return bmp;
        }
    }
}