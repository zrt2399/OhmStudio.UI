using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using OhmStudio.UI.Attaches;
using OhmStudio.UI.Controls;
using OhmStudio.UI.Helpers;
using Color = System.Drawing.Color;
using Expression = System.Linq.Expressions.Expression;

namespace OhmStudio.UI.PublicMethods
{
    public static class ExtensionMethod
    {
        public static T GetFirstVisualHit<T>(this Visual visual, System.Windows.Point point) where T : DependencyObject
        {
            List<T> hitElements = new List<T>();

            // 使用HitTest方法和回调函数获取所有命中的控件
            VisualTreeHelper.HitTest(
                visual,
                null,
                new HitTestResultCallback(
                    result =>
                    {
                        HitTestResultBehavior behavior = HitTestResultBehavior.Continue;
                        if (result.VisualHit is FrameworkElement frameworkElement && frameworkElement.TemplatedParent is T t && frameworkElement.IsVisible)
                        {
                            hitElements.Add(t);
                        }
                        return behavior;
                    }),
                new PointHitTestParameters(point));

            return hitElements.FirstOrDefault();
        }

        public static T GetVisualHit<T>(this Visual visual, System.Windows.Point point) where T : DependencyObject
        {
            var hitObject = VisualTreeHelper.HitTest(visual, point)?.VisualHit;
            return hitObject?.FindParentObject<T>();
        }

        public static double ToPhysicalPixels(this double logicalPixels)
        {
            return logicalPixels * VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip;
        }

        public static bool ComparePropertiesWith<T>(this T t1, T t2, bool ignoreBaseProperties = false)
        {
            return PropertyComparer<T>.CompareProperties(t1, t2, ignoreBaseProperties);
        }

        public static T CloneProperties<T>(this T t)
        {
            return PropertyCloner<T>.CloneProperties(t);
        }

        /// <summary>
        /// Returns full visual ancestry, starting at the leaf.
        /// <para>If element is not of <see cref="Visual"/> or <see cref="Visual3D"/> the
        /// logical ancestry is used.</para>
        /// </summary>
        /// <param name="leaf"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetVisualAncestry(this DependencyObject leaf)
        {
            while (leaf is not null)
            {
                yield return leaf;
                leaf = leaf is Visual or Visual3D ? VisualTreeHelper.GetParent(leaf) : LogicalTreeHelper.GetParent(leaf);
            }
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

        public static bool IsContains(this string str, string value, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            return str.IndexOf(value, stringComparison) >= 0;
        }

        public static Window SetOwner(this Window window, Window owner = null)
        {
            if (owner == null)
            {
                var app = Application.Current;
                if (app != null)
                {
                    window.Owner = app.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive && x.IsLoaded);
                    if (window.Owner == null && window != app.MainWindow && app.MainWindow.IsLoaded)
                    {
                        window.Owner = app.MainWindow;
                    }
                }
            }
            else
            {
                window.Owner = owner;
            }

            window.WindowStartupLocation = window.Owner == null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner;
            return window;
        }

        public static bool IsPlaceHolderObject(this DependencyObject obj)
        {
            var type = obj.GetType();
            if (type == typeof(ComboBox) || type == typeof(TextBox) || type == typeof(PasswordBox) || obj is ITextChanged)
            {
                return true;
            }
            return false;
        }

        public static SolidColorBrush ToSolidColorBrush(this string hexString)
        {
            return new BrushConverter().ConvertFromString(hexString) as SolidColorBrush;
        }

        public static void SaveAsImage(this UIElement uIElement, string filePath, ImageFormat imageFormat, int dpi = 300)
        {
            ImageHelper.SaveAsImage(uIElement, filePath, imageFormat, dpi);
        }

        public static Bitmap ToBitmap(this BitmapImage bitmapImage)
        {
            return ImageHelper.BitmapImageToBitmap(bitmapImage);
        }

        public static BitmapImage ToBitmapImage(this Bitmap bitmap, ImageFormat imageFormat = null, bool isDisposeBitmap = true)
        {
            return ImageHelper.BitmapToBitmapImage(bitmap, imageFormat, isDisposeBitmap);
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
            DependencyObject parent = obj is Visual or Visual3D ? VisualTreeHelper.GetParent(obj) : LogicalTreeHelper.GetParent(obj);
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

        /// <summary>
        /// Finds a Child of a given item in the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <param name="childName">x:Name or Name of child.</param>
        /// <returns>The first parent item that matches the submitted type parameter. If not matching item can be found, a null parent is being returned.</returns>
        public static T FindChild<T>(this DependencyObject parent, string childName = null) where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            T val = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is not T)
                {
                    val = child.FindChild<T>(childName);
                    if (val != null)
                    {
                        break;
                    }

                    continue;
                }

                if (!string.IsNullOrEmpty(childName))
                {
                    if (child is IFrameworkInputElement frameworkInputElement && frameworkInputElement.Name == childName)
                    {
                        val = (T)child;
                        break;
                    }

                    val = child.FindChild<T>(childName);
                    if (val != null)
                    {
                        break;
                    }

                    continue;
                }

                val = (T)child;
                break;
            }

            return val;
        }

        /// <summary>
        /// Analyzes both visual and logical tree in order to find all elements of a given type that are descendants of the source item.
        /// </summary>
        /// <typeparam name="T">The type of the queried items.</typeparam>
        /// <param name="source">The root element that marks the source of the search. If the source is already of the requested type, it will not be included in the result.</param>
        /// <param name="forceUsingTheVisualTreeHelper">Sometimes it's better to search in the VisualTree (e.g. in tests)</param>
        /// <returns>All descendants of source that match the requested type.</returns>
        public static IEnumerable<T> FindChildren<T>(this DependencyObject source, bool forceUsingTheVisualTreeHelper = false) where T : DependencyObject
        {
            if (source == null)
            {
                yield break;
            }

            IEnumerable<DependencyObject> childObjects = source.GetChildObjects(forceUsingTheVisualTreeHelper);
            foreach (DependencyObject child in childObjects)
            {
                if (child != null && child is T t)
                {
                    yield return t;
                }

                foreach (T item in child.FindChildren<T>(forceUsingTheVisualTreeHelper))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// This method is an alternative to WPF's System.Windows.Media.VisualTreeHelper.GetChild(System.Windows.DependencyObject,System.Int32)
        /// method, which also supports content elements.Keep in mind that for content elements, this method falls back to the logical tree of the element.
        /// </summary>
        /// <param name="parent">The item to be processed.</param>
        /// <param name="forceUsingTheVisualTreeHelper">Sometimes it's better to search in the VisualTree (e.g. in tests)</param>
        /// <returns>The submitted item's child elements, if available.</returns>
        public static IEnumerable<DependencyObject> GetChildObjects(this DependencyObject parent, bool forceUsingTheVisualTreeHelper = false)
        {
            if (parent == null)
            {
                yield break;
            }

            if (!forceUsingTheVisualTreeHelper && (parent is ContentElement || parent is FrameworkElement))
            {
                foreach (object child in LogicalTreeHelper.GetChildren(parent))
                {
                    if (child is DependencyObject dependencyObject)
                    {
                        yield return dependencyObject;
                    }
                }
            }
            else if (parent is Visual || parent is Visual3D)
            {
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    yield return VisualTreeHelper.GetChild(parent, i);
                }
            }
        }

        public static byte[] ToBytes(this Bitmap bitmap)
        {
            int m = 0, value = 0;
            int height = bitmap.Height;
            int width = bitmap.Width;

            if (height % 8 != 0)
            {
                height += 8 - (height % 8);
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
                                value = (value << 1) | 1;
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
        public bool CanShowPassword { get; set; } = true;

        public char PasswordChar { get; set; } = PasswordTextBox.DefaultPasswordChar;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ToolTipAttribute : Attribute
    {
        public string ToolTip { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class VirtualizingPanelAttribute : Attribute
    {
        public bool IsVirtualizing { get; set; } = true;

        public ScrollUnit ScrollUnit { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class TextBoxPlaceHolderAttribute : Attribute
    {
        public string PlaceHolder { get; set; } = TextBoxAttach.PlaceHolder;

        public double PlaceHolderOpacity { get; set; } = TextBoxAttach.PlaceHolderOpacity;

        public bool PlaceHolderIsHitTestVisible { get; set; } = TextBoxAttach.PlaceHolderIsHitTestVisible;
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class BaseObjectIgnoreAttribute : Attribute
    {
    }

    public static class PropertyCloner<T>
    {
        private static readonly ConcurrentDictionary<Type, Func<T, T>> _cloneFuncCache = new ConcurrentDictionary<Type, Func<T, T>>();

        public static T CloneProperties(T source)
        {
            var type = typeof(T);
            if (_cloneFuncCache.TryGetValue(type, out var cloneFunc))
            {
                return cloneFunc(source);
            }

            var param = Expression.Parameter(typeof(object), "x");
            var convertedParam = Expression.Convert(param, type);

            var memberBindings = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .Select(p => Expression.Bind(p, Expression.Property(convertedParam, p)));

            var memberInit = Expression.MemberInit(Expression.New(type), memberBindings);
            var lambda = Expression.Lambda<Func<T, T>>(memberInit, param);

            cloneFunc = lambda.Compile();
            _cloneFuncCache[type] = cloneFunc;

            return cloneFunc(source);
        }
    }

    public static class PropertyComparer<T>
    {
        private class CacheKey
        {
            public Type Type { get; }

            public bool IgnoreBaseProperties { get; }

            public CacheKey(Type type, bool ignoreBaseProperties)
            {
                Type = type;
                IgnoreBaseProperties = ignoreBaseProperties;
            }

            public override bool Equals(object obj)
            {
                if (obj is not CacheKey other)
                {
                    return false;
                }

                return Type == other.Type && IgnoreBaseProperties == other.IgnoreBaseProperties;
            }

            public override int GetHashCode()
            {
                return Type.GetHashCode() ^ IgnoreBaseProperties.GetHashCode();
            }
        }

        private static readonly ConcurrentDictionary<CacheKey, Func<T, T, bool>> _cache = new ConcurrentDictionary<CacheKey, Func<T, T, bool>>();

        public static bool CompareProperties(T obj1, T obj2, bool ignoreBaseProperties = false)
        {
            var type = typeof(T);
            var cacheKey = new CacheKey(type, ignoreBaseProperties);
            if (_cache.TryGetValue(cacheKey, out var comparer))
            {
                return comparer(obj1, obj2);
            }

            var param1 = Expression.Parameter(type, "x");
            var param2 = Expression.Parameter(type, "y");

            Expression body = null;

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            if (ignoreBaseProperties)
            {
                flags |= BindingFlags.DeclaredOnly;
            }

            foreach (var property in type.GetProperties(flags))
            {
                if (!property.CanRead)
                {
                    continue;
                }

                var prop1 = Expression.Property(param1, property);
                var prop2 = Expression.Property(param2, property);

                var equals = Expression.Equal(prop1, prop2);

                if (body == null)
                {
                    body = equals;
                }
                else
                {
                    body = Expression.AndAlso(body, equals);
                }
            }

            body ??= Expression.Constant(true);

            var lambda = Expression.Lambda<Func<T, T, bool>>(body, param1, param2);
            var compiled = lambda.Compile();

            _cache[cacheKey] = compiled;
            return compiled(obj1, obj2);
        }
    }
}