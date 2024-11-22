using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using OhmStudio.UI.Utilities;

namespace OhmStudio.UI.Attaches
{
    public class DataGridAttach
    {
        public static readonly DependencyProperty SelectionChangedUpdateRowNumberProperty =
            DependencyProperty.RegisterAttached("SelectionChangedUpdateRowNumber", typeof(bool), typeof(DataGridAttach));

        public static bool GetSelectionChangedUpdateRowNumber(DependencyObject target)
        {
            return (bool)target.GetValue(SelectionChangedUpdateRowNumberProperty);
        }

        public static void SetSelectionChangedUpdateRowNumber(DependencyObject target, bool value)
        {
            target.SetValue(SelectionChangedUpdateRowNumberProperty, value);
        }

        public static DependencyProperty ShowRowNumberProperty =
            DependencyProperty.RegisterAttached("ShowRowNumber", typeof(bool), typeof(DataGridAttach), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, OnShowRowNumberChanged));

        public static bool GetShowRowNumber(DependencyObject target)
        {
            return (bool)target.GetValue(ShowRowNumberProperty);
        }

        public static void SetShowRowNumber(DependencyObject target, bool value)
        {
            target.SetValue(ShowRowNumberProperty, value);
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
                if (sender is DataGrid data && GetSelectionChangedUpdateRowNumber(data))
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

        private static void SetColumnStyle<T>(DataGrid dataGrid, Style style, bool isEditingStyle)
        {
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

                foreach (var item in dataGrid.Columns.OfType<T>().OfType<DataGridColumn>())
                {
                    var type = item.GetType();
                    if (isEditingStyle)
                    {
                        if (!DataGridColumnAttach.GetIsIgnoreEditingElementStyle(item))
                        {
                            type.GetProperty("EditingElementStyle").SetValue(item, style);
                            //item.EditingElementStyle = style;
                        }
                    }
                    else
                    {
                        if (!DataGridColumnAttach.GetIsIgnoreElementStyle(item))
                        {
                            type.GetProperty("ElementStyle").SetValue(item, style);
                            //item.ElementStyle = style;
                        }
                    }
                }
            }
        }

        public static readonly DependencyProperty TextColumnElementStyleProperty =
          DependencyProperty.RegisterAttached("TextColumnElementStyle", typeof(Style), typeof(DataGridAttach), new PropertyMetadata(null, (sender, e) =>
          {
              if (sender is DataGrid dataGrid)
              {
                  SetColumnStyle<DataGridTextColumn>(dataGrid, (Style)e.NewValue, false);
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
                    SetColumnStyle<DataGridTextColumn>(dataGrid, (Style)e.NewValue, true);
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

        public static readonly DependencyProperty CheckBoxColumnElementStyleProperty =
          DependencyProperty.RegisterAttached("CheckBoxColumnElementStyle", typeof(Style), typeof(DataGridAttach), new PropertyMetadata(null, (sender, e) =>
          {
              if (sender is DataGrid dataGrid)
              {
                  SetColumnStyle<DataGridCheckBoxColumn>(dataGrid, (Style)e.NewValue, false);
              }
          }));

        public static Style GetCheckBoxColumnElementStyle(DependencyObject target)
        {
            return (Style)target.GetValue(CheckBoxColumnElementStyleProperty);
        }

        public static void SetCheckBoxColumnElementStyle(DependencyObject target, Style value)
        {
            target.SetValue(CheckBoxColumnElementStyleProperty, value);
        }

        public static readonly DependencyProperty CheckBoxColumnEditingElementStyleProperty =
            DependencyProperty.RegisterAttached("CheckBoxColumnEditingElementStyle", typeof(Style), typeof(DataGridAttach), new PropertyMetadata(null, (sender, e) =>
            {
                if (sender is DataGrid dataGrid)
                {
                    SetColumnStyle<DataGridCheckBoxColumn>(dataGrid, (Style)e.NewValue, true);
                }
            }));

        public static Style GetCheckBoxColumnEditingElementStyle(DependencyObject target)
        {
            return (Style)target.GetValue(CheckBoxColumnEditingElementStyleProperty);
        }

        public static void SetCheckBoxColumnEditingElementStyle(DependencyObject target, Style value)
        {
            target.SetValue(CheckBoxColumnEditingElementStyleProperty, value);
        }

        public static readonly DependencyProperty ComboBoxColumnElementStyleProperty =
          DependencyProperty.RegisterAttached("ComboBoxColumnElementStyle", typeof(Style), typeof(DataGridAttach), new PropertyMetadata(null, (sender, e) =>
          {
              if (sender is DataGrid dataGrid)
              {
                  SetColumnStyle<DataGridComboBoxColumn>(dataGrid, (Style)e.NewValue, false);
              }
          }));

        public static Style GetComboBoxColumnElementStyle(DependencyObject target)
        {
            return (Style)target.GetValue(ComboBoxColumnElementStyleProperty);
        }

        public static void SetComboBoxColumnElementStyle(DependencyObject target, Style value)
        {
            target.SetValue(ComboBoxColumnElementStyleProperty, value);
        }

        public static readonly DependencyProperty ComboBoxColumnEditingElementStyleProperty =
            DependencyProperty.RegisterAttached("ComboBoxColumnEditingElementStyle", typeof(Style), typeof(DataGridAttach), new PropertyMetadata(null, (sender, e) =>
            {
                if (sender is DataGrid dataGrid)
                {
                    SetColumnStyle<DataGridComboBoxColumn>(dataGrid, (Style)e.NewValue, true);
                }
            }));

        public static Style GetComboBoxColumnEditingElementStyle(DependencyObject target)
        {
            return (Style)target.GetValue(ComboBoxColumnEditingElementStyleProperty);
        }

        public static void SetComboBoxColumnEditingElementStyle(DependencyObject target, Style value)
        {
            target.SetValue(ComboBoxColumnEditingElementStyleProperty, value);
        }

        public static readonly DependencyProperty AutoGeneratedCheckBoxStyleProperty
            = DependencyProperty.RegisterAttached("AutoGeneratedCheckBoxStyle", typeof(Style), typeof(DataGridAttach),
                new PropertyMetadata(default(Style), AutoGeneratedCheckBoxStylePropertyChangedCallback));

        public static Style GetAutoGeneratedCheckBoxStyle(DataGrid element)
        {
            return (Style)element.GetValue(AutoGeneratedCheckBoxStyleProperty);
        }

        public static void SetAutoGeneratedCheckBoxStyle(DataGrid element, Style value)
        {
            element.SetValue(AutoGeneratedCheckBoxStyleProperty, value);
        }

        private static void AutoGeneratedCheckBoxStylePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            dataGrid.AutoGeneratingColumn -= SetGeneratedCheckboxColumnStyle;
            dataGrid.AutoGeneratingColumn += SetGeneratedCheckboxColumnStyle;
        }

        private static void SetGeneratedCheckboxColumnStyle(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column is DataGridCheckBoxColumn column && sender is DataGrid dataGrid)
            {
                column.ElementStyle = GetAutoGeneratedCheckBoxStyle(dataGrid);
            }
        }

        public static readonly DependencyProperty AutoGeneratedEditingCheckBoxStyleProperty
            = DependencyProperty.RegisterAttached("AutoGeneratedEditingCheckBoxStyle", typeof(Style), typeof(DataGridAttach),
                new PropertyMetadata(default(Style), AutoGeneratedEditingCheckBoxStylePropertyChangedCallback));

        public static Style GetAutoGeneratedEditingCheckBoxStyle(DataGrid element)
        {
            return (Style)element.GetValue(AutoGeneratedEditingCheckBoxStyleProperty);
        }

        public static void SetAutoGeneratedEditingCheckBoxStyle(DataGrid element, Style value)
        {
            element.SetValue(AutoGeneratedEditingCheckBoxStyleProperty, value);
        }

        private static void AutoGeneratedEditingCheckBoxStylePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            dataGrid.AutoGeneratingColumn -= SetGeneratedCheckBoxEditingStyle;
            dataGrid.AutoGeneratingColumn += SetGeneratedCheckBoxEditingStyle;
        }

        private static void SetGeneratedCheckBoxEditingStyle(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column is DataGridCheckBoxColumn column && sender is DataGrid dataGrid)
            {
                column.EditingElementStyle = GetAutoGeneratedEditingCheckBoxStyle(dataGrid);
            }
        }

        #region AttachedProperty : AutoGeneratedTextStyleProperty
        public static readonly DependencyProperty AutoGeneratedTextStyleProperty
            = DependencyProperty.RegisterAttached("AutoGeneratedTextStyle", typeof(Style), typeof(DataGridAttach),
                new PropertyMetadata(default(Style), AutoGeneratedTextStylePropertyChangedCallback));

        public static Style GetAutoGeneratedTextStyle(DataGrid element)
        {
            return (Style)element.GetValue(AutoGeneratedTextStyleProperty);
        }

        public static void SetAutoGeneratedTextStyle(DataGrid element, Style value)
        {
            element.SetValue(AutoGeneratedTextStyleProperty, value);
        }

        private static void AutoGeneratedTextStylePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            dataGrid.AutoGeneratingColumn -= SetGeneratedTextColumnStyle;
            dataGrid.AutoGeneratingColumn += SetGeneratedTextColumnStyle;
        }

        private static void SetGeneratedTextColumnStyle(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column is DataGridTextColumn column && sender is DataGrid dataGrid)
            {
                column.ElementStyle = GetAutoGeneratedTextStyle(dataGrid);
            }
        }
        #endregion

        #region AttachedProperty : AutoGeneratedEditingTextStyleProperty
        public static readonly DependencyProperty AutoGeneratedEditingTextStyleProperty
            = DependencyProperty.RegisterAttached("AutoGeneratedEditingTextStyle", typeof(Style), typeof(DataGridAttach),
                new PropertyMetadata(default(Style), AutoGeneratedEditingTextStylePropertyChangedCallback));

        public static Style GetAutoGeneratedEditingTextStyle(DataGrid element)
        {
            return (Style)element.GetValue(AutoGeneratedEditingTextStyleProperty);
        }

        public static void SetAutoGeneratedEditingTextStyle(DataGrid element, Style value)
        {
            element.SetValue(AutoGeneratedEditingTextStyleProperty, value);
        }

        private static void AutoGeneratedEditingTextStylePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            dataGrid.AutoGeneratingColumn -= SetGeneratedTextColumnEditingStyle;
            dataGrid.AutoGeneratingColumn += SetGeneratedTextColumnEditingStyle;
        }

        private static void SetGeneratedTextColumnEditingStyle(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column is DataGridTextColumn column && sender is DataGrid dataGrid)
            {
                column.EditingElementStyle = GetAutoGeneratedEditingTextStyle(dataGrid);
            }
        }
        #endregion

        #region AttachedProperty : AutoGeneratedComboBoxStyleProperty
        public static readonly DependencyProperty AutoGeneratedComboBoxStyleProperty
            = DependencyProperty.RegisterAttached("AutoGeneratedComboBoxStyle", typeof(Style), typeof(DataGridAttach),
                new PropertyMetadata(default(Style), AutoGeneratedComboBoxStylePropertyChangedCallback));

        public static Style GetAutoGeneratedComboBoxStyle(DataGrid element)
        {
            return (Style)element.GetValue(AutoGeneratedComboBoxStyleProperty);
        }

        public static void SetAutoGeneratedComboBoxStyle(DataGrid element, Style value)
        {
            element.SetValue(AutoGeneratedComboBoxStyleProperty, value);
        }

        private static void AutoGeneratedComboBoxStylePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            dataGrid.AutoGeneratingColumn -= SetGeneratedComboBoxColumnStyle;
            dataGrid.AutoGeneratingColumn += SetGeneratedComboBoxColumnStyle;
        }

        private static void SetGeneratedComboBoxColumnStyle(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column is DataGridComboBoxColumn column && sender is DataGrid dataGrid)
            {
                column.ElementStyle = GetAutoGeneratedComboBoxStyle(dataGrid);
            }
        }
        #endregion

        #region AttachedProperty : AutoGeneratedEditingComboBoxStyleProperty
        public static readonly DependencyProperty AutoGeneratedEditingComboBoxStyleProperty
            = DependencyProperty.RegisterAttached("AutoGeneratedEditingComboBoxStyle", typeof(Style), typeof(DataGridAttach),
                new PropertyMetadata(default(Style), AutoGeneratedEditingComboBoxStylePropertyChangedCallback));

        public static Style GetAutoGeneratedEditingComboBoxStyle(DataGrid element)
        {
            return (Style)element.GetValue(AutoGeneratedEditingComboBoxStyleProperty);
        }

        public static void SetAutoGeneratedEditingComboBoxStyle(DataGrid element, Style value)
        {
            element.SetValue(AutoGeneratedEditingComboBoxStyleProperty, value);
        }

        private static void AutoGeneratedEditingComboBoxStylePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            dataGrid.AutoGeneratingColumn -= SetGeneratedComboBoxColumnEditingStyle;
            dataGrid.AutoGeneratingColumn += SetGeneratedComboBoxColumnEditingStyle;
        }

        private static void SetGeneratedComboBoxColumnEditingStyle(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column is DataGridComboBoxColumn column && sender is DataGrid dataGrid)
            {
                column.EditingElementStyle = GetAutoGeneratedEditingComboBoxStyle(dataGrid);
            }
        }
        #endregion

        #region AttachedProperty : ApplyDataGridColumnStylesProperty
        public static readonly DependencyProperty ApplyDataGridColumnStylesProperty
            = DependencyProperty.RegisterAttached("ApplyDataGridColumnStyles", typeof(bool), typeof(DataGridAttach),
                new PropertyMetadata(default(bool), ApplyDataGridColumnStylesPropertyChangedCallback));

        public static void SetApplyDataGridColumnStyles(DataGrid element, bool value)
        {
            element.SetValue(ApplyDataGridColumnStylesProperty, value);
        }

        public static bool GetApplyDataGridColumnStyles(DataGrid element)
        {
            return (bool)element.GetValue(ApplyDataGridColumnStylesProperty);
        }

        private static void ApplyDataGridColumnStylesPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            dataGrid.Columns.CollectionChanged -= ColumnsCollectionChanged;
            if (Equals(true, e.NewValue))
            {
                dataGrid.Columns.CollectionChanged += ColumnsCollectionChanged;// Auto-generated columns are added later in the chain, thus we subscribe to changes.
                foreach (var column in dataGrid.Columns)
                {
                    ApplyDataGridColumnStyleForColumn(dataGrid, column);
                }
            }

            void ColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                foreach (DataGridColumn column in e.NewItems?.OfType<DataGridColumn>() ?? Enumerable.Empty<DataGridColumn>())
                {
                    ApplyDataGridColumnStyleForColumn(dataGrid, column);
                }
            }
        }

        private static void ApplyDataGridColumnStyleForColumn(DataGrid dataGrid, DataGridColumn column)
        {
            Style defaultElementStyle = (Style)DataGridBoundColumn.ElementStyleProperty.GetMetadata(column.GetType()).DefaultValue;
            Style defaultEditingElementStyle = (Style)DataGridBoundColumn.EditingElementStyleProperty.GetMetadata(column.GetType()).DefaultValue;

            bool applyElementStyle;
            bool applyEditingElementStyle;
            switch (column)
            {
                case DataGridCheckBoxColumn checkBoxColumn:
                    applyElementStyle = Equals(checkBoxColumn.ElementStyle, defaultElementStyle);
                    applyEditingElementStyle = Equals(checkBoxColumn.EditingElementStyle, defaultEditingElementStyle);
                    if (applyElementStyle && GetAutoGeneratedCheckBoxStyle(dataGrid) is Style checkBoxElementStyle)
                    {
                        checkBoxColumn.ElementStyle = checkBoxElementStyle;
                    }

                    if (applyEditingElementStyle && GetAutoGeneratedEditingCheckBoxStyle(dataGrid) is Style checkBoxEditingElementStyle)
                    {
                        checkBoxColumn.EditingElementStyle = checkBoxEditingElementStyle;
                    }
                    break;
                case DataGridTextColumn textColumn:
                    applyElementStyle = Equals(textColumn.ElementStyle, defaultElementStyle);
                    applyEditingElementStyle = Equals(textColumn.EditingElementStyle, defaultEditingElementStyle);
                    if (applyElementStyle && GetAutoGeneratedTextStyle(dataGrid) is Style textElementStyle)
                    {
                        textColumn.ElementStyle = textElementStyle;
                    }

                    if (applyEditingElementStyle && GetAutoGeneratedEditingTextStyle(dataGrid) is Style textEditingElementStyle)
                    {
                        textColumn.EditingElementStyle = textEditingElementStyle;
                    }
                    break;
                case DataGridComboBoxColumn comboBoxColumn:
                    applyElementStyle = Equals(comboBoxColumn.ElementStyle, defaultElementStyle);
                    applyEditingElementStyle = Equals(comboBoxColumn.EditingElementStyle, defaultEditingElementStyle);
                    if (applyElementStyle && GetAutoGeneratedComboBoxStyle(dataGrid) is Style comboBoxElementStyle)
                    {
                        comboBoxColumn.ElementStyle = comboBoxElementStyle;
                    }

                    if (applyEditingElementStyle && GetAutoGeneratedEditingComboBoxStyle(dataGrid) is Style comboBoxEditingElementStyle)
                    {
                        comboBoxColumn.EditingElementStyle = comboBoxEditingElementStyle;
                    }
                    break;
            }
        }
        #endregion

        #region AttachedProperty : IsEnableEditBoxAssistProperty
        public static readonly DependencyProperty IsEnableEditBoxAssistProperty
            = DependencyProperty.RegisterAttached("IsEnableEditBoxAssist", typeof(bool), typeof(DataGridAttach),
                new PropertyMetadata(default(bool), IsEnableEditBoxAssistPropertyChangedCallback));

        public static bool GetIsEnableEditBoxAssist(DataGrid element)
        {
            return (bool)element.GetValue(IsEnableEditBoxAssistProperty);
        }

        public static void SetIsEnableEditBoxAssist(DataGrid element, bool value)
        {
            element.SetValue(IsEnableEditBoxAssistProperty, value);
        }

        private static void IsEnableEditBoxAssistPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            var enableCheckBoxAssist = (bool)e.NewValue;

            if (enableCheckBoxAssist)
            {
                // Register for bubbling events from cells, even when the cell handles them (thus the 'true' parameter)
                dataGrid.AddHandler(UIElement.MouseLeftButtonDownEvent, (RoutedEventHandler)OnMouseLeftButtonDown, true);
                dataGrid.PreviewKeyDown += EditOnSpacebarPress;
            }
            else
            {
                dataGrid.RemoveHandler(UIElement.MouseLeftButtonDownEvent, (RoutedEventHandler)OnMouseLeftButtonDown);
                dataGrid.PreviewKeyDown -= EditOnSpacebarPress;
            }
        }

        // This relay is only needed because the UIElement.AddHandler() has strict requirements for the signature of the passed Delegate
        private static void OnMouseLeftButtonDown(object sender, RoutedEventArgs e) => AllowDirectEditWithoutFocus(sender, (MouseButtonEventArgs)e);

        private static void EditOnSpacebarPress(object sender, KeyEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            if (e.Key == Key.Space && e.OriginalSource is DataGridCell { IsReadOnly: false } cell)
            {
                if (cell.Column is DataGridComboBoxColumn or DataGridTextColumn)
                {
                    dataGrid.BeginEdit();
                    e.Handled = true;
                }
            }
        }

        #endregion

        /// <summary>
        /// Allows editing of components inside of a data grid cell with a single left click.
        /// </summary>
        private static void AllowDirectEditWithoutFocus(object sender, MouseButtonEventArgs mouseArgs)
        {
            var originalSource = (DependencyObject)mouseArgs.OriginalSource;
            var dataGridCell = originalSource
                .GetVisualAncestry()
                .OfType<DataGridCell>()
                .FirstOrDefault();

            // Readonly has to be handled as the pass-through ignores the
            // cell and interacts directly with the content
            if (dataGridCell?.IsReadOnly ?? true)
            {
                return;
            }

            if (dataGridCell.Content is UIElement element)
            {
                var dataGrid = (DataGrid)sender;
                // If it is a DataGridTemplateColumn we want the
                // click to be handled naturally by the control
                if (dataGridCell.Column.GetType() == typeof(DataGridTemplateColumn))
                {
                    return;
                }
                if (dataGridCell.IsEditing)
                {
                    // If the cell is already being edited, we don't want to (re)start editing
                    return;
                }
                //NB: Issue 2852 - Don't handle events from nested data grids
                var parentDataGrid = dataGridCell
                    .GetVisualAncestry()
                    .OfType<DataGrid>()
                    .FirstOrDefault();
                if (parentDataGrid != dataGrid)
                {
                    return;
                }

                dataGrid.CurrentCell = new DataGridCellInfo(dataGridCell);
                dataGrid.BeginEdit();

                switch (dataGridCell.Content)
                {
                    case TextBoxBase textBox:
                        {
                            // Send a 'left-click' routed event to the TextBox to place the I-beam at the position of the mouse cursor
                            var mouseDownEvent = new MouseButtonEventArgs(mouseArgs.MouseDevice, mouseArgs.Timestamp, mouseArgs.ChangedButton)
                            {
                                RoutedEvent = Mouse.MouseDownEvent,
                                Source = mouseArgs.Source
                            };
                            textBox.RaiseEvent(mouseDownEvent);
                            break;
                        }

                    case ToggleButton toggleButton:
                        {
                            // Check if the cursor actually hit the checkbox and not just the cell
                            var mousePosition = mouseArgs.GetPosition(element);
                            var elementHitBox = new Rect(element.RenderSize);
                            if (elementHitBox.Contains(mousePosition))
                            {
                                // Send a 'left click' routed command to the toggleButton to toggle the state
                                var newMouseEvent = new MouseButtonEventArgs(mouseArgs.MouseDevice, mouseArgs.Timestamp, mouseArgs.ChangedButton)
                                {
                                    RoutedEvent = Mouse.MouseDownEvent,
                                    Source = dataGrid
                                };

                                toggleButton.RaiseEvent(newMouseEvent);
                            }
                            break;
                        }

                    // Open the dropdown explicitly. Left clicking is not
                    // viable, as it would edit the text and not open the
                    // dropdown
                    case ComboBox comboBox:
                        {
                            comboBox.IsDropDownOpen = true;
                            break;
                        }
                }
            }
        }
    }
}