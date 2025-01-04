using System.Windows;
using System.Windows.Controls;

namespace OhmStudio.UI.Controls
{
    public class EmptyBox : Control
    {
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(EmptyBox), new PropertyMetadata("暂无数据"));

        static EmptyBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EmptyBox), new FrameworkPropertyMetadata(typeof(EmptyBox)));
        }
 
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }
    }
}