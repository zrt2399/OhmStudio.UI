using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace OhmStudio.UI.Controls
{
    public class CheckComboBox : ComboBox
    {
        const string AllText = "(全部)";
        private bool _stopRiseSelectionChanged = false;

        ListBox PART_ListBox;
        TextBox PART_EditableTextBox;
        ToggleButton DropDownButton;
        public CheckComboBox()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DropDownButton = GetTemplateChild("DropDownButton") as ToggleButton;
            PART_ListBox = GetTemplateChild("PART_ListBox") as ListBox;
            PART_EditableTextBox = GetTemplateChild("PART_EditableTextBox") as TextBox;
            DropDownButton.Click += delegate
            {
                if (!PART_EditableTextBox.IsFocused)
                {
                    PART_EditableTextBox.Focus();
                    PART_EditableTextBox.SelectAll();
                }
            };
        }

        public static new readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(CheckComboBox));

        //public static readonly DependencyProperty IsDropDownOpenProperty =
        //    DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(CheckComboBox));

        //public static readonly DependencyProperty IsReadOnlyProperty =
        //    DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(CheckComboBox), new PropertyMetadata(false));

        //public static readonly DependencyProperty ItemsSourceProperty =
        //    DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(CheckComboBox), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IEnumerable), typeof(CheckComboBox), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemsChanged)));

        public new string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        //public bool IsDropDownOpen
        //{
        //    get => (bool)GetValue(IsDropDownOpenProperty);
        //    set => SetValue(IsDropDownOpenProperty, value);
        //}

        //public bool IsReadOnly
        //{
        //    get => (bool)GetValue(IsReadOnlyProperty);
        //    set => SetValue(IsReadOnlyProperty, value);
        //}

        //public IEnumerable ItemsSource
        //{
        //    get => (IEnumerable)GetValue(ItemsSourceProperty);
        //    set => SetValue(ItemsSourceProperty, value);
        //}

        public IEnumerable SelectedItems
        {
            get => (IEnumerable)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        private static void UpdateItems(CheckComboBox control)
        {
            if (control.ItemsSource == null || control.PART_ListBox == null)
            {
                return;
            }

            control._stopRiseSelectionChanged = true;
            control.PART_ListBox.SelectedItems.Clear();

            if (control.SelectedItems == null)
            {
                foreach (var item in control.ItemsSource)
                {
                    if (!control.PART_ListBox.SelectedItems.Contains(item))
                    {
                        control.PART_ListBox.SelectedItems.Add(item);
                    }
                }
            }
            else
            {
                foreach (var item in control.SelectedItems)
                {
                    if (!control.PART_ListBox.SelectedItems.Contains(item))
                    {
                        control.PART_ListBox.SelectedItems.Add(item);
                    }
                }
            }
            control.UpdateText();
            control._stopRiseSelectionChanged = false;
        }

        private static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            CheckComboBox control = (CheckComboBox)obj;
            UpdateItems(control);
        }

        private static void OnSelectedItemsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            CheckComboBox control = (CheckComboBox)obj;
            UpdateItems(control);
        }

        private void UpdateText()
        {
            if (SelectedItems != null)
            {
                if (SelectedItems.Cast<object>().Count() == 0)
                {
                    Text = "（空）";
                }
                else
                {
                    var stringBuilder = new StringBuilder();
                    foreach (var item in SelectedItems)
                    {
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append(',');
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
            var array = new object[PART_ListBox.SelectedItems.Count];
            PART_ListBox.SelectedItems.CopyTo(array, 0);

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