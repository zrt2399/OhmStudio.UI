﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.Win32;
using OhmStudio.UI.Attaches;
using OhmStudio.UI.Commands;
using OhmStudio.UI.Controls;
using OhmStudio.UI.Converters;
using OhmStudio.UI.Messaging;
using OhmStudio.UI.PublicMethods;
using PropertyChanged;

namespace OhmStudio.UI.Demo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ChromeWindow, INotifyPropertyChanged
    {
        XmlFoldingStrategy xmlFoldingStrategy = new XmlFoldingStrategy();

        public MainWindow()
        {
            //if (new UserLoginWindow().SetOwner().ShowDialog() != true)
            //{
            //    Environment.Exit(Environment.ExitCode);
            //}
            InitializeComponent();
            DataContext = this;
            UptateFontList();
            textEditorcs.Text = "using System;\r\n\r\nclass Program\r\n{\r\n    static void Main()\r\n    {\r\n        Console.WriteLine(\"Hello World\");\r\n    }\r\n}";
            textEditorcpp.Text = "#include <iostream>\r\n\r\nint main() {\r\n    std::cout << \"Hello World\" << std::endl;\r\n    return 0;\r\n}";
            textEditorxml.Text = "<Project Sdk=\"Microsoft.NET.Sdk\">\r\n\r\n\t<PropertyGroup>\r\n\t\t<OutputType>WinExe</OutputType>\r\n\t\t<TargetFramework>net6.0-windows</TargetFramework>\r\n\t\t<UseWPF>true</UseWPF>\r\n\t</PropertyGroup>\r\n \r\n</Project>";
            var text = textEditorxml;
            var searchPanel = SearchPanel.Install(text);
            searchPanel.MarkerBrush = "#BEAA46".ToSolidColorBrush();

            var foldingManager = FoldingManager.Install(text.TextArea);
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += delegate
            {
                xmlFoldingStrategy.UpdateFoldings(foldingManager, text.Document);
            };
            dispatcherTimer.Start();

            PreviewMouseWheel += (sender, e) =>
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    if (e.Delta > 0)
                    {
                        ZoomOut();
                    }
                    else
                    {
                        ZoomIn();
                    }
                }
            };

            KeyDown += async (sender, e) =>
            {
                if (e.Key == Key.Tab)
                {
                    await Task.Delay(200);
                    StatusBarContent = "CurrentFocusedElement: " + Keyboard.FocusedElement;
                }
            };

            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
            SearchCommand = new RelayCommand(() => UIMessageTip.Show("什么也没搜索到..."));

            Result.Columns.Add("Time");
            Result.Columns.Add("V0");
            Result.Columns.Add("V1");
            Result.Columns.Add("RX");
            for (int i = 0; i < 1000; i++)
            {
                Result.Rows.Add(DateTime.Now, i, i + 1, "44");
            }
            DengGao.Add("风急天高猿啸哀");
            DengGao.Add("渚清沙白鸟飞回");
            DengGao.Add("无边落木萧萧下");
            DengGao.Add("不尽长江衮衮来");
            DengGao.Add("万里悲秋常作客");
            DengGao.Add("百年多病独登台");
            DengGao.Add("艰难苦恨繁霜鬓");
            DengGao.Add("潦倒新停浊酒杯");

            JingYeSi.Add("床前明月光");
            JingYeSi.Add("疑是地上霜");
            JingYeSi.Add("举头望明月");
            JingYeSi.Add("低头思故乡");

            HuangHeLou.Add("昔人已乘黄鹤去");
            HuangHeLou.Add("此地空余黄鹤楼");
            HuangHeLou.Add("黄鹤一去不复返");
            HuangHeLou.Add("白云千载空悠悠");
            HuangHeLou.Add("晴川历历汉阳树");
            HuangHeLou.Add("芳草萋萋鹦鹉洲");
            HuangHeLou.Add("日暮乡关何处是");
            HuangHeLou.Add("烟波江上使人愁");

            DengGuanQueLou.Add("白日依山尽");
            DengGuanQueLou.Add("黄河入海流");
            DengGuanQueLou.Add("欲穷千里目");
            DengGuanQueLou.Add("更上一层楼");

            Pro.Description = "1";
            Pro.Brush = Brushes.Red;
            SelectedObject = Pro;

            Stopwatch stopwatch = Stopwatch.StartNew();
            List<Pro> pros = new List<Pro>();
            for (int i = 0; i < 2000; i++)
            {
                Pro pro = new Pro();
                pro.Name = "Name:" + (i + 1);
                pro.Value = i;
                pro.Description = $"第{i / 10 + 1}组的第{i % 10 + 1}个";
                pros.Add(pro);
            }
            ProNodes = new ObservableCollection<Pro>(pros);
            stopwatch.Stop();
            var viewSource = new CollectionViewSource { Source = ProNodes };
            ProNodes.CollectionChanged += delegate
            {
                AlertDialog.Show("已改变");
            };
            UserInfos.Add(new UserInfoModel());
            UserInfos.Add(new UserInfoModel() { UserName = "jack" });
            UserInfos.Add(new UserInfoModel() { UserName = "rose", Password = "123456" });
            Messenger.Default.Register<string>(this, Rrecipient, msg => AlertDialog.Show(msg));
            SystemEvents.InstalledFontsChanged += SystemEvents_InstalledFontsChanged;
            StatusManager.IsRunningChanged += StatusManager_IsRunningChanged;
            XamlThemeDictionary.ThemeChanged += XamlThemeDictionary_ThemeChanged;
        }

        bool _can;
        const string Rrecipient = "Ohm";
        const string DefaultFont = "默认";
        public const string GlobalFontSize = nameof(GlobalFontSize);
        public const string GlobalFontFamily = nameof(GlobalFontFamily);
        readonly FontFamily _defaultFontFamily = (FontFamily)Application.Current.Resources[GlobalFontFamily];

        public DateTime? CurrentDateTime { get; set; }

        public double WindowScale { get; set; } = 1;

        public ObservableCollection<Pro> ProNodes { get; set; }

        public IList SelectedItemsFileNodes { get; set; }

        public IEnumerable<string> FileNodesSelectedItems => SelectedItemsFileNodes?.OfType<string>();

        public ObservableCollection<string> FontFamilyList { get; set; }

        public IEnumerable<double> FontSizeList { get; } = Enumerable.Range(10, 11).Select(x => (double)x);

        public Pro Pro { get; set; } = new Pro();

        public object SelectedObject { get; set; }

        public bool DocumentWrapPanelIsWrap { get; set; } = true;

        public string StatusBarContent { get; set; } = "Ready";

        public DataTable Result { get; set; } = new DataTable();

        public ObservableCollection<UserInfoModel> UserInfos { get; set; } = new ObservableCollection<UserInfoModel>();

        public IList UserInfoSelectedItems { get; set; }

        public double ProgressBarValue { get; set; }

        public bool IsAntiAliasing { get; set; } = true;

        public List<string> DengGao { get; set; } = new List<string>();

        public List<string> JingYeSi { get; set; } = new List<string>();

        public List<string> HuangHeLou { get; set; } = new List<string>();

        public List<string> DengGuanQueLou { get; set; } = new List<string>();

        public CornerRadius CornerRadius { get; set; }

        private int _globalCornerRadius;
        [DoNotNotify]
        public int GlobalCornerRadius
        {
            get => _globalCornerRadius;
            set
            {
                _globalCornerRadius = value;
                CornerRadius = new CornerRadius(value);
                OnPropertyChanged(nameof(GlobalCornerRadius));
            }
        }

        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand SearchCommand { get; }

        private RelayCommand _startCommand;
        public RelayCommand StartCommand
        {
            get => _startCommand ??= new RelayCommand(() =>
            {
                if (UserInfoSelectedItems != null)
                {
                    foreach (var item in UserInfoSelectedItems.OfType<UserInfoModel>())
                    {
                        AlertDialog.Show(item.UserName);
                    }
                }
            }, () =>
            {
                //Debug.WriteLine(Can);
                return _can;
            });
            set => _startCommand = value;
        }

        [DoNotNotify]
        public ThemeType CurrentTheme
        {
            get => XamlThemeDictionary.Instance.Theme;
            set
            {
                XamlThemeDictionary.Instance.Theme = value;
                StatusManager.Update();
                OnPropertyChanged(() => CurrentTheme);
            }
        }

        private string _currentFontFamily = DefaultFont;
        [DoNotNotify]
        public string CurrentFontFamily
        {
            get => _currentFontFamily;
            set
            {
                value ??= DefaultFont;
                _currentFontFamily = value;
                Application.Current.Resources[GlobalFontFamily] = value == DefaultFont ? _defaultFontFamily : new FontFamily(value);
                OnPropertyChanged(nameof(CurrentFontFamily));
            }
        }

        [DoNotNotify]
        public double CurrentFontSize
        {
            get => (double)(Application.Current?.Resources[GlobalFontSize]);
            set
            {
                if (FontSizeList.Contains(value))
                {
                    Application.Current.Resources[GlobalFontSize] = value;
                    OnPropertyChanged(nameof(CurrentFontSize));
                }
            }
        }

        private void XamlThemeDictionary_ThemeChanged(object sender, EventArgs e)
        {
            StatusBarContent = "软件主题已改变为" + (ThemeType)sender;
        }

        private void StatusManager_IsRunningChanged(object sender, EventArgs e)
        {
            StatusBarContent = "IsRunning:" + sender;
        }

        private void SystemEvents_InstalledFontsChanged(object sender, EventArgs e)
        {
            UptateFontList();
            StatusBarContent = "系统字体已改变";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged<T>(ref T property, T newValue, string propertyName)
        {
            property = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                string propertyName = GetPropertyName(propertyExpression);
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            if (propertyExpression.Body is not MemberExpression memberExpression)
            {
                throw new ArgumentException("Invalid argument", nameof(propertyExpression.Body));
            }

            if (memberExpression.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException("Argument is not a property", nameof(memberExpression.Member));
            }

            return propertyInfo.Name;
        }

        void UptateFontList()
        {
            FontFamilyList = new ObservableCollection<string>(new InstalledFontCollection().Families.Select(x => x.Name));
            FontFamilyList.Insert(0, DefaultFont);
        }

        void ZoomIn()
        {
            if (WindowScale <= 0.5)
            {
                return;
            }
            WindowScale -= 0.25;
            WindowScale = Math.Round(WindowScale, 2);
        }

        void ZoomOut()
        {
            if (WindowScale >= 3)
            {
                return;
            }
            WindowScale += 0.25;
            WindowScale = Math.Round(WindowScale, 2);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(500);
            var assembly = Assembly.GetAssembly(typeof(ChromeWindow)).GetName().Version.ToString();
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                stringBuilder.Append(i + "StringBuilder" + Environment.NewLine);
            }

            //AlertDialog.OhmUILanguage = OhmUILanguage.Zh_TW;
            var result = AlertDialog.Show(stringBuilder.ToString(), assembly, MessageBoxButton.YesNoCancel, MessageBoxImage.Error);
            MessageBox.Show("点击了" + result);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Task.Run(() =>
            //{
            //    AlertDialog.ShowError("AlertDialog.ShowError");
            //});
            Messenger.Default.Send("AlertDialog.Show", Rrecipient);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            UIMessageTip.Show("GC完成");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Pro.Value = 100;
            AlertDialog.Show(Pro.InstrumentType.ToString());
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (FileNodesSelectedItems != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in FileNodesSelectedItems)
                {
                    stringBuilder.Append(item.ToString() + Environment.NewLine);
                }
                AlertDialog.Show(stringBuilder.ToString());
            }
            StatusManager.IsRunning = !StatusManager.IsRunning;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in ProNodes)
            {
                item.IsExpanded = true;
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (var item in ProNodes)
            {
                item.IsExpanded = false;
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            UserLoginWindow userLoginWindow = new UserLoginWindow();
            userLoginWindow.SetOwner().ShowDialog();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            _can = !_can;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.Description = "请选择文件夹";
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadTreeView(folderBrowserDialog.SelectedPath);
            }
        }

        private void LoadSubDirectory(TreeViewItem node, string fullPath)
        {
            //try
            //{
            DirectoryInfo directoryInfo = new DirectoryInfo(fullPath);
            if (directoryInfo.Exists)
            {
                //加载子文件夹
                foreach (DirectoryInfo subDirInfo in directoryInfo.GetDirectories().OrderBy(x => x.Name))
                {
                    TreeViewItem subNode = new TreeViewItem();
                    subNode.Header = subDirInfo.Name;
                    LoadSubDirectory(subNode, subDirInfo.FullName);
                    node.Items.Add(subNode);
                }
                //加载文件
                foreach (FileInfo fileInfo in directoryInfo.GetFiles().OrderBy(x => x.Name))
                {
                    TreeViewItem subNode = new TreeViewItem();
                    subNode.Header = fileInfo.Name;
                    node.Items.Add(subNode);
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    AlertDialog.ShowError(ex.Message, "Error");
            //}
        }

        private void LoadTreeView(string rootFolderPath)
        {
            // 创建根节点
            TreeViewItem rootNode = new TreeViewItem();
            rootNode.Header = new DirectoryInfo(rootFolderPath).Name;

            // 加载根文件夹
            LoadSubDirectory(rootNode, rootFolderPath);

            // 将根节点添加到 TreeView
            treeView.Items.Add(rootNode);

        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            DocumentWrapPanelIsWrap = !DocumentWrapPanelIsWrap;
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            UIMessageTip.Show("UIMessageTip.Show");
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            UIMessageTip.ShowOk("UIMessageTip.ShowOk");
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            UIMessageTip.ShowWarning("UIMessageTip.ShowWarning");
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            UIMessageTip.ShowError("UIMessageTip.ShowError");
        }

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 50; i++)
            {
                stringBuilder.Append("UIMessageTip.ShowError;");
            }
            UIMessageTip.ShowError(stringBuilder.ToString());
        }

        private void Button_Click_14(object sender, RoutedEventArgs e)
        {
            //throw new Exception("ex");
            passwordTextBox.Focus();
            //AlertDialog.Show(checkComboBox.SelectedText);
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            SelectedObject = this;
        }

        private void Button_Click_15(object sender, RoutedEventArgs e)
        {
            rollBox.ItemsSource = new List<UIElement> { new Image() { Source = new BitmapImage(new Uri("https://pic1.zhimg.com/v2-ecac0aedda57bffecbbe90764828a825_r.jpg?source=1940ef5c")) }, new Button() { Content = "This is a new Button" } };
        }
    }

    [BaseObjectIgnore]
    public class Pro : ProBase
    {
        public string Name { get; set; }

        public bool IsExpanded { get; set; } = true;

        public ImageSource ImageSource { get; set; } = new BitmapImage(new Uri("/Images/1.jpg", UriKind.Relative));

        public int? IntString { get; set; } = null;

        [TextBoxPlaceHolder(PlaceHolder = "请输入密码..."), Password]
        public string Password { get; set; } = "123456";

        public Abs? Abs { get; set; } = null;

        public InstrumentType InstrumentType { get; set; }

        [PropertyGrid(DisplayName = "描述")]
        public string Description { get; set; }

        [PropertyGrid(DisplayName = "值")]
        public double? Value { get; set; }

        public SolidColorBrush Brush { get; set; }

        public Globals Globals { get; set; } = new Globals();
    }

    public class ProBase : ViewModelBase
    {
        public string Base { get; set; } = "Base";
    }

    public struct Abs
    {
        public Abs()
        {
        }

        [PropertyGrid(DisplayName = "Id号")]
        public string Id { get; set; } = "123";

        public int Num { get; set; } = 999;
    }

    [BaseObjectIgnore]
    public class Globals : ProBase
    {
        public string Name { get; set; }

        public double Value { get; set; }

        [ToolTip(ToolTip = "这个表示时间")]
        [PropertyGrid(DisplayName = "时间", IsReadOnly = false)]
        public DateTime DateTime { get; set; } = DateTime.Now;

        public List<string> DateTimes { get; set; } = new List<string>() { "123", "456", "789", "abc", "↑↓←→" };
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum InstrumentType
    {
        [Description("I/O控制单元")]
        ControlBoard,
        [Description("测试通道控制")]
        MatrixBoard,
        [Description("电压测量模块")]
        M3054,
        [Description("1510高压模块")]
        M9310,
        [Description("恒流恒压源")]
        P8605,
        [Description("LCR测量模块1")]
        M2810,
        [Description("LCR测量模块2")]
        M2821,
        [Description("HVM高压模块")]
        HVM,
        [Description("K2400测试仪")]
        K2400
    }

    public class UserInfoModel
    {
        public string UserName { get; set; } = "1";

        public string Password { get; set; } = "1";

        public string Remark { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LoginAt { get; set; } = DateTime.Now;
    }
}