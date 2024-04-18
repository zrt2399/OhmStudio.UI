using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using OhmStudio.UI.Attaches;
using OhmStudio.UI.Helpers;
using OhmStudio.UI.Views;
using Color = System.Drawing.Color;
using Expression = System.Linq.Expressions.Expression;

namespace OhmStudio.UI.PublicMethods
{
    public static class ExtensionMethod
    {
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

        public static bool IsTextBoxAttachObject(this DependencyObject obj)
        {
            if (obj is ComboBox || obj is TextBox || obj is DateTimePicker || obj is PasswordBox || obj is PasswordTextBox)
            {
                return true;
            }
            return false;
        }

        public static SolidColorBrush ToSolidColorBrush(this string hexString)
        {
            return new BrushConverter().ConvertFromString(hexString) as SolidColorBrush;
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

        public static T FindParentFrameworkElement<T>(this FrameworkElement obj) where T : FrameworkElement
        {
            FrameworkElement parent = obj.Parent as FrameworkElement;
            while (parent != null)
            {
                if (parent is T t)
                {
                    return t;
                }
                parent = (FrameworkElement)parent.Parent;
            }
            return null;
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
        public bool CanShowPassword { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ToolTipAttribute : Attribute
    {
        public string ToolTip { get; set; }
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
            if (tIn == null)
            {
                return default;
            }
            return _cache(tIn);
        }
    }

    public class PropertyValue<T>
    {
        private static ConcurrentDictionary<string, MemberGetDelegate> _memberGetDelegate = new ConcurrentDictionary<string, MemberGetDelegate>();
        delegate object MemberGetDelegate(T obj);
        public PropertyValue(T obj)
        {
            Target = obj;
        }

        public T Target { get; private set; }

        public object Get(string name)
        {
            MemberGetDelegate memberGet = _memberGetDelegate.GetOrAdd(name, BuildDelegate);
            return memberGet(Target);
        }

        private MemberGetDelegate BuildDelegate(string name)
        {
            Type type = typeof(T);
            PropertyInfo property = type.GetProperty(name);
            return (MemberGetDelegate)Delegate.CreateDelegate(typeof(MemberGetDelegate), property.GetGetMethod());
        }
    }
}