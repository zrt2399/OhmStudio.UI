using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OhmStudio.UI.PublicMethods
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

        public static string ToUnicode(this string str)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(str))
            {
                return stringBuilder.ToString();
            }
            for (int i = 0; i < str.Length; i++)
            {
                byte[] bytes = Encoding.Unicode.GetBytes(str[i].ToString());
                stringBuilder.Append($"{bytes[1]:X2}{bytes[0]:X2}");
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
            for (int i = 0; i < count - 1; i++)
            {
                stringBuilder.Append(bytes.ElementAt(i).ToString("X2") + separator);
            }
            stringBuilder.Append(bytes.ElementAt(count - 1).ToString("X2"));
            return stringBuilder.ToString();
        }

        public static bool ToHexString(this IEnumerable<byte> bytes, string condition, string separator = " ")
        {
            return bytes.ToHexString(separator).Contains(condition);
        }
    }
}