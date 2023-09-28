using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath(ItemsSourceProperty);
            itemsControl.SetBinding(ItemsControl.ItemsSourceProperty, binding);

            //for (int i = 0; i < 50; i++)
            //{
            //    StackPanel stackPanel = new StackPanel();
            //    stackPanel.Orientation = Orientation.Horizontal;
            //    stackPanel.Children.Add(new TextBlock() { Text = $"TextBlock{i + 1}:" });
            //    stackPanel.Children.Add(new TextBox());
            //    itemsControl.Items.Add(stackPanel);
            //}
        }

        public static readonly DependencyProperty ItemsSourceProperty = ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(PropertyGrid));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set
            {
                if (value == null)
                {
                    ClearValue(ItemsSourceProperty);
                }
                else
                {
                    SetValue(ItemsSourceProperty, value);
                }
            }
        }

        //private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    ItemsControl itemsControl = (ItemsControl)d;
        //    IEnumerable oldValue = (IEnumerable)e.OldValue;
        //    IEnumerable enumerable = (IEnumerable)e.NewValue;
        //    ((IContainItemStorage)itemsControl).Clear();
        //    BindingExpressionBase beb = BindingOperations.GetBindingExpressionBase(d, ItemsSourceProperty);
        //    if (beb != null)
        //    {
        //        itemsControl.Items.SetItemsSource(enumerable, (object x) => beb.GetSourceItem(x));
        //    }
        //    else if (e.NewValue != null)
        //    {
        //        itemsControl.Items.SetItemsSource(enumerable);
        //    }
        //    else
        //    {
        //        itemsControl.Items.ClearItemsSource();
        //    }

        //}
    }
}