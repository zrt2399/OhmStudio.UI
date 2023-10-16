using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace OhmStudio.UI.Controls
{
    public class WrapTextBox : WrapPanel
    {
        public WrapTextBox()
        {
            Children.Add(textBlock);
            Children.Add(textBox);
            Loaded += WrapTextBox_Loaded;
        }

        Binding CloneBinding(Binding binding)
        {
            if (binding == null)
            {
                return null;
            }
            Binding newBinding = new Binding();
            foreach (var item in binding.GetType().GetProperties())
            {
                foreach (var info in newBinding.GetType().GetProperties())
                {
                    if (item.Name == info.Name)
                    {
                        if (info.CanWrite && info.Name != "Source" && info.Name != "Path")
                        {
                            var value = item.GetValue(binding);
                            if (value != null)
                            {
                                info.SetValue(newBinding, value);
                            }
                        }
                        break;
                    }
                }
            }
            newBinding.Source = this;
            return newBinding;
        }

        private void WrapTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= WrapTextBox_Loaded;

            var textBlockBinding = CloneBinding(BindingOperations.GetBinding(this, TitleProperty));
            if (textBlockBinding == null)
            {
                textBlock.SetBinding(TextBlock.TextProperty, new Binding() { Source = this, Path = new PropertyPath(TitleProperty) });
            }
            else
            {
                textBlockBinding.Path = new PropertyPath(TitleProperty);
                textBlock.SetBinding(TextBlock.TextProperty, textBlockBinding);
            }

            var textBoxBinding = CloneBinding(BindingOperations.GetBinding(this, TextProperty));
            if (textBoxBinding == null)
            {
                textBox.SetBinding(TextBox.TextProperty, new Binding() { Source = this, Path = new PropertyPath(TextProperty), Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            }
            else
            {
                textBoxBinding.Path = new PropertyPath(TextProperty);
                textBox.SetBinding(TextBox.TextProperty, textBoxBinding);
            }
        }

        TextBlock textBlock = new TextBlock();
        TextBox textBox = new TextBox();

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(WrapTextBox), new PropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(WrapTextBox), new PropertyMetadata(string.Empty));
    }
}