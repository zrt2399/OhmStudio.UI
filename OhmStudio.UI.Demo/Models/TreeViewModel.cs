using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OhmStudio.UI.Commands;
using OhmStudio.UI.Demo.ViewModels;
using OhmStudio.UI.Helpers;
using OhmStudio.UI.Messaging;
using OhmStudio.UI.Mvvm;
using PropertyChanged;

namespace OhmStudio.UI.Demo.Models
{
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
                    string newPath = Path.Combine(Directory.GetParent(oldPath)?.FullName ?? string.Empty, treeViewModel.Header);
                    string filterate = new string(Path.GetInvalidFileNameChars());

                    if (Regex.IsMatch(treeViewModel.Header, $"[{filterate}]"))
                    {
                        treeViewModel.Header = treeViewModel.BeforeEditingName;
                        MessageTip.ShowWarning($"文件夹和文件命名不能包含以下字符：\r\n{filterate}");
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
                    MessageTip.ShowError("重命名异常：" + ex.Message);
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
                        try
                        {
                            foreach (var child in Children)
                            {
                                LoadSubDirectory(child, child.FullPath, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            AlertDialog.ShowError(ex.Message);
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
}