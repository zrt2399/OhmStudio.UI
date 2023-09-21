using System.Collections.Generic;
using System.Text;

namespace OhmStudio.UI.PublicMethod
{
    public static class Transformation
    {
        public static bool IsNotASCII(this string value)
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

        public static string ToHexString(this List<byte> bytes)
        {
            return bytes?.ToArray().ToHexString();
        }

        public static string ToHexString(this byte[] bytes, string separator = " ")
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (bytes?.Length > 0)
            {
                for (int i = 0; i < bytes.Length - 1; i++)
                {
                    stringBuilder.Append(bytes[i].ToString("X2") + separator);
                }
                stringBuilder.Append(bytes[bytes.Length - 1].ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        public static bool ToHexString(this byte[] bytes, string condition, string separator = " ")
        {
            return bytes.ToHexString(separator).Contains(condition);
        }
    }
}