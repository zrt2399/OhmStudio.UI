using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Attaches
{
    public class SelectorAttach
    {
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
            DependencyProperty.RegisterAttached("SelectedItemsAttach", typeof(bool), typeof(SelectorAttach), new PropertyMetadata(false, SelectedChangedCallBack));

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

        private static void SelectedChangedCallBack(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is Selector selector)
            {
                if ((bool)e.NewValue)
                {
                    selector.SelectionChanged += Selector_SelectionChanged;
                }
                else
                {
                    selector.SelectionChanged -= Selector_SelectionChanged;
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

        private static readonly ConcurrentDictionary<object, Selector> CollectionToSelectorMap = new();

        private static void OnIsAutoScrollToEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Selector selector && selector.Items.SourceCollection is INotifyCollectionChanged notifyCollectionChanged)
            {
                if (e.OldValue is bool oldValue && oldValue)
                {
                    notifyCollectionChanged.CollectionChanged -= SelectorAttach_CollectionChanged;
                    CollectionToSelectorMap.TryRemove(notifyCollectionChanged, out _);
                }

                if (e.NewValue is bool newValue && newValue)
                {
                    CollectionToSelectorMap[notifyCollectionChanged] = selector;
                    notifyCollectionChanged.CollectionChanged += SelectorAttach_CollectionChanged;
                }
            }
        }

        private static void SelectorAttach_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && CollectionToSelectorMap.TryGetValue(sender, out var selector) && selector.Items.Count > 0)
            {
                selector.ScrollToEnd();
            }
        }
    }
}