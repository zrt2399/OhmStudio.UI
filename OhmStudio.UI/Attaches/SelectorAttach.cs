using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using OhmStudio.UI.Utilities;

namespace OhmStudio.UI.Attaches
{
    public class SelectorAttach
    {
        private static readonly ConditionalWeakTable<object, WeakReference<Selector>> CollectionToSelectorMap = new();

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached("SelectedItems", typeof(IList), typeof(SelectorAttach), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static IList GetSelectedItems(DependencyObject obj)
        {
            return (IList)obj.GetValue(SelectedItemsProperty);
        }

        public static void SetSelectedItems(DependencyObject obj, IList value)
        {
            obj.SetValue(SelectedItemsProperty, value);
        }

        public static readonly DependencyProperty SelectedItemsAttachProperty =
            DependencyProperty.RegisterAttached("SelectedItemsAttach", typeof(bool), typeof(SelectorAttach), new PropertyMetadata(false, OnSelectedItemsAttachChanged));

        public static bool GetSelectedItemsAttach(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectedItemsAttachProperty);
        }

        public static void SetSelectedItemsAttach(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectedItemsAttachProperty, value);
        }

        public static readonly DependencyProperty IsAutoScrollToEndProperty =
            DependencyProperty.RegisterAttached("IsAutoScrollToEnd", typeof(bool), typeof(SelectorAttach), new PropertyMetadata(false, OnIsAutoScrollToEndChanged));

        public static void SetIsAutoScrollToEnd(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAutoScrollToEndProperty, value);
        }

        public static bool GetIsAutoScrollToEnd(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAutoScrollToEndProperty);
        }

        public static readonly DependencyProperty IgnoreAutoScrollOnMouseOverProperty =
            DependencyProperty.RegisterAttached("IgnoreAutoScrollOnMouseOver", typeof(bool), typeof(SelectorAttach));

        public static bool GetIgnoreAutoScrollOnMouseOver(DependencyObject obj)
        {
            return (bool)obj.GetValue(IgnoreAutoScrollOnMouseOverProperty);
        }

        public static void SetIgnoreAutoScrollOnMouseOver(DependencyObject obj, bool value)
        {
            obj.SetValue(IgnoreAutoScrollOnMouseOverProperty, value);
        }

        private static void OnSelectedItemsAttachChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is Selector selector)
            {
                selector.SelectionChanged -= Selector_SelectionChanged;
                if ((bool)e.NewValue)
                {
                    selector.SelectionChanged += Selector_SelectionChanged;
                }
            }
        }

        private static void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is Selector selector)
            {
                if (selector is DataGrid dataGrid)
                {
                    SetSelectedItems(dataGrid, dataGrid.SelectedItems);
                }
                else if (selector is ListBox listBox)
                {
                    SetSelectedItems(listBox, listBox.SelectedItems);
                }
                //BindingExpression bind = selector.GetBindingExpression(SelectedItemsProperty);
                //bind?.UpdateSource();
            }
        }

        private static void OnIsAutoScrollToEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Selector selector)
            {
                return;
            }
            var property = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(Selector));
            property?.RemoveValueChanged(selector, OnItemsSourceChanged);
            if ((bool)e.NewValue)
            {
                property?.AddValueChanged(selector, OnItemsSourceChanged);
            }

            if (selector.ItemsSource is INotifyCollectionChanged notifyCollectionChanged)
            {
                if ((bool)e.OldValue)
                {
                    notifyCollectionChanged.CollectionChanged -= SelectorAttach_CollectionChanged;
                    CollectionToSelectorMap.Remove(notifyCollectionChanged);
                }

                if ((bool)e.NewValue)
                {
                    CollectionToSelectorMap.Remove(notifyCollectionChanged);
                    CollectionToSelectorMap.Add(notifyCollectionChanged, new WeakReference<Selector>(selector));
                    notifyCollectionChanged.CollectionChanged += SelectorAttach_CollectionChanged;
                }
            }
        }

        private static void OnItemsSourceChanged(object sender, EventArgs e)
        {
            if (sender is Selector selector)
            {
                OnIsAutoScrollToEndChanged(selector, new DependencyPropertyChangedEventArgs(IsAutoScrollToEndProperty, true, GetIsAutoScrollToEnd(selector)));

                if (GetIsAutoScrollToEnd(selector))
                {
                    ScrollToEnd(selector);
                }
            }
        }

        private static void SelectorAttach_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (CollectionToSelectorMap.TryGetValue(sender, out var selector) && e.Action == NotifyCollectionChangedAction.Add)
            {
                if (selector.TryGetTarget(out var target))
                {
                    ScrollToEnd(target);
                }
                else
                {
                    if (sender is INotifyCollectionChanged notifyCollectionChanged)
                    {
                        notifyCollectionChanged.CollectionChanged -= SelectorAttach_CollectionChanged;
                    }
                    CollectionToSelectorMap.Remove(sender);
                }
            }
        }

        private static void ScrollToEnd(Selector selector)
        {
            if (GetIgnoreAutoScrollOnMouseOver(selector) && selector.IsMouseOver)
            {
                return;
            }
            if (selector.Items.Count > 0)
            {
                selector.Dispatcher.InvokeAsync(() =>
                {
                    selector.ScrollToEnd();
                }, DispatcherPriority.Render);
            }
        }

        public static readonly DependencyProperty IsAutoScrollToSelectedItemProperty =
            DependencyProperty.RegisterAttached("IsAutoScrollToSelectedItem", typeof(bool), typeof(SelectorAttach), new PropertyMetadata(false, OnIsAutoScrollToSelectedItemChange));

        public static void SetIsAutoScrollToSelectedItem(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAutoScrollToSelectedItemProperty, value);
        }

        public static bool GetIsAutoScrollToSelectedItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAutoScrollToSelectedItemProperty);
        }

        private static void OnIsAutoScrollToSelectedItemChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Selector selector)
            {
                selector.SelectionChanged -= Selector_SelectedItemChanged;
                if ((bool)e.NewValue)
                {
                    selector.SelectionChanged += Selector_SelectedItemChanged;
                }
            }
        }

        private static void Selector_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = (Selector)sender;
            if (GetIgnoreAutoScrollOnMouseOver(selector) && selector.IsMouseOver)
            {
                return;
            }
            selector.Dispatcher.InvokeAsync(() =>
            {
                if (selector is ListBox listBox)
                {
                    listBox.ScrollIntoView(listBox.SelectedItem);
                }
                else if (selector is DataGrid dataGrid)
                {
                    dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                }
            }, DispatcherPriority.Render);
        }
    }
}