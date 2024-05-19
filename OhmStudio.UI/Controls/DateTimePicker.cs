using System;
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
        public DateTimePicker()
        {
            GotFocus += DateTimePicker_GotFocus;
            CalendarClickCommand = new RelayCommand(() =>
            {
                if (PART_Popup == null)
                {
                    return;
                }
                if (PART_Popup.IsOpen)
                {
                    PART_Popup.IsOpen = false;
                }
                TDateTimeView dateTimeView = new TDateTimeView(this);// TDateTimeView  构造函数传入日期时间
                dateTimeView.DateTimeOK += (datetime) => //TDateTimeView 日期时间确定事件
                {
                    SelectedDateTime = datetime;
                    PART_Popup.IsOpen = false;//TDateTimeView 所在pop 关闭
                    PART_TextBox?.Focus();
                };
                dateTimeView.Closed += () =>
                {
                    PART_Popup.IsOpen = false;//TDateTimeView 所在pop 关闭
                    PART_TextBox?.Focus();
                };
                (PART_Popup.Child as SystemDropShadowChrome).Child = dateTimeView;
                PART_Popup.IsOpen = true;
            });
        }

        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
        }

        TextBox PART_TextBox;
        Popup PART_Popup;
        public event DependencyPropertyChangedEventHandler TextChanged;

        public bool IsDateOnly
        {
            get => (bool)GetValue(IsDateOnlyProperty);
            set => SetValue(IsDateOnlyProperty, value);
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

        public string DateTimeFormat
        {
            get => (string)GetValue(DateTimeFormatProperty);
            set => SetValue(DateTimeFormatProperty, value);
        }

        public static readonly DependencyProperty DateTimeFormatProperty =
            DependencyProperty.Register(nameof(DateTimeFormat), typeof(string), typeof(DateTimePicker), new PropertyMetadata("yyyy/MM/dd HH:mm:ss", (sender, e) =>
            {
                if (sender is DateTimePicker dateTimePicker)
                {
                    var format = e.NewValue as string;
                    dateTimePicker.Text = dateTimePicker.SelectedDateTime?.ToString(format);
                }
            }));

        /// <summary>
        /// 日期时间文本框是否只读。
        /// </summary>
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(DateTimePicker));

        public DateTime? DisplayDateStart
        {
            get => (DateTime?)GetValue(DisplayDateStartProperty);
            set => SetValue(DisplayDateStartProperty, value);
        }

        public static readonly DependencyProperty DisplayDateStartProperty =
            DependencyProperty.Register(nameof(DisplayDateStart), typeof(DateTime?), typeof(DateTimePicker), new PropertyMetadata((sender, e) =>
            {
                if (sender is DateTimePicker dateTimePicker)
                {
                    var dateTime = e.NewValue as DateTime?;
                    if (dateTime != null)
                    {
                        if (dateTime > dateTimePicker.SelectedDateTime)
                        {
                            dateTimePicker.SelectedDateTime = null;
                        }
                        CheckDisplayDateStart(dateTimePicker);
                    }
                }
            }));

        public DateTime? DisplayDateEnd
        {
            get => (DateTime?)GetValue(DisplayDateEndProperty);
            set => SetValue(DisplayDateEndProperty, value);
        }

        public static readonly DependencyProperty DisplayDateEndProperty =
            DependencyProperty.Register(nameof(DisplayDateEnd), typeof(DateTime?), typeof(DateTimePicker), new PropertyMetadata((sender, e) =>
            {
                if (sender is DateTimePicker dateTimePicker)
                {
                    var dateTime = e.NewValue as DateTime?;
                    if (dateTime != null)
                    {
                        if (dateTime < dateTimePicker.SelectedDateTime)
                        {
                            dateTimePicker.SelectedDateTime = null;
                        }
                        CheckDisplayDateStart(dateTimePicker);
                    }
                }
            }));

        public DayOfWeek FirstDayOfWeek
        {
            get => (DayOfWeek)GetValue(FirstDayOfWeekProperty);
            set => SetValue(FirstDayOfWeekProperty, value);
        }

        public static readonly DependencyProperty FirstDayOfWeekProperty =
            DependencyProperty.Register(nameof(FirstDayOfWeek), typeof(DayOfWeek), typeof(DateTimePicker), new PropertyMetadata(DayOfWeek.Monday));

        /// <summary>
        /// 获取或设置当前设置的日期和时间。
        /// </summary>
        public DateTime? SelectedDateTime
        {
            get => (DateTime?)GetValue(SelectedDateTimeProperty);
            set => SetValue(SelectedDateTimeProperty, value);
        }

        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register(nameof(SelectedDateTime), typeof(DateTime?), typeof(DateTimePicker), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) =>
            {
                if (sender is DateTimePicker dateTimePicker)
                {
                    var datetime = e.NewValue as DateTime?;
                    if (dateTimePicker.DisplayDateStart != null && datetime != null && datetime < dateTimePicker.DisplayDateStart)
                    {
                        dateTimePicker.SelectedDateTime = dateTimePicker.DisplayDateStart;
                    }
                    else if (dateTimePicker.DisplayDateEnd != null && datetime != null && datetime > dateTimePicker.DisplayDateEnd)
                    {
                        dateTimePicker.SelectedDateTime = dateTimePicker.DisplayDateEnd;
                    }
                    else
                    {
                        dateTimePicker.Text = datetime?.ToString(dateTimePicker.DateTimeFormat);
                    }
                }
            }));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(DateTimePicker), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) =>
            {
                if (sender is DateTimePicker dateTimePicker)
                {
                    dateTimePicker.TextChanged?.Invoke(sender, e);
                }
            }));

        public ICommand CalendarClickCommand { get; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (PART_TextBox != null)
            {
                PART_TextBox.LostFocus -= PART_TextBox_LostFocus;
            }
            PART_TextBox = GetTemplateChild("PART_TextBox") as TextBox;
            PART_Popup = GetTemplateChild("PART_Popup") as Popup;
            PART_TextBox.LostFocus += PART_TextBox_LostFocus;
        }

        static void CheckDisplayDateStart(DateTimePicker dateTimePicker)
        {
            if (dateTimePicker.DisplayDateStart > dateTimePicker.DisplayDateEnd)
            {
                dateTimePicker.DisplayDateEnd = dateTimePicker.DisplayDateStart;
            }
        }

        private void PART_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DateTime.TryParse(PART_TextBox.Text.Trim(), out var result))
            {
                SelectedDateTime = result;
            }
            else
            {
                //var format = textBoxDateTime.GetBindingExpression(TextBox.TextProperty)?.ParentBinding.StringFormat;
                PART_TextBox.Text = SelectedDateTime?.ToString(DateTimeFormat);
            }
        }

        private void DateTimePicker_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!e.Handled && PART_TextBox != null)
            {
                if (Equals(e.OriginalSource, this))
                {
                    PART_TextBox.Focus();
                    e.Handled = true;
                }
                else if (Equals(e.OriginalSource, PART_TextBox))
                {
                    PART_TextBox.SelectAll();
                    e.Handled = true;
                }
            }
        }
    }
}