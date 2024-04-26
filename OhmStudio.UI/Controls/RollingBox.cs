using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using OhmStudio.UI.Commands;

namespace OhmStudio.UI.Controls
{
    //[TemplatePart(Name = "PART_CURR_Content", Type = typeof(ContentControl))]
    //[TemplatePart(Name = "PART_NEXT_Content", Type = typeof(ContentControl))]
    //[TemplatePart(Name = "PART_ListBox", Type = typeof(ListBox))]
    public class RollingBox : Control, INotifyPropertyChanged
    {
        public RollingBox()
        {
            dispatcherTimer.Interval = TimeSpan.FromSeconds(RollingInterval);
            dispatcherTimer.Tick += (sender, e) =>
            {
                Index++;
            };
            dispatcherTimer.Start();
            PreviousCommand = new RelayCommand(() => Index--);
            NextCommand = new RelayCommand(() => Index++);
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

        /// <summary>
        /// ApplyTemplate 要比XAML赋值晚。
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_CURR_Content = GetTemplateChild("PART_CURR_Content") as ContentControl;
            PART_NEXT_Content = GetTemplateChild("PART_NEXT_Content") as ContentControl;
            PART_ListBox = GetTemplateChild("PART_ListBox") as ListBox;

            for (int i = 0; i < Items.Count; i++)
            {
                PART_ListBox.Items.Add(new ListBoxItem());
            }
            Binding binding = new Binding();
            binding.Path = new PropertyPath(nameof(Index));
            binding.Source = this;
            binding.Mode = BindingMode.TwoWay;
            PART_ListBox.SetBinding(Selector.SelectedIndexProperty, binding);
            Index = 0;
        }

        int preindex = 0;
        public event PropertyChangedEventHandler PropertyChanged;
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        ContentControl PART_CURR_Content;
        ContentControl PART_NEXT_Content;
        ListBox PART_ListBox;
        //public List<UIElement> Items { get; set; } = new List<UIElement>();

        public ICommand PreviousCommand { get; }

        public ICommand NextCommand { get; }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
           DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<UIElement>), typeof(RollingBox), new PropertyMetadata(new ObservableCollection<UIElement>()));

        public ObservableCollection<UIElement> Items
        {
            get => (ObservableCollection<UIElement>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
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

        public double RollingInterval
        {
            get => (double)GetValue(RollingIntervalProperty);
            set => SetValue(RollingIntervalProperty, value);
        }

        int _index = 0;
        public int Index
        {
            get => _index;
            set
            {
                preindex = _index;
                if (value >= Items.Count)
                {
                    value = 0;
                }
                else if (value < 0)
                {
                    value = Items.Count - 1;
                }
                _index = value;
                IndexChange();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Index)));
            }
        }

        void IndexChange()
        {
            if (Items.Count == 0)
            {
                return;
            }
            if (PART_CURR_Content == null || PART_NEXT_Content == null || PART_ListBox == null)
            {
                return;
            }
            if (PART_CURR_Content.Content == null)
            {
                PART_CURR_Content.Content = Items[Index];
                return;//首次不需要动画
            }
            dispatcherTimer.Stop();
            dispatcherTimer.Start();//重新开始计时
            AnimationStart();
        }

        void AnimationStart()
        {
            //System.Diagnostics.Debug.WriteLine($"next{Index}  curr{preindex}");
            PART_NEXT_Content.Content = Items[Index];
            PART_CURR_Content.Content = Items[preindex];

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

            DoubleAnimation currentAnimation = new DoubleAnimation();
            currentAnimation.From = 1;
            currentAnimation.To = 0;
            currentAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(400));

            DoubleAnimation nextAnimation = new DoubleAnimation();
            nextAnimation.From = 0;
            nextAnimation.To = 1;
            nextAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(400));

            PART_NEXT_Content.BeginAnimation(OpacityProperty, nextAnimation);
            PART_CURR_Content.BeginAnimation(OpacityProperty, currentAnimation);
        }
    }
}