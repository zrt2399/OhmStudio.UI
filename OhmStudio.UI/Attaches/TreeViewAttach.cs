using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OhmStudio.UI.Attaches
{
    public class TreeViewAttach
    {
        public static bool GetIsSelectWithMouseRightDown(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectWithMouseRightDownProperty);
        }

        public static void SetIsSelectWithMouseRightDown(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectWithMouseRightDownProperty, value);
        }

        public static readonly DependencyProperty IsSelectWithMouseRightDownProperty =
            DependencyProperty.RegisterAttached("IsSelectWithMouseRightDown", typeof(bool), typeof(TreeViewAttach), new PropertyMetadata(false, OnIsSelectWithMouseRightDownPropertyChanged));

        private static void OnIsSelectWithMouseRightDownPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
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
                //if (e.OriginalSource.GetType().Name == "TextBoxView")
                //{
                //    return;
                //}
                treeViewItem.IsSelected = true;
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
            DependencyProperty.RegisterAttached("SelectedItemAttach", typeof(bool), typeof(TreeViewAttach), new PropertyMetadata(false, SelectedItemChangedCallBack));

        public static bool GetSelectedItemAttach(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectedItemAttachProperty);
        }

        public static void SetSelectedItemAttach(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectedItemAttachProperty, value);
        }

        private static void SelectedItemChangedCallBack(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TreeView treeView)
            {
                if ((bool)e.NewValue)
                {
                    treeView.SelectedItemChanged += TreeView_SelectedItemChanged; 
                }
                else
                {
                    treeView.SelectedItemChanged -= TreeView_SelectedItemChanged;
                }
            }
        }

        private static void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is TreeView treeView)
            {
                SetSelectedItem(treeView, treeView.SelectedItem);
            }
        } 
    }
}