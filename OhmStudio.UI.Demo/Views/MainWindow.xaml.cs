using System;
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
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using OhmStudio.UI.Commands;
using OhmStudio.UI.Controls;
using OhmStudio.UI.Converters;
using OhmStudio.UI.Messaging;
using OhmStudio.UI.PublicMethods;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace OhmStudio.UI.Demo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ChromeWindow, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            FontFamilyList = new InstalledFontCollection().Families.Select(x => x.Name).ToList();
            FontFamilyList.Insert(0, DefaultFont);
            for (double i = 10; i <= 20; i++)
            {
                FontSizeList.Add(i);
            }
            //Loaded += async delegate
            //{
            //    await Task.Delay(10000); 
            //    ps.CanShowPasswordVisibility = Visibility.Collapsed;
            //}; 
            Result.Columns.Add("Time");
            Result.Columns.Add("V0");
            Result.Columns.Add("V1");
            Result.Columns.Add("RX");
            for (int i = 0; i < 100; i++)
            {
                Result.Rows.Add(DateTime.Now, i, i + 1, "44");
            } 
            //da.ItemsSource = Result.DefaultView;
            //PlotModel = new PlotModel();

            //// 添加一个线性系列
            //var series = new LineSeries();
            //series.Points.Add(new DataPoint(0, 0));
            //series.Points.Add(new DataPoint(1, 1));
            //series.Points.Add(new DataPoint(2, 2));

            //// 将系列添加到 PlotModel 中
            //PlotModel.Series.Add(series);
            PlotModel = new PlotModel();

            // 创建一个数值轴
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                TextColor = OxyColor.Parse("#ffffff"),
                TitleColor = OxyColor.Parse("#ffffff"),

                MajorGridlineColor = OxyColors.Red, // 设置主刻度线颜色
                MinorGridlineColor = OxyColors.Orange,
                Title = "X"
            };
            PlotModel.Axes.Add(xAxis);

            // 创建一个数值轴
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Y"
            };
            PlotModel.Axes.Add(yAxis);

            // 定义一条曲线
            var series = new LineSeries
            {
                Title = "Complex Curve",

                StrokeThickness = 3
            };

            // 添加数据点
            for (double x = -10; x <= 10; x += 0.1)
            {
                double y = Math.Cos(x) * Math.Exp(-x * x / 25) + Math.Sin(3 * x);
                series.Points.Add(new DataPoint(x, y));
            }

            // 将系列添加到 PlotModel 中
            PlotModel.Series.Add(series);

            Pro.Description = "1";
            Pro.Brush = Brushes.Red;

            Items = Pro;
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
            {
                Pro pro = new Pro();
                if (i % 3 == 0)
                {
                    pro.Name = 10;
                }
                else
                {
                    pro.Name = 10 + i;
                }
                pro.Value = 100 + i;
                pro.Description = "ABCghn" + i;
                FileNodes.Add(pro);
            }
            stopwatch.Stop();
            var res2 = stopwatch.ElapsedMilliseconds;
            var viewSource = new CollectionViewSource { Source = FileNodes };
            FileNodes.CollectionChanged += delegate
            {
                AlertDialog.Show("已改变");
            };
            UserInfos.Add(new UserInfoModel());
            UserInfos.Add(new UserInfoModel() { Name = "wang" });
            Messenger.Default.Register<string>(this, Rrecipient, msg => AlertDialog.Show(msg));
            //string directory = Path.Combine(App.DocumentDirectory, "Layout");
            //string path = Path.Combine(directory, "Layout.xml");
            //Loaded += delegate
            //{
            //    if (File.Exists(path))
            //    {
            //        var serializer = new XmlLayoutSerializer(dockingManager);
            //        serializer.Deserialize(path);
            //    }
            //};
            //Closing += delegate
            //{
            //    try
            //    {
            //        int i = 0;
            //        foreach (LayoutAnchorable item in dockingManager.AnchorablesSource)
            //        {
            //            item.ContentId = (++i).ToString();
            //        }
            //        foreach (LayoutDocument item in dockingManager.DocumentsSource)
            //        {
            //            item.ContentId = (++i).ToString();
            //        }
            //        if (!Directory.Exists(directory))
            //        {
            //            Directory.CreateDirectory(directory);
            //        }
            //        var serializer = new XmlLayoutSerializer(dockingManager);
            //        serializer.Serialize(path);
            //        XDocument xDoc = XDocument.Load(path);
            //        xDoc.Descendants("Hidden").Remove();//.SingleOrDefault(x => x.Attribute("id").Value.Equals("4"))
            //        xDoc.Save(path);
            //    }
            //    catch (Exception ex)
            //    {
            //        AlertDialog.ShowWarning("在保存Layout布局xml文件时遇到异常：" + ex.Message);
            //    }
            //};
            //DispatcherTimer dispatcherTimer = new DispatcherTimer();
            //dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            //dispatcherTimer.Tick += (sender, e) =>
            //{
            //    Can = !Can;
            //    CommandManager.InvalidateRequerySuggested();
            //    Debug.WriteLine(Can);
            //};
            //dispatcherTimer.Start();
        }

        const string Rrecipient = "Ohm";

        public PlotModel PlotModel { get; set; }

        public Pro Pro { get; set; } = new Pro();

        private DataTable _result = new DataTable();
        public DataTable Result
        {
            get => _result;
            set => _result = value;
        }

        private ObservableCollection<UserInfoModel> _uerInfos = new ObservableCollection<UserInfoModel>();
        public ObservableCollection<UserInfoModel> UserInfos
        {
            get => _uerInfos;
            set { _uerInfos = value; OnPropertyChanged(nameof(UserInfos)); }
        }

        private IList _userInfoSelectedItems;
        public IList UserInfoSelectedItems
        {
            get => _userInfoSelectedItems;
            set
            {
                _userInfoSelectedItems = value;
                OnPropertyChanged(nameof(UserInfoSelectedItems));
            }
        }


        bool Can;

        private RelayCommand startCommand;
        public RelayCommand StartCommand
        {
            get => startCommand ??= new RelayCommand(() =>
            {
                if (UserInfoSelectedItems != null)
                {
                    foreach (var item in UserInfoSelectedItems.OfType<UserInfoModel>())
                    {
                        AlertDialog.Show(item.Name);
                    }
                }
            }
            , () =>
            {
                //Debug.WriteLine(Can);
                return Can;
            });
            set => startCommand = value;
        }

        const string DefaultFont = "默认";
        public const string GlobalFontSize = "GlobalFontSize";
        public const string GlobalFontFamily = "GlobalFontFamily";
        public List<string> FontFamilyList { get; set; }
        readonly FontFamily _defaultFontFamily = (FontFamily)Application.Current.Resources[GlobalFontFamily]; /*new FontFamily(new Uri("pack://application:,,,/"), "/Fonts/OPPOSans-M.ttf#OPPOSans M");*/
        public List<double> FontSizeList { get; set; } = new List<double>();

        private OhmTheme currentTheme = OhmXamlUIResource.Instance.Theme;
        public OhmTheme CurrentTheme
        {
            get => currentTheme;
            set
            {
                OhmXamlUIResource.Instance.Theme = value;
                OnPropertyChanged(ref currentTheme, value, nameof(CurrentTheme));
            }
        }

        private string currentFontFamily = DefaultFont;
        public string CurrentFontFamily
        {
            get => currentFontFamily;
            set
            {
                value ??= DefaultFont;
                Application.Current.Resources[GlobalFontFamily] = value == DefaultFont ? _defaultFontFamily : new FontFamily(value);
                OnPropertyChanged(ref currentFontFamily, value, nameof(CurrentFontFamily));
            }
        }

        private double currentFontSize = (double)Application.Current?.Resources[GlobalFontSize];
        public double CurrentFontSize
        {
            get => currentFontSize;
            set
            {
                Application.Current.Resources[GlobalFontSize] = value;
                OnPropertyChanged(ref currentFontSize, value, nameof(CurrentFontSize));
            }
        }

        private DateTime currentDateTime;
        public DateTime CurrentDateTime
        {
            get => currentDateTime;
            set => OnPropertyChanged(ref currentDateTime, value, nameof(CurrentDateTime));
        }

        private object items = new object();
        public object Items
        {
            get => items;
            set => OnPropertyChanged(ref items, value, nameof(Items));
        }

        private ObservableCollection<Pro> fileNodes = new();
        public ObservableCollection<Pro> FileNodes
        {
            get => fileNodes;
            set { fileNodes = value; OnPropertyChanged(() => FileNodes); }
        }

        private IList selectedItemsFileNodes;
        public IList SelectedItemsFileNodes
        {
            get => selectedItemsFileNodes;
            set { selectedItemsFileNodes = value; OnPropertyChanged(() => SelectedItemsFileNodes); }
        }

        public IEnumerable<UserInfoModel> FileNodesSelectedItems => SelectedItemsFileNodes.OfType<UserInfoModel>();

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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1000);
            var assembly = Assembly.GetAssembly(typeof(ChromeWindow));
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                stringBuilder.Append(i + "StringBuilder" + "\r\n");
            }

            AlertDialog.Show(stringBuilder.ToString(), assembly.GetName().Version.ToString(), MessageButton.OK, MessageImage.Error);
            //AlertDialog.Show("private void Button_Click(object sender, RoutedEventArgs e)\r\npublic abstract class OhmTheme : ResourceDictionary\r\n" + assembly.GetName().Version, "", MessageButton.OK, MessageImage.Information);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send("发送了消息", Rrecipient);
            UIMessageTip.ShowError("发生了错误");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            UIMessageTip.Show("GC完成");


            var args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Right)
            {
                RoutedEvent = MouseRightButtonDownEvent,
            };

            Border.RaiseEvent(args);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            (Items as Pro).Value = 100;
            AlertDialog.Show((Items as Pro).InstrumentType.ToString());
        }

        //static readonly ObservableCollection<string> itemss = new ObservableCollection<string>() { "ADSDADSA", "ASDSAD", "ASDASD" };
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(se.Focus().ToString());
            //if (SelectedItemsFileNodes != null)
            //{
            //    StringBuilder stringBuilder = new StringBuilder();
            //    foreach (var item in SelectedItemsFileNodes)
            //    {
            //        stringBuilder.AppendLine(item.ToString());
            //    }
            //    AlertDialog.Show(stringBuilder.ToString());
            //}
            //FileNodes = new ObservableCollection<Pro>(); 
            //foreach (var item in FileNodes)
            //    item. IsExpanded = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in FileNodes)
            {
                item.IsExpanded = true;
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (var item in FileNodes)
            {
                item.IsExpanded = false;
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.SetOwner();
            loginWindow.ShowDialog();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            Can = !Can;
        }

        public class Globals
        {
            public string Greeting { get; set; }
        }

        private async void Button_Click_7(object sender, RoutedEventArgs e)
        {
            //int a = 1;

            //string code = "int result = 1 + a;  return result;";

            //var options = ScriptOptions.Default.WithImports("System");

            //var script = CSharpScript.Create(code, options);

            //var result = await script.RunAsync();

            //await Task.Run(async () =>
            //{
            //    var result = await CSharpScript.RunAsync(code);
            //    AlertDialog.Show(result.ReturnValue.ToString());
            //});

            string code = " return Greeting  + \"123\"  ;";

            var scriptOptions = ScriptOptions.Default.WithImports("System");

            var globals = new Globals { Greeting = "Hello, World!" };

            var result = await CSharpScript.RunAsync(code, scriptOptions, globals);
            AlertDialog.Show(result.ReturnValue.ToString());

        }

        private void Border_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border.ContextMenu.IsOpen = true;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new();
            folderBrowserDialog.Description = "请选择文件夹";
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadTreeView(folderBrowserDialog.SelectedPath);
            }
        }

        private void LoadFolders(string path, TreeViewItem parentNode)
        {
            // 获取文件夹中的子文件夹
            string[] subDirectories = Directory.GetDirectories(path);

            // 遍历子文件夹
            foreach (string directory in subDirectories)
            {
                TreeViewItem folderItem = new TreeViewItem();
                folderItem.Header = new DirectoryInfo(directory).Name;

                // 递归调用以加载子文件夹
                LoadFolders(directory, folderItem);

                // 将该子文件夹添加到父节点
                parentNode.Items.Add(folderItem);
            } 
        }

        private void LoadTreeView(string rootFolderPath)
        {
            // 创建根节点
            TreeViewItem rootNode = new TreeViewItem();
            rootNode.Header = new DirectoryInfo(rootFolderPath).Name;

            // 加载根文件夹
            LoadFolders(rootFolderPath, rootNode);

            // 将根节点添加到 TreeView
            treeView.Items.Add(rootNode);

        }
    }

    public abstract class OhmTheme : ResourceDictionary
    {
        public abstract string Name { get; }

        public abstract IEnumerable<string> ThemeResources { get; }

        protected OhmTheme()
        {
            foreach (var item in ThemeResources)
            {
                MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri(item, UriKind.Relative)
                });
            }
        }
    }

    public static class OhmThemeCollection
    {
        private const string AssemblyPath = "/OhmStudio.UI;component/";
        private const string ThemesPath = AssemblyPath + "Themes/";
        private const string AvalonDockThemesPath = AssemblyPath + "AvalonDockThemes/";
        private sealed class OhmVS2019Blue : OhmTheme
        {
            public override string Name => "2019 Blue";

            public override IEnumerable<string> ThemeResources
            {
                get
                {
                    yield return ThemesPath + "VisualStudio2019/BlueTheme.xaml";
                    yield return AvalonDockThemesPath + "VisualStudio2019/BlueTheme.xaml";
                }
            }
        }

        private sealed class OhmVS2019Dark : OhmTheme
        {
            public override string Name => "2019 Dark";

            public override IEnumerable<string> ThemeResources
            {
                get
                {
                    yield return ThemesPath + "VisualStudio2019/DarkTheme.xaml";
                    yield return AvalonDockThemesPath + "VisualStudio2019/DarkTheme.xaml";
                }
            }
        }

        private sealed class OhmVS2019Light : OhmTheme
        {
            public override string Name => "2019 Light";

            public override IEnumerable<string> ThemeResources
            {
                get
                {
                    yield return ThemesPath + "VisualStudio2019/LightTheme.xaml";
                    yield return AvalonDockThemesPath + "VisualStudio2019/LightTheme.xaml";
                }
            }
        }

        private sealed class OhmVS2022Blue : OhmTheme
        {
            public override string Name => "2022 Blue";

            public override IEnumerable<string> ThemeResources
            {
                get
                {
                    yield return ThemesPath + "VisualStudio2022/BlueTheme.xaml";
                    yield return AvalonDockThemesPath + "VisualStudio2022/BlueTheme.xaml";
                }
            }
        }

        private sealed class OhmVS2022Dark : OhmTheme
        {
            public override string Name => "2022 Dark";

            public override IEnumerable<string> ThemeResources
            {
                get
                {
                    yield return ThemesPath + "VisualStudio2022/DarkTheme.xaml";
                    yield return AvalonDockThemesPath + "VisualStudio2022/DarkTheme.xaml";
                }
            }
        }

        private sealed class OhmVS2022Light : OhmTheme
        {
            public override string Name => "2022 Light";

            public override IEnumerable<string> ThemeResources
            {
                get
                {
                    yield return ThemesPath + "VisualStudio2022/LightTheme.xaml";
                    yield return AvalonDockThemesPath + "VisualStudio2022/LightTheme.xaml";
                }
            }
        }

        public static List<OhmTheme> AllThemes { get; }

        static OhmThemeCollection()
        {
            AllThemes = new List<OhmTheme>()
            {
                new OhmVS2022Blue(),
                new OhmVS2022Dark(),
                new OhmVS2022Light(),
                new OhmVS2019Blue(),
                new OhmVS2019Dark(),
                new OhmVS2019Light()
            };
        }
    }

    [BaseObjectIgnore]
    public class Pro : ProBase
    {
        public double Name { get; set; }

        private bool isExpanded = true;
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }

        public ImageSource ImageSource { get; set; } = new BitmapImage(new Uri("/download.jpg", UriKind.Relative));
        public int? Abstring1 { get; set; } = null;
        [TextBoxPlaceHolder(PlaceHolder = "请输入密码")]
        [Password]
        public string Abstring { get; set; }
        public Abs? Abs { get; set; } = null;

        public InstrumentType InstrumentType { get; set; }

        [PropertyGrid(DisplayName = "名字")]

        public string Description { get; set; }
        [PropertyGrid(DisplayName = "值")]
        public double? Value { get; set; }

        public SolidColorBrush Brush { get; set; }
        public Pro1 Pro1 { get; set; } = new Pro1();
    }

    public class ProBase : ViewModelBase
    {
        public string Base { get; set; } = "ProBase";
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


    public class Pro1
    {
        public string Name { get; set; }
        public double Value { get; set; }
        [PropertyGrid(DisplayName = "时间", IsReadOnly = false)]
        public DateTime DateTime { get; set; }

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
        MHVM,
        [Description("K2400高压模块")]
        K2400
    }

    public class UserInfoModel
    {
        public string Name { get; set; } = "1";

        public string Password { get; set; } = "1";

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime LoginAt { get; set; }
    }

    public class OhmXamlUIResource : ResourceDictionary
    {
        public OhmXamlUIResource()
        {
            instance = this;
            theme = OhmThemeCollection.AllThemes.FirstOrDefault();
            InitializeThemes();
        }

        private static OhmXamlUIResource instance;
        public static OhmXamlUIResource Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("The OhmXamlUIResource is not loaded!");
                }
                return instance;
            }
        }

        private OhmTheme theme;
        public OhmTheme Theme
        {
            get => theme;
            set => UpdateOhmTheme(theme = value);
        }

        private void InitializeThemes()
        {
            MergedDictionaries.Add(Theme);
        }

        private void UpdateOhmTheme(OhmTheme theme)
        {
            MergedDictionaries[0] = theme;
        }
    }
}