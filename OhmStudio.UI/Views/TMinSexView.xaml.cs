using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace OhmStudio.UI.Views
{
    /// <summary>
    /// TMinSexView.xaml 的交互逻辑。
    /// </summary>
    [DesignTimeVisible(false)]//在工具箱中 隐藏该窗口。
    public partial class TMinSexView : UserControl
    {
        public TMinSexView()
        {
            InitializeComponent();
            LoadMin();
        }

        /// <summary>
        /// 类：分钟数据。
        /// </summary>
        public class Min
        {
            public int C0 { get; set; }
            public int C1 { get; set; }
            public int C2 { get; set; }
            public int C3 { get; set; }
            public int C4 { get; set; }
            public int C5 { get; set; }
            public int C6 { get; set; }
            public int C7 { get; set; }
            public int C8 { get; set; }
            public int C9 { get; set; }

            public Min(int c0, int c1, int c2, int c3, int c4, int c5, int c6, int c7, int c8, int c9)
            {
                C0 = c0;
                C1 = c1;
                C2 = c2;
                C3 = c3;
                C4 = c4;
                C5 = c5;
                C6 = c6;
                C7 = c7;
                C8 = c8;
                C9 = c9;
            }
        }

        /// <summary>
        /// dgMinSex控件 绑定类Min 加载初始化数据。
        /// </summary>
        public void LoadMin()
        {
            Min[] min = new Min[6];
            min[0] = new Min(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
            min[1] = new Min(10, 11, 12, 13, 14, 15, 16, 17, 18, 19);
            min[2] = new Min(20, 21, 22, 23, 24, 25, 26, 27, 28, 29);
            min[3] = new Min(30, 31, 32, 33, 34, 35, 36, 37, 38, 39);
            min[4] = new Min(40, 41, 42, 43, 44, 45, 46, 47, 48, 49);
            min[5] = new Min(50, 51, 52, 53, 54, 55, 56, 57, 58, 59);

            //dgMinSex.Items.Clear();
            dgMinSex.ItemsSource = min;
        }

        /// <summary>
        /// dgMinSex控件 单元格点击（选择）事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgMinSex_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGridCellInfo cell = dgMinSex.CurrentCell;
            if (cell.Column == null)
            {
                return;
            }
            Min min = cell.Item as Min;
            //string str = cell.Column.DisplayIndex.ToString();

            var time = cell.Column.DisplayIndex switch// 通过所在列 获取类Min的坐标 确定具体的min数据
            {
                0 => min.C0.ToString(),
                1 => min.C1.ToString(),
                2 => min.C2.ToString(),
                3 => min.C3.ToString(),
                4 => min.C4.ToString(),
                5 => min.C5.ToString(),
                6 => min.C6.ToString(),
                7 => min.C7.ToString(),
                8 => min.C8.ToString(),
                9 => min.C9.ToString(),
                _ => string.Empty
            };
            time = time.PadLeft(2, '0');
            MinClick?.Invoke(time);
        }
 
        /// <summary>
        /// 分钟数据点击（确定）后 的传递事件。
        /// </summary>
        public event Action<string> MinClick;

        public event Action Closed;

        private void IconButton_Click(object sender, RoutedEventArgs e)
        {
            Closed?.Invoke();
        }
    }
}