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
using OhmStudio.UI.Helpers;
using OhmStudio.UI.Messaging;
using OhmStudio.UI.Utilities;

namespace OhmStudio.UI.Demo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ChromeWindow
    {
        private XmlFoldingStrategy _xmlFoldingStrategy = new XmlFoldingStrategy();
        private BraceFoldingStrategy _braceFoldingStrategy = new BraceFoldingStrategy();
        private MainViewModel _mainViewModel = ViewModelLocator.GetInstance<MainViewModel>();

        public MainWindow()
        {
            //if (new UserLoginWindow().SetOwner().ShowDialog() != true)
            //{
            //    Environment.Exit(Environment.ExitCode);
            //}
            InitializeComponent();

            textEditorxmltsql.Text = "select * from ATE_TEST_OS where SN is not null";
            textEditorxmljson.Text = "{\r\n  \"runtimeOptions\": {\r\n    \"tfm\": \"net6.0\",\r\n    \"frameworks\": [\r\n      {\r\n        \"name\": \"Microsoft.NETCore.App\",\r\n        \"version\": \"6.0.0\"\r\n      },\r\n      {\r\n        \"name\": \"Microsoft.WindowsDesktop.App\",\r\n        \"version\": \"6.0.0\"\r\n      }\r\n    ]\r\n  }\r\n}";
            textEditorcs.Text = "using System;\r\n\r\nclass Program\r\n{\r\n    static void Main()\r\n    {\r\n        Console.WriteLine(\"Hello World\");\r\n    }\r\n}";
            textEditorcpp.Text = "#include <iostream>\r\n\r\nint main() \r\n{\r\n    std::cout << \"Hello World\" << std::endl;\r\n    return 0;\r\n}";
            textEditorxml.Text = "<Project Sdk=\"Microsoft.NET.Sdk\">\r\n\r\n\t<PropertyGroup>\r\n\t\t<OutputType>WinExe</OutputType>\r\n\t\t<TargetFramework>net6.0-windows</TargetFramework>\r\n\t\t<UseWPF>true</UseWPF>\r\n\t</PropertyGroup>\r\n \r\n</Project>";

            var searchPanel = SearchPanel.Install(textEditorxml);
            searchPanel.MarkerBrush = "#BEAA46".ToSolidColorBrush();

            var xmlFoldingManager = FoldingManager.Install(textEditorxml.TextArea);
            var csFoldingManager = FoldingManager.Install(textEditorcs.TextArea);
            var cppFoldingManager = FoldingManager.Install(textEditorcpp.TextArea);
            var jsonFoldingManager = FoldingManager.Install(textEditorxmljson.TextArea);
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += delegate
            {
                _xmlFoldingStrategy.UpdateFoldings(xmlFoldingManager, textEditorxml.Document);
                _braceFoldingStrategy.UpdateFolding(csFoldingManager, textEditorcs.Document);
                _braceFoldingStrategy.UpdateFolding(cppFoldingManager, textEditorcpp.Document);
                _braceFoldingStrategy.UpdateFolding(jsonFoldingManager, textEditorxmljson.Document);
            };
            dispatcherTimer.Start();
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);
            if (KeyboardHelper.IsCtrlKeyDown)
            {
                if (e.Delta > 0)
                {
                    _mainViewModel.ZoomOut();
                }
                else
                {
                    _mainViewModel.ZoomIn();
                }
                e.Handled = true;
            }
        }

        protected override async void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Tab)
            {
                await Task.Delay(100);
                _mainViewModel.StatusBarContent = $"当前获取焦点元素：{Keyboard.FocusedElement}";
            }
        }

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var version = Assembly.GetAssembly(typeof(ChromeWindow)).GetName().Version.ToString();
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 1; i <= 100; i++)
            {
                stringBuilder.Append("Item:" + i + Environment.NewLine);
            }

            var result = AlertDialog.Show(stringBuilder.ToString(), version, MessageBoxButton.YesNoCancel, MessageBoxImage.Error);
            MessageBox.Show("点击了" + result, version);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                AlertDialog.ShowError("AlertDialog.ShowError(In the Task)", "Demo");
            });
            //Messenger.Default.Send("AlertDialog.Show", MessageType.AlertDialog);
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
            StatusManager.Current.IsRunning = !StatusManager.Current.IsRunning;
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
            rollBox.ItemsSource = new ObservableCollection<object> { new Image() { Source = new BitmapImage(new Uri("https://pic1.zhimg.com/v2-ecac0aedda57bffecbbe90764828a825_r.jpg?source=1940ef5c")) }, new Button() { Content = "This is a new Button" }, "string" };
        }

        private void Button_Click_16(object sender, RoutedEventArgs e)
        {
            rollBox.ItemsSource = null;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _mainViewModel.StatusBarContent = $"DataGrid当前编辑元素：{e.Column.GetCellContent(e.Row)}";
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

        private void Button_Click_19(object sender, RoutedEventArgs e)
        {
            workflowEditor.BringIntoView(new Point());
        }

        private Point _dragStartPoint;
        private void SourceListBox_MouseMove(object sender, MouseEventArgs e)
        {
            // 如果没有按住鼠标左键，返回
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            Vector diff = _dragStartPoint - e.GetPosition(null);

            // 如果拖动距离很小，则不处理拖动
            if (Math.Abs(diff.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(diff.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            // 获取当前选中的项
            var listBox = sender as ListBox;
            if (listBox.SelectedItem != null)
            {
                // 开始拖动
                DragDrop.DoDragDrop(listBox, listBox.SelectedItem, DragDropEffects.Move);
            }
        }

        // 记录鼠标按下的位置
        private void SourceListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void Button_Click_20(object sender, RoutedEventArgs e)
        {
            appHost.ExePath = null;
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

        public void UpdateFolding(FoldingManager manager, TextDocument document)
        {
            IEnumerable<NewFolding> newFolding = CreateNewFolding(document, out int firstErrorOffset);
            manager.UpdateFoldings(newFolding, firstErrorOffset);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFolding(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFolding(document);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFolding(ITextSource document)
        {
            List<NewFolding> newFolding = new List<NewFolding>();

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
                        newFolding.Add(new NewFolding(startOffset, i + 1));
                    }
                }
                else if (c is '\r' or '\n')
                {
                    lastNewLineOffset = i + 1;
                }
            }
            newFolding.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFolding;
        }
    }
}