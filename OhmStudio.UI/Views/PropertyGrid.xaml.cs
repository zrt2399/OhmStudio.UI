using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using OhmStudio.UI.Attachs;
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
                        foreach (var uIElement in item.Children.OfType<UIElement>())
                        {
                            BindingOperations.ClearAllBindings(uIElement);
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

        public static readonly List<Type> NumericTypes = new List<Type>()
        {
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(short),
            typeof(ushort),
            typeof(byte),
            typeof(sbyte),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(char),

            typeof(int?),
            typeof(uint?),
            typeof(long?),
            typeof(ulong?),
            typeof(short?),
            typeof(ushort?),
            typeof(byte?),
            typeof(sbyte?),
            typeof(float?),
            typeof(double?),
            typeof(decimal?),
            typeof(char?),
        };

        void SetPlaceHolder(PropertyInfo propertyInfo, UIElement uIElement)
        {
            if (uIElement.IsTextBoxAttachObject())
            {
                if (propertyInfo.GetCustomAttribute(typeof(PropertyGridPlaceHolderAttribute)) is not PropertyGridPlaceHolderAttribute placeHolder)
                {
                    return;
                }
                TextBoxPlaceholderAttach.SetPlaceHolder(uIElement, placeHolder.PlaceHolder);
                TextBoxAttachBase.SetForeground(uIElement, placeHolder.PlaceHolderForeground);
                TextBoxAttachBase.SetMargin(uIElement, placeHolder.PlaceHolderMargin);
                TextBoxAttachBase.SetOpacity(uIElement, placeHolder.PlaceHolderOpacity);
            }
        }

        PropertyGridAttribute GetAttribute(PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetCustomAttribute(typeof(PropertyGridIgnoreAttribute)) != null)
            {
                return null;
            }
            PropertyGridAttribute result = new PropertyGridAttribute();
            var customAttributes = propertyInfo.GetCustomAttributes(false);
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

        bool IsNotSystemClass(Type type)
        {
            bool isStruct = !type.IsPrimitive && !type.IsEnum && type.IsValueType;
            return (type.IsClass || type.IsInterface || isStruct) && !type.Namespace.StartsWith("System") && !type.Namespace.StartsWith("Microsoft");
        }

        Binding GetBinding(object obj, PropertyInfo propertyInfo)
        {
            var mode = propertyInfo.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay;
            var binding = new Binding() { Source = obj, Path = new PropertyPath(propertyInfo.Name), Mode = mode };
            if (propertyInfo.GetCustomAttribute(typeof(PropertyChangedUpdateSourceAttribute)) != null)
            {
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            }
            return binding;
        }

        BindingFlags GetBindingFlags(Type type)
        {
            if (type.GetCustomAttribute(typeof(BaseObjectIgnoreAttribute)) == null)
            {
                return BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
            }
            else
            {
                return BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly;
            }
        }

        List<double> widths = new List<double>();
        void Create(object obj, ItemsControl itemsControl)
        {
            if (obj == null)
            {
                return;
            }
            var type = obj.GetType();
            foreach (PropertyInfo item in type.GetProperties(GetBindingFlags(type)))
            {
                var attribute = GetAttribute(item);
                if (attribute == null)
                {
                    continue;
                }
                UIElement uIElement = null;
                bool isUnknown = false;

                if (item.PropertyType.IsEnum)
                {
                    var ComboBox = new ComboBox();
                    Binding selectedItem = GetBinding(obj, item);
                    Binding itemsSource = new Binding() { Source = Enum.GetValues(item.PropertyType) };
                    ComboBox.SetBinding(Selector.SelectedItemProperty, selectedItem);
                    ComboBox.SetBinding(ItemsControl.ItemsSourceProperty, itemsSource);
                    uIElement = ComboBox;
                }
                else if (item.PropertyType == typeof(bool) || item.PropertyType == typeof(bool?))
                {
                    var checkBox = new CheckBox();
                    Binding binding = GetBinding(obj, item);
                    checkBox.SetBinding(ToggleButton.IsCheckedProperty, binding);
                    uIElement = checkBox;
                }
                else if (item.PropertyType == typeof(DateTime) || item.PropertyType == typeof(DateTime?))
                {
                    var dateTimePicker = new DateTimePicker();
                    Binding binding = GetBinding(obj, item);
                    dateTimePicker.SetBinding(DateTimePicker.DateAndTimeProperty, binding);
                    uIElement = dateTimePicker;
                }
                else if (item.PropertyType == typeof(string) || NumericTypes.Contains(item.PropertyType))
                {
                    var customattribute = item.GetCustomAttribute(typeof(PasswordAttribute));
                    if (customattribute == null)
                    {
                        var textBox = new TextBox();
                        Binding binding = GetBinding(obj, item);
                        textBox.SetBinding(TextBox.TextProperty, binding);
                        uIElement = textBox;
                    }
                    else
                    {
                        var passwordAttribute = customattribute as PasswordAttribute;
                        var passwordBoxControl = new PasswordBoxControl() { CanShowPassword = passwordAttribute.CanShowPassword };
                        Binding binding = GetBinding(obj, item);
                        passwordBoxControl.SetBinding(PasswordBoxControl.PasswordProperty, binding);
                        uIElement = passwordBoxControl;
                    }
                }
                else if (typeof(IEnumerable).IsAssignableFrom(item.PropertyType))
                {
                    var ComboBox = new ComboBox() { SelectedIndex = 0 };
                    Binding binding = GetBinding(obj, item);
                    ComboBox.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                    VirtualizingPanel.SetIsVirtualizing(ComboBox, true);
                    VirtualizingPanel.SetVirtualizationMode(ComboBox, VirtualizationMode.Recycling);
                    uIElement = ComboBox;
                }
                else if (IsNotSystemClass(item.PropertyType))
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
                    SetPlaceHolder(item, uIElement);
                    if (uIElement is TextBox textBox)
                    {
                        textBox.IsReadOnly = attribute.IsReadOnly;
                        if (isUnknown || !item.CanWrite)
                        {
                            textBox.IsReadOnly = true;
                        }
                    }
                    else
                    {
                        uIElement.IsEnabled = !attribute.IsReadOnly;
                        if (!item.CanWrite)
                        {
                            uIElement.IsEnabled = false;
                        }
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