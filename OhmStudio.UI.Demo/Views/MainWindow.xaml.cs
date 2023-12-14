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
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.Win32;
using OhmStudio.UI.Commands;
using OhmStudio.UI.Controls;
using OhmStudio.UI.Converters;
using OhmStudio.UI.Messaging;
using OhmStudio.UI.PublicMethods;

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
            InitializeComponent();
            DataContext = this;
            FontFamilyList = new InstalledFontCollection().Families.Select(x => x.Name).ToList();
            FontFamilyList.Insert(0, DefaultFont);
            for (double i = 10; i <= 20; i++)
            {
                FontSizeList.Add(i);
            }
            var text = textEditor;
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

            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
            Result.Columns.Add("Time");
            Result.Columns.Add("V0");
            Result.Columns.Add("V1");
            Result.Columns.Add("RX");
            for (int i = 0; i < 100; i++)
            {
                Result.Rows.Add(DateTime.Now, i, i + 1, "44");
            }
            //da.ItemsSource = Result.DefaultView;

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

        private OhmTheme currentTheme = XamlThemeResource.Instance.Theme;
        public OhmTheme CurrentTheme
        {
            get => currentTheme;
            set
            {
                XamlThemeResource.Instance.Theme = value;
                StatusManager.Update();
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

        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }

        private double _windowScale = 1;
        public double WindowScale
        {
            get => _windowScale;
            set
            {
                _windowScale = value;
                OnPropertyChanged(() => WindowScale);
            }
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

        void ZoomIn()
        {
            if (WindowScale <= 0.2)
            {
                return;
            }
            WindowScale -= 0.1;
            WindowScale = Math.Round(WindowScale, 1);
        }

        void ZoomOut()
        {
            if (WindowScale >= 4)
            {
                return;
            }
            WindowScale += 0.1;
            WindowScale = Math.Round(WindowScale, 1);
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
            Task.Run(() =>
            {
                AlertDialog.ShowError("发生了错误Rrecipient");
            });
            //Messenger.Default.Send("发送了消息Rrecipient", Rrecipient);
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
            StatusManager.IsRunning = !StatusManager.IsRunning;
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

        private void Button_Click_7(object sender, RoutedEventArgs e)
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

            //string code = " return Greeting  + \"123\"  ;";

            //var scriptOptions = ScriptOptions.Default.WithImports("System");

            //var globals = new Globals { Greeting = "Hello, World!" };

            //var result = await CSharpScript.RunAsync(code, scriptOptions, globals);
            //AlertDialog.Show(result.ReturnValue.ToString());

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
            //    AlertDialog.Show(ex.Message, "Error", MessageButton.OK, MessageImage.Error);
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

    public static class ThemeCollection
    {
        static ThemeCollection()
        {
            AllThemes = new() { new VisualStudio2022Blue(), new VisualStudio2022Dark(), new VisualStudio2022Light(), new VisualStudio2019Blue(), new VisualStudio2019Dark(), new VisualStudio2019Light() };
        }

        public static List<OhmTheme> AllThemes { get; }

        public static OhmTheme InitialTheme => AllThemes.First();

        private const string ThemesPath = "/OhmStudio.UI;component/Themes/";

        private sealed class VisualStudio2019Blue : OhmTheme
        {
            public override string Name => "2019 Blue";

            public override IEnumerable<string> ThemeResources
            {
                get
                {
                    yield return ThemesPath + "VisualStudio2019/BlueTheme.xaml";
                }
            }
        }

        private sealed class VisualStudio2019Dark : OhmTheme
        {
            public override string Name => "2019 Dark";

            public override IEnumerable<string> ThemeResources
            {
                get
                {
                    yield return ThemesPath + "VisualStudio2019/DarkTheme.xaml";
                }
            }
        }

        private sealed class VisualStudio2019Light : OhmTheme
        {
            public override string Name => "2019 Light";

            public override IEnumerable<string> ThemeResources
            {
                get
                {
                    yield return ThemesPath + "VisualStudio2019/LightTheme.xaml";
                }
            }
        }

        private sealed class VisualStudio2022Blue : OhmTheme
        {
            public override string Name => "2022 Blue";

            public override IEnumerable<string> ThemeResources
            {
                get
                {
                    yield return ThemesPath + "VisualStudio2022/BlueTheme.xaml";
                }
            }
        }

        private sealed class VisualStudio2022Dark : OhmTheme
        {
            public override string Name => "2022 Dark";

            public override IEnumerable<string> ThemeResources
            {
                get
                {
                    yield return ThemesPath + "VisualStudio2022/DarkTheme.xaml";
                }
            }
        }

        private sealed class VisualStudio2022Light : OhmTheme
        {
            public override string Name => "2022 Light";

            public override IEnumerable<string> ThemeResources
            {
                get
                {
                    yield return ThemesPath + "VisualStudio2022/LightTheme.xaml";
                }
            }
        }
    }

    public class XamlThemeResource : ResourceDictionary
    {
        public XamlThemeResource()
        {
            instance = this;
            InitializeThemes();
        }

        private static XamlThemeResource instance;
        public static XamlThemeResource Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("The XamlThemeResource is not loaded!");
                }
                return instance;
            }
        }

        private OhmTheme theme = ThemeCollection.InitialTheme;
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
}