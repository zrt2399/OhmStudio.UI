using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace OhmStudio.UI.Views
{
    /// <summary>
    /// THourView.xaml 的交互逻辑。
    /// </summary>
    [DesignTimeVisible(false)]//在工具箱中 隐藏该窗口。
    public partial class THourView : UserControl
    {
        public THourView()
        {
            InitializeComponent();
            LoadHour();
        }

        /// <summary>
        /// 类：小时数据。
        /// </summary>
        public class Hour
        {
            /// <summary>
            /// 第1列 小时数据。
            /// </summary>
            public int H1 { get; set; }
            public int H2 { get; set; }
            public int H3 { get; set; }
            public int H4 { get; set; }
            public int H5 { get; set; }
            public int H6 { get; set; }

            /// <summary>
            /// 构造函数。
            /// </summary>
            /// <param name="hour1"></param>
            /// <param name="hour2"></param>
            /// <param name="hour3"></param>
            /// <param name="hour4"></param>
            /// <param name="hour5"></param>
            /// <param name="hour6"></param>
            public Hour(int hour1, int hour2, int hour3, int hour4, int hour5, int hour6)
            {
                H1 = hour1;
                H2 = hour2;
                H3 = hour3;
                H4 = hour4;
                H5 = hour5;
                H6 = hour6;
            }
        }

        /// <summary>
        /// dgHour控件 绑定类Hour 加载初始化数据。
        /// </summary>
        public void LoadHour()
        {
            Hour[] hour = new Hour[4];
 
            hour[0] = new Hour(0, 1, 2, 3, 4, 5);
            hour[1] = new Hour(6, 7, 8, 9, 10, 11);
            hour[2] = new Hour(12, 13, 14, 15, 16, 17);
            hour[3] = new Hour(18, 19, 20, 21, 22, 23);

            dgHour.ItemsSource = hour;
        }

        /// <summary>
        /// dgHour控件 单元格点击（选择）事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgHour_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGridCellInfo cell = dgHour.CurrentCell;
            if (cell.Column == null)
            {
                return;
            }

            Hour hour = cell.Item as Hour;

            // string str = cell.Column.DisplayIndex.ToString();

            string time = cell.Column.DisplayIndex switch// 通过所在列 获取类Hour的坐标 确定具体的hour数据
            {
                0 => hour.H1.ToString(),
                1 => hour.H2.ToString(),
                2 => hour.H3.ToString(),
                3 => hour.H4.ToString(),
                4 => hour.H5.ToString(),
                5 => hour.H6.ToString(),
                _ => string.Empty
            };
            time = time.PadLeft(2, '0');
            HourClick?.Invoke(time);
        }

        /// <summary>
        /// 小时数据点击（确定）后 的传递事件。
        /// </summary>
        public event Action<string> HourClick;
        public event Action Closed;

        private void IconButton_Click(object sender, RoutedEventArgs e)
        {
            Closed?.Invoke();
        }
    }
}