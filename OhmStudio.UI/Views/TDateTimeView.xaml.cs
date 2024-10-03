using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using OhmStudio.UI.Controls;

namespace OhmStudio.UI.Views
{
    /// <summary>
    /// TDateTime.xaml 的交互逻辑。
    /// </summary>
    [DesignTimeVisible(false)]//在工具箱中 隐藏该窗口。
    public partial class TDateTimeView : UserControl
    {
        public TDateTimeView(DateTimePicker dateTimePicker)
        {
            InitializeComponent();
            Loaded += (sender, e) =>
            {
                if (dateTimePicker.SelectedDateTime == null)
                {
                    calendar.DisplayDate = dateTimePicker.DisplayDateStart == null ? DateTime.MinValue.Date : (DateTime)dateTimePicker.DisplayDateStart;
                }
                else
                {
                    calendar.DisplayDate = (DateTime)dateTimePicker.SelectedDateTime;
                }
                calendar.SelectedDate = dateTimePicker.SelectedDateTime;
                calendar.FirstDayOfWeek = dateTimePicker.FirstDayOfWeek;
                calendar.DisplayDateStart = dateTimePicker.DisplayDateStart;
                calendar.DisplayDateEnd = dateTimePicker.DisplayDateEnd;
                if (dateTimePicker.IsDateOnly)
                {
                    UpdateBtnContent(null);
                    //txtTime.IsEnabled = textBoxDateTime.IsEnabled = btnNow.IsEnabled = false;
                    txtTime.Visibility = textBoxDateTime.Visibility = btnNow.Visibility = Visibility.Hidden;
                }
                else
                {
                    UpdateBtnContent(dateTimePicker.SelectedDateTime);
                }
            };
        }

        void UpdateBtnContent(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                //00:00:00
                btnhh.Content = "00";
                btnmm.Content = "00";
                btnss.Content = "00";
            }
            else
            {
                DateTime time = (DateTime)dateTime;
                btnhh.Content = time.Hour.ToString().PadLeft(2, '0');
                btnmm.Content = time.Minute.ToString().PadLeft(2, '0');
                btnss.Content = time.Second.ToString().PadLeft(2, '0');
            }
        }

        /// <summary>
        /// 确定按钮事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var date = calendar.SelectedDate == null ? (calendar.DisplayDateStart == null ? DateTime.MinValue.Date : calendar.DisplayDateStart) : calendar.SelectedDate;
            DateTime dateTime = Convert.ToDateTime(date).Date;
            string timeStr = btnhh.Content + ":" + btnmm.Content + ":" + btnss.Content;

            var time = Convert.ToDateTime(timeStr).TimeOfDay;

            if (time == TimeSpan.Zero)
            {
                DateTimeOK?.Invoke(dateTime);
            }
            else
            {
                DateTimeOK?.Invoke(dateTime.Add(time));
            }
        }

        /// <summary>
        /// 当前按钮事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNow_Click(object sender, RoutedEventArgs e)
        {
            popChioce.IsOpen = false;//THourView 或 TMinSexView 所在pop 的关闭动作
            if (btnNow.Content.ToString() == Properties.Resources.DateTimePicker_Midnight)
            {
                UpdateBtnContent(null);
                btnNow.Content = Properties.Resources.DateTimePicker_Current;
            }
            else
            {
                UpdateBtnContent(DateTime.Now);
                btnNow.Content = Properties.Resources.DateTimePicker_Midnight;
            }
        }

        /// <summary>
        /// 小时点击事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnhh_Click(object sender, RoutedEventArgs e)
        {
            if (popChioce.IsOpen)
            {
                popChioce.IsOpen = false;
            }
            THourView hourView = new THourView();// THourView 构造函数传递小时数据
            hourView.HourClick += (hourstr) => //THourView 点击所选小时后的 传递动作
            {
                btnhh.Content = hourstr;
                popChioce.IsOpen = false;//THourView 所在pop 的关闭动作
            };
            hourView.Closed += () => popChioce.IsOpen = false;
            content.Child = hourView;
            popChioce.IsOpen = true;
        }

        /// <summary>
        /// 分钟点击事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnmm_Click(object sender, RoutedEventArgs e)
        {
            if (popChioce.IsOpen)
            {
                popChioce.IsOpen = false;
            }
            TMinSexView minSexView = new TMinSexView();//TMinSexView 构造函数传递 分钟数据
            minSexView.tbTitleMinute.Visibility = Visibility.Visible;
            minSexView.MinClick += (minStr) => //TMinSexView 中 点击选择的分钟数据的 传递动作
            {
                btnmm.Content = minStr;
                popChioce.IsOpen = false;//TMinSexView 所在的 pop 关闭动作
            };
            minSexView.Closed += () => popChioce.IsOpen = false;
            content.Child = minSexView;
            popChioce.IsOpen = true;
        }

        /// <summary>
        /// 秒钟点击事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnss_Click(object sender, RoutedEventArgs e)
        {
            if (popChioce.IsOpen)
            {
                popChioce.IsOpen = false;
            }
            //秒钟 跟分钟 都是60，所有秒钟共用 分钟的窗口即可
            TMinSexView minSexView = new TMinSexView();//TMinSexView 构造函数 传入秒钟数据
            minSexView.tbTitleSecond.Visibility = Visibility.Visible;
            minSexView.MinClick += (sexStr) => //TMinSexView 中 所选择确定的 秒钟数据 的传递动作
            {
                btnss.Content = sexStr;
                popChioce.IsOpen = false;//TMinSexView 所在的 pop 关闭动作
            };
            minSexView.Closed += () => popChioce.IsOpen = false;
            content.Child = minSexView;
            popChioce.IsOpen = true;
        }

        /// <summary>
        /// 时间确定后的传递事件。
        /// </summary>
        public event Action<DateTime> DateTimeOK;
        public event Action Closed;

        /// <summary>
        /// 解除calendar点击后 影响其他按钮首次点击无效的问题。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.Captured is CalendarItem)
            {
                Mouse.Capture(null);
            }
        }

        private void IconButton_Click(object sender, RoutedEventArgs e)
        {
            Closed?.Invoke();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (calendar.DisplayDateStart != null && calendar.DisplayDateStart > DateTime.Now)
            {
                calendar.SelectedDate = calendar.DisplayDateStart;
                calendar.DisplayDate = (DateTime)calendar.DisplayDateStart;
            }
            else if (calendar.DisplayDateEnd != null && calendar.DisplayDateEnd < DateTime.Now)
            {
                calendar.SelectedDate = calendar.DisplayDateEnd;
                calendar.DisplayDate = (DateTime)calendar.DisplayDateEnd;
            }
            else
            {
                calendar.SelectedDate = DateTime.Now.Date;
                calendar.DisplayDate = DateTime.Now.Date;
            }
        }
    }
}