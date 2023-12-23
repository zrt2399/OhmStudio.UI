using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OhmStudio.UI.Views
{
    //[ToolboxBitmap(typeof(DateTimePicker), "pack://application:,,,/OhmStudio.UI;component/Images/DateTimePicker.bmp")]
    /// <summary>
    /// DateTimePicker.xaml 的交互逻辑
    /// </summary>    
    [ToolboxBitmap(typeof(DatePicker))]
    public partial class DateTimePicker : UserControl
    {
        public DateTimePicker()
        {
            InitializeComponent();
            Loaded += DateTimePicker_Loaded;
            GotFocus += DateTimePicker_GotFocus;
        }

        private void DateTimePicker_GotFocus(object sender, RoutedEventArgs e)
        {
            var dateTimePicker = (DateTimePicker)sender;
            if (!e.Handled && dateTimePicker.textBoxDateTime != null)
            {
                if (Equals(e.OriginalSource, dateTimePicker))
                {
                    dateTimePicker.textBoxDateTime.Focus();
                    e.Handled = true;
                }
                else if (Equals(e.OriginalSource, dateTimePicker.textBoxDateTime))
                {
                    dateTimePicker.textBoxDateTime.SelectAll();
                    e.Handled = true;
                }
            }
        }

        private void DateTimePicker_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= DateTimePicker_Loaded;
            Dispatcher.InvokeAsync(() =>
            {
                if (textBoxDateTime.Template?.FindName("Border", textBoxDateTime) is UIElement uIElement)
                {
                    popChioce.PlacementTarget = uIElement;
                }
            }, DispatcherPriority.Render);
        }

        /// <summary>
        /// 日历图标点击事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconButton_Click(object sender, RoutedEventArgs e)
        {
            if (popChioce.IsOpen)
            {
                popChioce.IsOpen = false;
            }
            TDateTimeView dateTimeView = new TDateTimeView(SelectedDateTime);// TDateTimeView  构造函数传入日期时间
            dateTimeView.DateTimeOK += (datetime) => //TDateTimeView 日期时间确定事件
            {
                SelectedDateTime = datetime;
                popChioce.IsOpen = false;//TDateTimeView 所在pop 关闭
                textBoxDateTime.Focus();
            };
            dateTimeView.Closed += () =>
            {
                popChioce.IsOpen = false;//TDateTimeView 所在pop 关闭
                textBoxDateTime.Focus();
            };
            content.Child = dateTimeView;
            popChioce.IsOpen = true;
        }

        public string SelectedDateTimeFormat
        {
            get => (string)GetValue(SelectedDateTimeFormatProperty);
            set => SetValue(SelectedDateTimeFormatProperty, value);
        }

        public static readonly DependencyProperty SelectedDateTimeFormatProperty =
            DependencyProperty.Register(nameof(SelectedDateTimeFormat), typeof(string), typeof(DateTimePicker), new PropertyMetadata("yyyy/MM/dd HH:mm:ss", (sender, e) =>
            {
                if (sender is DateTimePicker dateTimePicker)
                {
                    var format = e.NewValue as string;
                    dateTimePicker.textBoxDateTime.Text = dateTimePicker.SelectedDateTime?.ToString(format ?? string.Empty);
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

        /// <summary>
        /// 日期和时间。
        /// </summary>
        public DateTime? SelectedDateTime
        {
            get => (DateTime?)GetValue(SelectedDateTimeProperty);
            set => SetValue(SelectedDateTimeProperty, value);
        }

        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register(nameof(SelectedDateTime), typeof(DateTime?), typeof(DateTimePicker), new PropertyMetadata(null, (sender, e) =>
            {
                if (sender is DateTimePicker dateTimePicker)
                {
                    var datetime = e.NewValue as DateTime?;
                    dateTimePicker.textBoxDateTime.Text = datetime?.ToString(dateTimePicker.SelectedDateTimeFormat);
                }
            }));

        private void textBoxDateTime_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DateTime.TryParse(textBoxDateTime.Text.Trim(), out var result))
            {
                SelectedDateTime = result;
            }
            else
            {
                //var format = textBoxDateTime.GetBindingExpression(TextBox.TextProperty)?.ParentBinding.StringFormat;
                textBoxDateTime.Text = SelectedDateTime?.ToString(SelectedDateTimeFormat);
            }
        }
    }
}