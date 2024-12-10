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
using OhmStudio.UI.Utilities;

namespace OhmStudio.UI.Controls
{
    public class PropertyGrid : Control
    {
        private readonly List<double> _widths = new List<double>();

        static PropertyGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
        }

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

        public static readonly DependencyProperty PropertyBindingFlagsProperty =
            DependencyProperty.Register(nameof(PropertyBindingFlags), typeof(BindingFlags), typeof(PropertyGrid), new PropertyMetadata(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, OnUpdateSelectedObject));

        public BindingFlags PropertyBindingFlags
        {
            get => (BindingFlags)GetValue(PropertyBindingFlagsProperty);
            set => SetValue(PropertyBindingFlagsProperty, value);
        }

        internal static readonly DependencyProperty ItemsCollectionProperty =
            DependencyProperty.Register(nameof(ItemsCollection), typeof(ObservableCollection<Panel>), typeof(PropertyGrid));

        internal ObservableCollection<Panel> ItemsCollection
        {
            get => (ObservableCollection<Panel>)GetValue(ItemsCollectionProperty);
            set => SetValue(ItemsCollectionProperty, value);
        }

        public static readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register(nameof(SelectedObject), typeof(object), typeof(PropertyGrid), new PropertyMetadata(default, OnUpdateSelectedObject));

        public object SelectedObject
        {
            get => GetValue(SelectedObjectProperty);
            set => SetValue(SelectedObjectProperty, value);
        }

        public static List<Type> NumericTypes { get; } = new List<Type>()
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
            typeof(char?)
        };

        private static void OnUpdateSelectedObject(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var propertyGrid = (PropertyGrid)d;
            propertyGrid.OnUpdateSelectedObject(propertyGrid.SelectedObject);
        }

        protected virtual void OnUpdateSelectedObject(object selectedObject)
        {
            ItemsCollection ??= new ObservableCollection<Panel>();
            for (int i = 0; i < ItemsCollection.Count; i++)
            {
                var item = ItemsCollection[i];
                for (int j = 0; j < item.Children.Count; j++)
                {
                    BindingOperations.ClearAllBindings(item.Children[j]);
                }
                ItemsCollection[i] = null;
            }
            _widths.Clear();
            ItemsCollection.Clear();
            if (selectedObject != null)
            {
                Create(selectedObject, ItemsCollection);
            }
        }

        private void SetVirtualizing(PropertyInfo propertyInfo, UIElement uIElement)
        {
            if (uIElement is ComboBox comboBox)
            {
                var virtualizingPanelAttribute = propertyInfo.GetCustomAttribute<VirtualizingPanelAttribute>();
                if (virtualizingPanelAttribute == null)
                {
                    VirtualizingPanel.SetIsVirtualizing(comboBox, true);
                    VirtualizingPanel.SetVirtualizationMode(comboBox, VirtualizationMode.Recycling);
                }
                else
                {
                    VirtualizingPanel.SetIsVirtualizing(comboBox, virtualizingPanelAttribute.IsVirtualizing);
                    VirtualizingPanel.SetVirtualizationMode(comboBox, virtualizingPanelAttribute.IsVirtualizing ? VirtualizationMode.Recycling : VirtualizationMode.Standard);
                    VirtualizingPanel.SetScrollUnit(comboBox, virtualizingPanelAttribute.ScrollUnit);
                }
            }
        }

        private void SetPlaceHolder(PropertyInfo propertyInfo, UIElement uIElement)
        {
            if (uIElement.IsPlaceHolderObject() && propertyInfo.GetCustomAttribute<TextBoxPlaceHolderAttribute>() is TextBoxPlaceHolderAttribute placeHolder)
            {
                TextBoxAttach.SetPlaceHolder(uIElement, placeHolder.PlaceHolder);
                TextBoxAttach.SetPlaceHolderOpacity(uIElement, placeHolder.PlaceHolderOpacity);
            }
        }

        private PropertyGridAttribute GetAttribute(PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetCustomAttribute<PropertyGridIgnoreAttribute>() != null)
            {
                return null;
            }
            PropertyGridAttribute result = new PropertyGridAttribute();
            if (propertyInfo.GetCustomAttribute<PropertyGridAttribute>() is PropertyGridAttribute propertyGridAttribute)
            {
                result.DisplayName = propertyGridAttribute.DisplayName;
                result.IsReadOnly = propertyGridAttribute.IsReadOnly;
            }

            result.DisplayName ??= propertyInfo.Name;
            return result;
        }

        private bool IsNotSystemClass(Type type)
        {
            bool isStruct = !type.IsPrimitive && !type.IsEnum && type.IsValueType;
            return (type.IsClass || type.IsInterface || isStruct) &&
                !type.Namespace.StartsWith("System", StringComparison.OrdinalIgnoreCase) &&
                !type.Namespace.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase);
        }

        private Binding GetBinding(object obj, PropertyInfo propertyInfo)
        {
            var mode = propertyInfo.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay;
            var binding = new Binding() { Source = obj, Path = new PropertyPath(propertyInfo.Name), Mode = mode };
            if (propertyInfo.GetCustomAttribute<PropertyChangedUpdateSourceAttribute>() != null)
            {
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            }
            return binding;
        }

        private BindingFlags GetBindingFlags(Type type)
        {
            var flag = PropertyBindingFlags;
            if (type.GetCustomAttribute<BaseObjectIgnoreAttribute>() != null)
            {
                flag |= BindingFlags.DeclaredOnly;
            }
            return flag;
        }

        private void Create(object obj, ObservableCollection<Panel> itemsCollection)
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
                Binding binding = GetBinding(obj, item);
                if (item.PropertyType.IsEnum)
                {
                    var comboBox = new ComboBox();
                    Binding itemsSource = new Binding() { Source = Enum.GetValues(item.PropertyType) };
                    comboBox.SetBinding(Selector.SelectedItemProperty, binding);
                    comboBox.SetBinding(ItemsControl.ItemsSourceProperty, itemsSource);
                    uIElement = comboBox;
                }
                else if (item.PropertyType == typeof(bool) || item.PropertyType == typeof(bool?))
                {
                    var checkBox = new CheckBox();
                    checkBox.SetBinding(ToggleButton.IsCheckedProperty, binding);
                    uIElement = checkBox;
                }
                else if (item.PropertyType == typeof(DateTime) || item.PropertyType == typeof(DateTime?))
                {
                    var dateTimePicker = new DateTimePicker();
                    dateTimePicker.SetBinding(DateTimePicker.SelectedDateTimeProperty, binding);
                    uIElement = dateTimePicker;
                }
                else if (item.PropertyType == typeof(string) || NumericTypes.Contains(item.PropertyType))
                {
                    var passwordAttribute = item.GetCustomAttribute<PasswordAttribute>();
                    if (passwordAttribute == null)
                    {
                        var textBox = new TextBox();
                        textBox.SetBinding(TextBox.TextProperty, binding);
                        uIElement = textBox;
                    }
                    else
                    {
                        var passwordTextBox = new PasswordTextBox() { CanShowPassword = passwordAttribute.CanShowPassword, PasswordChar = passwordAttribute.PasswordChar };
                        passwordTextBox.SetBinding(PasswordTextBox.PasswordProperty, binding);
                        uIElement = passwordTextBox;
                    }
                }
                else if (typeof(IEnumerable).IsAssignableFrom(item.PropertyType))
                {
                    var comboBox = new ComboBox();
                    comboBox.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                    comboBox.SelectedIndex = 0;
                    uIElement = comboBox;
                }
                else if (IsNotSystemClass(item.PropertyType))
                {
                    Create(item.GetValue(obj), itemsCollection);
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

                    TextBox textBox = new TextBox()
                    {
                        Text = attribute.DisplayName,
                        ToolTip = toolTip,
                        Padding = new Thickness(0),
                        IsReadOnly = true,
                        BorderBrush = Brushes.Transparent,
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0),
                        Tag = "Text_Title"
                    };
                    SetPlaceHolder(item, uIElement);
                    SetVirtualizing(item, uIElement);
                    if (uIElement is TextBoxBase textBoxBase)
                    {
                        textBoxBase.IsReadOnly = isUnknown || !item.CanWrite || attribute.IsReadOnly;
                    }
                    else
                    {
                        uIElement.IsEnabled = item.CanWrite && !attribute.IsReadOnly;
                    }
                    DockPanel dockPanel = new DockPanel();
                    dockPanel.Children.Add(textBox);
                    dockPanel.Children.Add(uIElement);
                    dockPanel.Margin = itemsCollection.Count > 0 ? ItemSpacing : new Thickness(0);
                    itemsCollection.Add(dockPanel);
                    textBox.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    _widths.Add(textBox.DesiredSize.Width + 20);
                    var max = _widths.Max();
                    foreach (var panel in itemsCollection)
                    {
                        foreach (var text in panel.Children.OfType<TextBox>().Where(x => x.Tag?.ToString() == "Text_Title" && (double.IsNaN(x.Width) || x.Width < max)))
                        {
                            text.Width = max;
                        }
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyGridAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public bool IsReadOnly { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyGridIgnoreAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyChangedUpdateSourceAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PasswordAttribute : Attribute
    {
        public bool CanShowPassword { get; set; } = true;

        public char PasswordChar { get; set; } = PasswordTextBox.DefaultPasswordChar;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ToolTipAttribute : Attribute
    {
        public string ToolTip { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class VirtualizingPanelAttribute : Attribute
    {
        public bool IsVirtualizing { get; set; } = true;

        public ScrollUnit ScrollUnit { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class TextBoxPlaceHolderAttribute : Attribute
    {
        public string PlaceHolder { get; set; } = TextBoxAttach.PlaceHolder;

        public double PlaceHolderOpacity { get; set; } = TextBoxAttach.PlaceHolderOpacity;
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class BaseObjectIgnoreAttribute : Attribute
    {
    }
}