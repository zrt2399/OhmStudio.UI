using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using OhmStudio.UI.PublicMethod;

namespace OhmStudio.UI.Views
{
    /// <summary>
    /// PropertyGrid.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyGrid : UserControl
    {
        public PropertyGrid()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register(nameof(SelectedObject), typeof(object), typeof(PropertyGrid), new PropertyMetadata(null, (sender, e) =>
            {
                if (sender is PropertyGrid propertyGrid)
                {
                    foreach (var item in propertyGrid.itemsControl.Items.OfType<DockPanel>())
                    {
                        foreach (var uiElement in item.Children.OfType<UIElement>())
                        {
                            BindingOperations.ClearAllBindings(uiElement);
                        }
                    }
                    for (int i = 0; i < propertyGrid.itemsControl.Items.Count; i++)
                    {
                        propertyGrid.itemsControl.Items[i] = null;
                    }
                    propertyGrid.itemsControl.Items.Clear();
                    propertyGrid.widths.Clear();
                    if (e.NewValue != null)
                    {
                        propertyGrid.Create(e.NewValue, propertyGrid.itemsControl);
                    }
                }
            }));

        public object SelectedObject
        {
            get => GetValue(SelectedObjectProperty);
            set => SetValue(SelectedObjectProperty, value);
        }

        PropertyGridAttribute GetAttribute(PropertyInfo propertyInfo)
        {
            PropertyGridAttribute result = new PropertyGridAttribute();
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(PropertyGridAttribute), false);
            foreach (var customAttribute in customAttributes)
            {
                if (customAttribute is PropertyGridAttribute propertyGridAttribute)
                {
                    result.DisplayName = propertyGridAttribute.DisplayName;
                    result.IsReadOnly = propertyGridAttribute.IsReadOnly;
                    break;
                }
            }
            result.DisplayName ??= propertyInfo.Name;
            return result;
        }

        bool IsNumber(PropertyInfo propertyInfo)
        {
            bool isNumber = false;
            //if (propertyInfo.PropertyType.IsPrimitive)
            //{
            TypeCode typeCode = Type.GetTypeCode(propertyInfo.PropertyType);
            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    isNumber = true;
                    break;
            }
            //}
            return isNumber;
        }

        List<double> widths = new List<double>();
        void Create(object obj, ItemsControl itemsControl)
        {
            if (obj == null)
            {
                return;
            }
            foreach (PropertyInfo item in obj.GetType().GetProperties())
            {
                var attribute = GetAttribute(item);
                UIElement uIElement = null;
                bool isUnknown = false;

                if (item.PropertyType.IsEnum)
                {
                    var ComboBox = new ComboBox();
                    Binding selectedItem = new Binding() { Source = obj, Path = new PropertyPath(item.Name) };
                    Binding itemsSource = new Binding() { Source = Enum.GetValues(item.PropertyType) };
                    ComboBox.SetBinding(Selector.SelectedItemProperty, selectedItem);
                    ComboBox.SetBinding(ItemsControl.ItemsSourceProperty, itemsSource);
                    uIElement = ComboBox;
                }
                else if (item.PropertyType == typeof(bool))
                {
                    var checkBox = new CheckBox();
                    Binding binding = new Binding() { Source = obj, Path = new PropertyPath(item.Name) };
                    checkBox.SetBinding(ToggleButton.IsCheckedProperty, binding);
                    uIElement = checkBox;
                }
                else if (item.PropertyType == typeof(DateTime))
                {
                    var dateTimePicker = new DateTimePicker();
                    Binding binding = new Binding() { Source = obj, Path = new PropertyPath(item.Name), Mode = BindingMode.TwoWay };
                    dateTimePicker.SetBinding(DateTimePicker.DateTimeProperty, binding);
                    uIElement = dateTimePicker;
                }
                else if (item.PropertyType == typeof(string) || item.PropertyType == typeof(char) || IsNumber(item))
                {
                    var textBox = new TextBox();
                    Binding binding = new Binding() { Source = obj, Path = new PropertyPath(item.Name) };
                    textBox.SetBinding(TextBox.TextProperty, binding);
                    uIElement = textBox;
                }
                else if (typeof(IEnumerable).IsAssignableFrom(item.PropertyType))
                {
                    var ComboBox = new ComboBox() { SelectedIndex = 0 };
                    Binding binding = new Binding() { Source = obj, Path = new PropertyPath(item.Name) };
                    ComboBox.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                    VirtualizingPanel.SetIsVirtualizing(ComboBox, true);
                    VirtualizingPanel.SetVirtualizationMode(ComboBox, VirtualizationMode.Recycling);
                    uIElement = ComboBox;
                }
                else if ((item.PropertyType.IsClass || item.PropertyType.IsInterface) && !item.PropertyType.Namespace.StartsWith("System"))
                {
                    Create(item.GetValue(obj), itemsControl);
                }
                else
                {
                    uIElement = new TextBox() { Text = item.GetValue(obj)?.ToString() };
                    isUnknown = true;
                }
                if (uIElement != null)
                {
                    DockPanel dockPanel = new DockPanel() { Margin = new Thickness(8, 4, 8, 4) };
                    TextBlock textBlock = new TextBlock() { Text = attribute.DisplayName, Margin = new Thickness(0, 0, 4, 0) };
                    if (uIElement is TextBox textBox)
                    {
                        textBox.IsReadOnly = attribute.IsReadOnly;
                        if (isUnknown)
                        {
                            textBox.IsReadOnly = true;
                        }
                    }
                    else
                    {
                        uIElement.IsEnabled = !attribute.IsReadOnly;
                    }
                    dockPanel.Children.Add(textBlock);
                    dockPanel.Children.Add(uIElement);
                    itemsControl.Items.Add(dockPanel);
                    textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    widths.Add(textBlock.DesiredSize.Width);
                    var max = widths.Max();
                    foreach (var panel in itemsControl.Items.OfType<DockPanel>())
                    {
                        foreach (var text in panel.Children.OfType<TextBlock>())
                        {
                            if (double.IsNaN(text.Width) || text.Width < max)
                            {
                                text.Width = max;
                            }
                        }
                    }
                }
            }
        }
    }
}