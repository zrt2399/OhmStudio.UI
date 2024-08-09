using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Search;
using OhmStudio.UI.Commands;
using OhmStudio.UI.Controls;
using OhmStudio.UI.Demo.ViewModels;
using OhmStudio.UI.Helpers;
using OhmStudio.UI.Messaging;
using OhmStudio.UI.PublicMethods;
using PropertyChanged;

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

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            _mainViewModel.SelectedObject = this;
        }

        private void Button_Click_15(object sender, RoutedEventArgs e)
        {
            rollBox.ItemsSource = new ObservableCollection<UIElement> { new Image() { Source = new BitmapImage(new Uri("https://pic1.zhimg.com/v2-ecac0aedda57bffecbbe90764828a825_r.jpg?source=1940ef5c")) }, new Button() { Content = "This is a new Button" } };
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