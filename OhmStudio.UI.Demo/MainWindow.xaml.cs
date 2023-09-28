using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using OhmStudio.UI.Controls;
using OhmStudio.UI.PublicMethod;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace OhmStudio.UI.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : CustomChromeWindow, INotifyPropertyChanged
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
            for (int i = 0; i < 2048; i++)
            {
                Result.Rows.Add(DateTime.Now, i, i + 1, "44");
            }
            li.ItemsSource = Result.DefaultView;
            da.ItemsSource = Result.DefaultView;
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
                TextColor= OxyColor.Parse("#ffffff"),
                TitleColor= OxyColor.Parse("#ffffff"),
                 
                MajorGridlineColor = OxyColors.Red, // 设置主刻度线颜色
                MinorGridlineColor = OxyColors.Orange  ,
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
        }

        public PlotModel PlotModel { get; set; }

        private DataTable _result = new DataTable();
        public DataTable Result
        {
            get => _result;
            set
            {
                _result = value;

            }
        }

        const string DefaultFont = "默认";
        public const string GlobalFontSize = "GlobalFontSize";
        public List<string> FontFamilyList { get; set; }
        FontFamily _defaultFontFamily = new FontFamily(new Uri("pack://application:,,,/"), "/Fonts/OPPOSans-M.ttf#OPPOSans M");
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
                Application.Current.Resources["GlobalFontFamily"] = value == DefaultFont ? _defaultFontFamily : new FontFamily(value);
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

            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("Argument is not a property", nameof(memberExpression.Member));
            }

            return propertyInfo.Name;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1000);
            var assembly = Assembly.GetAssembly(typeof(CustomChromeWindow));
            AlertDialog.Show("private void Button_Click(object sender, RoutedEventArgs e)\r\npublic abstract class OhmTheme : ResourceDictionary\r\n" + assembly.GetName().Version, "", MessageButton.OK, MessageImage.Question);
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

    public static class OhmhemeCollection
    {
        private sealed class OhmVS2019Blue : OhmTheme
        {
            public override string Name => "2019 Blue";

            public override IEnumerable<string> ThemeResources
            {
                get
                {
                    yield return "/OhmStudio.UI;component/Themes/VisualStudio2019/BlueTheme.xaml";
                    yield return "/OhmStudio.UI;component/AvalonDockThemes/VisualStudio2019/BlueTheme.xaml";
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
                    yield return "/OhmStudio.UI;component/Themes/VisualStudio2019/DarkTheme.xaml";
                    yield return "/OhmStudio.UI;component/AvalonDockThemes/VisualStudio2019/DarkTheme.xaml";
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
                    yield return "/OhmStudio.UI;component/Themes/VisualStudio2019/LightTheme.xaml";
                    yield return "/OhmStudio.UI;component/AvalonDockThemes/VisualStudio2019/LightTheme.xaml";
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
                    yield return "/OhmStudio.UI;component/Themes/VisualStudio2022/BlueTheme.xaml";
                    yield return "/OhmStudio.UI;component/AvalonDockThemes/VisualStudio2022/BlueTheme.xaml";
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
                    yield return "/OhmStudio.UI;component/Themes/VisualStudio2022/DarkTheme.xaml";
                    yield return "/OhmStudio.UI;component/AvalonDockThemes/VisualStudio2022/DarkTheme.xaml";
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
                    yield return "/OhmStudio.UI;component/Themes/VisualStudio2022/LightTheme.xaml";
                    yield return "/OhmStudio.UI;component/AvalonDockThemes/VisualStudio2022/LightTheme.xaml";
                }
            }
        }

        public static List<OhmTheme> AllThemes { get; }

        static OhmhemeCollection()
        {
            AllThemes = new List<OhmTheme>(){
                new OhmVS2022Blue(),
                new OhmVS2022Dark(),
                new OhmVS2022Light(),
                new OhmVS2019Blue(),
                new OhmVS2019Dark(),
                new OhmVS2019Light()
            };
        }
    }

    public class OhmXamlUIResource : ResourceDictionary
    {
        public OhmXamlUIResource()
        {
            instance = this;
            theme = OhmhemeCollection.AllThemes.FirstOrDefault();
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