using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace OhmStudio.UI.Attaches
{
    public class TreeViewAttach
    {
        public static bool GetIsFocusWithMouseRightDown(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusWithMouseRightDownProperty);
        }

        public static void SetIsFocusWithMouseRightDown(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusWithMouseRightDownProperty, value);
        }

        public static readonly DependencyProperty IsFocusWithMouseRightDownProperty =
            DependencyProperty.RegisterAttached("IsFocusWithMouseRightDown", typeof(bool), typeof(TreeViewAttach), new PropertyMetadata(false, OnIsFocusWithMouseRightDownPropertyChanged));

        private static void OnIsFocusWithMouseRightDownPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TreeView treeView)
            {
                if ((bool)e.NewValue)
                {
                    treeView.PreviewMouseRightButtonDown += TreeViewItem_PreviewMouseRightButtonDown;
                }
                else
                {
                    treeView.PreviewMouseRightButtonDown -= TreeViewItem_PreviewMouseRightButtonDown;
                }
            }
        }

        private static void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) is TreeViewItem treeViewItem)
            {
                if (e.OriginalSource.GetType().Name == "TextBoxView")
                {
                    return;
                }
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            return source;
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached("SelectedItem", typeof(object), typeof(TreeViewAttach), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static object GetSelectedItem(DependencyObject obj)
        {
            return obj.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItem(DependencyObject obj, object value)
        {
            obj.SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty SelectedItemAttachProperty =
            DependencyProperty.RegisterAttached("SelectedItemAttach", typeof(bool), typeof(TreeViewAttach), new PropertyMetadata(false, SelectedChangedCallBack));

        public static bool GetSelectedItemAttach(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectedItemAttachProperty);
        }

        public static void SetSelectedItemAttach(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectedItemAttachProperty, value);
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
            if (sender is TreeView treeView)
            {
                SetSelectedItem(treeView, treeView.SelectedItem);
            }
        }
    }
}