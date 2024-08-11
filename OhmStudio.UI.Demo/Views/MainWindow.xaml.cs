using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Search;
using OhmStudio.UI.Controls;
using OhmStudio.UI.Demo.ViewModels;
using OhmStudio.UI.Messaging;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Demo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ChromeWindow
    {
        public MainWindow()
        {
            //if (new UserLoginWindow().SetOwner().ShowDialog() != true)
            //{
            //    Environment.Exit(Environment.ExitCode);
            //}
            InitializeComponent();

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
                        _mainViewModel.ZoomOut();
                    }
                    else
                    {
                        _mainViewModel.ZoomIn();
                    }
                }
            };
        }

        private XmlFoldingStrategy _xmlFoldingStrategy = new XmlFoldingStrategy();
        private BraceFoldingStrategy _braceFoldingStrategy = new BraceFoldingStrategy();
        private MainViewModel _mainViewModel = ViewModelLocator.GetInstance<MainViewModel>();

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _mainViewModel.Employees)
            {
                item.IsExpanded = true;
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (var item in _mainViewModel.Employees)
            {
                item.IsExpanded = false;
            }
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
            _mainViewModel.Employee.Value = 100;
            AlertDialog.Show(_mainViewModel.Employee.InstrumentType.ToString());
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            StatusManager.IsRunning = !StatusManager.IsRunning;
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
            _mainViewModel.Can = !_mainViewModel.Can;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            _mainViewModel.SelectedObject = this;
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            _mainViewModel.DocumentWrapPanelIsWrap = !_mainViewModel.DocumentWrapPanelIsWrap;
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

        private void Button_Click_15(object sender, RoutedEventArgs e)
        { 
            rollBox.ItemsSource = new ObservableCollection<UIElement> { new Image() { Source = new BitmapImage(new Uri("https://pic1.zhimg.com/v2-ecac0aedda57bffecbbe90764828a825_r.jpg?source=1940ef5c")) }, new Button() { Content = "This is a new Button" } };
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _mainViewModel.StatusBarContent = $"DataGrid当前编辑元素：{e.Column.GetCellContent(e.Row)}";
        }

        private void Button_Click_16(object sender, RoutedEventArgs e)
        {
            stackPanel.SaveAsImage("1.png", ImageType.Png);
        }

        private void Button_Click_17(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.EmployeeSelectedItems != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in _mainViewModel.EmployeeSelectedItems.OfType<Employee>())
                {
                    stringBuilder.Append(item.Name + Environment.NewLine);
                }
                AlertDialog.Show(stringBuilder.ToString());
            }
        }

        private void Button_Click_18(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.CurrentUserPermission != null)
            {
                var userPermission = UserPermission.None;
                foreach (var item in _mainViewModel.CurrentUserPermission.OfType<UserPermission>())
                {
                    userPermission |= item;
                }
                AlertDialog.Show(userPermission.ToString());
            }
        }
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