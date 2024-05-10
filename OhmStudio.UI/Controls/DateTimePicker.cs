﻿using System;
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
                if (PART_Popup.IsOpen)
                {
                    PART_Popup.IsOpen = false;
                }
                TDateTimeView dateTimeView = new TDateTimeView(SelectedDateTime);// TDateTimeView  构造函数传入日期时间
                dateTimeView.DateTimeOK += (datetime) => //TDateTimeView 日期时间确定事件
                {
                    SelectedDateTime = datetime;
                    PART_Popup.IsOpen = false;//TDateTimeView 所在pop 关闭
                    PART_TextBox.Focus();
                };
                dateTimeView.Closed += () =>
                {
                    PART_Popup.IsOpen = false;//TDateTimeView 所在pop 关闭
                    PART_TextBox.Focus();
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
        public event EventHandler<DependencyPropertyChangedEventArgs> TextChanged;

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
                    dateTimePicker.Text = dateTimePicker.SelectedDateTime?.ToString(format ?? string.Empty);
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
            DependencyProperty.Register(nameof(SelectedDateTime), typeof(DateTime?), typeof(DateTimePicker), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) =>
            {
                if (sender is DateTimePicker dateTimePicker)
                {
                    var datetime = e.NewValue as DateTime?;
                    dateTimePicker.Text = datetime?.ToString(dateTimePicker.SelectedDateTimeFormat);
                }
            }));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(DateTimePicker), new PropertyMetadata((sender, e) =>
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

        private void PART_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DateTime.TryParse(PART_TextBox.Text.Trim(), out var result))
            {
                SelectedDateTime = result;
            }
            else
            {
                //var format = textBoxDateTime.GetBindingExpression(TextBox.TextProperty)?.ParentBinding.StringFormat;
                PART_TextBox.Text = SelectedDateTime?.ToString(SelectedDateTimeFormat);
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