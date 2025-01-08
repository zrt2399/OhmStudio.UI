using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using OhmStudio.UI.Controls;
using OhmStudio.UI.Helpers;
using Expression = System.Linq.Expressions.Expression;

namespace OhmStudio.UI.Utilities
{
    public static class ExtensionMethod
    {
        public static void StartAnimation(this UIElement animatableElement, DependencyProperty dependencyProperty, Point toValue, double animationDurationSeconds, EventHandler completedEvent = null)
        {
            var fromValue = (Point)animatableElement.GetValue(dependencyProperty);

            PointAnimation animation = new PointAnimation
            {
                From = fromValue,
                To = toValue,
                Duration = TimeSpan.FromSeconds(animationDurationSeconds)
            };

            animation.Completed += delegate (object sender, EventArgs e)
            {
                animatableElement.SetValue(dependencyProperty, animatableElement.GetValue(dependencyProperty));
                animatableElement.CancelAnimation(dependencyProperty);

                completedEvent?.Invoke(sender, e);
            };

            animation.Freeze();

            animatableElement.BeginAnimation(dependencyProperty, animation);
        }

        public static void StartLoopingAnimation(this UIElement animatableElement, DependencyProperty dependencyProperty, double toValue, double durationInSeconds)
        {
            var fromValue = (double)animatableElement.GetValue(dependencyProperty);

            var animation = new DoubleAnimation
            {
                From = fromValue,
                To = toValue,
                Duration = TimeSpan.FromSeconds(durationInSeconds)
            };

            animation.RepeatBehavior = RepeatBehavior.Forever;

            animation.Freeze();
            animatableElement.BeginAnimation(dependencyProperty, animation);
        }

        public static void CancelAnimation(this UIElement animatableElement, DependencyProperty dependencyProperty)
        {
            animatableElement.BeginAnimation(dependencyProperty, null);
        }

        public static Geometry DrawPolygon(this DrawingContext dc, Brush brush, Pen pen, params Point[] points)
        {
            if (!points.Any())
            {
                return Geometry.Empty;
            }

            var geometry = new StreamGeometry();
            using (var geometryContext = geometry.Open())
            {
                geometryContext.BeginFigure(points[0], true, true);
                for (var i = 1; i < points.Length; i++)
                {
                    geometryContext.LineTo(points[i], true, false);
                }
            }
            geometry.Freeze();
            dc.DrawGeometry(brush, pen, geometry);
            return geometry;
        }

        public static IEnumerable<T> GetVisualHitsOfZ<T>(this Visual visual, Point point) where T : DependencyObject
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
                        if (result.VisualHit is T t1 && !hitElements.Contains(t1))
                        {
                            hitElements.Add(t1);
                        }
                        if (result.VisualHit.GetParentOfType<T>() is T t2 && !hitElements.Contains(t2))
                        {
                            hitElements.Add(t2);
                        }
                        return behavior;
                    }),
                new PointHitTestParameters(point));

            return hitElements;
        }

        public static T GetVisualHitOfType<T>(this Visual visual, Point point)
        {
            var hitObject = VisualTreeHelper.HitTest(visual, point)?.VisualHit;
            if (hitObject == null)
            {
                return default;
            }
            else
            {
                return hitObject is T t ? t : hitObject.GetParentOfType<T>();
            }
        }

        public static double ToPhysicalPixels(this double logicalPixels)
        {
            return logicalPixels * VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip;
        }

        public static bool IsEqualTo<T>(this T t1, T t2, bool ignoreBase = false)
        {
            if (t1 == null && t2 == null)
            {
                return true;
            }

            // 如果其中一个为 null，另一个不为 null，则不相等
            if (t1 == null || t2 == null)
            {
                return false;
            }

            return PropertyComparer<T>.IsEqualTo(t1, t2, ignoreBase);
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

        public static bool IsContained(this string str, string value, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
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

        public static void ScrollToEnd(this Selector selector)
        {
            if (VisualTreeHelper.GetChildrenCount(selector) > 0) // 先检查子元素数量
            {
                if (VisualTreeHelper.GetChild(selector, 0) is Decorator border)
                {
                    if (border.Child is ScrollViewer scrollViewer)
                    {
                        scrollViewer.ScrollToEnd();
                    }
                }
            }
        }

        public static SolidColorBrush ToSolidColorBrush(this string hexString)
        {
            return new BrushConverter().ConvertFromString(hexString) as SolidColorBrush;
        }

        public static void InitCustomWindowStyle(this Window window)
        {
            if (window.AllowsTransparency)
            {
                FullScreenHelper.SetWPFWindowFullScreen(window);
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

        public static UIElement GetFirstFocusable(this object obj)
        {
            if (obj is not UIElement uIElement)
            {
                return null;
            }
            if (uIElement.Focusable)
            {
                return uIElement;
            }
            return uIElement.FindChildrenOfType<UIElement>().FirstOrDefault(x => x.Focusable);
        }

        public static T GetParentOfType<T>(this DependencyObject obj)
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
            return default;
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <param name="childName">x:Name or Name of child.</param>
        /// <returns>The first parent item that matches the submitted type parameter. If not matching item can be found, a null parent is being returned.</returns>
        public static T FindChildOfType<T>(this DependencyObject parent, string childName = null) where T : DependencyObject
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
                    val = child.FindChildOfType<T>(childName);
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

                    val = child.FindChildOfType<T>(childName);
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
        public static IEnumerable<T> FindChildrenOfType<T>(this DependencyObject source, bool forceUsingTheVisualTreeHelper = false) where T : DependencyObject
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

                foreach (T item in child.FindChildrenOfType<T>(forceUsingTheVisualTreeHelper))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// This method is an alternative to WPF's <see cref="VisualTreeHelper.GetChild"/> method,
        /// which also supports content elements.Keep in mind that for content elements, this method falls back to the logical tree of the element.
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

        public static bool IsEqualTo(T obj1, T obj2, bool ignoreBase = false)
        {
            var type = typeof(T);
            var cacheKey = new CacheKey(type, ignoreBase);
            if (_cache.TryGetValue(cacheKey, out var comparer))
            {
                return comparer(obj1, obj2);
            }

            var param1 = Expression.Parameter(type, "x");
            var param2 = Expression.Parameter(type, "y");

            Expression body = null;

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            if (ignoreBase)
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