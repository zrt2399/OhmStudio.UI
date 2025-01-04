using System.Windows;
using System.Windows.Controls;

namespace OhmStudio.UI.Controls
{
    public class PasswordTextBox : Control, ITextChanged
    {
        private TextBox PART_TextBox;
        private PasswordBox PART_PasswordBox;
        public event TextChangedEventHandler TextChanged;

        string ITextChanged.Text => Password;

        public PasswordTextBox()
        {
            GotFocus += PasswordTextBox_GotFocus;
        }

        static PasswordTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordTextBox), new FrameworkPropertyMetadata(typeof(PasswordTextBox)));
        }

        public const char DefaultPasswordChar = '●';

        public static readonly DependencyProperty PasswordCharProperty =
            DependencyProperty.Register(nameof(PasswordChar), typeof(char), typeof(PasswordTextBox), new PropertyMetadata(DefaultPasswordChar));

        /// <summary>
        /// 获取或设置的掩码字符 <see cref="PasswordTextBox"/>。
        /// </summary>
        /// <returns>回显在用户输入到文本时的掩码字符 <see cref="PasswordTextBox"/>。默认值是项目符号字符 (●)。</returns>
        public char PasswordChar
        {
            get => (char)GetValue(PasswordCharProperty);
            set => SetValue(PasswordCharProperty, value);
        }

        /// <summary>
        /// 控制PasswordTextBox显示或者隐藏CheckBox眼睛，来控制是否可以显示和隐藏密码。
        /// </summary>
        public bool CanShowPassword
        {
            get => (bool)GetValue(CanShowPasswordProperty);
            set => SetValue(CanShowPasswordProperty, value);
        }

        // Using a DependencyProperty as the backing store for CanShowPassword.  This enables animation,styling,binding,etc...
        public static readonly DependencyProperty CanShowPasswordProperty =
            DependencyProperty.Register(nameof(CanShowPassword), typeof(bool), typeof(PasswordTextBox), new PropertyMetadata(true, (sender, e) =>
            {
                if (!(bool)e.NewValue && sender is PasswordTextBox passwordTextBox)
                {
                    passwordTextBox.IsChecked = false;
                }
            }));

        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(PasswordTextBox), new PropertyMetadata(default(int)));

        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        /// <summary>
        /// 控制TextBox显示或者隐藏----TextBox来显示明文。
        /// </summary>
        internal Visibility TbVisibility
        {
            get => (Visibility)GetValue(TbVisibilityProperty);
            set => SetValue(TbVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for TbVisibility.  This enables animation,styling,binding,etc...
        internal static readonly DependencyProperty TbVisibilityProperty =
            DependencyProperty.Register("TbVisibility", typeof(Visibility), typeof(PasswordTextBox), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 控制PassworBox显示或者隐藏----PasswordBox控件来显密文。
        /// </summary>
        internal Visibility PwVisibility
        {
            get => (Visibility)GetValue(PwVisibilityProperty);
            set => SetValue(PwVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for PwVisibility.  This enables animation,styling,binding,etc...
        internal static readonly DependencyProperty PwVisibilityProperty =
            DependencyProperty.Register("PwVisibility", typeof(Visibility), typeof(PasswordTextBox));

        /// <summary>
        /// 和"眼睛"进行绑定。
        /// </summary>
        internal bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        // Using a DependencyProperty as the backing store for Check.  This enables animation,styling,binding,etc...
        internal static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(PasswordTextBox), new PropertyMetadata((sender, e) =>
            {
                if (sender is PasswordTextBox passwordTextBox)
                {
                    if ((bool)e.NewValue)
                    {
                        passwordTextBox.TbVisibility = Visibility.Visible;
                        passwordTextBox.PwVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        passwordTextBox.TbVisibility = Visibility.Collapsed;
                        passwordTextBox.PwVisibility = Visibility.Visible;
                    }
                    passwordTextBox.Focus();
                }
            }));

        /// <summary>
        /// 点击图标"x",使密码框清空。
        /// </summary>
        internal bool IsCleared
        {
            get => (bool)GetValue(IsClearedProperty);
            set => SetValue(IsClearedProperty, value);
        }
        internal static readonly DependencyProperty IsClearedProperty =
            DependencyProperty.Register("IsCleared", typeof(bool), typeof(PasswordTextBox), new PropertyMetadata((sender, e) =>
            {
                if (sender is PasswordTextBox passwordTextBox)
                {
                    passwordTextBox.Password = string.Empty;
                    passwordTextBox.Focus();
                }
            }));

        /// <summary>
        /// 获取或设置当前保留的密码 <see cref="PasswordTextBox"/>。
        /// </summary>
        /// <returns>表示当前保留的密码的字符串 <see cref="PasswordTextBox"/>。默认值为 <see cref="string.Empty"/>。</returns>
        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation,styling,binding,etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(PasswordTextBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty ClearButtonIsTabStopProperty =
            DependencyProperty.Register(nameof(ClearButtonIsTabStop), typeof(bool), typeof(PasswordTextBox));

        public bool ClearButtonIsTabStop
        {
            get => (bool)GetValue(ClearButtonIsTabStopProperty);
            set => SetValue(ClearButtonIsTabStopProperty, value);
        }

        public static readonly DependencyProperty ShowPasswordButtonIsTabStopProperty =
            DependencyProperty.Register(nameof(ShowPasswordButtonIsTabStop), typeof(bool), typeof(PasswordTextBox));

        public bool ShowPasswordButtonIsTabStop
        {
            get => (bool)GetValue(ShowPasswordButtonIsTabStopProperty);
            set => SetValue(ShowPasswordButtonIsTabStopProperty, value);
        }

        public override void OnApplyTemplate()
        {
            if (PART_TextBox != null)
            {
                PART_TextBox.TextChanged -= PART_TextBox_TextChanged;
            }
            base.OnApplyTemplate();
            PART_TextBox = GetTemplateChild("PART_TextBox") as TextBox;
            PART_PasswordBox = GetTemplateChild("PART_PasswordBox") as PasswordBox;
            PART_TextBox.TextChanged += PART_TextBox_TextChanged;
        }

        private void PART_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, e);
        }

        private void PasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!e.Handled && PART_TextBox != null && PART_PasswordBox != null)
            {
                if (Equals(e.OriginalSource, this))
                {
                    if (TbVisibility == Visibility.Visible)
                    {
                        PART_TextBox.Focus();
                        e.Handled = true;
                    }
                    else if (PwVisibility == Visibility.Visible)
                    {
                        PART_PasswordBox.Focus();
                        e.Handled = true;
                    }
                }
            }
        }
    }
}