using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using OhmStudio.UI.Commands;

namespace OhmStudio.UI.Controls
{
    [ContentProperty(nameof(ItemsSource))]
    public class RollBox : Control
    {
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private ContentPresenter PART_CURR_Content;
        //private ContentControl PART_NEXT_Content;
        private ListBox PART_ListBox;
        private Button PART_PreviousButton;

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(ObservableCollection<object>), typeof(RollBox), new PropertyMetadata(new ObservableCollection<object>(), (sender, e) =>
            {
                RollBox rollBox = (RollBox)sender;
                if (e.OldValue is INotifyCollectionChanged oldCollectionChanged)
                {
                    oldCollectionChanged.CollectionChanged -= CollectionChanged;
                }
                if (e.NewValue is INotifyCollectionChanged newCollectionChanged)
                {
                    newCollectionChanged.CollectionChanged += CollectionChanged;
                }

                rollBox.UpdateListBoxItem();

                void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
                {
                    rollBox.UpdateListBoxItem();
                }
            }));

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(nameof(Interval), typeof(TimeSpan), typeof(RollBox), new PropertyMetadata(TimeSpan.FromSeconds(5), (sender, e) =>
            {
                if (sender is RollBox rollBox && e.NewValue is TimeSpan newValue)
                {
                    rollBox.dispatcherTimer.Interval = newValue;
                    rollBox.dispatcherTimer.Stop();
                    rollBox.dispatcherTimer.Start();
                }
            }));

        public ObservableCollection<object> ItemsSource
        {
            get => (ObservableCollection<object>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// 获取或设置滚动的间隔，默认值为5s。
        /// </summary>
        public TimeSpan Interval
        {
            get => (TimeSpan)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        public ICommand PreviousCommand { get; }

        public ICommand NextCommand { get; }

        public int Index
        {
            get => PART_ListBox?.SelectedIndex ?? -1;
            set
            {
                //preindex = _index;
                if (ItemsSource == null || ItemsSource.Count < 1)
                {
                    return;
                }
                if (value >= ItemsSource.Count)
                {
                    value = 0;
                }
                else if (value < 0)
                {
                    value = ItemsSource.Count - 1;
                }
                if (PART_ListBox != null)
                {
                    PART_ListBox.SelectedIndex = value;
                }
            }
        }

        public RollBox()
        {
            PreviousCommand = new RelayCommand(() => Index--);
            NextCommand = new RelayCommand(() => Index++);
            GotFocus += RollBox_GotFocus;
        }

        static RollBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RollBox), new FrameworkPropertyMetadata(typeof(RollBox)));
        }

        ~RollBox()
        {
            dispatcherTimer?.Stop();
            dispatcherTimer = null;
        }

        private void AnimationStart()
        {
            if (PART_CURR_Content == null /*|| PART_NEXT_Content == null*/ || PART_ListBox == null)
            {
                return;
            }
            if (PART_CURR_Content.Content == null)
            {
                PART_CURR_Content.Content = ItemsSource[Index];
                return;//首次不需要动画
            }
            dispatcherTimer.Stop();
            dispatcherTimer.Start();//重新开始计时

            //System.Diagnostics.Debug.WriteLine($"next{Index}  curr{preindex}");
            //PART_NEXT_Content.Content = Items[Index];
            //PART_CURR_Content.Content = Items[preindex];
            //if (ItemsSource.Count > 0 && SelectedIndex < 0)
            //{
            //    Index = 0;
            //}

            PART_CURR_Content.Content = ItemsSource[Index];

            //bool isNext = Index > preindex;
            //ThicknessAnimation Curr_marginAnimation = new ThicknessAnimation();
            //Curr_marginAnimation.From = new Thickness(0);
            //Curr_marginAnimation.To = new Thickness(isNext ? -ActualWidth : ActualWidth, 0, isNext ? ActualWidth : -ActualWidth, 0);
            //Curr_marginAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));

            //ThicknessAnimation Next_marginAnimation = new ThicknessAnimation(); 
            //Next_marginAnimation.From = new Thickness(isNext ? ActualWidth : -ActualWidth, 0, isNext ? -ActualWidth : ActualWidth, 0);
            //Next_marginAnimation.To = new Thickness(0);
            //Next_marginAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));

            //PART_NEXT_Content.BeginAnimation(ContentControl.MarginProperty, Next_marginAnimation);
            //PART_CURR_Content.BeginAnimation(ContentControl.MarginProperty, Curr_marginAnimation);

            //DoubleAnimation currentAnimation = new DoubleAnimation();
            //currentAnimation.From = 1;
            //currentAnimation.To = 0;
            //currentAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(400));

            DoubleAnimation nextAnimation = new DoubleAnimation();
            nextAnimation.From = 0;
            nextAnimation.To = 1;
            nextAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(400));

            //PART_NEXT_Content.BeginAnimation(OpacityProperty, nextAnimation);
            //PART_CURR_Content.BeginAnimation(OpacityProperty, currentAnimation);

            PART_CURR_Content.BeginAnimation(OpacityProperty, nextAnimation);
        }

        /// <summary>
        /// <see cref="OnApplyTemplate"/> 要比XAML赋值、构造函数晚，在第一次Loaded时触发。
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (PART_ListBox != null)
            {
                PART_ListBox.SelectionChanged -= PART_ListBox_SelectionChanged;
            }
            base.OnApplyTemplate();
            PART_CURR_Content = GetTemplateChild("PART_CURR_Content") as ContentPresenter;
            //PART_NEXT_Content = GetTemplateChild("PART_NEXT_Content") as ContentControl;
            PART_ListBox = GetTemplateChild("PART_ListBox") as ListBox;
            PART_PreviousButton = GetTemplateChild("PART_PreviousButton") as Button;
            PART_ListBox.SelectionChanged += PART_ListBox_SelectionChanged;
            //Binding binding = new Binding();
            //binding.Path = new PropertyPath(nameof(Index));
            //binding.Source = this;
            //binding.Mode = BindingMode.TwoWay;
            //PART_ListBox.SetBinding(Selector.SelectedIndexProperty, binding);

            dispatcherTimer.Interval = Interval;
            dispatcherTimer.Tick -= DispatcherTimer_Tick;
            dispatcherTimer.Tick += DispatcherTimer_Tick;

            UpdateListBoxItem();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Index++;
        }

        private void PART_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox.SelectedIndex >= 0)
            {
                AnimationStart();
            }
        }

        private void UpdateListBoxItem()
        {
            if (PART_ListBox == null || PART_CURR_Content == null)
            {
                return;
            }
            dispatcherTimer.Stop();
            PART_ListBox.Items.Clear();
            PART_CURR_Content.Content = null;
            if (ItemsSource == null || ItemsSource.Count == 0)
            {
                return;
            }
            for (int i = 0; i < ItemsSource.Count; i++)
            {
                PART_ListBox.Items.Add(new ListBoxItem());
            }
            Index = 0;
            dispatcherTimer.Start();
        }

        private void RollBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!e.Handled && PART_CURR_Content != null /*&& PART_NEXT_Content != null*/)
            {
                if (Equals(e.OriginalSource, this))
                {
                    //UIElement uIElement;
                    //if ((uIElement = PART_CURR_Content.Content.GetFirstFocusable()) != null)
                    //{
                    //    uIElement.Focus();
                    //    e.Handled = true;
                    //}
                    //else if ((uIElement = GetFirstFocusable(PART_NEXT_Content.Content)) != null)
                    //{
                    //    uIElement.Focus();
                    //    e.Handled = true;
                    //}
                    if (PART_PreviousButton != null)
                    {
                        PART_PreviousButton.Focus();
                        e.Handled = true;
                    }
                }
            }
        }
    }
}