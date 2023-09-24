using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OhmStudio.UI.Controls
{
    public class SearchBar : TextBox, ICommandSource
    {
        public SearchBar()
        {
            TextChanged += delegate
            {
                ShowMark = string.IsNullOrEmpty(Text) ? Visibility.Visible : Visibility.Collapsed;
            };
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(SearchBar));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(SearchBar));

        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(SearchBar));

        public static readonly DependencyProperty PlaceHolderProperty = DependencyProperty.Register("PlaceHolder", typeof(string), typeof(SearchBar), new PropertyMetadata("请输入搜索内容"));

        public static readonly DependencyProperty ShowMarkProperty = DependencyProperty.Register("ShowMark", typeof(Visibility), typeof(SearchBar), new PropertyMetadata(Visibility.Visible));
 
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

        public string PlaceHolder
        {
            get => GetValue(PlaceHolderProperty)?.ToString();
            set => SetValue(PlaceHolderProperty, value);
        }

        public Visibility ShowMark
        {
            get => (Visibility)GetValue(ShowMarkProperty);
            set => SetValue(ShowMarkProperty, value);
        }
    }
}