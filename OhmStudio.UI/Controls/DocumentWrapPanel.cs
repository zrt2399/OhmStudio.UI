using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AvalonDock.Controls;
using AvalonDock.Layout;
using OhmStudio.UI.Utilities;

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
 
        public static readonly DependencyProperty IsWrapProperty =
            DependencyProperty.RegisterAttached(nameof(IsWrap), typeof(bool), typeof(DocumentWrapPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, OnIsWrapChanged));

        public static readonly DependencyProperty UseMouseWheelWrapProperty =
            DependencyProperty.RegisterAttached(nameof(UseMouseWheelWrap), typeof(bool), typeof(DocumentWrapPanel), new PropertyMetadata(false, OnUseMouseWheelWrapChanged));
         
        public static void SetIsWrap(DependencyObject element, bool value) => element.SetValue(IsWrapProperty, value);

        public static bool GetIsWrap(DependencyObject element) => (bool)element.GetValue(IsWrapProperty);

        public static void SetUseMouseWheelWrap(DependencyObject element, bool value) => element.SetValue(UseMouseWheelWrapProperty, value);

        public static bool GetUseMouseWheelWrap(DependencyObject element) => (bool)element.GetValue(UseMouseWheelWrapProperty);

        public bool IsWrap
        {
            get => (bool)GetValue(IsWrapProperty);
            set => SetValue(IsWrapProperty, value);
        }

        public bool UseMouseWheelWrap
        {
            get => (bool)GetValue(UseMouseWheelWrapProperty);
            set => SetValue(UseMouseWheelWrapProperty, value);
        }

        private static void DocumentWrapPanel_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var documentWrapPanel = sender as DocumentWrapPanel;
            bool isWrap = e.Delta < 0;
            documentWrapPanel.IsWrap = isWrap;
        }

        private static void OnIsWrapChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is DocumentWrapPanel documentWrapPanel && (bool)e.NewValue)
            {
                foreach (var item in documentWrapPanel.Children.OfType<UIElement>().Where(x => x.Visibility == Visibility.Hidden))
                {
                    item.Visibility = Visibility.Visible;
                }
            }
        }

        private static void OnUseMouseWheelWrapChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is DocumentWrapPanel documentWrapPanel && e.NewValue is bool newValue)
            {
                if (newValue)
                {
                    documentWrapPanel.PreviewMouseWheel += DocumentWrapPanel_PreviewMouseWheel;
                }
                else
                {
                    documentWrapPanel.PreviewMouseWheel -= DocumentWrapPanel_PreviewMouseWheel;
                }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (IsWrap)
            {
                var orientation = Orientation.Horizontal;
                UVSize uVSize = new UVSize(orientation);
                UVSize uVSize2 = new UVSize(orientation);
                UVSize uVSize3 = new UVSize(orientation, constraint.Width, constraint.Height);

                Size availableSize = new Size(constraint.Width, constraint.Height);
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
                    UVSize uVSize4 = new UVSize(orientation, uIElement.DesiredSize.Width, uIElement.DesiredSize.Height);
                    if (DoubleUtil.GreaterThan(uVSize.U + uVSize4.U, uVSize3.U))
                    {
                        uVSize2.U = Math.Max(uVSize.U, uVSize2.U);
                        uVSize2.V += uVSize.V;
                        uVSize = uVSize4;
                        if (DoubleUtil.GreaterThan(uVSize4.U, uVSize3.U))
                        {
                            uVSize2.U = Math.Max(uVSize4.U, uVSize2.U);
                            uVSize2.V += uVSize4.V;
                            uVSize = new UVSize(orientation);
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
                return base.MeasureOverride(constraint);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (IsWrap)
            {
                int num = 0;
                double num2 = 0.0;
                var orientation = Orientation.Horizontal;
                UVSize uVSize = new UVSize(orientation);
                UVSize uVSize2 = new UVSize(orientation, finalSize.Width, finalSize.Height);

                UIElementCollection internalChildren = InternalChildren;
                int i = 0;
                for (int count = internalChildren.Count; i < count; i++)
                {
                    UIElement uIElement = internalChildren[i];
                    if (uIElement == null)
                    {
                        continue;
                    }

                    UVSize uVSize3 = new UVSize(orientation, uIElement.DesiredSize.Width, uIElement.DesiredSize.Height);
                    if (DoubleUtil.GreaterThan(uVSize.U + uVSize3.U, uVSize2.U))
                    {
                        ArrangeLine(num2, uVSize.V, num, i);
                        num2 += uVSize.V;
                        uVSize = uVSize3;
                        if (DoubleUtil.GreaterThan(uVSize3.U, uVSize2.U))
                        {
                            ArrangeLine(num2, uVSize3.V, i, ++i);
                            num2 += uVSize3.V;
                            uVSize = new UVSize(orientation);
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
                    ArrangeLine(num2, uVSize.V, num, internalChildren.Count);
                }

                return finalSize;
            }
            else
            {
                double num = 0.0;
                bool flag = false;
                foreach (var item in Children.OfType<UIElement>().Where(x => x.Visibility != Visibility.Collapsed))
                {
                    if (flag || num + item.DesiredSize.Width > finalSize.Width)
                    {
                        bool isSelected = false;
                        LayoutContent layoutContent = null;
                        if (item is TabItem tabItem)
                        {
                            layoutContent = tabItem.Content as LayoutContent;
                        }

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
                        num += item.DesiredSize.Width;
                    }
                }

                return finalSize;
            }
        }

        private void ArrangeLine(double v, double lineV, int start, int end)
        {
            double num = 0.0;
            var orientation = Orientation.Horizontal;
            bool flag = orientation == Orientation.Horizontal;
            UIElementCollection internalChildren = InternalChildren;
            for (int i = start; i < end; i++)
            {
                UIElement uIElement = internalChildren[i];
                if (uIElement != null)
                {
                    UVSize uVSize = new UVSize(orientation, uIElement.DesiredSize.Width, uIElement.DesiredSize.Height);
                    double num2 = uVSize.U;
                    uIElement.Arrange(new Rect(flag ? num : v, flag ? v : num, flag ? num2 : lineV, flag ? lineV : num2));
                    num += num2;
                }
            }
        }
    }
}