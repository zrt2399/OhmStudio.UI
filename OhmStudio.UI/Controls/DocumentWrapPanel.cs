using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using AvalonDock.Controls;
using AvalonDock.Layout;

namespace OhmStudio.UI.Controls
{
    public class DocumentWrapPanel : DocumentPaneTabPanel
    {
        private struct UVSize
        {
            internal UVSize(Orientation orientation, double width, double height)
            {
                U = V = 0.0;
                _orientation = orientation;
                Width = width;
                Height = height;
            }

            internal UVSize(Orientation orientation)
            {
                U = V = 0.0;
                _orientation = orientation;
            }

            internal double U;

            internal double V;

            private Orientation _orientation;

            internal double Width
            {
                get
                {
                    if (_orientation != 0)
                    {
                        return V;
                    }

                    return U;
                }
                set
                {
                    if (_orientation == Orientation.Horizontal)
                    {
                        U = value;
                    }
                    else
                    {
                        V = value;
                    }
                }
            }

            internal double Height
            {
                get
                {
                    if (_orientation != 0)
                    {
                        return U;
                    }

                    return V;
                }
                set
                {
                    if (_orientation == Orientation.Horizontal)
                    {
                        V = value;
                    }
                    else
                    {
                        U = value;
                    }
                }
            }
        }

        public static readonly DependencyProperty ItemWidthProperty;

        public static readonly DependencyProperty ItemHeightProperty;

        public static readonly DependencyProperty OrientationProperty;

        public static readonly DependencyProperty IsWrapProperty;

        private Orientation _orientation;

        [TypeConverter(typeof(LengthConverter))]
        public double ItemWidth
        {
            get => (double)GetValue(ItemWidthProperty);
            set => SetValue(ItemWidthProperty, value);
        }

        [TypeConverter(typeof(LengthConverter))]
        public double ItemHeight
        {
            get => (double)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        public Orientation Orientation
        {
            get => _orientation;
            set => SetValue(OrientationProperty, value);
        }

        public bool IsWrap
        {
            get => (bool)GetValue(IsWrapProperty);
            set => SetValue(OrientationProperty, value);
        }

        public DocumentWrapPanel()
        {
            _orientation = (Orientation)OrientationProperty.GetMetadata(this).DefaultValue;
        }

        static DocumentWrapPanel()
        {
            ItemWidthProperty = DependencyProperty.Register(nameof(ItemWidth), typeof(double), typeof(DocumentWrapPanel), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure), IsWidthHeightValid);
            ItemHeightProperty = DependencyProperty.Register(nameof(ItemHeight), typeof(double), typeof(DocumentWrapPanel), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure), IsWidthHeightValid);
            OrientationProperty = StackPanel.OrientationProperty.AddOwner(typeof(DocumentWrapPanel), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure, OnOrientationChanged));
            IsWrapProperty = DependencyProperty.Register(nameof(IsWrap), typeof(bool), typeof(DocumentWrapPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, IsWrapChanged));
            //ControlsTraceLogger.AddControl(TelemetryControls.DocumentWrapPanel);
        }

        private static void IsWrapChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is DocumentWrapPanel uIElement && e.NewValue is bool newValue)
            {
                //uIElement.InvalidateMeasure();
                uIElement.InvalidateVisual();
                foreach (var item in uIElement.Children.OfType<UIElement>())
                {
                    if (newValue && item.Visibility == Visibility.Hidden)
                    {
                        item.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private static bool IsWidthHeightValid(object value)
        {
            double num = (double)value;
            if (!DoubleUtil.IsNaN(num))
            {
                if (num >= 0.0)
                {
                    return !double.IsPositiveInfinity(num);
                }

                return false;
            }

            return true;
        }

        private static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DocumentWrapPanel wrapPanel = (DocumentWrapPanel)sender;
            wrapPanel._orientation = (Orientation)e.NewValue;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (IsWrap)
            {
                UVSize uVSize = new UVSize(Orientation);
                UVSize uVSize2 = new UVSize(Orientation);
                UVSize uVSize3 = new UVSize(Orientation, constraint.Width, constraint.Height);
                double itemWidth = ItemWidth;
                double itemHeight = ItemHeight;
                bool flag = !DoubleUtil.IsNaN(itemWidth);
                bool flag2 = !DoubleUtil.IsNaN(itemHeight);
                Size availableSize = new Size(flag ? itemWidth : constraint.Width, flag2 ? itemHeight : constraint.Height);
                UIElementCollection internalChildren = InternalChildren;
                int i = 0;
                for (int count = internalChildren.Count; i < count; i++)
                {
                    UIElement uIElement = internalChildren[i];
                    if (uIElement == null)
                    {
                        continue;
                    }

                    uIElement.Measure(availableSize);
                    UVSize uVSize4 = new UVSize(Orientation, flag ? itemWidth : uIElement.DesiredSize.Width, flag2 ? itemHeight : uIElement.DesiredSize.Height);
                    if (DoubleUtil.GreaterThan(uVSize.U + uVSize4.U, uVSize3.U))
                    {
                        uVSize2.U = Math.Max(uVSize.U, uVSize2.U);
                        uVSize2.V += uVSize.V;
                        uVSize = uVSize4;
                        if (DoubleUtil.GreaterThan(uVSize4.U, uVSize3.U))
                        {
                            uVSize2.U = Math.Max(uVSize4.U, uVSize2.U);
                            uVSize2.V += uVSize4.V;
                            uVSize = new UVSize(Orientation);
                        }
                    }
                    else
                    {
                        uVSize.U += uVSize4.U;
                        uVSize.V = Math.Max(uVSize4.V, uVSize.V);
                    }
                }

                uVSize2.U = Math.Max(uVSize.U, uVSize2.U);
                uVSize2.V += uVSize.V;
                return new Size(uVSize2.Width, uVSize2.Height);
            }
            else
            {
                Size size = default;
                foreach (FrameworkElement child in Children)
                {
                    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    size.Width += child.DesiredSize.Width;
                    size.Height = Math.Max(size.Height, child.DesiredSize.Height);
                }

                return new Size(Math.Min(size.Width, constraint.Width), size.Height);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (IsWrap)
            {
                int num = 0;
                double itemWidth = ItemWidth;
                double itemHeight = ItemHeight;
                double num2 = 0.0;
                double itemU = (Orientation == Orientation.Horizontal) ? itemWidth : itemHeight;
                UVSize uVSize = new UVSize(Orientation);
                UVSize uVSize2 = new UVSize(Orientation, finalSize.Width, finalSize.Height);
                bool flag = !DoubleUtil.IsNaN(itemWidth);
                bool flag2 = !DoubleUtil.IsNaN(itemHeight);
                bool useItemU = (Orientation == Orientation.Horizontal) ? flag : flag2;
                UIElementCollection internalChildren = InternalChildren;
                int i = 0;
                for (int count = internalChildren.Count; i < count; i++)
                {
                    UIElement uIElement = internalChildren[i];
                    if (uIElement == null)
                    {
                        continue;
                    }

                    UVSize uVSize3 = new UVSize(Orientation, flag ? itemWidth : uIElement.DesiredSize.Width, flag2 ? itemHeight : uIElement.DesiredSize.Height);
                    if (DoubleUtil.GreaterThan(uVSize.U + uVSize3.U, uVSize2.U))
                    {
                        ArrangeLine(num2, uVSize.V, num, i, useItemU, itemU);
                        num2 += uVSize.V;
                        uVSize = uVSize3;
                        if (DoubleUtil.GreaterThan(uVSize3.U, uVSize2.U))
                        {
                            ArrangeLine(num2, uVSize3.V, i, ++i, useItemU, itemU);
                            num2 += uVSize3.V;
                            uVSize = new UVSize(Orientation);
                        }

                        num = i;
                    }
                    else
                    {
                        uVSize.U += uVSize3.U;
                        uVSize.V = Math.Max(uVSize3.V, uVSize.V);
                    }
                }

                if (num < internalChildren.Count)
                {
                    ArrangeLine(num2, uVSize.V, num, internalChildren.Count, useItemU, itemU);
                }

                return finalSize;
            }
            else
            {
                IEnumerable<UIElement> enumerable = from UIElement ch in Children
                                                    where ch.Visibility != Visibility.Collapsed
                                                    select ch;
                double num = 0.0;
                bool flag = false;
                foreach (TabItem item in enumerable.OfType<TabItem>())
                {
                    if (flag || num + item.DesiredSize.Width > finalSize.Width)
                    {
                        bool isSelected = false;
                        LayoutContent layoutContent = item.Content as LayoutContent;
                        if (layoutContent != null)
                        {
                            isSelected = layoutContent.IsSelected;
                        }

                        if (isSelected && !item.IsVisible)
                        {
                            ILayoutContainer parent = layoutContent.Parent;
                            ILayoutContentSelector layoutContentSelector = layoutContent.Parent as ILayoutContentSelector;
                            ILayoutPane layoutPane = layoutContent.Parent as ILayoutPane;
                            int num2 = layoutContentSelector.IndexOf(layoutContent);
                            if (num2 > 0 && parent.ChildrenCount > 1)
                            {
                                layoutPane.MoveChild(num2, 0);
                                layoutContentSelector.SelectedContentIndex = 0;
                                return ArrangeOverride(finalSize);
                            }
                        }

                        item.Visibility = Visibility.Hidden;
                        flag = true;
                    }
                    else
                    {
                        item.Visibility = Visibility.Visible;
                        item.Arrange(new Rect(num, 0.0, item.DesiredSize.Width, finalSize.Height));
                        num += item.ActualWidth + item.Margin.Left + item.Margin.Right;
                    }
                }

                return finalSize;
            }
        }

        private void ArrangeLine(double v, double lineV, int start, int end, bool useItemU, double itemU)
        {
            double num = 0.0;
            bool flag = Orientation == Orientation.Horizontal;
            UIElementCollection internalChildren = InternalChildren;
            for (int i = start; i < end; i++)
            {
                UIElement uIElement = internalChildren[i];
                if (uIElement != null)
                {
                    UVSize uVSize = new UVSize(Orientation, uIElement.DesiredSize.Width, uIElement.DesiredSize.Height);
                    double num2 = useItemU ? itemU : uVSize.U;
                    uIElement.Arrange(new Rect(flag ? num : v, flag ? v : num, flag ? num2 : lineV, flag ? lineV : num2));
                    num += num2;
                }
            }
        }
    }

    public static class DoubleUtil
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion
        {
            [FieldOffset(0)]
            internal double DoubleValue;

            [FieldOffset(0)]
            internal ulong UintValue;
        }

        internal const double DBL_EPSILON = 2.2204460492503131E-16;

        internal const float FLT_MIN = 1.17549435E-38f;

        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }

            double num = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * 2.2204460492503131E-16;
            double num2 = value1 - value2;
            if (0.0 - num < num2)
            {
                return num > num2;
            }

            return false;
        }

        public static bool LessThan(double value1, double value2)
        {
            if (value1 < value2)
            {
                return !AreClose(value1, value2);
            }

            return false;
        }

        public static bool GreaterThan(double value1, double value2)
        {
            if (value1 > value2)
            {
                return !AreClose(value1, value2);
            }

            return false;
        }

        public static bool LessThanOrClose(double value1, double value2)
        {
            if (!(value1 < value2))
            {
                return AreClose(value1, value2);
            }

            return true;
        }

        public static bool GreaterThanOrClose(double value1, double value2)
        {
            if (!(value1 > value2))
            {
                return AreClose(value1, value2);
            }

            return true;
        }

        public static bool IsOne(double value)
        {
            return Math.Abs(value - 1.0) < 2.2204460492503131E-15;
        }

        public static bool IsZero(double value)
        {
            return Math.Abs(value) < 2.2204460492503131E-15;
        }

        public static bool AreClose(Point point1, Point point2)
        {
            if (AreClose(point1.X, point2.X))
            {
                return AreClose(point1.Y, point2.Y);
            }

            return false;
        }

        public static bool AreClose(Size size1, Size size2)
        {
            if (AreClose(size1.Width, size2.Width))
            {
                return AreClose(size1.Height, size2.Height);
            }

            return false;
        }

        public static bool AreClose(Vector vector1, Vector vector2)
        {
            if (AreClose(vector1.X, vector2.X))
            {
                return AreClose(vector1.Y, vector2.Y);
            }

            return false;
        }

        public static bool AreClose(Rect rect1, Rect rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }

            if (!rect2.IsEmpty && AreClose(rect1.X, rect2.X) && AreClose(rect1.Y, rect2.Y) && AreClose(rect1.Height, rect2.Height))
            {
                return AreClose(rect1.Width, rect2.Width);
            }

            return false;
        }

        public static bool IsBetweenZeroAndOne(double val)
        {
            if (GreaterThanOrClose(val, 0.0))
            {
                return LessThanOrClose(val, 1.0);
            }

            return false;
        }

        public static int DoubleToInt(double val)
        {
            if (!(0.0 < val))
            {
                return (int)(val - 0.5);
            }

            return (int)(val + 0.5);
        }

        public static bool RectHasNaN(Rect r)
        {
            if (IsNaN(r.X) || IsNaN(r.Y) || IsNaN(r.Height) || IsNaN(r.Width))
            {
                return true;
            }

            return false;
        }

        public static bool IsNaN(double value)
        {
            NanUnion nanUnion = default(NanUnion);
            nanUnion.DoubleValue = value;
            ulong num = nanUnion.UintValue & 0xFFF0000000000000uL;
            ulong num2 = nanUnion.UintValue & 0xFFFFFFFFFFFFFuL;
            if (num == 9218868437227405312L || num == 18442240474082181120uL)
            {
                return num2 != 0;
            }

            return false;
        }
    }
}