using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OhmStudio.UI.Controls
{
    public interface ITextChanged
    {
        string Text { get; }

        event DependencyPropertyChangedEventHandler TextChanged;
    }

    public class SearchBar : Control, ICommandSource, ITextChanged
    {
        public SearchBar()
        {
            GotFocus += SearchBar_GotFocus;
        }

        static SearchBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBar), new FrameworkPropertyMetadata(typeof(SearchBar)));
        }

        TextBox PART_TextBox;
        public event DependencyPropertyChangedEventHandler TextChanged;

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(SearchBar), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) =>
            {
                if (sender is SearchBar searchBar)
                {
                    searchBar.TextChanged?.Invoke(sender, e);
                    if (searchBar.IsRealTime)
                    {
                        searchBar.Command?.Execute(searchBar.CommandParameter);
                    }
                }
            }));

        /// <summary>
        /// 是否实时搜索。
        /// </summary>
        public bool IsRealTime
        {
            get => (bool)GetValue(IsRealTimeProperty);
            set => SetValue(IsRealTimeProperty, value);
        }

        public static readonly DependencyProperty IsRealTimeProperty =
            DependencyProperty.Register(nameof(IsRealTime), typeof(bool), typeof(SearchBar));

        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(SearchBar), new PropertyMetadata(TextWrapping.NoWrap));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(SearchBar));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(SearchBar));

        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(nameof(CommandTarget), typeof(IInputElement), typeof(SearchBar));

        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_TextBox = GetTemplateChild("PART_TextBox") as TextBox;
        }

        private void SearchBar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!e.Handled && PART_TextBox != null)
            {
                if (Equals(e.OriginalSource, this))
                {
                    PART_TextBox.Focus();
                    e.Handled = true;
                }
            }
        }
    }
}