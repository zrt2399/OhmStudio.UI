using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace OhmStudio.UI.PublicMethod
{
    public static class ExtensionMethod
    {
        public static SolidColorBrush ToSolidColorBrush(this string hexString)
        {
            return new BrushConverter().ConvertFromString(hexString) as SolidColorBrush;
        }

        public static BitmapImage ToBitmapImage(this Bitmap bitmap, ImageFormat imageFormat)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }
            try
            {
                using MemoryStream stream = new MemoryStream();
                bitmap.Save(stream, imageFormat);
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
                bitmap?.Dispose();
            }
        }

        public static void InitCustomWindowStyle(this Window window)
        {
            if (window.AllowsTransparency)
            {
                FullScreenHelper.WPFWindowFullScreen(window);
            }
            else
            {
                window.Loaded += delegate
                {
                    window.Hide();
                    window.Show();
                };
            }
        }

        public static byte[] ToBytes(this Bitmap bitmap)
        {
            int m = 0, value = 0;
            int height = bitmap.Height;
            int Gwidth = bitmap.Width;
            switch (height % 8)
            {
                case 0:
                    height += 0;
                    break;
                case 1:
                    height += 7;
                    break;
                case 2:
                    height += 6;
                    break;
                case 3:
                    height += 5;
                    break;
                case 4:
                    height += 4;
                    break;
                case 5:
                    height += 3;
                    break;
                case 6:
                    height += 2;
                    break;
                case 7:
                    height += 1;
                    break;
                default:
                    break;
            }
            byte[] bytesTemp = new byte[height / 8 * Gwidth];
            if (bitmap.Palette.Entries.Length == 2)//单色位图
            {
                for (int j = 0; j < Gwidth; j++)
                {
                    for (int i = 0; i < height / 8; i++)
                    {
                        for (int k = 0; k < 8; k++)
                        {
                            if (i * 8 + k < bitmap.Height)
                            {
                                Color color = bitmap.GetPixel(j, i * 8 + k);
                                if (color.R == Color.Black.R && color.G == Color.Black.G && color.B == Color.Black.B)
                                {
                                    //bytesTemp[j * Gheight / 8 + i] = (byte)((byte)(bytesTemp[j * Gheight / 8 + i]) + (byte)(0x80 >> k));
                                    value = (value << 1) | 1;
                                }
                                else
                                {
                                    //bytesTemp[j * Gheight / 8 + i] = (byte)((byte)(bytesTemp[j * Gheight / 8 + i]) + 0);
                                    value <<= 1;
                                }
                            }
                            else
                            {
                                //bytesTemp[j * Gheight / 8 + i] = (byte)((byte)(bytesTemp[j * Gheight / 8 + i]) + 0);
                                value <<= 1;
                            }
                        }
                        bytesTemp[m++] = (byte)value;
                        value = 0;
                    }
                }
                return bytesTemp;
            }
            else
            {
                return null;
            }
        }
    }

    public class PropertyGridAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public PropertyGridAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}