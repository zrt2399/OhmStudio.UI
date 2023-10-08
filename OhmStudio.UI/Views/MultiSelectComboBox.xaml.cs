using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace OhmStudio.UI.Views
{
    /// <summary>
    /// MultiSelectComboBox.xaml 的交互逻辑
    /// </summary>
    public partial class MultiSelectComboBox : UserControl
    {
        const string AllText = "(全部)";
        private bool _stopRiseSelectionChanged = false;

        ListBox listBox;
        Popup popChioce;

        public MultiSelectComboBox()
        {
            InitializeComponent();              
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            listBox = Template.FindName("listBox", this) as ListBox;
            popChioce = Template.FindName("popChioce", this) as Popup;
            listBox.SetBinding(ItemsControl.ItemsSourceProperty, new Binding() { Source = this, Path = new PropertyPath(ItemsSourceProperty) });
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(MultiSelectComboBox));

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(MultiSelectComboBox));

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(MultiSelectComboBox), new PropertyMetadata(false));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(MultiSelectComboBox), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IEnumerable), typeof(MultiSelectComboBox), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemsChanged)));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public IEnumerable SelectedItems
        {
            get => (IEnumerable)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        private static void UpdateItems(MultiSelectComboBox control)
        {
            if (control.ItemsSource == null || control.listBox == null)
            {
                return;
            }

            control._stopRiseSelectionChanged = true;
            control.listBox.SelectedItems.Clear();

            if (control.SelectedItems == null)
            {
                foreach (var item in control.ItemsSource)
                {
                    if (!control.listBox.SelectedItems.Contains(item))
                    {
                        control.listBox.SelectedItems.Add(item);
                    }
                }
            }
            else
            {
                foreach (var item in control.SelectedItems)
                {
                    if (!control.listBox.SelectedItems.Contains(item))
                    {
                        control.listBox.SelectedItems.Add(item);
                    }
                }
            }
            control.UpdateText();
            control._stopRiseSelectionChanged = false;
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectComboBox control = (MultiSelectComboBox)d;
            UpdateItems(control);
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectComboBox control = (MultiSelectComboBox)d;
            UpdateItems(control);
        }

        private void UpdateText()
        {
            if (SelectedItems != null)
            {
                if (SelectedItems.Cast<object>().Count() == 0)
                {
                    Text = "(空)";
                }
                else
                {
                    var stringBuilder = new StringBuilder();
                    foreach (var item in SelectedItems)
                    {
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append(",");
                        }
                        stringBuilder.Append(item.ToString());
                    }
                    Text = stringBuilder.ToString();
                }
            }
            else
            {
                Text = AllText;
            }
        }


        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_stopRiseSelectionChanged || ItemsSource == null)
            {
                return;
            }
            var array = new object[listBox.SelectedItems.Count];
            listBox.SelectedItems.CopyTo(array, 0);

            if (ItemsSource.Cast<object>().Except(array).Count() == 0)
            {
                SelectedItems = null;
            }
            else
            {
                IList items = (IList)Activator.CreateInstance(typeof(ObservableCollection<>).MakeGenericType(ItemsSource.GetType().GetGenericArguments()[0]));
                if (SelectedItems != null)
                {
                    foreach (var item in SelectedItems)
                    {
                        items.Add(item);
                    }
                }

                foreach (var item in e.AddedItems)
                {
                    if (item is ListBoxItem)
                    {
                        SelectedItems = null;
                        return;
                    }
                    if (!items.Contains(item))
                    {
                        items.Add(item);
                    }
                }

                foreach (var item in e.RemovedItems)
                {
                    if (items.Contains(item))
                    {
                        items.Remove(item);
                    }
                }

                SelectedItems = items;
            }
        } 
    }
}