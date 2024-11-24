using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using OhmStudio.UI.Utilities;

namespace OhmStudio.UI.Attaches
{
    public class TextBlockAttach
    {
        public static readonly DependencyProperty HighlightComparisonTypeProperty =
            DependencyProperty.RegisterAttached("HighlightComparisonType", typeof(StringComparison), typeof(TextBlockAttach), new PropertyMetadata(StringComparison.OrdinalIgnoreCase));
        public static StringComparison GetHighlightComparisonType(DependencyObject element)
        {
            return (StringComparison)element.GetValue(HighlightComparisonTypeProperty);
        }

        public static void SetHighlightComparisonType(DependencyObject element, StringComparison value)
        {
            element.SetValue(HighlightComparisonTypeProperty, value);
        }

        public static readonly DependencyProperty HighlightForegroundProperty =
            DependencyProperty.RegisterAttached("HighlightForeground", typeof(Brush), typeof(TextBlockAttach), new PropertyMetadata("#FF19AF19".ToSolidColorBrush()));
        public static Brush GetHighlightForeground(DependencyObject element)
        {
            return (Brush)element.GetValue(HighlightForegroundProperty);
        }

        public static void SetHighlightForeground(DependencyObject element, Brush value)
        {
            element.SetValue(HighlightForegroundProperty, value);
        }

        public static readonly DependencyProperty HighlightBackgroundProperty =
            DependencyProperty.RegisterAttached("HighlightBackground", typeof(Brush), typeof(TextBlockAttach), new PropertyMetadata(default(Brush)));
        public static Brush GetHighlightBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(HighlightBackgroundProperty);
        }

        public static void SetHighlightBackground(DependencyObject element, Brush value)
        {
            element.SetValue(HighlightBackgroundProperty, value);
        }

        public static readonly DependencyProperty LowlightForegroundProperty =
            DependencyProperty.RegisterAttached("LowlightForeground", typeof(Brush), typeof(TextBlockAttach), new PropertyMetadata(default(Brush)));
        public static Brush GetLowlightForeground(DependencyObject element)
        {
            return (Brush)element.GetValue(LowlightForegroundProperty);
        }

        public static void SetLowlightForeground(DependencyObject element, Brush value)
        {
            element.SetValue(LowlightForegroundProperty, value);
        }

        /// <summary>
        /// 标识 HighlightText 依赖项属性。
        /// </summary>
        public static readonly DependencyProperty HighlightTextProperty =
            DependencyProperty.RegisterAttached("HighlightText", typeof(string), typeof(TextBlockAttach), new PropertyMetadata(null, OnHighlightTextChanged));
        public static string GetHighlightText(DependencyObject element)
        {
            return (string)element.GetValue(HighlightTextProperty);
        }

        public static void SetHighlightText(DependencyObject element, string value)
        {
            element.SetValue(HighlightTextProperty, value);
        }

        private static void OnHighlightTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TextBlock target && e.NewValue is string newValue)
            {
                MarkHighlight(target, newValue);
            }
        }

        private static void MarkHighlight(TextBlock textBlock, string highLightText)
        {
            var text = textBlock.Text;
            textBlock.Inlines.Clear();
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(highLightText))
            {
                textBlock.Inlines.Add(new Run { Text = text });
                return;
            }
            var comparisonType = GetHighlightComparisonType(textBlock);
            var highlightForeground = GetHighlightForeground(textBlock);
            var highlightBackground = GetHighlightBackground(textBlock);
            var lowlightForeground = GetLowlightForeground(textBlock);

            while (text.Length > 0)
            {
                var runText = string.Empty;
                var index = text.IndexOf(highLightText, comparisonType);
                if (index > 0)
                {
                    runText = text.Substring(0, index);
                    var run = new Run { Text = runText };
                    if (lowlightForeground != null)
                    {
                        run.Foreground = lowlightForeground;
                    }

                    textBlock.Inlines.Add(run);
                }
                else if (index == 0)
                {
                    runText = text.Substring(0, highLightText.Length);
                    var run = new Run { Text = runText };
                    if (highlightForeground != null)
                    {
                        run.Foreground = highlightForeground;
                    }

                    if (highlightBackground != null)
                    {
                        run.Background = highlightBackground;
                    }

                    textBlock.Inlines.Add(run);
                }
                else if (index == -1)
                {
                    runText = text;
                    var run = new Run { Text = runText };
                    if (lowlightForeground != null)
                    {
                        run.Foreground = lowlightForeground;
                    }

                    textBlock.Inlines.Add(run);
                }

                text = text.Substring(runText.Length);
            }
        }
    }
}