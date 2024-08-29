using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Helpers
{
    public class ImageHelper
    {
        /// <summary>
        /// 图片转化。
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <returns></returns>
        public static BitmapImage GetBitmapImage(Uri imagePath)
        {
            if (imagePath == null || string.IsNullOrEmpty(imagePath.ToString().Trim()))
            {
                return null;
            }

            BitmapImage bitmap = new BitmapImage();
            if (File.Exists(imagePath.LocalPath))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                using Stream ms = new MemoryStream(File.ReadAllBytes(imagePath.LocalPath));
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();
            }
            return bitmap;
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
        /// <param name="source"></param>
        /// <returns></returns>
        public static async Task<Bitmap> GetBitmapAsync(ImageSource source)
        {
            var uriString = source.ToString();
            Uri uri = new Uri(uriString);
            if (uriString.IsContains("pack://siteoforigin:"))
            {
                var localUri = Environment.CurrentDirectory + uri.LocalPath;
                return new Bitmap(localUri);
            }
            if (uriString.IsContains("pack://application:"))
            {
                return new Bitmap(Application.GetResourceStream(uri).Stream);
            }
            if (uriString.IsContains("http:") || uriString.IsContains("https:"))
            {
                using Stream stream = (await WebRequest.Create(uri).GetResponseAsync()).GetResponseStream();
                return new Bitmap(stream);
            }
            string file = uriString;
            return new Bitmap(file);
        }

        /// <summary>
        /// BitmapImage转Bitmap。
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using MemoryStream outStream = new MemoryStream();
            BitmapEncoder enc = new BmpBitmapEncoder();
            BitmapFrame bitmapFrame = BitmapFrame.Create(bitmapImage);
            enc.Frames.Add(bitmapFrame);
            enc.Save(outStream);
            return new Bitmap(outStream);
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap, ImageFormat imageFormat = null, bool isDisposeBitmap = true)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }
            try
            {
                using MemoryStream stream = new MemoryStream();
                bitmap.Save(stream, imageFormat ?? ImageFormat.Bmp);
                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
            finally
            {
                if (isDisposeBitmap)
                {
                    bitmap?.Dispose();
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
        public static void SaveAsImage(UIElement uIElement, string filePath, ImageType imageType, int dpi = 300)
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

        /// <summary>
        /// BitmapSource转Bitmap。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Bitmap BitmapSourceToBitmap(BitmapSource source)
        {
            return BitmapSourceToBitmap(source, source.PixelWidth, source.PixelHeight);
        }

        /// <summary>
        /// Convert BitmapSource to Bitmap.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap BitmapSourceToBitmap(BitmapSource source, int width, int height)
        {
            Bitmap bmp = null;
            try
            {
                System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                /*set the translate type according to the in param(source)*/
                switch (source.Format.ToString())
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
                source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
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