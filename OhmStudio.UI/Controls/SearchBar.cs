using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace OhmStudio.UI.Controls
{
    public interface ITextChanged
    {
        string Text { get; }

        event TextChangedEventHandler TextChanged;
    }

    [ContentProperty(nameof(SearchedContent))]
    public class SearchBar : Control, ICommandSource, ITextChanged
    {
        private TextBox PART_TextBox;
        public event TextChangedEventHandler TextChanged;

        public SearchBar()
        {
            GotFocus += SearchBar_GotFocus;
        }

        static SearchBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBar), new FrameworkPropertyMetadata(typeof(SearchBar)));
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(SearchBar), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty IsRealTimeProperty =
            DependencyProperty.Register(nameof(IsRealTime), typeof(bool), typeof(SearchBar));

        public static readonly DependencyProperty IsDropDownOpenOnTextChangedProperty =
            DependencyProperty.Register(nameof(IsDropDownOpenOnTextChanged), typeof(bool), typeof(SearchBar));

        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(SearchBar), new PropertyMetadata(TextWrapping.NoWrap));

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(SearchBar), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SearchedContentProperty =
            DependencyProperty.Register(nameof(SearchedContent), typeof(object), typeof(SearchBar));

        public static readonly DependencyProperty SearchedContentTemplateProperty =
            DependencyProperty.Register(nameof(SearchedContentTemplate), typeof(DataTemplate), typeof(SearchBar));

        public static readonly DependencyProperty SearchedContentTemplateSelectorProperty =
            DependencyProperty.Register(nameof(SearchedContentTemplateSelector), typeof(DataTemplateSelector), typeof(SearchBar));

        public static readonly DependencyProperty MinDropDownWidthProperty =
           DependencyProperty.Register(nameof(MinDropDownWidth), typeof(double), typeof(SearchBar), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register(nameof(MaxDropDownHeight), typeof(double), typeof(SearchBar), new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(SearchBar));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(SearchBar));

        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register(nameof(CommandTarget), typeof(IInputElement), typeof(SearchBar));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// 是否实时搜索。
        /// </summary>
        public bool IsRealTime
        {
            get => (bool)GetValue(IsRealTimeProperty);
            set => SetValue(IsRealTimeProperty, value);
        }
 
        public bool IsDropDownOpenOnTextChanged
        {
            get => (bool)GetValue(IsDropDownOpenOnTextChangedProperty);
            set => SetValue(IsDropDownOpenOnTextChangedProperty, value);
        }

        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public object SearchedContent
        {
            get => GetValue(SearchedContentProperty);
            set => SetValue(SearchedContentProperty, value);
        }

        public DataTemplate SearchedContentTemplate
        {
            get => (DataTemplate)GetValue(SearchedContentTemplateProperty);
            set => SetValue(SearchedContentTemplateProperty, value);
        }

        public DataTemplateSelector SearchedContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(SearchedContentTemplateSelectorProperty);
            set => SetValue(SearchedContentTemplateSelectorProperty, value);
        }

        [TypeConverter(typeof(LengthConverter))]
        public double MinDropDownWidth
        {
            get => (double)GetValue(MinDropDownWidthProperty);
            set => SetValue(MinDropDownWidthProperty, value);
        }

        [TypeConverter(typeof(LengthConverter))]
        public double MaxDropDownHeight
        {
            get => (double)GetValue(MaxDropDownHeightProperty);
            set => SetValue(MaxDropDownHeightProperty, value);
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
            if (PART_TextBox != null)
            {
                PART_TextBox.TextChanged -= PART_TextBox_TextChanged;
            }
            base.OnApplyTemplate();
            PART_TextBox = GetTemplateChild("PART_TextBox") as TextBox;
            PART_TextBox.TextChanged += PART_TextBox_TextChanged;
        }
 
        private void PART_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, e);
            if (IsRealTime)
            {
                if (Command is RoutedCommand routedCommand)
                {
                    routedCommand.Execute(CommandParameter, CommandTarget);
                }
                else
                {
                    Command?.Execute(CommandParameter);
                }
            }

            if (IsDropDownOpenOnTextChanged)
            {
                IsDropDownOpen = true;
            }
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