using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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
            //Binding binding = new Binding();
            //binding.Source = this;
            //binding.Path = new PropertyPath(ItemsSourceProperty);
            //itemsControl.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }

        public static readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register(nameof(SelectedObject), typeof(object), typeof(PropertyGrid), new PropertyMetadata(null, (sender, e) =>
            {
                if (sender is PropertyGrid propertyGrid)
                {
                    propertyGrid.itemsControl.Items.Clear();
                    Debug.WriteLine(123);
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

        List<double> widths = new List<double>();
        void Create(object obj, ItemsControl itemsControl)
        {
            foreach (var item in obj.GetType().GetProperties())
            {
                string name = null;
                var customAttributes = item.GetCustomAttributes(typeof(PropertyGridAttribute), false);
                foreach (var customAttribute in customAttributes)
                {
                    if (customAttribute is PropertyGridAttribute propertyGridAttribute)
                    {
                        name = propertyGridAttribute.DisplayName;
                        break;
                    }
                }
                name ??= item.Name;


                UIElement uIElement = null;

                if (item.PropertyType == typeof(string))
                {
                    uIElement = new TextBox();
                }
                else if (item.PropertyType == typeof(bool))
                {
                    uIElement = new CheckBox();
                }
                else if (item.PropertyType.IsClass)
                {
                    Create(item.GetValue(obj), itemsControl);
                }
                else
                {
                    uIElement = new TextBox();
                }
                if (uIElement != null)
                {
                    DockPanel dockPanel = new DockPanel() { Margin = new Thickness(8) };
                    TextBlock textBlock = new TextBlock() { Text = name, Margin = new Thickness(0, 0, 4, 0) };
                    widths.Add(textBlock.Width);
                    dockPanel.Children.Add(textBlock);
                    dockPanel.Children.Add(uIElement);
                    itemsControl.Items.Add(dockPanel);
                }
            }
        }
    }
}