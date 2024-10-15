using System;
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
        /// Identifies the <see cref="ShowIcon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowIconProperty =
            DependencyProperty.Register(nameof(ShowIcon), typeof(bool),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="ShowTitle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowTitleProperty =
            DependencyProperty.Register(nameof(ShowTitle), typeof(bool),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="ShowTitleBar"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowTitleBarProperty =
            DependencyProperty.Register(nameof(ShowTitleBar), typeof(bool),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="ShowMinimizeButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowMinimizeButtonProperty =
            DependencyProperty.Register(nameof(ShowMinimizeButton), typeof(bool),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="ShowMaximizeButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowMaximizeButtonProperty =
            DependencyProperty.Register(nameof(ShowMaximizeButton), typeof(bool),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="ShowRestoreButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowRestoreButtonProperty =
            DependencyProperty.Register(nameof(ShowRestoreButton), typeof(bool),
                typeof(ChromeWindow), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="ShowCloseButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowCloseButtonProperty =
            DependencyProperty.Register(nameof(ShowCloseButton), typeof(bool),
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
        public bool ShowIcon
        {
            get => (bool)GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        /// <summary>
        /// Show or hide the window title in the title bar of the window.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool ShowTitle
        {
            get => (bool)GetValue(ShowTitleProperty);
            set => SetValue(ShowTitleProperty, value);
        }

        /// <summary>
        /// Show or hide the title bar of the window.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool ShowTitleBar
        {
            get => (bool)GetValue(ShowTitleBarProperty);
            set => SetValue(ShowTitleBarProperty, value);
        }

        /// <summary>
        /// Show or hide the minimize button in the title bar of the window.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool ShowMinimizeButton
        {
            get => (bool)GetValue(ShowMinimizeButtonProperty);
            set => SetValue(ShowMinimizeButtonProperty, value);
        }

        /// <summary>
        /// Show or hide the maximize button in the title bar of the window.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool ShowMaximizeButton
        {
            get => (bool)GetValue(ShowMaximizeButtonProperty);
            set => SetValue(ShowMaximizeButtonProperty, value);
        }

        /// <summary>
        /// Show or hide the restore button in the title bar of the window.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool ShowRestoreButton
        {
            get => (bool)GetValue(ShowRestoreButtonProperty);
            set => SetValue(ShowRestoreButtonProperty, value);
        }

        /// <summary>
        /// Show or hide the close button in the title bar of the window.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool ShowCloseButton
        {
            get => (bool)GetValue(ShowCloseButtonProperty);
            set => SetValue(ShowCloseButtonProperty, value);
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
            //Loaded += OnLoaded;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            InitializeGlowWindowBehaviorEx();
            InitializeWindowChromeEx();
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
                new Binding { Path = new PropertyPath(ShowMinimizeButtonProperty), Source = this });
            BindingOperations.SetBinding(behavior, WindowChromeBehavior.EnableMaxRestoreProperty,
                new Binding { Path = new PropertyPath(ShowMaximizeButtonProperty), Source = this });

            Interaction.GetBehaviors(this).Add(behavior);
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ChromeWindowAutomationPeer(this);
        }
    }
}