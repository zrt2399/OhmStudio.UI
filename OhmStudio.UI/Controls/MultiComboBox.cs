using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;

namespace OhmStudio.UI.Controls
{
    [TemplatePart(Name = "PART_Panel", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_SelectAll", Type = typeof(CheckComboBoxItem))]
    public class MultiComboBox : ListBox
    {
        private const string ElementPanel = "PART_Panel";

        private const string ElementSelectAll = "PART_SelectAll";

        private Panel _panel;

        private CheckComboBoxItem _selectAllItem;

        private bool _isInternalAction;

        public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register("IsError", typeof(bool), typeof(CheckComboBox), new PropertyMetadata(false));

        public static readonly DependencyProperty ErrorStrProperty = DependencyProperty.Register("ErrorStr", typeof(string), typeof(CheckComboBox), new PropertyMetadata((object)null));

        public static readonly DependencyProperty TextTypeProperty = DependencyProperty.Register("TextType", typeof(TextType), typeof(CheckComboBox), new PropertyMetadata(TextType.Common));

        public static readonly DependencyProperty ShowClearButtonProperty = DependencyProperty.Register("ShowClearButton", typeof(bool), typeof(CheckComboBox), new PropertyMetadata(ValueBoxes.FalseBox));

        public static readonly DependencyProperty MaxDropDownHeightProperty = ComboBox.MaxDropDownHeightProperty.AddOwner(typeof(CheckComboBox), new FrameworkPropertyMetadata(SystemParameters.PrimaryScreenHeight / 3.0));

        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(CheckComboBox), new PropertyMetadata(false, OnIsDropDownOpenChanged));

        public static readonly DependencyProperty TagStyleProperty = DependencyProperty.Register("TagStyle", typeof(Style), typeof(CheckComboBox), new PropertyMetadata((object)null));

        public static readonly DependencyProperty ShowSelectAllButtonProperty = DependencyProperty.Register("ShowSelectAllButton", typeof(bool), typeof(CheckComboBox), new PropertyMetadata(false));

        public bool IsError
        {
            get
            {
                return (bool)GetValue(IsErrorProperty);
            }
            set
            {
                SetValue(IsErrorProperty, value);
            }
        }

        public string ErrorStr
        {
            get
            {
                return (string)GetValue(ErrorStrProperty);
            }
            set
            {
                SetValue(ErrorStrProperty, value);
            }
        }

        public TextType TextType
        {
            get
            {
                return (TextType)GetValue(TextTypeProperty);
            }
            set
            {
                SetValue(TextTypeProperty, value);
            }
        }

        public bool ShowClearButton
        {
            get
            {
                return (bool)GetValue(ShowClearButtonProperty);
            }
            set
            {
                SetValue(ShowClearButtonProperty, value);
            }
        }

        [Bindable(true)]
        [Category("Layout")]
        [TypeConverter(typeof(LengthConverter))]
        public double MaxDropDownHeight
        {
            get
            {
                return (double)GetValue(MaxDropDownHeightProperty);
            }
            set
            {
                SetValue(MaxDropDownHeightProperty, value);
            }
        }

        public bool IsDropDownOpen
        {
            get
            {
                return (bool)GetValue(IsDropDownOpenProperty);
            }
            set
            {
                SetValue(IsDropDownOpenProperty, value);
            }
        }

        public Style TagStyle
        {
            get
            {
                return (Style)GetValue(TagStyleProperty);
            }
            set
            {
                SetValue(TagStyleProperty, value);
            }
        }

        public bool ShowSelectAllButton
        {
            get
            {
                return (bool)GetValue(ShowSelectAllButtonProperty);
            }
            set
            {
                SetValue(ShowSelectAllButtonProperty, value);
            }
        }

        public Func<string, OperationResult<bool>> VerifyFunc { get; set; }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckComboBox checkComboBox = (CheckComboBox)d;
            if (!(bool)e.NewValue)
            {
                checkComboBox.Dispatcher.BeginInvoke((Action)delegate
                {
                    Mouse.Capture(null);
                }, DispatcherPriority.Send);
            }
        }

        public CheckComboBox()
        {
            AddHandler(HandyControl.Controls.Tag.ClosedEvent, new RoutedEventHandler(Tags_OnClosed));
            base.CommandBindings.Add(new CommandBinding(ControlCommands.Clear, delegate
            {
                SetCurrentValue(Selector.SelectedValueProperty, null);
                SetCurrentValue(Selector.SelectedItemProperty, null);
                SetCurrentValue(Selector.SelectedIndexProperty, -1);
                base.SelectedItems.Clear();
            }));
        }

        public override void OnApplyTemplate()
        {
            if (_selectAllItem != null)
            {
                _selectAllItem.Selected -= SelectAllItem_Selected;
                _selectAllItem.Unselected -= SelectAllItem_Unselected;
            }

            base.OnApplyTemplate();
            _panel = GetTemplateChild("PART_Panel") as Panel;
            _selectAllItem = GetTemplateChild("PART_SelectAll") as CheckComboBoxItem;
            if (_selectAllItem != null)
            {
                _selectAllItem.Selected += SelectAllItem_Selected;
                _selectAllItem.Unselected += SelectAllItem_Unselected;
            }

            UpdateTags();
        }

        public bool VerifyData()
        {
            OperationResult<bool> operationResult = ((VerifyFunc != null) ? VerifyFunc(null) : ((base.SelectedItems.Count > 0) ? OperationResult.Success() : ((!InfoElement.GetNecessary(this)) ? OperationResult.Success() : OperationResult.Failed(Lang.IsNecessary))));
            bool flag = !operationResult.Data;
            if (flag)
            {
                SetCurrentValue(IsErrorProperty, ValueBoxes.TrueBox);
                SetCurrentValue(ErrorStrProperty, operationResult.Message);
            }
            else
            {
                flag = Validation.GetHasError(this);
                if (flag)
                {
                    SetCurrentValue(ErrorStrProperty, Validation.GetErrors(this)[0].ErrorContent?.ToString());
                }
                else
                {
                    SetCurrentValue(IsErrorProperty, ValueBoxes.FalseBox);
                    SetCurrentValue(ErrorStrProperty, null);
                }
            }

            return !flag;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            UpdateTags();
            VerifyData();
            base.OnSelectionChanged(e);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CheckComboBoxItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CheckComboBoxItem();
        }

        protected override void OnDisplayMemberPathChanged(string oldDisplayMemberPath, string newDisplayMemberPath)
        {
            UpdateTags();
        }

        private void Tags_OnClosed(object sender, RoutedEventArgs e)
        {
            Tag tag = e.OriginalSource as Tag;
            if (tag != null)
            {
                base.SelectedItems.Remove(tag.Tag);
                _panel.Children.Remove(tag);
            }
        }

        private void SwitchAllItems(bool selected)
        {
            if (!_isInternalAction)
            {
                _isInternalAction = true;
                if (!selected)
                {
                    UnselectAll();
                }
                else
                {
                    SelectAll();
                }

                _isInternalAction = false;
                UpdateTags();
            }
        }

        private void SelectAllItem_Selected(object sender, RoutedEventArgs e)
        {
            SwitchAllItems(selected: true);
        }

        private void SelectAllItem_Unselected(object sender, RoutedEventArgs e)
        {
            SwitchAllItems(selected: false);
        }

        private void UpdateTags()
        {
            if (_panel == null || _isInternalAction)
            {
                return;
            }

            if (_selectAllItem != null)
            {
                _isInternalAction = true;
                _selectAllItem.SetCurrentValue(Selector.IsSelectedProperty, base.SelectedItems.Count == base.Items.Count);
                _isInternalAction = false;
            }

            _panel.Children.Clear();
            foreach (object selectedItem in base.SelectedItems)
            {
                Tag tag = new Tag
                {
                    Style = TagStyle,
                    Tag = selectedItem
                };
                if (base.ItemsSource != null)
                {
                    tag.SetBinding(ContentControl.ContentProperty, new Binding(base.DisplayMemberPath)
                    {
                        Source = selectedItem
                    });
                }
                else
                {
                    tag.Content = (IsItemItsOwnContainerOverride(selectedItem) ? ((CheckComboBoxItem)selectedItem).Content : selectedItem);
                }

                _panel.Children.Add(tag);
            }
        }
    }
}