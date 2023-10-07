﻿using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace OhmStudio.UI.Views
{
    //[ToolboxBitmap(typeof(DateTimePicker), "pack://application:,,,/OhmStudio.UI;component/Images/DateTimePicker.bmp")]
    /// <summary>
    /// DateTimePicker.xaml 的交互逻辑。
    /// </summary>    
    [ToolboxBitmap(typeof(DatePicker))]
    public partial class DateTimePicker : UserControl
    {
        public DateTimePicker()
        {
            InitializeComponent();
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
            TDateTimeView dtView = new TDateTimeView(textBoxDateTime.Text);// TDateTimeView  构造函数传入日期时间
            dtView.DateTimeOK += (dateTimeStr) => //TDateTimeView 日期时间确定事件
            {
                textBoxDateTime.Text = dateTimeStr;
                DateTime.TryParse(dateTimeStr, out var time);
                DateTime = time;

                popChioce.IsOpen = false;//TDateTimeView 所在pop 关闭
            };
            popChioce.Child = dtView;
            popChioce.IsOpen = true;
        }

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
        /// 日期时间。
        /// </summary>
        public DateTime DateTime
        {
            get => (DateTime)GetValue(DateTimeProperty);
            set => SetValue(DateTimeProperty, value);
        }

        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register(nameof(DateTime), typeof(DateTime), typeof(DateTimePicker), new PropertyMetadata(DateTime.Now));

        private void textBoxDateTime_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!DateTime.TryParse(textBoxDateTime.Text.Trim(), out _))
            {
                var format = textBoxDateTime.GetBindingExpression(TextBox.TextProperty)?.ParentBinding.StringFormat;
                textBoxDateTime.Text = DateTime.ToString(format);
            }
        }
    }
}