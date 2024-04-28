using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using OhmStudio.UI.Commands;
using OhmStudio.UI.PublicMethods;

namespace OhmStudio.UI.Controls
{
    //[TemplatePart(Name = "PART_CURR_Content", Type = typeof(ContentControl))]
    //[TemplatePart(Name = "PART_NEXT_Content", Type = typeof(ContentControl))]
    //[TemplatePart(Name = "PART_ListBox", Type = typeof(ListBox))]
    public class RollingBox : Control
    {
        public RollingBox()
        {
            PreviousCommand = new RelayCommand(() => Index--);
            NextCommand = new RelayCommand(() => Index++);
            GotFocus += RollingBox_GotFocus;
        }

        static RollingBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RollingBox), new FrameworkPropertyMetadata(typeof(RollingBox)));
        }

        ~RollingBox()
        {
            dispatcherTimer?.Stop();
            dispatcherTimer = null;
        }
        //int preindex = 0;
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        ContentControl PART_CURR_Content;
        //ContentControl PART_NEXT_Content;
        ListBox PART_ListBox;
        Button PART_Previous;

        //public List<UIElement> Items { get; set; } = new List<UIElement>();

        public ICommand PreviousCommand { get; }

        public ICommand NextCommand { get; }
 
        public static readonly DependencyProperty ItemsSourceProperty =
           DependencyProperty.Register(nameof(ItemsSource), typeof(ObservableCollection<UIElement>), typeof(RollingBox), new PropertyMetadata(new ObservableCollection<UIElement>(), (sender, e) =>
           {
               if (sender is RollingBox rollingBox)
               {
                   rollingBox.UpdateListBoxItem();
               }
           }));

        public ObservableCollection<UIElement> ItemsSource
        {
            get => (ObservableCollection<UIElement>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty RollingIntervalProperty =
            DependencyProperty.Register(nameof(RollingInterval), typeof(double), typeof(RollingBox), new PropertyMetadata(5d, (sender, e) =>
            {
                if (sender is RollingBox rollingBox && e.NewValue is double newValue)
                {
                    rollingBox.dispatcherTimer.Interval = TimeSpan.FromSeconds(newValue);
                    rollingBox.dispatcherTimer.Stop();
                    rollingBox.dispatcherTimer.Start();
                }
            }));

        /// <summary>
        /// 获取或设置滚动的间隔，默认值为5。
        /// </summary>
        public double RollingInterval
        {
            get => (double)GetValue(RollingIntervalProperty);
            set => SetValue(RollingIntervalProperty, value);
        }

        public int Index
        {
            get => PART_ListBox.SelectedIndex;
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
                PART_ListBox.SelectedIndex = value;
                IndexChange();
            }
        }

        void IndexChange()
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
            AnimationStart();
        }

        void AnimationStart()
        {
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


        void UpdateListBoxItem()
        {
            if (PART_ListBox == null || PART_CURR_Content == null)
            {
                return;
            }
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
            dispatcherTimer.Stop();
            dispatcherTimer.Start();
        }

        /// <summary>
        /// <see cref="OnApplyTemplate"/> 要比XAML赋值，构造函数晚，在第一次Loaded时触发。
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (PART_ListBox != null)
            {
                PART_ListBox.SelectionChanged -= PART_ListBox_SelectionChanged;
            }
            PART_CURR_Content = GetTemplateChild("PART_CURR_Content") as ContentControl;
            //PART_NEXT_Content = GetTemplateChild("PART_NEXT_Content") as ContentControl;
            PART_ListBox = GetTemplateChild("PART_ListBox") as ListBox;
            PART_Previous = GetTemplateChild("PART_Previous") as Button;

            //Binding binding = new Binding();
            //binding.Path = new PropertyPath(nameof(Index));
            //binding.Source = this;
            //binding.Mode = BindingMode.TwoWay;
            //PART_ListBox.SetBinding(Selector.SelectedIndexProperty, binding);

            dispatcherTimer.Interval = TimeSpan.FromSeconds(RollingInterval);
            dispatcherTimer.Tick += (sender, e) =>
            {
                Index++;
            };

            UpdateListBoxItem();
            PART_ListBox.SelectionChanged += PART_ListBox_SelectionChanged;
        }

        private void PART_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox.SelectedIndex >= 0)
            {
                IndexChange();
            }
        }
          
        private void RollingBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!e.Handled && PART_CURR_Content != null /*&& PART_NEXT_Content != null*/)
            {
                if (Equals(e.OriginalSource, this))
                {
                    UIElement uIElement;
                    if ((uIElement = GetFirstFocusable(PART_CURR_Content.Content)) != null)
                    {
                        uIElement.Focus();
                        e.Handled = true;
                    }
                    //else if ((uIElement = GetFirstFocusable(PART_NEXT_Content.Content)) != null)
                    //{
                    //    uIElement.Focus();
                    //    e.Handled = true;
                    //}
                    else if (PART_Previous != null)
                    {
                        PART_Previous.Focus();
                        e.Handled = true;
                    }
                }
            }
        }

        UIElement GetFirstFocusable(object obj)
        {
            if (obj is not UIElement uIElement)
            {
                return null;
            }
            if (uIElement.Focusable)
            {
                return uIElement;
            }
            return uIElement.FindChildren<UIElement>().FirstOrDefault(x => x.Focusable);
        }
    }
}