using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using OhmStudio.UI.Commands;
using OhmStudio.UI.Helpers;

namespace OhmStudio.UI.Controls
{
    public class PathPicker : Control
    {
        private TextBox PART_TextBox;

        static PathPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathPicker), new FrameworkPropertyMetadata(typeof(PathPicker)));
        }

        public PathPicker()
        {
            BrowseCommand = new RelayCommand(Browse);
            ExploreCommand = new RelayCommand(Explore);
            OpenCommand = new RelayCommand(Open);
            GotFocus += PathPicker_GotFocus;
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(PathPicker), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty InitialDirectoryProperty =
            DependencyProperty.Register(nameof(InitialDirectory), typeof(string), typeof(PathPicker), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty DefaultExtProperty =
            DependencyProperty.Register(nameof(DefaultExt), typeof(string), typeof(PathPicker), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty MultiselectProperty =
            DependencyProperty.Register(nameof(Multiselect), typeof(bool), typeof(PathPicker));

        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register(nameof(FileName), typeof(string), typeof(PathPicker), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty FileNamesProperty =
            DependencyProperty.Register(nameof(FileNames), typeof(string[]), typeof(PathPicker), new FrameworkPropertyMetadata(Array.Empty<string>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnFileNamesChanged));

        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register(nameof(Filter), typeof(string), typeof(PathPicker), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty UseFolderDialogProperty =
            DependencyProperty.Register(nameof(UseFolderDialog), typeof(bool), typeof(PathPicker));

        public static readonly DependencyProperty UseSaveDialogProperty =
            DependencyProperty.Register(nameof(UseSaveDialog), typeof(bool), typeof(PathPicker));

        public static readonly DependencyProperty BrowseButtonContentProperty =
            DependencyProperty.Register(nameof(BrowseButtonContent), typeof(object), typeof(PathPicker), new PropertyMetadata("  ...  "));

        public static readonly DependencyProperty ExploreButtonContentProperty =
            DependencyProperty.Register(nameof(ExploreButtonContent), typeof(object), typeof(PathPicker));

        public static readonly DependencyProperty OpenButtonContentProperty =
            DependencyProperty.Register(nameof(OpenButtonContent), typeof(object), typeof(PathPicker));

        public static readonly DependencyProperty BrowseButtonToolTipProperty =
            DependencyProperty.Register(nameof(BrowseButtonToolTip), typeof(object), typeof(PathPicker));

        public static readonly DependencyProperty ExploreButtonToolTipProperty =
            DependencyProperty.Register(nameof(ExploreButtonToolTip), typeof(object), typeof(PathPicker));

        public static readonly DependencyProperty OpenButtonToolTipProperty =
            DependencyProperty.Register(nameof(OpenButtonToolTip), typeof(object), typeof(PathPicker));

        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(PathPicker), new PropertyMetadata(TextWrapping.NoWrap));

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(PathPicker), new PropertyMetadata(true));

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register(nameof(Spacing), typeof(Thickness), typeof(PathPicker), new PropertyMetadata(new Thickness(4, 0, 0, 0)));

        public ICommand BrowseCommand { get; }

        public ICommand ExploreCommand { get; }

        public ICommand OpenCommand { get; }

        /// <summary>
        /// Gets or sets the title of the file dialog.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string InitialDirectory
        {
            get => (string)GetValue(InitialDirectoryProperty);
            set => SetValue(InitialDirectoryProperty, value);
        }

        public string DefaultExt
        {
            get => (string)GetValue(DefaultExtProperty);
            set => SetValue(DefaultExtProperty, value);
        }

        public bool Multiselect
        {
            get => (bool)GetValue(MultiselectProperty);
            set => SetValue(MultiselectProperty, value);
        }

        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }

        public string[] FileNames
        {
            get => (string[])GetValue(FileNamesProperty);
            set => SetValue(FileNamesProperty, value);
        }

        public string Filter
        {
            get => (string)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        public bool UseFolderDialog
        {
            get => (bool)GetValue(UseFolderDialogProperty);
            set => SetValue(UseFolderDialogProperty, value);
        }

        public bool UseSaveDialog
        {
            get => (bool)GetValue(UseSaveDialogProperty);
            set => SetValue(UseSaveDialogProperty, value);
        }

        public object BrowseButtonContent
        {
            get => GetValue(BrowseButtonContentProperty);
            set => SetValue(BrowseButtonContentProperty, value);
        }

        public object ExploreButtonContent
        {
            get => GetValue(ExploreButtonContentProperty);
            set => SetValue(ExploreButtonContentProperty, value);
        }

        public object OpenButtonContent
        {
            get => GetValue(OpenButtonContentProperty);
            set => SetValue(OpenButtonContentProperty, value);
        }

        public object BrowseButtonToolTip
        {
            get => GetValue(BrowseButtonToolTipProperty);
            set => SetValue(BrowseButtonToolTipProperty, value);
        }

        public object ExploreButtonToolTip
        {
            get => GetValue(ExploreButtonToolTipProperty);
            set => SetValue(ExploreButtonToolTipProperty, value);
        }

        public object OpenButtonToolTip
        {
            get => GetValue(OpenButtonToolTipProperty);
            set => SetValue(OpenButtonToolTipProperty, value);
        }

        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public Thickness Spacing
        {
            get => (Thickness)GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }

        private static void OnFileNamesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PathPicker)d).OnFileNamesChanged((string[])e.NewValue);
        }

        protected virtual void OnFileNamesChanged(string[] fileNames)
        {
            FileName = fileNames == null || fileNames.Length == 0 ? string.Empty : string.Join("|", fileNames);
        }

        private void Browse()
        {
            if (UseFolderDialog)
            {
                var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                folderBrowserDialog.Description = Title;
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
                {
                    FileNames = new string[] { folderBrowserDialog.SelectedPath };
                }
            }
            else
            {
                FileDialog fileDialog;
                if (UseSaveDialog)
                {
                    fileDialog = new SaveFileDialog();
                }
                else
                {
                    fileDialog = new OpenFileDialog() { Multiselect = Multiselect, CheckFileExists = true };
                }
                fileDialog.Title = Title;
                fileDialog.CheckPathExists = true;
                fileDialog.InitialDirectory = InitialDirectory;
                fileDialog.Filter = Filter;
                fileDialog.DefaultExt = DefaultExt;

                if (fileDialog.ShowDialog() != true)
                {
                    return;
                }
                FileNames = Multiselect ? fileDialog.FileNames : new string[] { fileDialog.FileName };
            }
        }

        private void Explore()
        {
            if (FileNames?.Length > 0)
            {
                PathHelper.OpenFileLocation(FileNames.First());
            }
        }

        private void Open()
        {
            if (FileNames?.Length > 0)
            {
                PathHelper.OpenFlie(FileNames.First());
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_TextBox = GetTemplateChild("PART_TextBox") as TextBox;
        }

        private void PathPicker_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!e.Handled && PART_TextBox != null)
            {
                if (Equals(e.OriginalSource, this))
                {
                    PART_TextBox.Focus();
                    PART_TextBox.SelectAll();
                    e.Handled = true;
                }
            }
        }
    }
}