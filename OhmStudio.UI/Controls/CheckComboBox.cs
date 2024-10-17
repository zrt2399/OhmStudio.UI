using System;
using System.Collections;
using System.Collections.Specialized;
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
        private ListBox PART_ListBox;
        private TextBox PART_TextBox;
        private bool _isUpdatingSelectedItems;

        public CheckComboBox()
        {
            SetEmpty();
            GotFocus += CheckComboBox_GotFocus;
            SelectAllCommand = new RelayCommand(SelectAll);
            UnselectAllCommand = new RelayCommand(UnselectAll);
            InvertSelectCommand = new RelayCommand(InvertSelect);
        }

        static CheckComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckComboBox), new FrameworkPropertyMetadata(typeof(CheckComboBox)));
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(CheckComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsChanged));

        public static readonly DependencyProperty SeparatorProperty =
            DependencyProperty.Register(nameof(Separator), typeof(string), typeof(CheckComboBox), new PropertyMetadata(",", UpdateSelectedItems));

        public static readonly DependencyProperty ItemDisplayStringFormatProperty =
            DependencyProperty.Register(nameof(ItemDisplayStringFormat), typeof(string), typeof(CheckComboBox), new PropertyMetadata("{0}", UpdateSelectedItems));

        public static readonly DependencyProperty UnselectedstringProperty =
            DependencyProperty.Register(nameof(Unselectedstring), typeof(string), typeof(CheckComboBox), new PropertyMetadata("(未选择)", UpdateSelectedItems));

        public static readonly DependencyProperty SelectedAllStringProperty =
            DependencyProperty.Register(nameof(SelectedAllString), typeof(string), typeof(CheckComboBox), new PropertyMetadata("(已选择全部)", UpdateSelectedItems));

        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(CheckComboBox), new PropertyMetadata(TextWrapping.NoWrap));

        public static readonly DependencyProperty SelectedTextProperty =
            DependencyProperty.Register(nameof(SelectedText), typeof(string), typeof(CheckComboBox));

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(nameof(SelectionMode), typeof(SelectionMode), typeof(CheckComboBox), new PropertyMetadata(SelectionMode.Multiple, UpdateSelectedItems));

        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public string Separator
        {
            get => (string)GetValue(SeparatorProperty);
            set => SetValue(SeparatorProperty, value);
        }

        public string ItemDisplayStringFormat
        {
            get => (string)GetValue(ItemDisplayStringFormatProperty);
            set => SetValue(ItemDisplayStringFormatProperty, value);
        }

        public string Unselectedstring
        {
            get => (string)GetValue(UnselectedstringProperty);
            set => SetValue(UnselectedstringProperty, value);
        }

        public string SelectedAllString
        {
            get => (string)GetValue(SelectedAllStringProperty);
            set => SetValue(SelectedAllStringProperty, value);
        }

        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

        public string SelectedText
        {
            get => (string)GetValue(SelectedTextProperty);
            set => SetValue(SelectedTextProperty, value);
        }

        public SelectionMode SelectionMode
        {
            get => (SelectionMode)GetValue(SelectionModeProperty);
            set => SetValue(SelectionModeProperty, value);
        }

        public bool IsInit { get; private set; }

        public ICommand SelectAllCommand { get; }

        public ICommand UnselectAllCommand { get; }

        public ICommand InvertSelectCommand { get; }

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
            if (!IsInit)
            {
                IsInit = true;
                OnSelectedItemsChanged(null, SelectedItems);
            }
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var checkComboBox = (CheckComboBox)d;
            IList oldSelectedItems = (IList)e.OldValue;
            IList newSelectedItems = (IList)e.NewValue;
            checkComboBox.OnSelectedItemsChanged(oldSelectedItems, newSelectedItems);
        }

        protected virtual void OnSelectedItemsChanged(IList oldSelectedItems, IList newSelectedItems)
        {
            if (_isUpdatingSelectedItems || !IsInit)
            {
                return;
            }

            PART_ListBox.UnselectAll();
            if (oldSelectedItems != null)
            {
                if (oldSelectedItems is INotifyCollectionChanged notifyCollectionChanged)
                {
                    notifyCollectionChanged.CollectionChanged -= CollectionChanged;
                }
            }
            if (newSelectedItems != null)
            {
                if (SelectionMode == SelectionMode.Single)
                {
                    PART_ListBox.SelectedItem = newSelectedItems.Count > 0 ? newSelectedItems[newSelectedItems.Count - 1] : null;
                }
                else
                {
                    foreach (var item in newSelectedItems)
                    {
                        PART_ListBox.SelectedItems.Add(item);
                    }
                }

                if (newSelectedItems is INotifyCollectionChanged notifyCollectionChanged)
                {
                    notifyCollectionChanged.CollectionChanged += CollectionChanged;
                }
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isUpdatingSelectedItems)
            {
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (SelectionMode == SelectionMode.Single)
                {
                    PART_ListBox.SelectedItem = e.NewItems.Count > 0 ? e.NewItems[e.NewItems.Count - 1] : null;
                }
                else
                {
                    foreach (var item in e.NewItems)
                    {
                        PART_ListBox.SelectedItems.Add(item);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (SelectionMode == SelectionMode.Single)
                {
                    PART_ListBox.SelectedItem = null;
                }
                else
                {
                    foreach (var oldItem in e.OldItems)
                    {
                        PART_ListBox.SelectedItems.Remove(oldItem);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var newItem = e.NewItems[0];
                var oldItem = e.OldItems[0];
                if (SelectionMode == SelectionMode.Single)
                {
                    PART_ListBox.SelectedItem = newItem;
                }
                else
                {
                    PART_ListBox.SelectedItems.Add(newItem);
                    PART_ListBox.SelectedItems.Remove(oldItem);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                PART_ListBox.UnselectAll();
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            SetEmpty();
        }

        public void SelectAll()
        {
            SelectElement(true);
        }

        public void UnselectAll()
        {
            SelectElement(false);
        }

        public void InvertSelect()
        {
            SelectElement(true, true);
        }

        private void SelectElement(bool value, bool invert = false)
        {
            if (!IsInit)
            {
                return;
            }

            try
            {
                PART_ListBox.SelectionChanged -= PART_ListBox_SelectionChanged;
                if (invert)
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
            }
            finally
            {
                PART_ListBox.SelectionChanged += PART_ListBox_SelectionChanged;
                UpdateSelectedItems(PART_ListBox);
            }
        }

        private void SetEmpty()
        {
            SelectedItems = ItemsSource == null ? null : Array.Empty<object>();
            SelectedText = Unselectedstring;
        }

        private static void UpdateSelectedItems(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var checkComboBox = d as CheckComboBox;
            if (checkComboBox.IsInit)
            {
                checkComboBox.UpdateSelectedItems(checkComboBox.PART_ListBox);
            }
        }

        private void UpdateSelectedItems(ListBox listBox)
        {
            if (ItemsSource == null || Items.Count == 0 || listBox.SelectedItems.Count == 0)
            {
                SetEmpty();
                return;
            }

            try
            {
                _isUpdatingSelectedItems = true;
                SelectedItems = listBox.SelectedItems;
                var stringResult = new string[SelectedItems.Count];

                for (int i = 0; i < SelectedItems.Count; i++)
                {
                    Binding binding = new Binding(DisplayMemberPath) { Source = SelectedItems[i] };
                    TextBlock textBlock = new TextBlock();
                    textBlock.SetBinding(TextBlock.TextProperty, binding);
                    stringResult[i] = textBlock.Text;
                    BindingOperations.ClearAllBindings(textBlock);
                }

                if (SelectedItems.Count == Items.Count && !string.IsNullOrEmpty(SelectedAllString))
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
            finally
            {
                _isUpdatingSelectedItems = false;
            }
        }

        private void PART_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedItems(sender as ListBox);
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