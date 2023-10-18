using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OhmStudio.UI.Controls
{
    public class SearchBar : TextBox, ICommandSource
    {
        public SearchBar()
        {
            //TextChanged += delegate
            //{
            //    PlaceHolderVisibility = string.IsNullOrEmpty(Text) ? Visibility.Visible : Visibility.Collapsed;
            //};
          
        }
        //static SearchBar()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBar), new FrameworkPropertyMetadata(typeof(SearchBar)));
        //}
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(SearchBar));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(SearchBar));

        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(nameof(CommandTarget), typeof(IInputElement), typeof(SearchBar));
         
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public IInputElement CommandTarget
        {
            get => (IInputElement)GetValue(CommandTargetProperty);
            set => SetValue(CommandTargetProperty, value);
        }

        //public static readonly DependencyProperty PlaceHolderProperty = DependencyProperty.Register(nameof(PlaceHolder), typeof(string), typeof(SearchBar), new PropertyMetadata("请输入搜索内容"));

        //public static readonly DependencyProperty PlaceHolderForegroundProperty = DependencyProperty.Register(nameof(PlaceHolderForeground), typeof(Brush), typeof(SearchBar), new PropertyMetadata("#FF999999".ToSolidColorBrush()));

        //public static readonly DependencyProperty PlaceHolderOpacityProperty = DependencyProperty.Register(nameof(PlaceHolderOpacity), typeof(double), typeof(SearchBar), new PropertyMetadata(1d));

        //public static readonly DependencyProperty PlaceHolderMarginProperty = DependencyProperty.Register(nameof(PlaceHolderMargin), typeof(Thickness), typeof(SearchBar), new PropertyMetadata(new Thickness(2, 0, 2, 0)));

        //public static readonly DependencyProperty PlaceHolderVisibilityProperty = DependencyProperty.Register(nameof(PlaceHolderVisibility), typeof(Visibility), typeof(SearchBar), new PropertyMetadata(Visibility.Visible));

        //public string PlaceHolder
        //{
        //    get => (string)GetValue(PlaceHolderProperty);
        //    set => SetValue(PlaceHolderProperty, value);
        //}

        //public Brush PlaceHolderForeground
        //{
        //    get => (Brush)GetValue(PlaceHolderForegroundProperty);
        //    set => SetValue(PlaceHolderForegroundProperty, value);
        //}

        //public Brush PlaceHolderOpacity
        //{
        //    get => (Brush)GetValue(PlaceHolderOpacityProperty);
        //    set => SetValue(PlaceHolderOpacityProperty, value);
        //}

        //public Thickness PlaceHolderMargin
        //{
        //    get => (Thickness)GetValue(PlaceHolderMarginProperty);
        //    set => SetValue(PlaceHolderMarginProperty, value);
        //}

        //public Visibility PlaceHolderVisibility
        //{
        //    get => (Visibility)GetValue(PlaceHolderVisibilityProperty);
        //    set => SetValue(PlaceHolderVisibilityProperty, value);
        //}
    }
}