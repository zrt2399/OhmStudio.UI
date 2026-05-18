using System.Windows;
using System.Windows.Controls;

namespace OhmStudio.UI.Controls
{
    public class EmptyBox : Control
    {
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(EmptyBox), new PropertyMetadata("暂无数据"));

        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register(nameof(IconWidth), typeof(double), typeof(EmptyBox), new PropertyMetadata(60d));

        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register(nameof(IconHeight), typeof(double), typeof(EmptyBox), new PropertyMetadata(40d));

        static EmptyBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EmptyBox), new FrameworkPropertyMetadata(typeof(EmptyBox)));
        }
 
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }
        
        public double IconWidth
        {
            get => (double)GetValue(IconWidthProperty);
            set => SetValue(IconWidthProperty, value);
        }
        
        public double IconHeight
        {
            get => (double)GetValue(IconHeightProperty);
            set => SetValue(IconHeightProperty, value);
        }
    }
}