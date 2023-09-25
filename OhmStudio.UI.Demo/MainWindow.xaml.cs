using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using OhmStudio.UI.Controls;
using OhmStudio.UI.PublicMethod;

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

        private double currentFontSize = (double)Application.Current.Resources[GlobalFontSize];
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var assembly = Assembly.GetAssembly(typeof(CustomChromeWindow));
            MessageTip.Show("private void Button_Click(object sender, RoutedEventArgs e)\r\npublic abstract class OhmTheme : ResourceDictionary\r\n" + assembly.GetName().Version);
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
                    //yield return "/OhmStudio;component/Themes/BlueTheme.xaml";
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
                    //yield return "/OhmStudio;component/Themes/DarkTheme.xaml";
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
                    //yield return "/OhmStudio;component/Themes/LightTheme.xaml";
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
                    //yield return "/OhmStudio;component/Themes/BlueTheme.xaml";
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
                    //yield return "/OhmStudio;component/Themes/DarkTheme.xaml";
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
                    //yield return "/OhmStudio;component/Themes/LightTheme.xaml";
                }
            }
        }

        public static List<OhmTheme> AllThemes { get; }

        static OhmhemeCollection()
        {
            AllThemes = new List<OhmTheme>(){
                //new OhmVS2019Blue(),
                //new OhmVS2019Dark(),
                //new OhmVS2019Light(),
                new OhmVS2022Blue(),
                new OhmVS2022Dark(),
                new OhmVS2022Light()
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