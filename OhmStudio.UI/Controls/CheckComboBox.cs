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
        }

        ListBox PART_ListBox;
        TextBox PART_EditableTextBox;
        ToggleButton DropDownButton;
        //Button PART_Invert;
        Button PART_SelectAll;
        Button PART_DeSelectAll;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_SelectAll = GetTemplateChild("PART_SelectAll") as Button;
            PART_DeSelectAll = GetTemplateChild("PART_DeSelectAll") as Button;
            //PART_Invert = GetTemplateChild("PART_Invert") as Button;
            DropDownButton = GetTemplateChild("DropDownButton") as ToggleButton;
            PART_ListBox = GetTemplateChild("PART_ListBox") as ListBox;
            PART_EditableTextBox = GetTemplateChild("PART_EditableTextBox") as TextBox;
            DropDownButton.Click += delegate
            {
                if (!PART_EditableTextBox.IsKeyboardFocusWithin)
                {
                    PART_EditableTextBox.Focus();
                    PART_EditableTextBox.SelectAll();
                }
            };
            //PART_Invert.Click += delegate
            //{
            //    SelectElement(true, true);
            //};
            PART_SelectAll.Click += delegate
            {
                SelectElement(true);
            };
            PART_DeSelectAll.Click += delegate
            {
                SelectElement(false);
            };
            PART_ListBox.SelectionChanged += PART_ListBox_SelectionChanged;
        }

        void SelectElement(bool value)
        {
            PART_ListBox.SelectionChanged -= PART_ListBox_SelectionChanged;
            if (value)
            {
                PART_ListBox.SelectAll();
            }
            else
            {
                PART_ListBox.UnselectAll();
            }

            //var rr = PART_ListBox.FindVisualChildren<ListBoxItem>();
            //for (int i = 0; i < PART_ListBox.Items.Count; i++)
            //{

            //    var res = PART_ListBox.Items[i];

            //    var res1 = res as ListBoxItem;
            //    var obj = PART_ListBox.ItemContainerGenerator.ContainerFromIndex(i);
            //    var listBoxItem = obj as ListBoxItem;
            //    if (listBoxItem != null)
            //    {
            //        if (isInvert)
            //        {
            //            listBoxItem.IsSelected = !listBoxItem.IsSelected;
            //        }
            //        else
            //        {
            //            listBoxItem.IsSelected = value;
            //        }
            //    }
            //}
            PART_ListBox.SelectionChanged += PART_ListBox_SelectionChanged;
            //PART_ListBox_SelectionChanged(PART_ListBox, null);
            var eventArg = new SelectionChangedEventArgs(SelectionChangedEvent, new List<object>(), new List<object>());
            PART_ListBox.RaiseEvent(eventArg);
        }

        void SetEmpty()
        {
            SelectedItems = new List<object>();
            SelectedText = "(未选择)";
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
                stringBuilder.Append(list[i].ToString() + ",");
            }
            if (list.Count > 0)
            {
                stringBuilder.Append(list[list.Count - 1].ToString());
            }
            SelectedText = stringBuilder.ToString();
            //SelectedText = string.Join(",", list);
            if (list.Count == 0)
            {
                SelectedText = "(未选择)";
            }
            else if (list.Count == ItemsSource.Cast<object>().Count())
            {
                SelectedText = "(已选择全部)";
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            SetEmpty();
        }

        public static readonly DependencyProperty SelectedTextProperty =
            DependencyProperty.Register(nameof(SelectedText), typeof(string), typeof(CheckComboBox), new PropertyMetadata("(未选择)"));

        //public static readonly DependencyProperty IsDropDownOpenProperty =
        //    DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(CheckComboBox));

        //public static readonly DependencyProperty IsReadOnlyProperty =
        //    DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(CheckComboBox), new PropertyMetadata(false));

        //public static readonly DependencyProperty ItemsSourceProperty =
        //    DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(CheckComboBox), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IEnumerable), typeof(CheckComboBox));

        public string SelectedText
        {
            get => (string)GetValue(SelectedTextProperty);
            set => SetValue(SelectedTextProperty, value);
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
    }
}