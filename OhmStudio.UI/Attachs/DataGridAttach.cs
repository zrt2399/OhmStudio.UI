using System.Linq;
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

        public static readonly DependencyProperty TextColumnElementStyleProperty =
          DependencyProperty.RegisterAttached("TextColumnElementStyle", typeof(Style), typeof(DataGridAttach), new PropertyMetadata(null, (sender, e) =>
          {
              if (sender is DataGrid dataGrid)
              {
                  var style = (Style)e.NewValue;
                  if (dataGrid.IsLoaded)
                  {
                      DataGrid_Loaded(dataGrid, null);
                  }
                  else
                  {
                      dataGrid.Loaded += DataGrid_Loaded;
                  }
                  void DataGrid_Loaded(object sender, RoutedEventArgs e)
                  {
                      dataGrid.Loaded -= DataGrid_Loaded;
                      foreach (var textColumn in dataGrid.Columns.OfType<DataGridTextColumn>())
                      {
                          if (!GetIsIgnoreElementStyle(textColumn))
                          {
                              textColumn.ElementStyle = style;
                          }
                      }
                  }
              }
          }));


        public static Style GetTextColumnElementStyle(DependencyObject target)
        {
            return (Style)target.GetValue(TextColumnElementStyleProperty);
        }

        public static void SetTextColumnElementStyle(DependencyObject target, Style value)
        {
            target.SetValue(TextColumnElementStyleProperty, value);
        }

        public static readonly DependencyProperty TextColumnEditingElementStyleProperty =
            DependencyProperty.RegisterAttached("TextColumnEditingElementStyle", typeof(Style), typeof(DataGridAttach), new PropertyMetadata(null, (sender, e) =>
            {
                if (sender is DataGrid dataGrid)
                {
                    var style = (Style)e.NewValue;
                    if (dataGrid.IsLoaded)
                    {
                        DataGrid_Loaded(dataGrid, null);
                    }
                    else
                    {
                        dataGrid.Loaded += DataGrid_Loaded;
                    }
                    void DataGrid_Loaded(object sender, RoutedEventArgs e)
                    {
                        dataGrid.Loaded -= DataGrid_Loaded;
                        foreach (var textColumn in dataGrid.Columns.OfType<DataGridTextColumn>())
                        {
                            if (!GetIsIgnoreEditingElementStyle(textColumn))
                            {
                                textColumn.EditingElementStyle = style;
                            }
                        }
                    }
                }
            }));

        public static Style GetTextColumnEditingElementStyle(DependencyObject target)
        {
            return (Style)target.GetValue(TextColumnEditingElementStyleProperty);
        }

        public static void SetTextColumnEditingElementStyle(DependencyObject target, Style value)
        {
            target.SetValue(TextColumnEditingElementStyleProperty, value);
        }

        public static readonly DependencyProperty IsIgnoreElementStyleProperty =
            DependencyProperty.RegisterAttached("IsIgnoreElementStyle", typeof(bool), typeof(DataGridAttach));

        public static bool GetIsIgnoreElementStyle(DependencyObject target)
        {
            return (bool)target.GetValue(IsIgnoreElementStyleProperty);
        }

        public static void SetIsIgnoreElementStyle(DependencyObject target, bool value)
        {
            target.SetValue(IsIgnoreElementStyleProperty, value);
        }

        public static readonly DependencyProperty IsIgnoreEditingElementStyleProperty =
            DependencyProperty.RegisterAttached("IsIgnoreEditingElementStyle", typeof(bool), typeof(DataGridAttach));

        public static bool GetIsIgnoreEditingElementStyle(DependencyObject target)
        {
            return (bool)target.GetValue(IsIgnoreEditingElementStyleProperty);
        }

        public static void SetIsIgnoreEditingElementStyle(DependencyObject target, bool value)
        {
            target.SetValue(IsIgnoreEditingElementStyleProperty, value);
        }
    }
}