using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace OhmStudio.UI.Controls
{
    public class CheckComboBox : ComboBox
    {
        public CheckComboBox()
        {
            IsReadOnly = true;
            Text = "(未选择)";
            GotFocus += CheckComboBox_GotFocus;
        }

        private void CheckComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var checkComboBox = (CheckComboBox)sender;
            if (!e.Handled && checkComboBox.PART_EditableTextBox != null)
            {
                if (Equals(e.OriginalSource, checkComboBox))
                {
                    checkComboBox.PART_EditableTextBox.Focus();
                    e.Handled = true;
                }
                else if (Equals(e.OriginalSource, checkComboBox.PART_EditableTextBox))
                {
                    checkComboBox.PART_EditableTextBox.SelectAll();
                    e.Handled = true;
                }
            }
        }

        ListBox PART_ListBox;
        TextBox PART_EditableTextBox;
        ToggleButton DropDownButton;
        Button PART_Invert;
        Button PART_SelectAll;
        Button PART_DeSelectAll;

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IEnumerable), typeof(CheckComboBox));

        public IEnumerable SelectedItems
        {
            get => (IEnumerable)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public static readonly DependencyProperty SeparatorProperty =
            DependencyProperty.Register(nameof(Separator), typeof(string), typeof(CheckComboBox), new PropertyMetadata(","));

        public string Separator
        {
            get => (string)GetValue(SeparatorProperty);
            set => SetValue(SeparatorProperty, value);
        }

        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register(nameof(StringFormat), typeof(string), typeof(CheckComboBox), new PropertyMetadata("{0}"));

        public string StringFormat
        {
            get => (string)GetValue(StringFormatProperty);
            set => SetValue(StringFormatProperty, value);
        }

        public override void OnApplyTemplate()
        {
            if (DropDownButton != null)
            {
                DropDownButton.Click -= DropDownButton_Click;
            }
            if (PART_Invert != null)
            {
                PART_Invert.Click -= PART_Invert_Click;
            }
            if (PART_SelectAll != null)
            {
                PART_SelectAll.Click -= PART_SelectAll_Click;
            }
            if (PART_DeSelectAll != null)
            {
                PART_DeSelectAll.Click -= PART_DeSelectAll_Click;
            }
            if (PART_ListBox != null)
            {
                PART_ListBox.SelectionChanged -= PART_ListBox_SelectionChanged;
            }
            base.OnApplyTemplate();
            PART_SelectAll = GetTemplateChild("PART_SelectAll") as Button;
            PART_DeSelectAll = GetTemplateChild("PART_DeSelectAll") as Button;
            PART_Invert = GetTemplateChild("PART_Invert") as Button;
            DropDownButton = GetTemplateChild("DropDownButton") as ToggleButton;
            PART_ListBox = GetTemplateChild("PART_ListBox") as ListBox;
            PART_EditableTextBox = GetTemplateChild("PART_EditableTextBox") as TextBox;
            DropDownButton.Click += DropDownButton_Click;
            PART_Invert.Click += PART_Invert_Click;
            PART_SelectAll.Click += PART_SelectAll_Click;
            PART_DeSelectAll.Click += PART_DeSelectAll_Click;
            PART_ListBox.SelectionChanged += PART_ListBox_SelectionChanged;
        }

        private void DropDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (!PART_EditableTextBox.IsKeyboardFocusWithin)
            {
                PART_EditableTextBox.Focus();
                PART_EditableTextBox.SelectAll();
            }
        }

        private void PART_Invert_Click(object sender, RoutedEventArgs e)
        {
            SelectElement(false, true);
        }

        private void PART_DeSelectAll_Click(object sender, RoutedEventArgs e)
        {
            SelectElement(false);
        }

        private void PART_SelectAll_Click(object sender, RoutedEventArgs e)
        {
            SelectElement(true);
        }

        void SelectElement(bool value, bool isInvert = false)
        {
            PART_ListBox.SelectionChanged -= PART_ListBox_SelectionChanged;

            if (isInvert)
            {
                foreach (var item in PART_ListBox.Items)
                {
                    if (PART_ListBox.SelectedItems.Contains(item))
                    {
                        PART_ListBox.SelectedItems.Remove(item);
                    }
                    else
                    {
                        PART_ListBox.SelectedItems.Add(item);
                    }
                }
            }
            else
            {
                if (value)
                {
                    PART_ListBox.SelectAll();
                }
                else
                {
                    PART_ListBox.UnselectAll();
                }
            }
            //for (int i = 0; i < PART_ListBox.Items.Count; i++)
            //{  
            //    var obj = PART_ListBox.ItemContainerGenerator.ContainerFromIndex(i);
            //    if (obj is not ListBoxItem listBoxItem)
            //    {
            //        PART_ListBox.ScrollIntoView(PART_ListBox.Items[i]);
            //        listBoxItem = PART_ListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
            //    }
            //    listBoxItem.IsSelected = !listBoxItem.IsSelected;  
            //}
            PART_ListBox.SelectionChanged += PART_ListBox_SelectionChanged;
            //PART_ListBox_SelectionChanged(PART_ListBox, null);
            var eventArg = new SelectionChangedEventArgs(SelectionChangedEvent, new List<object>(), new List<object>());
            PART_ListBox.RaiseEvent(eventArg);
        }

        void SetEmpty()
        {
            SelectedItems = new List<object>();
            Text = "(未选择)";
        }

        private void PART_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemsSource == null || PART_ListBox.SelectedItems.Count == 0)
            {
                SetEmpty();
                return;
            }
            var array = new object[PART_ListBox.SelectedItems.Count];
            PART_ListBox.SelectedItems.CopyTo(array, 0);

            if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
            {
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = array[i].GetType().GetProperty(DisplayMemberPath)?.GetValue(array[i]);
                }
            }

            IList list = new List<object>();
            foreach (var item in array)
            {
                list.Add(item);
            }
            SelectedItems = list;

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < list.Count - 1; i++)
            {
                stringBuilder.Append(string.Format(StringFormat, list[i] + Separator));
            }
            if (list.Count > 0)
            {
                stringBuilder.Append(string.Format(StringFormat, list[list.Count - 1]));
            }
            Text = stringBuilder.ToString();
            //SelectedText = string.Join(",", list);
            if (list.Count == 0)
            {
                Text = "(未选择)";
            }
            else if (list.Count == ItemsSource.Cast<object>().Count())
            {
                Text = "(已选择全部)";
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            SetEmpty();
        }
    }
}