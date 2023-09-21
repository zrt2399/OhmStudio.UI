using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace OhmStudio.UI.Controls
{
    [TemplatePart(Name = "PART_CURR_Content", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_NEXT_Content", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_ListBox", Type = typeof(ListBox))]
    public class RollBox : ContentControl, INotifyPropertyChanged
    {
        public RollBox()
        {
            dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            dispatcherTimer.Tick += (sender, e) =>
            {
                Index++;
            };
            dispatcherTimer.Start();
        }

        /// <summary>
        /// ApplyTemplate 要比XAML赋值晚。
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CURR_Content = GetTemplateChild("PART_CURR_Content") as ContentControl;
            NEXT_Content = GetTemplateChild("PART_NEXT_Content") as ContentControl;
            PART_ListBox = GetTemplateChild("PART_ListBox") as ListBox;

            for (int i = 0; i < Items.Count; i++)
            {
                PART_ListBox.Items.Add(new ListBoxItem());
            }
            //foreach (var item in Items)
            //{
            //    PART_ListBox.Items.Add(new ListBoxItem());
            //}

            Binding binding = new Binding();
            binding.Path = new PropertyPath(nameof(Index));
            binding.Source = this;
            binding.Mode = BindingMode.TwoWay;
            PART_ListBox.SetBinding(System.Windows.Controls.Primitives.Selector.SelectedIndexProperty, binding);
            Index = 0;
        }

        int preindex = 0;
        public event PropertyChangedEventHandler PropertyChanged;
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        ContentControl CURR_Content;
        ContentControl NEXT_Content;
        ListBox PART_ListBox;
        public List<FrameworkElement> Items { get; set; } = new List<FrameworkElement>();

        //public ObservableCollection<FrameworkElement> Items
        //{
        //    get => (ObservableCollection<FrameworkElement>)GetValue(ItemsProperty);
        //    set => SetValue(ItemsProperty, value);
        //}

        //// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ItemsProperty =
        //    DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<FrameworkElement>), typeof(RollBox), new FrameworkPropertyMetadata(new ObservableCollection<FrameworkElement>()));

        int _index = 0;
        public int Index
        {
            get => _index;
            set
            {
                preindex = _index;
                _index = value >= Items.Count ? 0 : value;
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

            if (CURR_Content.Content == null)
            {
                CURR_Content.Content = Items[Index];
                return;//首次不需要动画
            }
            dispatcherTimer.Stop();
            dispatcherTimer.Start();//鼠标点击后重新开始计时
            AnimationStart();
        }

        void AnimationStart()
        {
            //System.Diagnostics.Debug.WriteLine($"next{Index}  curr{preindex}");
            NEXT_Content.Content = Items[Index];
            CURR_Content.Content = Items[preindex];

            bool isNext = Index > preindex;
            ThicknessAnimation Curr_marginAnimation = new ThicknessAnimation();
            Curr_marginAnimation.From = new Thickness(0);
            Curr_marginAnimation.To = new Thickness(isNext ? -ActualWidth : ActualWidth, 0, isNext ? ActualWidth : -ActualWidth, 0);
            Curr_marginAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));


            ThicknessAnimation Next_marginAnimation = new ThicknessAnimation();

            Next_marginAnimation.From = new Thickness(isNext ? ActualWidth : -ActualWidth, 0, isNext ? -ActualWidth : ActualWidth, 0);
            Next_marginAnimation.To = new Thickness(0);
            Next_marginAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));

            NEXT_Content.BeginAnimation(MarginProperty, Next_marginAnimation);

            CURR_Content.BeginAnimation(MarginProperty, Curr_marginAnimation);
        }
    }
}