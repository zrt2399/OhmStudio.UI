using System;
using System.Collections;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using OhmStudio.UI.Commands;

namespace OhmStudio.UI.Controls
{
    public class CheckComboBox : ComboBox
    {
        public CheckComboBox()
        {
            SetEmpty();
            GotFocus += CheckComboBox_GotFocus;
            SelectAllCommand = new RelayCommand(() => SelectElement(true));
            UnselectAllCommand = new RelayCommand(() => SelectElement(false));
            InvertSelectionCommand = new RelayCommand(() => SelectElement(false, true));
        }

        static CheckComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckComboBox), new FrameworkPropertyMetadata(typeof(CheckComboBox)));
        }

        ListBox PART_ListBox;
        TextBox PART_TextBox;

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(CheckComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) =>
            {
                if (sender is CheckComboBox checkComboBox && checkComboBox.PART_ListBox != null)
                {
                    checkComboBox.IsSelectedAll = checkComboBox.SelectedItems.Count > 0 && checkComboBox.SelectedItems.Count == checkComboBox.PART_ListBox.Items.Count;
                }
            }));

        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public static readonly DependencyProperty SeparatorProperty =
            DependencyProperty.Register(nameof(Separator), typeof(string), typeof(CheckComboBox), new PropertyMetadata(","));

        public string Separator
        {
            get => (string)GetValue(SeparatorProperty);
            set => SetValue(SeparatorProperty, value);
        }

        public static readonly DependencyProperty ItemDisplayStringFormatProperty =
            DependencyProperty.Register(nameof(ItemDisplayStringFormat), typeof(string), typeof(CheckComboBox), new PropertyMetadata("{0}"));

        public string ItemDisplayStringFormat
        {
            get => (string)GetValue(ItemDisplayStringFormatProperty);
            set => SetValue(ItemDisplayStringFormatProperty, value);
        }

        public static readonly DependencyProperty UnselectedstringProperty =
            DependencyProperty.Register(nameof(Unselectedstring), typeof(string), typeof(CheckComboBox), new PropertyMetadata("(未选择)"));

        public string Unselectedstring
        {
            get => (string)GetValue(UnselectedstringProperty);
            set => SetValue(UnselectedstringProperty, value);
        }

        public static readonly DependencyProperty SelectedAllStringProperty =
            DependencyProperty.Register(nameof(SelectedAllString), typeof(string), typeof(CheckComboBox), new PropertyMetadata("(已选择全部)", (sender, e) =>
            {
                if (sender is CheckComboBox checkComboBox && checkComboBox.IsSelectedAll && !string.IsNullOrEmpty((string)e.NewValue))
                {
                    checkComboBox.SelectedText = (string)e.NewValue;
                }
            }));

        public string SelectedAllString
        {
            get => (string)GetValue(SelectedAllStringProperty);
            set => SetValue(SelectedAllStringProperty, value);
        }

        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(CheckComboBox), new PropertyMetadata(TextWrapping.NoWrap));

        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

        public static readonly DependencyProperty SelectedTextProperty =
            DependencyProperty.Register(nameof(SelectedText), typeof(string), typeof(CheckComboBox));

        public string SelectedText
        {
            get => (string)GetValue(SelectedTextProperty);
            set => SetValue(SelectedTextProperty, value);
        }

        public static readonly DependencyProperty IsSelectedAllProperty =
            DependencyProperty.Register(nameof(IsSelectedAll), typeof(bool), typeof(CheckComboBox));

        public bool IsSelectedAll
        {
            get => (bool)GetValue(IsSelectedAllProperty);
            set => SetValue(IsSelectedAllProperty, value);
        }

        public ICommand SelectAllCommand { get; }

        public ICommand UnselectAllCommand { get; }

        public ICommand InvertSelectionCommand { get; }

        public override void OnApplyTemplate()
        {
            if (PART_ListBox != null)
            {
                PART_ListBox.SelectionChanged -= PART_ListBox_SelectionChanged;
            }
            base.OnApplyTemplate();
            PART_ListBox = GetTemplateChild("PART_ListBox") as ListBox;
            PART_TextBox = GetTemplateChild("PART_TextBox") as TextBox;
            PART_ListBox.SelectionChanged += PART_ListBox_SelectionChanged;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            SetEmpty();
        }

        public void SelectElement(bool isSelectAll, bool isInvertSelection = false)
        {
            PART_ListBox.SelectionChanged -= PART_ListBox_SelectionChanged;

            if (isInvertSelection)
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
                if (isSelectAll)
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
            var eventArg = new SelectionChangedEventArgs(SelectionChangedEvent, Array.Empty<object>(), Array.Empty<object>());
            PART_ListBox.RaiseEvent(eventArg);
        }

        void SetEmpty()
        {
            SelectedItems = ItemsSource == null ? null : Array.Empty<object>();
            SelectedText = Unselectedstring;
        }

        private void PART_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (ItemsSource == null || listBox.Items.Count == 0 || listBox.SelectedItems.Count == 0)
            {
                SetEmpty();
                return;
            }

            var array = new object[listBox.SelectedItems.Count];
            var stringResult = new string[listBox.SelectedItems.Count];
            listBox.SelectedItems.CopyTo(array, 0);

            for (int i = 0; i < array.Length; i++)
            {
                Binding binding = new Binding(DisplayMemberPath) { Source = array[i] };
                TextBlock textBlock = new TextBlock();
                textBlock.SetBinding(TextBlock.TextProperty, binding);
                stringResult[i] = textBlock.Text;
                BindingOperations.ClearAllBindings(textBlock);
            }
            SelectedItems = array;

            if (SelectedItems.Count == listBox.Items.Count && !string.IsNullOrEmpty(SelectedAllString))
            {
                SelectedText = SelectedAllString;
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < stringResult.Length; i++)
                {
                    stringBuilder.Append(string.Format(ItemDisplayStringFormat, stringResult[i]));
                    if (i < stringResult.Length - 1)
                    {
                        stringBuilder.Append(Separator);
                    }
                }
                SelectedText = stringBuilder.ToString();
            }
        }

        private void CheckComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!e.Handled && PART_TextBox != null)
            {
                if (Equals(e.OriginalSource, this))
                {
                    PART_TextBox.Focus();
                    PART_TextBox.SelectAll();
                    e.Handled = true;
                }
            }
        }
    }
}