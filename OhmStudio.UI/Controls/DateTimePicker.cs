using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using OhmStudio.UI.Commands;
using OhmStudio.UI.Views;

namespace OhmStudio.UI.Controls
{
    public class DateTimePicker : Control, ITextChanged
    {
        private TextBox PART_TextBox;
        private Popup PART_Popup;
        public event TextChangedEventHandler TextChanged;

        string ITextChanged.Text => DateTimeText;

        public DateTimePicker()
        {
            GotFocus += DateTimePicker_GotFocus;
            CalendarClickCommand = new RelayCommand(() =>
            {
                IsDropDownOpen = !IsDropDownOpen;
                //PART_Popup.IsOpen = true;
            });
        }

        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
        }

        public static readonly DependencyProperty IsDateOnlyProperty =
            DependencyProperty.Register(nameof(IsDateOnly), typeof(bool), typeof(DateTimePicker), new PropertyMetadata((sender, e) =>
            {
                if (sender is DateTimePicker dateTimePicker && dateTimePicker.SelectedDateTime != null && (bool)e.NewValue)
                {
                    var dateTime = (DateTime)dateTimePicker.SelectedDateTime;
                    dateTimePicker.SelectedDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
                }
            }));

        public static readonly DependencyProperty SelectedDateTimeFormatProperty =
            DependencyProperty.Register(nameof(SelectedDateTimeFormat), typeof(string), typeof(DateTimePicker), new PropertyMetadata("yyyy/MM/dd HH:mm:ss", UpdateDateTimeText));

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(DateTimePicker));

        public static readonly DependencyProperty DisplayDateStartProperty =
            DependencyProperty.Register(nameof(DisplayDateStart), typeof(DateTime?), typeof(DateTimePicker), new PropertyMetadata((sender, e) =>
            {
                if (sender is DateTimePicker dateTimePicker && e.NewValue is DateTime dateTime)
                {
                    if (dateTime > dateTimePicker.SelectedDateTime)
                    {
                        dateTimePicker.SelectedDateTime = null;
                    }
                    CoerceDisplayDate(dateTimePicker);
                }
            }));

        public static readonly DependencyProperty DisplayDateEndProperty =
            DependencyProperty.Register(nameof(DisplayDateEnd), typeof(DateTime?), typeof(DateTimePicker), new PropertyMetadata((sender, e) =>
            {
                if (sender is DateTimePicker dateTimePicker && e.NewValue is DateTime dateTime)
                {
                    if (dateTime < dateTimePicker.SelectedDateTime)
                    {
                        dateTimePicker.SelectedDateTime = null;
                    }
                    CoerceDisplayDate(dateTimePicker);
                }
            }));

        public static readonly DependencyProperty FirstDayOfWeekProperty =
            DependencyProperty.Register(nameof(FirstDayOfWeek), typeof(DayOfWeek), typeof(DateTimePicker), new PropertyMetadata(CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek));

        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register(nameof(SelectedDateTime), typeof(DateTime?), typeof(DateTimePicker), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, UpdateDateTimeText));

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(DateTimePicker), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        internal static readonly DependencyProperty DateTimeTextProperty =
            DependencyProperty.Register(nameof(DateTimeText), typeof(string), typeof(DateTimePicker), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsDateOnly
        {
            get => (bool)GetValue(IsDateOnlyProperty);
            set => SetValue(IsDateOnlyProperty, value);
        }

        public string SelectedDateTimeFormat
        {
            get => (string)GetValue(SelectedDateTimeFormatProperty);
            set => SetValue(SelectedDateTimeFormatProperty, value);
        }

        /// <summary>
        /// 日期时间文本框是否只读。
        /// </summary>
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public DateTime? DisplayDateStart
        {
            get => (DateTime?)GetValue(DisplayDateStartProperty);
            set => SetValue(DisplayDateStartProperty, value);
        }

        public DateTime? DisplayDateEnd
        {
            get => (DateTime?)GetValue(DisplayDateEndProperty);
            set => SetValue(DisplayDateEndProperty, value);
        }

        public DayOfWeek FirstDayOfWeek
        {
            get => (DayOfWeek)GetValue(FirstDayOfWeekProperty);
            set => SetValue(FirstDayOfWeekProperty, value);
        }

        /// <summary>
        /// 获取或设置当前设置的日期和时间。
        /// </summary>
        public DateTime? SelectedDateTime
        {
            get => (DateTime?)GetValue(SelectedDateTimeProperty);
            set => SetValue(SelectedDateTimeProperty, value);
        }

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        internal string DateTimeText
        {
            get => (string)GetValue(DateTimeTextProperty);
            set => SetValue(DateTimeTextProperty, value);
        }

        public ICommand CalendarClickCommand { get; }

        public override void OnApplyTemplate()
        {
            if (PART_TextBox != null)
            {
                PART_TextBox.LostFocus -= PART_TextBox_LostFocus;
                PART_TextBox.TextChanged -= PART_TextBox_TextChanged;
                PART_TextBox.KeyDown -= PART_TextBox_KeyDown;
            }
            base.OnApplyTemplate();
            PART_TextBox = GetTemplateChild("PART_TextBox") as TextBox;
            PART_Popup = GetTemplateChild("PART_Popup") as Popup;
            PART_TextBox.LostFocus += PART_TextBox_LostFocus;
            PART_TextBox.TextChanged += PART_TextBox_TextChanged;
            PART_TextBox.KeyDown += PART_TextBox_KeyDown;
            TDateTimeView dateTimeView = new TDateTimeView(this);// TDateTimeView  构造函数传入日期时间
            dateTimeView.DateTimeOK += (datetime) => //TDateTimeView 日期时间确定事件
            {
                SelectedDateTime = datetime;
                IsDropDownOpen = false;//TDateTimeView 所在pop 关闭
                PART_TextBox?.Focus();
                PART_TextBox?.SelectAll();
            };
            dateTimeView.Closed += () =>
            {
                IsDropDownOpen = false;//TDateTimeView 所在pop 关闭
                //PART_TextBox?.Focus();
            };
            ((Decorator)PART_Popup.Child).Child = dateTimeView;
        }

        private void PART_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = (TextBox)sender;
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    SelectedDateTime = null;
                }
                else
                {
                    UpdateSelectedDateTime(textBox);
                }
            }
        }

        private void PART_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, e);
        }

        private static void CoerceDisplayDate(DateTimePicker dateTimePicker)
        {
            if (dateTimePicker.DisplayDateStart > dateTimePicker.DisplayDateEnd)
            {
                dateTimePicker.DisplayDateEnd = dateTimePicker.DisplayDateStart;
            }
        }

        private static void UpdateDateTimeText(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is DateTimePicker dateTimePicker)
            {
                dateTimePicker.UpdateDateTimeText();
            }
        }

        private void UpdateSelectedDateTime(TextBox textBox)
        {
            if (DateTime.TryParse(textBox.Text.Trim(), out var result))
            {
                if (SelectedDateTime == result)
                {
                    UpdateDateTimeText();
                }
                SelectedDateTime = result;
            }
            else
            {
                //var format = textBoxDateTime.GetBindingExpression(TextBox.TextProperty)?.ParentBinding.StringFormat;
                UpdateDateTimeText();
            }
        }

        private void UpdateDateTimeText()
        {
            if (DisplayDateStart != null && SelectedDateTime != null && SelectedDateTime < DisplayDateStart)
            {
                SelectedDateTime = DisplayDateStart;
            }
            else if (DisplayDateEnd != null && SelectedDateTime != null && SelectedDateTime > DisplayDateEnd)
            {
                SelectedDateTime = DisplayDateEnd;
            }
            else
            {
                DateTimeText = SelectedDateTime?.ToString(SelectedDateTimeFormat);
            }
        }

        private void PART_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateSelectedDateTime((TextBox)sender);
        }

        private void DateTimePicker_GotFocus(object sender, RoutedEventArgs e)
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