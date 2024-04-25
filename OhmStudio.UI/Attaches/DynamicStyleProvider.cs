using System;
using System.Windows;

namespace OhmStudio.UI.Attaches
{
    public static class DynamicStyleProvider
    {
        public static readonly DependencyProperty BasedOnProperty =
            DependencyProperty.RegisterAttached("BasedOn", typeof(Style),
                typeof(DynamicStyleProvider), new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender, OnStyleChanged));

        public static readonly DependencyProperty DerivedProperty =
            DependencyProperty.RegisterAttached("Derived", typeof(Style),
                typeof(DynamicStyleProvider), new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender, OnStyleChanged));

        private static void OnStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not FrameworkElement frameworkElement)
            {
                return;
            }

            Style newStyle = null;
            if (e.NewValue is not null)
            {
                newStyle = new Style();

                var baseStyle = GetBasedOn(frameworkElement);
                if (baseStyle is not null)
                {
                    MergeStyle(baseStyle, newStyle);
                }

                var derivedStyle = GetDerived(frameworkElement);
                if (derivedStyle is not null)
                {
                    MergeStyle(derivedStyle, newStyle);
                }

                if (frameworkElement.Style is not null)
                {
                    MergeAttachProperties(frameworkElement.Style, newStyle);
                }
            }
            frameworkElement.Style = newStyle;
            //frameworkElement.SetValue(FrameworkElement.StyleProperty, newStyle);
        }

        private static void MergeAttachProperties(Style source, Style target)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            foreach (var item in source.Setters)
            {
                if (item is Setter setter)
                {
                    if (setter.Property == BasedOnProperty || setter.Property == DerivedProperty)
                    {
                        //if (setter.Value is not DynamicResourceExtension)
                        //{
                        //    throw new ArgumentException("The value can't be StaticResource!");
                        //}
                        target.Setters.Add(item);
                    }
                }
            }
        }

        private static void MergeStyle(Style source, Style target)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (target.TargetType.IsAssignableFrom(source.TargetType))
            {
                target.TargetType = source.TargetType;
            }

            if (source.BasedOn is not null)
            {
                MergeStyle(source.BasedOn, target);
            }

            foreach (var setter in source.Setters)
            {
                target.Setters.Add(setter);
            }

            foreach (var trigger in source.Triggers)
            {
                target.Triggers.Add(trigger);
            }

            if (source.Resources is not null)
            {
                target.Resources ??= new ResourceDictionary();
                foreach (var key in source.Resources.Keys)
                {
                    target.Resources[key] = source.Resources[key];
                }
            }
        }

        public static Style GetBasedOn(DependencyObject obj) => (Style)obj.GetValue(BasedOnProperty);

        public static void SetBasedOn(DependencyObject obj, Style value) => obj.SetValue(BasedOnProperty, value);

        public static Style GetDerived(DependencyObject obj) => (Style)obj.GetValue(DerivedProperty);

        public static void SetDerived(DependencyObject obj, Style value) => obj.SetValue(DerivedProperty, value);
    }
}