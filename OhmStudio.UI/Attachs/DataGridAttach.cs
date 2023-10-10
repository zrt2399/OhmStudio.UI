using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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
                    //dataGrid.CurrentCellChanged += DataGrid_CurrentCellChanged;

                }
                else
                {
                    dataGrid.LoadingRow -= DataGrid_LoadingRow;
                    dataGrid.ItemContainerGenerator.ItemsChanged -= ItemContainerGeneratorItemsChanged;
                    dataGrid.SelectionChanged -= DataGrid_SelectionChanged;
                }
                //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(dataGrid.Items);

                //// 监听分组规则的更改事件
                //view.GroupDescriptions.CollectionChanged += GroupDescriptions_CollectionChanged;  
            }


            void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                if (GetIsSelectionChangedUpdateRowNumber(sender as DataGrid))
                {
                    ItemContainerGeneratorItemsChanged(sender, null);
                }
            }

            //void GroupDescriptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            //{
            //    ItemContainerGeneratorItemsChanged(dataGrid, null);
            //}

            //void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
            //{
            //    ICollectionView view = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            //    ListCollectionView listCollectionView = view as ListCollectionView;
            //    listCollectionView?.CommitEdit();
            //}


            //void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
            //{

            //}

            //void DataGrid_CurrentCellChanged(object sender, EventArgs e)
            //{
            //    //ICollectionView view = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            //    //ListCollectionView listCollectionView = view as ListCollectionView;
            //    //listCollectionView?.CommitEdit();
            //    //listCollectionView?.Refresh(); 
            //}

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
    }
}