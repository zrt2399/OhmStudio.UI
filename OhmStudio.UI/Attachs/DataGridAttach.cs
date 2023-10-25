using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Attachs
{
    public class DataGridAttach
    {
        public static readonly DependencyProperty IsSelectionChangedUpdateRowNumberProperty =
            DependencyProperty.RegisterAttached("IsSelectionChangedUpdateRowNumber", typeof(bool), typeof(DataGridAttach));
        public static bool GetIsSelectionChangedUpdateRowNumber(DependencyObject target)
        {
            return (bool)target.GetValue(IsSelectionChangedUpdateRowNumberProperty);
        }

        public static void SetIsSelectionChangedUpdateRowNumber(DependencyObject target, bool value)
        {
            target.SetValue(IsSelectionChangedUpdateRowNumberProperty, value);
        }

        public static DependencyProperty IsShowRowNumberProperty =
            DependencyProperty.RegisterAttached("IsShowRowNumber", typeof(bool), typeof(DataGridAttach), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, OnShowRowNumberChanged));
        public static bool GetIsShowRowNumber(DependencyObject target)
        {
            return (bool)target.GetValue(IsShowRowNumberProperty);
        }

        public static void SetIsShowRowNumber(DependencyObject target, bool value)
        {
            target.SetValue(IsShowRowNumberProperty, value);
        }

        private static void OnShowRowNumberChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            bool show;
            if (target is DataGrid dataGrid)
            {
                show = (bool)e.NewValue;
                if (show)
                {
                    dataGrid.LoadingRow += DataGrid_LoadingRow;
                    dataGrid.ItemContainerGenerator.ItemsChanged += ItemContainerGeneratorItemsChanged;
                    dataGrid.SelectionChanged += DataGrid_SelectionChanged;

                }
                else
                {
                    dataGrid.LoadingRow -= DataGrid_LoadingRow;
                    dataGrid.ItemContainerGenerator.ItemsChanged -= ItemContainerGeneratorItemsChanged;
                    dataGrid.SelectionChanged -= DataGrid_SelectionChanged;
                }
            }

            void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                if (sender is DataGrid data && GetIsSelectionChangedUpdateRowNumber(data))
                {
                    ItemContainerGeneratorItemsChanged(sender, null);
                }
            }

            void ItemContainerGeneratorItemsChanged(object sender, ItemsChangedEventArgs e)
            {
                ItemContainerGenerator itemContainerGenerator = dataGrid.ItemContainerGenerator;
                int count = dataGrid.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    if (itemContainerGenerator.ContainerFromIndex(i) is DataGridRow dataGridRow)
                    {
                        dataGridRow.Header = show ? i + 1 : null;
                    }
                }
            }
        }

        private static void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        //public static readonly DependencyProperty IsExpandItemProperty =
        //  DependencyProperty.RegisterAttached("IsExpandItem", typeof(bool), typeof(DataGridAttach), new PropertyMetadata(false, (sender, e) =>
        //  {
        //      if (sender is DataGrid dataGrid)
        //      {
        //          var expanders = dataGrid.FindVisualChildren<GroupItem>();
        //          foreach (var item in expanders)
        //          {

        //          }
        //      }
        //  }));

        //public static bool GetIsExpandItem(DependencyObject target)
        //{
        //    return (bool)target.GetValue(IsExpandItemProperty);
        //}

        //public static void SetIsExpandItem(DependencyObject target, bool value)
        //{
        //    target.SetValue(IsExpandItemProperty, value);
        //}
    }
}