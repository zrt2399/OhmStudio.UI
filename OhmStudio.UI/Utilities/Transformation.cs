using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace OhmStudio.UI.Utilities
{
    public static class Transformation
    {
        public static bool IsNotAscii(this string value)
        {
            foreach (var item in value)
            {
                if (item > 127)
                {
                    return true;
                }
            }
            return false;
        }

        public static string ToUnicode(this string value, bool bigEndian = true)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                byte[] bytes = (bigEndian ? Encoding.BigEndianUnicode : Encoding.Unicode).GetBytes(value[i].ToString());
                stringBuilder.Append($"{bytes[0]:X2}{bytes[1]:X2}");
            }
            return stringBuilder.ToString();
        }

        public static string ToHexString(this IEnumerable<byte> bytes, string separator = " ")
        {
            if (bytes == null)
            {
                return string.Empty;
            }
            var count = bytes.Count();
            if (count == 0)
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                stringBuilder.Append(bytes.ElementAt(i).ToString("X2"));
                if (i < count - 1)
                {
                    stringBuilder.Append(separator);
                }
            }
            return stringBuilder.ToString();
        }

        public static bool ToHexString(this IEnumerable<byte> bytes, string condition, string separator = " ")
        {
            return bytes.ToHexString(separator).Contains(condition);
        }

        public static byte[] ToBytes(this Bitmap bitmap)
        {
            int m = 0, value = 0;
            int height = bitmap.Height;
            int width = bitmap.Width;

            if (height % 8 != 0)
            {
                height += 8 - height % 8;
            }

            byte[] bytesTemp = new byte[height / 8 * width];
            if (bitmap.Palette.Entries.Length != 2)//不是单色位图
            {
                return null;
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height / 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        if (j * 8 + k < bitmap.Height)
                        {
                            Color color = bitmap.GetPixel(i, j * 8 + k);
                            if (color.R == Color.Black.R && color.G == Color.Black.G && color.B == Color.Black.B)
                            {
                                value = value << 1 | 1;
                            }
                            else
                            {
                                value <<= 1;
                            }
                        }
                        else
                        {
                            value <<= 1;
                        }
                    }
                    bytesTemp[m++] = (byte)value;
                    value = 0;
                }
            }
            return bytesTemp;
        }

        public static double Adsorb(this double value, uint gridSpacing)
        {
            if (double.IsNaN(value))
            {
                return 0;
            }
            int quotient = (int)(value / gridSpacing);

            double min, max;
            if (value < 0)
            {
                max = gridSpacing * quotient;
                min = max - gridSpacing;
            }
            else
            {
                min = gridSpacing * quotient;
                max = min + gridSpacing;
            }

            if (value - min > gridSpacing / 2)
            {
                return max;
            }
            else
            {
                return min;
            }
        }

        /// <summary> Wraps a value within a specified range.</summary>
        public static double WrapToRange(this double value, double min, double max)
        {
            double range = max - min;
            value = (value - min) % range;

            return value < 0 ? value + range + min : value + min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEven(this int number)
        {
            return (number & 1) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEven(this long number)
        {
            return (number & 1L) == 0L;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOdd(this int number)
        {
            return (number & 1) == 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOdd(this long number)
        {
            return (number & 1L) == 1L;
        }
    }
}