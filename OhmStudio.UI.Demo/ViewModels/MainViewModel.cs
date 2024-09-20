using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Bogus;
using Microsoft.Win32;
using OhmStudio.UI.Attaches;
using OhmStudio.UI.Commands;
using OhmStudio.UI.Controls;
using OhmStudio.UI.Converters;
using OhmStudio.UI.Demo.Models;
using OhmStudio.UI.Demo.Views;
using OhmStudio.UI.Messaging;
using OhmStudio.UI.Mvvm;
using OhmStudio.UI.PublicMethods;
using PropertyChanged;

namespace OhmStudio.UI.Demo.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Messenger.Default.Register<string>(this, MessageType.AlertDialog, msg => AlertDialog.Show(msg));
            Messenger.Default.Register<string>(this, MessageType.TreeViewItemLoaded, (message) => StatusBarContent = message);
            var fontFamilies = new InstalledFontCollection().Families.Select(x => new FontFamilyItem() { Name = x.Name, FontFamily = new FontFamily(x.Name) });
            FontFamilyList = new ObservableCollection<FontFamilyItem>(fontFamilies);
            FontFamilyList.Insert(0, _defaultFontFamilyItem);

            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
            SearchCommand = new RelayCommand(() => UIMessageTip.Show("什么也没搜索到..."));
            TreeViewAddCommand = new RelayCommand(() =>
            {
                var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                folderBrowserDialog.Description = "请选择文件夹";
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    TreeViewModel.LoadRootDirectory(TreeViewModels, folderBrowserDialog.SelectedPath);
                    stopwatch.Stop();
                    StatusBarContent = $"TreeView根节点加载完成耗时：{stopwatch.Elapsed.TotalMilliseconds}ms";
                }
            });
            CollapseAllCommand = new RelayCommand(() =>
            {
                foreach (var item in TreeViewModels)
                {
                    ExpandAllTreeViewItem(item, false);
                }
            });

            DeleteWorkflowItemCommand = new RelayCommand(() =>
            {
                if (SelectedWorkflowItems != null)
                {
                    for (int i = SelectedWorkflowItems.Count - 1; i >= 0; i--)
                    {
                        WorkflowItemModels.Remove(SelectedWorkflowItems[i] as WorkflowItemModel);
                    }
                }
            });
            SaveAsImageCommand = new RelayCommand(() =>
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PNG 图片|*.png";
                saveFileDialog.Title = "另存为图片";
                if (saveFileDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(saveFileDialog.FileName))
                {
                    MainWindow.SaveAsImage(saveFileDialog.FileName, ImageType.Png);
                }
            });
            WorkflowEditorDropCommand = new RelayCommand<DragEventArgs>((e) =>
            {
                if (e.Data.GetData(typeof(StepType)) is StepType stepType)
                {
                    Point point = MainWindow.workflowEditor.GetDragPosition(e);

                    WorkflowItemModel workflowItemModel = new WorkflowItemModel();
                    workflowItemModel.Name = EnumDescriptionConverter.GetEnumDesc(stepType);
                    workflowItemModel.StepType = stepType;
                    workflowItemModel.Left = point.X;
                    workflowItemModel.Top = point.Y;
                    WorkflowItemModels.Add(workflowItemModel);
                }
            });
            WorkflowEditorTestCommand = new RelayCommand(() =>
            {
                Random random = new Random();
                for (int i = 0; i < 20; i++)
                {
                    WorkflowItemModel workflowItemModel = new WorkflowItemModel();
                    workflowItemModel.Name = "测试节点";
                    workflowItemModel.StepType = StepType.Nomal;
                    workflowItemModel.Left = random.Next(-2000, 2000);
                    workflowItemModel.Top = random.Next(-2000, 2000);
                    WorkflowItemModels.Add(workflowItemModel);
                    if (WorkflowItemModels.Count > 1)
                    {
                        var index = WorkflowItemModels.IndexOf(workflowItemModel);
                        var last = WorkflowItemModels[index - 1];
                        workflowItemModel.LastStep = last;
                        last.NextStep = workflowItemModel;
                    }
                }
            });

            Result.Columns.Add("Time");
            Result.Columns.Add("V0");
            Result.Columns.Add("V1");
            Result.Columns.Add("RX");
            for (int i = 0; i < 1000; i++)
            {
                Result.Rows.Add(DateTime.Now, i, i + 1, "4396");
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

            Employee.Description = "MyEmployee";
            Employee.Brush = Brushes.Red;
            SelectedObject = Employee;

            Stopwatch stopwatch = Stopwatch.StartNew();
            List<Employee> pros = new List<Employee>();
            for (int i = 0; i < 5000; i++)
            {
                Employee pro = new Employee();
                pro.Name = "Name:" + (i + 1);
                pro.Value = i;
                pro.Description = $"第{i / 10 + 1}组的第{i % 10 + 1}个";
                pros.Add(pro);
            }
            Employees = new ObservableCollection<Employee>(pros);
            stopwatch.Stop();
            var viewSource = new CollectionViewSource() { Source = Employees };
            Employees.CollectionChanged += delegate
            {
                AlertDialog.Show("已改变");
            };
            PackIcons = new ObservableCollection<PackIconKind>(PackIconDataFactory.Create().Keys);

            var userInfoModels = new Faker<UserInfoModel>("zh_CN")
                .RuleFor(u => u.UserName, f => f.Name.LastName() + f.Name.FirstName())
                .RuleFor(u => u.Password, f => f.Internet.Password())
                .RuleFor(u => u.LoginAt, f => f.Date.Recent())
                .RuleFor(u => u.CreatedAt, f => f.Date.Recent())
                .RuleFor(u => u.Remark, f => f.Lorem.Text())
                .Generate(1000);

            UserInfos = new ObservableCollection<UserInfoModel>(userInfoModels);

            WorkflowItemModels.Add(new WorkflowItemModel() { Name = "开始", StepType = StepType.Nomal, Left = 100 });
            WorkflowItemModels.Add(new WorkflowItemModel() { Name = "测试", StepType = StepType.Nomal, Left = 200, Top = 240 });
            WorkflowItemModels.Add(new WorkflowItemModel() { Name = "结束", StepType = StepType.Nomal, Left = 300, Top = 480 });

            WorkflowItemModels.Last().LastStep = WorkflowItemModels[1];
            WorkflowItemModels[1].NextStep = WorkflowItemModels.Last();

            WorkflowItems.Add(new WorkflowItem() { Content = new TextBox() { Margin = new Thickness(8), Text = "开始" }, StepType = StepType.Begin });
            var workflowItem = new WorkflowItem() { Content = new TextBlock() { Text = "中间节点" } };
            Canvas.SetLeft(workflowItem, 100);
            Canvas.SetTop(workflowItem, 100);
            WorkflowItems.Add(workflowItem);
            WorkflowItems.Add(new WorkflowItem() { Content = new TextBox() { Margin = new Thickness(8), Text = "结束" }, StepType = StepType.End });

            StatusManager.IsRunningChanged += StatusManager_IsRunningChanged;
            XamlThemeDictionary.ThemeChanged += XamlThemeDictionary_ThemeChanged;

            CurrentUserPermission = new List<UserPermission>() { UserPermission.Test, UserPermission.Edit, UserPermission.Settings };
        }

        public bool Can;
        private const string DefaultFontName = "默认";
        public const string GlobalFontSize = nameof(GlobalFontSize);
        public const string GlobalFontFamily = nameof(GlobalFontFamily);
        private static readonly FontFamily _defaultFontFamily = (FontFamily)Application.Current.Resources[GlobalFontFamily];
        private static readonly FontFamilyItem _defaultFontFamilyItem = new FontFamilyItem() { Name = DefaultFontName, FontFamily = _defaultFontFamily };

        public MainWindow MainWindow => Application.Current.MainWindow as MainWindow;

        public IList CurrentUserPermission { get; set; }

        public IList EmployeeSelectedItems { get; set; }

        public DateTime? CurrentDateTime { get; set; }

        public double WindowScale { get; set; } = 1;

        public ObservableCollection<Employee> Employees { get; set; }

        public ObservableCollection<PackIconKind> PackIcons { get; set; }

        public ObservableCollection<WorkflowItem> WorkflowItems { get; set; } = new ObservableCollection<WorkflowItem>();

        public ObservableCollection<WorkflowItemModel> WorkflowItemModels { get; set; } = new ObservableCollection<WorkflowItemModel>();

        public IList SelectedWorkflowItems { get; set; }

        public ObservableCollection<TreeViewModel> TreeViewModels { get; set; } = new ObservableCollection<TreeViewModel>();

        public TreeViewModel TreeViewSelectedItem { get; set; }

        public ObservableCollection<FontFamilyItem> FontFamilyList { get; }

        public IEnumerable<double> FontSizeList { get; } = Enumerable.Range(10, 11).Select(x => (double)x);

        public Employee Employee { get; set; } = new Employee();

        public object SelectedObject { get; set; }

        public bool DocumentWrapPanelIsWrap { get; set; } = true;

        public string StatusBarContent { get; set; } = "Ready";

        public DataTable Result { get; set; } = new DataTable();

        public ObservableCollection<UserInfoModel> UserInfos { get; set; }

        public IList UserInfoSelectedItems { get; set; }

        public double ProgressBarValue { get; set; }

        public double NumericUpDownValue { get; set; } = 1.99;

        [Range(0, 1024, ErrorMessage = "值必须在0-1024之间")]
        public double Range { get; set; }

        public bool IsAntiAliasing { get; set; } = true;

        public ObservableCollection<string> DengGao { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> JingYeSi { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> HuangHeLou { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> DengGuanQueLou { get; set; } = new ObservableCollection<string>();

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

        public ICommand TreeViewAddCommand { get; }

        public ICommand CollapseAllCommand { get; }

        public ICommand AddWorkflowItemCommand { get; }

        public ICommand DeleteWorkflowItemCommand { get; }

        public ICommand SaveAsImageCommand { get; }

        public ICommand WorkflowEditorDropCommand { get; }

        public ICommand WorkflowEditorTestCommand { get; }

        private ICommand _startCommand;
        public ICommand StartCommand
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
                return Can;
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
                OnPropertyChanged(() => CurrentTheme);
            }
        }

        private FontFamilyItem _currentFontFamily = _defaultFontFamilyItem;
        [DoNotNotify]
        public FontFamilyItem CurrentFontFamily
        {
            get => _currentFontFamily;
            set
            {
                if (value.Name == _currentFontFamily.Name)
                {
                    return;
                }
                Application.Current.Resources[GlobalFontFamily] = value.Name == DefaultFontName ? _defaultFontFamily : FontFamilyList.FirstOrDefault(x => x.Name == value.Name).FontFamily;
                _currentFontFamily = value;
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
            CurrentTheme = (ThemeType)sender;
            StatusBarContent = "软件主题已改变为" + (ThemeType)sender;
        }

        private void StatusManager_IsRunningChanged(object sender, EventArgs e)
        {
            StatusBarContent = "IsRunning:" + sender;
        }

        public void ZoomIn()
        {
            WindowScale = Math.Max(0.5, WindowScale - 0.25);
            WindowScale = Math.Round(WindowScale, 2);
        }

        public void ZoomOut()
        {
            WindowScale = Math.Min(3, WindowScale + 0.25);
            WindowScale = Math.Round(WindowScale, 2);
        }

        private void ExpandAllTreeViewItem(TreeViewModel treeViewModel, bool isExpand)
        {
            treeViewModel.IsExpanded = isExpand;
            foreach (var item in treeViewModel.Children)
            {
                ExpandAllTreeViewItem(item, isExpand);
            }
        }
    }

    [BaseObjectIgnore]
    public class Employee : EmployeeBase
    {
        public string Name { get; set; }

        public bool IsExpanded { get; set; } = true;

        public ImageSource ImageSource { get; set; } = new BitmapImage(new Uri("/Assets/Images/close.png", UriKind.Relative));

        public int? IntString { get; set; } = null;

        [TextBoxPlaceHolder(PlaceHolder = "请输入密码..."), Password]
        public string Password { get; set; } = "123456";

        public Abs? Abs { get; set; } = null;

        public InstrumentType InstrumentType { get; set; }

        [PropertyGrid(DisplayName = "描述")]
        public string Description { get; set; }

        [PropertyGrid(DisplayName = "值")]
        public double? Value { get; set; }

        public SolidColorBrush Brush { get; set; } = Brushes.Transparent;

        public Global Global { get; set; } = new Global();
    }

    public class EmployeeBase : ViewModelBase
    {
        public string Base => GetType().Name;
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
    public class Global : EmployeeBase
    {
        public string Name { get; set; }

        public double Value { get; set; }

        [ToolTip(ToolTip = "这个表示时间")]
        [PropertyGrid(DisplayName = "时间", IsReadOnly = false)]
        public DateTime DateTime { get; set; } = DateTime.Now;

        public List<string> DateTimes { get; set; } = new List<string>() { "123", "456", "789", "abc", "↑↓←→" };
    }

    [TypeConverter(typeof(EnumDescriptionConverter))]
    public enum InstrumentType
    {
        [Description("I/O控制单元")]
        ControlBoard,
        [Description("测试通道控制")]
        MatrixBoard,
        [Description("低压测量模块")]
        M3054,
        [Description("高压测量模块")]
        M9310,
        [Description("LCR测量模块")]
        M2821
    }

    public enum MessageType
    {
        AlertDialog,
        TreeViewItemLoaded
    }

    [Flags]
    [TypeConverter(typeof(EnumDescriptionConverter))]
    public enum UserPermission
    {
        [Description("无")]
        None = 0,
        [Description("测试")]
        Test = 1,
        [Description("编辑")]
        Edit = 1 << 1,
        [Description("设置")]
        Settings = 1 << 2
    }

    public class FontFamilyItem
    {
        public string Name { get; set; }
        public FontFamily FontFamily { get; set; }
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