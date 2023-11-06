using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using ControlzEx.Behaviors;
using Microsoft.Xaml.Behaviors;
using OhmStudio.UI.Automation.Peers;

namespace OhmStudio.UI.Controls
{
    /// <summary>
    /// A custom window chrome and glow window.
    /// </summary>
    [TemplatePart(Name = PART_Icon, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_Title, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_TitleBar, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_LeftWindowCommands, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_LeftWindowCommands, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_MinimizeButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_MaximizeButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(ButtonBase))]
    public class ChromeWindow : Window
    {
        private const string PART_Icon = nameof(PART_Icon);
        private const string PART_Title = nameof(PART_Title);
        private const string PART_TitleBar = nameof(PART_TitleBar);
        private const string PART_LeftWindowCommands = nameof(PART_LeftWindowCommands);
        private const string PART_RightWindowCommands = nameof(PART_RightWindowCommands);
        private const string PART_MinimizeButton = nameof(PART_MinimizeButton);
        private const string PART_MaximizeButton = nameof(PART_MaximizeButton);
        private const string PART_CloseButton = nameof(PART_CloseButton);

        /// <summary>
        /// Identifies the <see cref="ActiveGlowBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveGlowBrushProperty =
            DependencyProperty.Register(nameof(ActiveGlowBrush), typeof(SolidColorBrush),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(ObjectBoxes.TransparentBox));

        /// <summary>
        /// Identifies the <see cref="InactiveGlowBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InactiveGlowBrushProperty =
            DependencyProperty.Register(nameof(InactiveGlowBrush), typeof(SolidColorBrush),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(ObjectBoxes.TransparentBox));

        /// <summary>
        /// Identifies the <see cref="LeftWindowCommands"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftWindowCommandsProperty =
            DependencyProperty.Register(nameof(LeftWindowCommands), typeof(FrameworkElement),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(ObjectBoxes.NullBox));

        /// <summary>
        /// Identifies the <see cref="RightWindowCommands"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightWindowCommandsProperty =
            DependencyProperty.Register(nameof(RightWindowCommands), typeof(FrameworkElement),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(ObjectBoxes.NullBox));

        /// <summary>
        /// Identifies the <see cref="IsShowIcon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsShowIconProperty =
            DependencyProperty.Register(nameof(IsShowIcon), typeof(bool),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsShowTitle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsShowTitleProperty =
            DependencyProperty.Register(nameof(IsShowTitle), typeof(bool),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsShowTitleBar"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsShowTitleBarProperty =
            DependencyProperty.Register(nameof(IsShowTitleBar), typeof(bool),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsShowMinimizeButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsShowMinimizeButtonProperty =
            DependencyProperty.Register(nameof(IsShowMinimizeButton), typeof(bool),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsShowMaximizeButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsShowMaximizeButtonProperty =
            DependencyProperty.Register(nameof(IsShowMaximizeButton), typeof(bool),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsShowRestoreButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsShowRestoreButtonProperty =
            DependencyProperty.Register(nameof(IsShowRestoreButton), typeof(bool),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsShowCloseButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsShowCloseButtonProperty =
            DependencyProperty.Register(nameof(IsShowCloseButton), typeof(bool),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="OnMaximizedPadding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OnMaximizedPaddingProperty =
            DependencyProperty.Register(nameof(OnMaximizedPadding), typeof(Thickness),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(ObjectBoxes.ThicknessBox));

        /// <summary>
        /// The glow brush of the window when activated.
        /// </summary>
        [Bindable(true)]
        public SolidColorBrush ActiveGlowBrush
        {
            get => (SolidColorBrush)GetValue(ActiveGlowBrushProperty);
            set => SetValue(ActiveGlowBrushProperty, value);
        }

        /// <summary>
        /// The glow brush of the window when deactivated.
        /// </summary>
        [Bindable(true)]
        public SolidColorBrush InactiveGlowBrush
        {
            get => (SolidColorBrush)GetValue(InactiveGlowBrushProperty);
            set => SetValue(InactiveGlowBrushProperty, value);
        }

        /// <summary>
        /// The left side window commands in the title bar of the window.
        /// </summary>
        [Bindable(true)]
        public FrameworkElement LeftWindowCommands
        {
            get => (FrameworkElement)GetValue(LeftWindowCommandsProperty);
            set => SetValue(LeftWindowCommandsProperty, value);
        }

        /// <summary>
        /// The right side window commands in the title bar of the window.
        /// </summary>
        [Bindable(true)]
        public FrameworkElement RightWindowCommands
        {
            get => (FrameworkElement)GetValue(RightWindowCommandsProperty);
            set => SetValue(RightWindowCommandsProperty, value);
        }

        /// <summary>
        /// Show or hide the window icon in the title bar of the window.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool IsShowIcon
        {
            get => (bool)GetValue(IsShowIconProperty);
            set => SetValue(IsShowIconProperty, value);
        }

        /// <summary>
        /// Show or hide the window title in the title bar of the window.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool IsShowTitle
        {
            get => (bool)GetValue(IsShowTitleProperty);
            set => SetValue(IsShowTitleProperty, value);
        }

        /// <summary>
        /// Show or hide the title bar of the window.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool IsShowTitleBar
        {
            get => (bool)GetValue(IsShowTitleBarProperty);
            set => SetValue(IsShowTitleBarProperty, value);
        }

        /// <summary>
        /// Show or hide the minimize button in the title bar of the window.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool IsShowMinimizeButton
        {
            get => (bool)GetValue(IsShowMinimizeButtonProperty);
            set => SetValue(IsShowMinimizeButtonProperty, value);
        }

        /// <summary>
        /// Show or hide the maximize button in the title bar of the window.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool IsShowMaximizeButton
        {
            get => (bool)GetValue(IsShowMaximizeButtonProperty);
            set => SetValue(IsShowMaximizeButtonProperty, value);
        }

        /// <summary>
        /// Show or hide the restore button in the title bar of the window.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool IsShowRestoreButton
        {
            get => (bool)GetValue(IsShowRestoreButtonProperty);
            set => SetValue(IsShowRestoreButtonProperty, value);
        }

        /// <summary>
        /// Show or hide the close button in the title bar of the window.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool IsShowCloseButton
        {
            get => (bool)GetValue(IsShowCloseButtonProperty);
            set => SetValue(IsShowCloseButtonProperty, value);
        }

        /// <summary>
        /// The padding of the <see cref="ChromeWindow"/> when maximized.
        /// </summary>
        [Bindable(true)]
        [Category("Layout")]
        public Thickness OnMaximizedPadding
        {
            get => (Thickness)GetValue(OnMaximizedPaddingProperty);
            set => SetValue(OnMaximizedPaddingProperty, value);
        }

        /// <summary>
		/// The static class constructor of the <see cref="ChromeWindow"/>.
		/// </summary>
        static ChromeWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChromeWindow), new FrameworkPropertyMetadata(typeof(ChromeWindow)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeWindow"/> class.
        /// </summary>
        public ChromeWindow()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            InitializeGlowWindowBehaviorEx();
            InitializeWindowChromeEx();
        }

        /// <summary>
        /// Initialize the <see cref="GlowWindowBehavior"/> for the <see cref="ChromeWindow"/>.
        /// </summary>
        private void InitializeGlowWindowBehaviorEx()
        {
            var behavior = new GlowWindowBehavior();

            BindingOperations.SetBinding(behavior, GlowWindowBehavior.GlowColorProperty,
                new Binding { Path = new PropertyPath("ActiveGlowBrush.Color"), Source = this });
            BindingOperations.SetBinding(behavior, GlowWindowBehavior.NonActiveGlowColorProperty,
                new Binding { Path = new PropertyPath("InactiveGlowBrush.Color"), Source = this });

            Interaction.GetBehaviors(this).Add(behavior);
        }

        /// <summary>
        /// Initialize the <see cref="WindowChromeBehavior"/> for the <see cref="ChromeWindow"/>.
        /// </summary>
        private void InitializeWindowChromeEx()
        {
            var behavior = new WindowChromeBehavior();

            BindingOperations.SetBinding(behavior, WindowChromeBehavior.EnableMinimizeProperty,
                new Binding { Path = new PropertyPath(IsShowMinimizeButtonProperty), Source = this });
            BindingOperations.SetBinding(behavior, WindowChromeBehavior.EnableMaxRestoreProperty,
                new Binding { Path = new PropertyPath(IsShowMaximizeButtonProperty), Source = this });

            Interaction.GetBehaviors(this).Add(behavior);
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ChromeWindowAutomationPeer(this);
        }
    }
}