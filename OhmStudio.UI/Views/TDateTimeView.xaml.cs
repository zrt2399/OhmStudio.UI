using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace OhmStudio.UI.Views
{
    [DesignTimeVisible(false)]//在工具箱中 隐藏该窗口
    /// <summary>
    /// TDateTime.xaml 的交互逻辑。
    /// </summary>
    public partial class TDateTimeView : UserControl
    {
        public TDateTimeView(DateTime? dateTime)
        {
            InitializeComponent();
            _initialDateTime = dateTime;
        }

        readonly DateTime? _initialDateTime;

        /// <summary>
        /// TDateTimeView 窗口登录事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_initialDateTime == null)
            {
                calDate.DisplayDate = DateTime.MinValue.Date;
            }
            else
            {
                calDate.DisplayDate = (DateTime)_initialDateTime;
            }
            calDate.SelectedDate = _initialDateTime;
            UpdateBtnContent(_initialDateTime);
        }

        void UpdateBtnContent(DateTime? dateTime)
        {
            if (dateTime == null)
            {   //00:00:00
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
        /// 确定按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var date = calDate.SelectedDate == null ? DateTime.MinValue.Date : calDate.SelectedDate;
            DateTime dateTime = Convert.ToDateTime(date).Date;
            string timeStr = btnhh.Content + ":" + btnmm.Content + ":" + btnss.Content;

            var time = Convert.ToDateTime(timeStr).TimeOfDay;

            if (time == TimeSpan.Zero)
            {
                DateTimeOK?.Invoke(dateTime);
            }
            else
            {
                DateTimeOK?.Invoke(dateTime.Add(Convert.ToDateTime(timeStr).TimeOfDay));
            }
        }

        /// <summary>
        /// 当前按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNow_Click(object sender, RoutedEventArgs e)
        {
            popChioce.IsOpen = false;//THourView 或 TMinSexView 所在pop 的关闭动作
            if (btnNow.Content.ToString() == "零点")
            {
                UpdateBtnContent(null);
                btnNow.Content = "当前";
            }
            else
            {
                UpdateBtnContent(DateTime.Now);
                btnNow.Content = "零点";
            }
        }

        /// <summary>
        /// 小时点击事件
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
        /// 分钟点击事件
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
            minSexView.textBlockTitle.Text = "秒   钟";//修改 TMinSexView 的标题名称为秒钟
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
        /// 解除calendar点击后 影响其他按钮首次点击无效的问题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calDate_PreviewMouseUp(object sender, MouseButtonEventArgs e)
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
    }
}