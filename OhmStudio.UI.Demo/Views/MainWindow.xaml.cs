using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.Win32;
using OhmStudio.UI.Attaches;
using OhmStudio.UI.Commands;
using OhmStudio.UI.Controls;
using OhmStudio.UI.Converters;
using OhmStudio.UI.Helpers;
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
        XmlFoldingStrategy _xmlFoldingStrategy = new XmlFoldingStrategy();
        BraceFoldingStrategy _braceFoldingStrategy = new BraceFoldingStrategy();

        public MainWindow()
        {
            //if (new UserLoginWindow().SetOwner().ShowDialog() != true)
            //{
            //    Environment.Exit(Environment.ExitCode);
            //}
            InitializeComponent();
            DataContext = this;

            Messenger.Default.Register<string>(this, MessageType.AlertDialog, msg => AlertDialog.Show(msg));
            Messenger.Default.Register<string>(this, MessageType.TreeViewItemLoaded, (message) => StatusBarContent = message);
            var fontFamilys = new InstalledFontCollection().Families.Select(x => new FontFamilyItem() { Name = x.Name, FontFamily = new FontFamily(x.Name) });
            FontFamilyList = new ObservableCollection<FontFamilyItem>(fontFamilys);
            FontFamilyList.Insert(0, _defaultFontFamilyItem);
            textEditorcs.Text = "using System;\r\n\r\nclass Program\r\n{\r\n    static void Main()\r\n    {\r\n        Console.WriteLine(\"Hello World\");\r\n    }\r\n}";
            textEditorcpp.Text = "#include <iostream>\r\n\r\nint main() {\r\n    std::cout << \"Hello World\" << std::endl;\r\n    return 0;\r\n}";
            textEditorxml.Text = "<Project Sdk=\"Microsoft.NET.Sdk\">\r\n\r\n\t<PropertyGroup>\r\n\t\t<OutputType>WinExe</OutputType>\r\n\t\t<TargetFramework>net6.0-windows</TargetFramework>\r\n\t\t<UseWPF>true</UseWPF>\r\n\t</PropertyGroup>\r\n \r\n</Project>";
            var mainTextEditor = textEditorxml;
            var searchPanel = SearchPanel.Install(mainTextEditor);
            searchPanel.MarkerBrush = "#BEAA46".ToSolidColorBrush();

            var xmlFoldingManager = FoldingManager.Install(mainTextEditor.TextArea);
            var csFoldingManager = FoldingManager.Install(textEditorcs.TextArea);
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += delegate
            {
                _xmlFoldingStrategy.UpdateFoldings(xmlFoldingManager, mainTextEditor.Document);
                _braceFoldingStrategy.UpdateFoldings(csFoldingManager, textEditorcs.Document);
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

            AddWorkflowItemCommand = new RelayCommand<StepType>((stepType) =>
            {
                var point = workflowEditor.ContextMenu.TranslatePoint(new Point(), workflowEditor);
                WorkflowItemViewModel workflowItemViewModel = new WorkflowItemViewModel();
                workflowItemViewModel.Name = EnumDescriptionConverter.GetEnumDesc(stepType);
                workflowItemViewModel.StepType = stepType;
                workflowItemViewModel.Left = point.X.Adsorb(workflowEditor.GridSize);
                workflowItemViewModel.Top = point.Y.Adsorb(workflowEditor.GridSize);
                WorkflowItemViewModels.Add(workflowItemViewModel);
            });
            DeleteWorkflowItemCommand = new RelayCommand(() =>
            {
                for (int i = SelectedWorkflowItems.Count - 1; i >= 0; i--)
                {
                    WorkflowItemViewModels.Remove(SelectedWorkflowItems[i] as WorkflowItemViewModel);
                }
                //foreach (var item in SelectedWorkflowItems)
                //{
                //    WorkflowItems.Remove(item);
                //}
                //UpdateMultiSelectionMask();
            });
            SaveAsImageCommand = new RelayCommand(() =>
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PNG 文件|*.png";
                saveFileDialog.Title = "另存为图片";
                if (saveFileDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(saveFileDialog.FileName))
                {
                    this.SaveAsImage(saveFileDialog.FileName, ImageType.Png);
                }
            });
            SelectAllCommand = new RelayCommand(() =>
            {
                foreach (var item in WorkflowItemViewModels)
                {
                    item.IsSelected = true;
                }
            });
            UnselectAllCommand = new RelayCommand(() =>
            {
                foreach (var item in WorkflowItemViewModels)
                {
                    item.IsSelected = false;
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

            UserInfos.Add(new UserInfoModel());
            UserInfos.Add(new UserInfoModel() { UserName = "jack" });
            UserInfos.Add(new UserInfoModel() { UserName = "rose", Password = "123456" });

            WorkflowItemViewModels.Add(new WorkflowItemViewModel() { Name = "开始", StepType = StepType.Begin, Left = 100 });
            WorkflowItemViewModels.Add(new WorkflowItemViewModel() { Name = "love", StepType = StepType.Nomal, Left = 200, Top = 200 });
            WorkflowItemViewModels.Add(new WorkflowItemViewModel() { Name = "结束", StepType = StepType.End, Left = 300, Top = 400 });

            WorkflowItemViewModels.Last().LastStep = WorkflowItemViewModels[1];
            WorkflowItemViewModels[1].NextStep = WorkflowItemViewModels.Last();

            WorkflowItems.Add(new WorkflowItem() { Content = new TextBox() { Margin = new Thickness(8), Text = "开始" }, StepType = StepType.Begin });
            var workflowItem = new WorkflowItem() { Content = new TextBlock() { Text = "中间节点" } };
            Canvas.SetLeft(workflowItem, 100);
            Canvas.SetTop(workflowItem, 100);
            WorkflowItems.Add(workflowItem);
            WorkflowItems.Add(new WorkflowItem() { Content = new TextBox() { Margin = new Thickness(8), Text = "结束" }, StepType = StepType.End });

            StatusManager.IsRunningChanged += StatusManager_IsRunningChanged;
            XamlThemeDictionary.ThemeChanged += XamlThemeDictionary_ThemeChanged;
        }

        bool _can;
        const string DefaultFontName = "默认";
        public const string GlobalFontSize = nameof(GlobalFontSize);
        public const string GlobalFontFamily = nameof(GlobalFontFamily);
        static readonly FontFamily _defaultFontFamily = (FontFamily)Application.Current.Resources[GlobalFontFamily];
        static readonly FontFamilyItem _defaultFontFamilyItem = new FontFamilyItem() { Name = DefaultFontName, FontFamily = _defaultFontFamily };

        public IList CurrentUserPermission { get; set; }

        public IList EmployeeSelectedItems { get; set; }

        public DateTime? CurrentDateTime { get; set; }

        public double WindowScale { get; set; } = 1;

        public ObservableCollection<Employee> Employees { get; set; }

        public ObservableCollection<PackIconKind> PackIcons { get; set; }

        public ObservableCollection<WorkflowItem> WorkflowItems { get; set; } = new ObservableCollection<WorkflowItem>();

        public ObservableCollection<WorkflowItemViewModel> WorkflowItemViewModels { get; set; } = new ObservableCollection<WorkflowItemViewModel>();

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

        public ObservableCollection<UserInfoModel> UserInfos { get; set; } = new ObservableCollection<UserInfoModel>();

        public IList UserInfoSelectedItems { get; set; }

        public double ProgressBarValue { get; set; }

        public double NumericUpDownValue { get; set; } = 1.99;

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

        public ICommand TreeViewAddCommand { get; }

        public ICommand CollapseAllCommand { get; }

        public ICommand AddWorkflowItemCommand { get; }

        public ICommand DeleteWorkflowItemCommand { get; }

        public ICommand SaveAsImageCommand { get; }

        public ICommand SelectAllCommand { get; }

        public ICommand UnselectAllCommand { get; }

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

        void ZoomIn()
        {
            WindowScale = Math.Max(0.5, WindowScale - 0.25);
            WindowScale = Math.Round(WindowScale, 2);
        }

        void ZoomOut()
        {
            WindowScale = Math.Min(3, WindowScale + 0.25);
            WindowScale = Math.Round(WindowScale, 2);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);
            var version = Assembly.GetAssembly(typeof(ChromeWindow)).GetName().Version.ToString();
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 1; i <= 100; i++)
            {
                stringBuilder.Append("Item:" + i + Environment.NewLine);
            }

            AlertDialog.Language = LanguageType.En_US;
            var result = AlertDialog.Show(stringBuilder.ToString(), version, MessageBoxButton.YesNoCancel, MessageBoxImage.Error);
            MessageBox.Show("点击了" + result, version);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Task.Run(() =>
            //{
            //    AlertDialog.ShowError("AlertDialog.ShowError");
            //});
            Messenger.Default.Send("AlertDialog.Show", MessageType.AlertDialog);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            UIMessageTip.Show("GC完成");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Employee.Value = 100;
            AlertDialog.Show(Employee.InstrumentType.ToString());
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            StatusManager.IsRunning = !StatusManager.IsRunning;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in Employees)
            {
                item.IsExpanded = true;
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (var item in Employees)
            {
                item.IsExpanded = false;
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            UserLoginWindow userLoginWindow = new UserLoginWindow();
            txtLogin.Text = "登录中...";
            userLoginWindow.SetOwner().ShowDialog();
            txtLogin.Text = "登录";
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            _can = !_can;
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
            StringBuilder stringBuilder = new StringBuilder("UIMessageTip.ShowError");
            for (int i = 0; i < 50; i++)
            {
                stringBuilder.Append(";UIMessageTip.ShowError");
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
            rollBox.ItemsSource = new ObservableCollection<UIElement> { new Image() { Source = new BitmapImage(new Uri("https://pic1.zhimg.com/v2-ecac0aedda57bffecbbe90764828a825_r.jpg?source=1940ef5c")) }, new Button() { Content = "This is a new Button" } };
        }

        void ExpandAllTreeViewItem(TreeViewModel treeViewModel, bool isExpand)
        {
            treeViewModel.IsExpanded = isExpand;
            foreach (var item in treeViewModel.Children)
            {
                ExpandAllTreeViewItem(item, isExpand);
            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            StatusBarContent = $"DataGrid当前编辑元素：{e.Column.GetCellContent(e.Row)}";
        }

        private void Button_Click_17(object sender, RoutedEventArgs e)
        {
            if (EmployeeSelectedItems != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in EmployeeSelectedItems.OfType<Employee>())
                {
                    stringBuilder.Append(item.Name + Environment.NewLine);
                }
                AlertDialog.Show(stringBuilder.ToString());
            }
        }

        private void Button_Click_18(object sender, RoutedEventArgs e)
        {
            if (CurrentUserPermission != null)
            {
                var userPermission = UserPermission.None;
                foreach (var item in CurrentUserPermission.OfType<UserPermission>())
                {
                    userPermission |= item;
                }
                AlertDialog.Show(userPermission.ToString());
            }
        }

        private void Button_Click_16(object sender, RoutedEventArgs e)
        {
            stackPanel.SaveAsImage("1.png", ImageType.Png);
        }
    }

    public class TreeViewModel : ObservableObject
    {
        static TreeViewModel()
        {
            OpenFileCommand = new RelayCommand<TreeViewModel>((treeViewModel) =>
            {
                if (!treeViewModel.IsFolder)
                {
                    PathHelper.OpenFlie(treeViewModel.FullPath);
                }
            });
            OpenFileLocationCommand = new RelayCommand<TreeViewModel>((treeViewModel) =>
            {
                PathHelper.OpenFileLocation(treeViewModel.FullPath);
            });
            CopyCommand = new RelayCommand<TreeViewModel>((treeViewModel) =>
            {
                StringCollection stringCollection = new StringCollection();//收集路径
                stringCollection.Add(treeViewModel.FullPath);
                Clipboard.SetFileDropList(stringCollection);
            });
            CopyFullPathCommand = new RelayCommand<TreeViewModel>((treeViewModel) =>
            {
                Clipboard.SetDataObject(treeViewModel.FullPath, true);
            });
            StartRenameCommand = new RelayCommand<TreeViewModel>((treeViewModel) =>
            {
                treeViewModel.IsEditing = true;
                treeViewModel.BeforeEditingName = treeViewModel.Header;
            });
            EndRenameCommand = new RelayCommand<TreeViewModel>((treeViewModel) =>
            {
                try
                {
                    treeViewModel.IsEditing = false;
                    string oldPath = treeViewModel.FullPath;
                    DirectoryInfo directory = new DirectoryInfo(oldPath);
                    string newPath = Path.Combine(directory.Parent?.FullName ?? string.Empty, treeViewModel.Header);
                    string filterate = @"\/:*?""<>|";

                    if (Regex.IsMatch(treeViewModel.Header, $"[{filterate}]"))
                    {
                        treeViewModel.Header = treeViewModel.BeforeEditingName;
                        UIMessageTip.ShowWarning($"文件夹和文件命名不能包含以下字符：\r\n{filterate}");
                        return;
                    }
                    if (oldPath == newPath)
                    {
                        return;
                    }
                    if (treeViewModel.IsFolder)
                    {
                        Directory.Move(oldPath, newPath);
                    }
                    else
                    {
                        File.Move(oldPath, newPath);
                    }

                    treeViewModel.FullPath = newPath;
                    treeViewModel.LoadIcon();
                    RenameSubTreeViewItem(treeViewModel, newPath);
                }
                catch (Exception ex)
                {
                    treeViewModel.Header = treeViewModel.BeforeEditingName;
                    UIMessageTip.ShowError("重命名异常：" + ex.Message);
                }
            });
            RenameVisibleCommand = new RelayCommand<TextBox>((textbox) =>
            {
                if (textbox.IsVisible)
                {
                    textbox.Focus();
                    var index = textbox.Text.LastIndexOf('.');
                    if (index > 0)
                    {
                        textbox.Select(0, index);
                    }
                    else
                    {
                        textbox.SelectAll();
                    }
                }
            });
            ShowPropertiesCommand = new RelayCommand<TreeViewModel>((treeViewModel) =>
            {
                PathHelper.ShowPathProperties(treeViewModel?.FullPath);
            });
            RemoveCommand = new RelayCommand<TreeViewModel>((treeViewModel) =>
            {
                treeViewModel.Delete();
            });
            DeleteFileCommand = new RelayCommand<TreeViewModel>((treeViewModel) =>
            {
                if (PathHelper.DeletePath(treeViewModel.FullPath, true, true, true, out string errMsg) == 0)
                {
                    treeViewModel.Delete();
                }
                else
                {
                    AlertDialog.ShowError(errMsg);
                }
            });
        }

        public TreeViewModel() { }

        public TreeViewModel(string header, bool isFolder, string fullPath, TreeViewModel parent = null) : this()
        {
            Header = header;
            IsFolder = isFolder;
            FullPath = fullPath;
            Parent = parent;
            LoadIcon();
        }

        [DoNotNotify]
        public bool IsLoaded { get; private set; }

        public string Header { get; set; }

        private bool _isExpanded;
        [DoNotNotify]
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    if (!IsLoaded && value)
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        foreach (var child in Children)
                        {
                            LoadSubDirectory(child, child.FullPath, false);
                        }
                        stopwatch.Stop();
                        Messenger.Default.Send($"TreeView子节点加载完成耗时：{stopwatch.Elapsed.TotalMilliseconds}ms", MessageType.TreeViewItemLoaded);
                        IsLoaded = true;
                    }
                    OnPropertyChanged(nameof(IsExpanded));
                }
            }
        }

        public bool IsSelected { get; set; }

        [DoNotNotify]
        public bool IsRootNode => CollectionParent != null;

        public bool IsFolder { get; set; }

        [DoNotNotify]
        private string BeforeEditingName { get; set; }

        public bool IsEditing { get; set; }

        public string FullPath { get; set; }

        public ImageSource IconImageSource { get; set; }

        [DoNotNotify]
        public TreeViewModel Parent { get; private set; }

        [DoNotNotify]
        public IList<TreeViewModel> CollectionParent { get; private set; }

        public ObservableCollection<TreeViewModel> Children { get; set; } = new ObservableCollection<TreeViewModel>();

        public static ICommand OpenFileCommand { get; }

        public static ICommand OpenFileLocationCommand { get; }

        public static ICommand CopyCommand { get; }

        public static ICommand CopyFullPathCommand { get; }

        public static ICommand StartRenameCommand { get; }

        public static ICommand EndRenameCommand { get; }

        public static ICommand RenameVisibleCommand { get; }

        public static ICommand ShowPropertiesCommand { get; }

        public static ICommand RemoveCommand { get; }

        public static ICommand DeleteFileCommand { get; }

        public void Add(TreeViewModel treeViewModel)
        {
            Children.Add(treeViewModel);
        }

        public void Delete()
        {
            if (IsRootNode)
            {
                CollectionParent?.Remove(this);
            }
            else
            {
                Parent?.Children.Remove(this);
            }
        }

        public void LoadIcon()
        {
            Application.Current?.Dispatcher.InvokeAsync(() =>
            {
                //IconImageSource = IsFolder ? PathHelper.DirectoryIcon : PathHelper.GetFileIcon(FullPath);
                IconImageSource = PathHelper.GetIcon(FullPath, IsFolder);
            }, DispatcherPriority.ApplicationIdle);
        }

        public static void LoadRootDirectory(IList<TreeViewModel> source, string rootFolderPath)
        {
            try
            {
                //创建根节点
                TreeViewModel rootNode = new TreeViewModel(new DirectoryInfo(rootFolderPath).Name, true, rootFolderPath);
                rootNode.CollectionParent = source;
                // 加载根文件夹
                LoadSubDirectory(rootNode, rootFolderPath, false);
                source.Add(rootNode);
            }
            catch (Exception ex)
            {
                AlertDialog.ShowError(ex.Message);
            }
        }

        private static void LoadSubDirectory(TreeViewModel node, string fullPath, bool isBreak)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(fullPath);
            if (directoryInfo.Exists)
            {
                //加载子文件夹
                foreach (DirectoryInfo subDirInfo in directoryInfo.GetDirectories().OrderBy(x => x.Name))
                {
                    TreeViewModel subNode = new TreeViewModel(subDirInfo.Name, true, subDirInfo.FullName, node);

                    if (!isBreak)
                    {
                        node.IsLoaded = true;
                        LoadSubDirectory(subNode, subDirInfo.FullName, true);
                    }

                    node.Add(subNode);
                }
                //加载文件
                foreach (FileInfo fileInfo in directoryInfo.GetFiles().OrderBy(x => x.Name))
                {
                    TreeViewModel subNode = new TreeViewModel(fileInfo.Name, false, fileInfo.FullName, node);
                    node.Add(subNode);
                }
            }
        }

        private static void RenameSubTreeViewItem(TreeViewModel treeViewModel, string newPath)
        {
            foreach (var item in treeViewModel.Children)
            {
                item.FullPath = Path.Combine(newPath, item.Header);
                RenameSubTreeViewItem(item, newPath);
            }
        }
    }

    [BaseObjectIgnore]
    public class Employee : EmployeeBase
    {
        public string Name { get; set; }

        public bool IsExpanded { get; set; } = true;

        public ImageSource ImageSource { get; set; } = new BitmapImage(new Uri("/Images/close.png", UriKind.Relative));

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

        public Globals Globals { get; set; } = new Globals();
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
    public class Globals : EmployeeBase
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

    public class WorkflowItemViewModel : ViewModelBase
    {
        public bool IsSelected { get; set; }

        public void OnIsSelectedChanged()
        {
            Debug.WriteLine(PathContent);
        }

        public string Name { get; set; }

        public StepType StepType { get; set; } = StepType.Begin;

        public double Width { get; set; } = 200;
        public double Height { get; set; } = 80;

        public double Left { get; set; }
        public double Top { get; set; }

        public string PathContent { get; set; } = "下一节点";

        public WorkflowItemViewModel LastStep { get; set; }
        public WorkflowItemViewModel NextStep { get; set; }
        public WorkflowItemViewModel FromStep { get; set; }
        public WorkflowItemViewModel JumpStep { get; set; }
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

    public class BraceFoldingStrategy
    {
        /// <summary>
        /// Gets/Sets the opening brace. The default value is '{'.
        /// </summary>
        public char OpeningBrace { get; set; }

        /// <summary>
        /// Gets/Sets the closing brace. The default value is '}'.
        /// </summary>
        public char ClosingBrace { get; set; }

        /// <summary>
        /// Creates a new BraceFoldingStrategy.
        /// </summary>
        public BraceFoldingStrategy()
        {
            OpeningBrace = '{';
            ClosingBrace = '}';
        }

        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            IEnumerable<NewFolding> newFoldings = CreateNewFoldings(document, out int firstErrorOffset);
            manager.UpdateFoldings(newFoldings, firstErrorOffset);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
        {
            List<NewFolding> newFoldings = new List<NewFolding>();

            Stack<int> startOffsets = new Stack<int>();
            int lastNewLineOffset = 0;
            char openingBrace = OpeningBrace;
            char closingBrace = ClosingBrace;
            for (int i = 0; i < document.TextLength; i++)
            {
                char c = document.GetCharAt(i);
                if (c == openingBrace)
                {
                    startOffsets.Push(i);
                }
                else if (c == closingBrace && startOffsets.Count > 0)
                {
                    int startOffset = startOffsets.Pop();
                    // don't fold if opening and closing brace are on the same line
                    if (startOffset < lastNewLineOffset)
                    {
                        newFoldings.Add(new NewFolding(startOffset, i + 1));
                    }
                }
                else if (c is '\r' or '\n')
                {
                    lastNewLineOffset = i + 1;
                }
            }
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }
    }
}