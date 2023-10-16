using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OhmStudio.UI.Attachs;
using OhmStudio.UI.Views;
using Brush = System.Windows.Media.Brush;
using Color = System.Drawing.Color;
using Expression = System.Linq.Expressions.Expression;

namespace OhmStudio.UI.PublicMethod
{
    public static class ExtensionMethod
    {
        public static bool IsTextBoxAttachObject(this DependencyObject obj)
        {
            if (obj is TextBox || obj is DateTimePicker || obj is PasswordBox || obj is PasswordBoxControl)
            {
                return true;
            }
            return false;
        }

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

        public static T FindParentObject<T>(this DependencyObject obj) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                if (parent is T t)
                {
                    return t;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        public static T FindFirstChild<T>(this DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T matchingChild)
                {
                    return matchingChild;
                }
                // 递归查找子元素的子元素
                var subChild = FindFirstChild<T>(child);
                if (subChild != null)
                {
                    return subChild;
                }
            }
            return null;
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                    {
                        yield return t;
                    }
                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
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

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyGridAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public bool IsReadOnly { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyGridIgnoreAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyChangedUpdateSourceAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PasswordAttribute : Attribute
    {
        public Visibility CanShowPassword { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyGridPlaceHolderAttribute : Attribute
    {
        public string PlaceHolder { get; set; } = TextBoxAttach.PlaceHolder;

        public Brush PlaceHolderForeground { get; set; } = TextBoxAttach.PlaceHolderForeground;

        public double PlaceHolderOpacity { get; set; } = TextBoxAttach.PlaceHolderOpacity;

        public Thickness PlaceHolderMargin { get; set; } = TextBoxAttach.PlaceHolderMargin;
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class BaseObjectIgnoreAttribute : Attribute
    {
    }

    public static class DeepClone<TIn, TOut>
    {
        private static readonly Func<TIn, TOut> _cache = GetFunc();

        private static Func<TIn, TOut> GetFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");

            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite)
                {
                    continue;
                }
                MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));

                MemberBinding memberBinding = Expression.Bind(item, property);

                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());

            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }

        public static TOut Clone(TIn tIn)
        {
            return _cache(tIn);
        }
    }
}