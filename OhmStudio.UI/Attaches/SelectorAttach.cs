using System.Collections;
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

        public static readonly DependencyProperty IsScrollToBottomProperty =
            DependencyProperty.RegisterAttached("IsScrollToBottom", typeof(bool), typeof(SelectorAttach), new PropertyMetadata(false, OnIsScrollToBottomChanged));

        public static void SetIsScrollToBottom(DependencyObject obj, bool value)
        {
            obj.SetValue(IsScrollToBottomProperty, value);
        }

        public static bool GetIsScrollToBottom(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsScrollToBottomProperty);
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

        private static void OnIsScrollToBottomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Selector selector && selector.ItemsSource is INotifyCollectionChanged notifyCollectionChanged)
            {
                if ((bool)e.NewValue)
                {
                    notifyCollectionChanged.CollectionChanged += SelectorAttach_CollectionChanged;
                }
                else
                {
                    notifyCollectionChanged.CollectionChanged -= SelectorAttach_CollectionChanged;
                }
            }
        }

        private static void SelectorAttach_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var selector = (Selector)sender;
            if (e.Action == NotifyCollectionChangedAction.Add && selector.Items.Count > 0)
            {
                selector.ScrollToEnd();
            }
        }
    }
}