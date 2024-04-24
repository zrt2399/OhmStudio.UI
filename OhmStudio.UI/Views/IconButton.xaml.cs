using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OhmStudio.UI.Views
{
    [DesignTimeVisible(false)]//在工具箱中 隐藏该窗口
    public partial class IconButton : ContentControl
    {
        public IconButton()
        {
            InitializeComponent();
            button.Click += delegate
            {
                RaiseEvent(new RoutedEventArgs(ClickEvent, this));
            };
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(IconButton));

        public ImageSource Icon
        {
            set => SetValue(IconProperty, value);
            get => (ImageSource)GetValue(IconProperty);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(IconButton));

        public ICommand Command
        {
            set => SetValue(CommandProperty, value);
            get => (ICommand)GetValue(CommandProperty);
        }

        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(IconButton));

        public event RoutedEventHandler Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }
    }
}