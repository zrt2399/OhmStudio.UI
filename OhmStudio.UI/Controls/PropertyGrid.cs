using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using OhmStudio.UI.Attaches;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Controls
{
    public class PropertyGrid : Control
    {
        public static readonly DependencyProperty ItemSpacingProperty =
            DependencyProperty.Register(nameof(ItemSpacing), typeof(Thickness), typeof(PropertyGrid), new PropertyMetadata(new Thickness(0, 8, 0, 0)));

        public Thickness ItemSpacing
        {
            get => (Thickness)GetValue(ItemSpacingProperty);
            set => SetValue(ItemSpacingProperty, value);
        }

        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register(nameof(HorizontalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(PropertyGrid), new PropertyMetadata(ScrollBarVisibility.Disabled));

        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
            set => SetValue(HorizontalScrollBarVisibilityProperty, value);
        }

        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register(nameof(VerticalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(PropertyGrid), new PropertyMetadata(ScrollBarVisibility.Auto));

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
            set => SetValue(VerticalScrollBarVisibilityProperty, value);
        }

        internal static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(ObservableCollection<UIElement>), typeof(PropertyGrid));

        internal ObservableCollection<UIElement> ItemsSource
        {
            get => (ObservableCollection<UIElement>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register(nameof(SelectedObject), typeof(object), typeof(PropertyGrid), new PropertyMetadata(null, (sender, e) =>
            {
                if (sender is PropertyGrid propertyGrid)
                {
                    propertyGrid.ItemsSource ??= new ObservableCollection<UIElement>();
                    foreach (var item in propertyGrid.ItemsSource.OfType<DockPanel>())
                    {
                        foreach (var uIElement in item.Children.OfType<UIElement>())
                        {
                            BindingOperations.ClearAllBindings(uIElement);
                        }
                    }
                    for (int i = 0; i < propertyGrid.ItemsSource.Count; i++)
                    {
                        propertyGrid.ItemsSource[i] = null;
                    }
                    propertyGrid.ItemsSource.Clear();
                    propertyGrid._widths.Clear();
                    if (e.NewValue != null)
                    {
                        propertyGrid.Create(e.NewValue, propertyGrid.ItemsSource);
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
            if (uIElement.IsTextBoxAttachObject() && propertyInfo.GetCustomAttribute<TextBoxPlaceHolderAttribute>() is TextBoxPlaceHolderAttribute placeHolder)
            {
                TextBoxAttach.SetPlaceHolder(uIElement, placeHolder.PlaceHolder);
                TextBoxAttach.SetPlaceHolderOpacity(uIElement, placeHolder.PlaceHolderOpacity);
                TextBoxAttach.SetPlaceHolderIsHitTestVisible(uIElement, placeHolder.PlaceHolderIsHitTestVisible);
            }
        }

        PropertyGridAttribute GetAttribute(PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetCustomAttribute<PropertyGridIgnoreAttribute>() != null)
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
            if (propertyInfo.GetCustomAttribute<PropertyChangedUpdateSourceAttribute>() != null)
            {
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            }
            return binding;
        }

        BindingFlags GetBindingFlags(Type type)
        {
            if (type.GetCustomAttribute<BaseObjectIgnoreAttribute>() == null)
            {
                return BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
            }
            else
            {
                return BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly;
            }
        }

        List<double> _widths = new List<double>();
        void Create(object obj, ObservableCollection<UIElement> itemsControl)
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
                    dateTimePicker.SetBinding(DateTimePicker.SelectedDateTimeProperty, binding);
                    uIElement = dateTimePicker;
                }
                else if (item.PropertyType == typeof(string) || NumericTypes.Contains(item.PropertyType))
                {
                    var passwordAttribute = item.GetCustomAttribute<PasswordAttribute>();
                    if (passwordAttribute == null)
                    {
                        var textBox = new TextBox();
                        Binding binding = GetBinding(obj, item);
                        textBox.SetBinding(TextBox.TextProperty, binding);
                        uIElement = textBox;
                    }
                    else
                    {
                        var passwordTextBox = new PasswordTextBox() { CanShowPassword = passwordAttribute.CanShowPassword };
                        Binding binding = GetBinding(obj, item);
                        passwordTextBox.SetBinding(PasswordTextBox.PasswordProperty, binding);
                        uIElement = passwordTextBox;
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
                    var toolTipAttribute = item.GetCustomAttribute<ToolTipAttribute>();
                    string toolTip = toolTipAttribute == null ? attribute.DisplayName : toolTipAttribute.ToolTip;
                    DockPanel dockPanel = new DockPanel();
                    TextBox textBlock = new TextBox()
                    {
                        Text = attribute.DisplayName,
                        ToolTip = toolTip,
                        Padding = new Thickness(0),
                        IsReadOnly = true,
                        BorderBrush = Brushes.Transparent,
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0),
                        Tag = "Title"
                    };
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
                    dockPanel.Margin = itemsControl.Count > 0 ? ItemSpacing : new Thickness(0);
                    itemsControl.Add(dockPanel);
                    textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    _widths.Add(textBlock.DesiredSize.Width + 20);
                    var max = _widths.Max();
                    foreach (var panel in itemsControl.OfType<DockPanel>())
                    {
                        foreach (var text in panel.Children.OfType<TextBox>().Where(x => x.Tag?.ToString() == "Title" && (double.IsNaN(x.Width) || x.Width < max)))
                        {
                            text.Width = max;
                        }
                    }
                }
            }
        }
    }
}
