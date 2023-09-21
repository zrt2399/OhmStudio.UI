using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System;

namespace OhmStudio.UI.PublicMethod
{
    public class ChangePropertyAction : TargetedTriggerAction<object>
    {
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(ChangePropertyAction), null);

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(ChangePropertyAction), null);

        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(Duration), typeof(ChangePropertyAction), null);

        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(bool), typeof(ChangePropertyAction), null);

        public string PropertyName
        {
            get
            {
                return (string)GetValue(PropertyNameProperty);
            }
            set
            {
                SetValue(PropertyNameProperty, value);
            }
        }

        public object Value
        {
            get
            {
                return GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public Duration Duration
        {
            get
            {
                return (Duration)GetValue(DurationProperty);
            }
            set
            {
                SetValue(DurationProperty, value);
            }
        }

        public bool Increment
        {
            get
            {
                return (bool)GetValue(IncrementProperty);
            }
            set
            {
                SetValue(IncrementProperty, value);
            }
        }

        protected override void Invoke(object parameter)
        {
            if (AssociatedObject == null || string.IsNullOrEmpty(PropertyName) || Target == null)
            {
                return;
            }

            Type type = base.Target.GetType();
            PropertyInfo property = type.GetProperty(PropertyName);
            ValidateProperty(property);
            object obj = Value;
            TypeConverter typeConverter = TypeConverterHelper.GetTypeConverter(property.PropertyType);
            Exception ex = null;
            try
            {
                if (Value != null)
                {
                    if (typeConverter != null && typeConverter.CanConvertFrom(Value.GetType()))
                    {
                        obj = typeConverter.ConvertFrom(null, CultureInfo.InvariantCulture, Value);
                    }
                    else
                    {
                        typeConverter = TypeConverterHelper.GetTypeConverter(Value.GetType());
                        if (typeConverter != null && typeConverter.CanConvertTo(property.PropertyType))
                        {
                            obj = typeConverter.ConvertTo(null, CultureInfo.InvariantCulture, Value, property.PropertyType);
                        }
                    }
                }

                if (Duration.HasTimeSpan)
                {
                    ValidateAnimationPossible(type);
                    object currentPropertyValue = GetCurrentPropertyValue(base.Target, property);
                    AnimatePropertyChange(property, currentPropertyValue, obj);
                }
                else
                {
                    if (Increment)
                    {
                        obj = IncrementCurrentValue(property);
                    }

                    property.SetValue(base.Target, obj, new object[0]);
                }
            }
            catch (FormatException ex2)
            {
                ex = ex2;
            }
            catch (ArgumentException ex3)
            {
                ex = ex3;
            }
            catch (MethodAccessException ex4)
            {
                ex = ex4;
            }

            if (ex == null)
            {
                return;
            }

            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, ExceptionStringTable.ChangePropertyActionCannotSetValueExceptionMessage, (Value != null) ? Value.GetType().Name : "null", PropertyName, property.PropertyType.Name), ex);
        }

        private void AnimatePropertyChange(PropertyInfo propertyInfo, object fromValue, object newValue)
        {
            Storyboard storyboard = new Storyboard();
            Timeline timeline = typeof(double).IsAssignableFrom(propertyInfo.PropertyType) ? CreateDoubleAnimation((double)fromValue, (double)newValue) : (typeof(Color).IsAssignableFrom(propertyInfo.PropertyType) ? CreateColorAnimation((Color)fromValue, (Color)newValue) : ((!typeof(Point).IsAssignableFrom(propertyInfo.PropertyType)) ? CreateKeyFrameAnimation(fromValue, newValue) : CreatePointAnimation((Point)fromValue, (Point)newValue)));
            timeline.Duration = Duration;
            storyboard.Children.Add(timeline);
            if (TargetObject == null && TargetName != null && Target is Freezable)
            {
                Storyboard.SetTargetName(storyboard, TargetName);
            }
            else
            {
                Storyboard.SetTarget(storyboard, (DependencyObject)Target);
            }

            Storyboard.SetTargetProperty(storyboard, new PropertyPath(propertyInfo.Name));
            storyboard.Completed += delegate
            {
                propertyInfo.SetValue(Target, newValue, new object[0]);
            };
            storyboard.FillBehavior = FillBehavior.Stop;
            if (AssociatedObject is FrameworkElement frameworkElement)
            {
                storyboard.Begin(frameworkElement);
            }
            else
            {
                storyboard.Begin();
            }
        }

        private static object GetCurrentPropertyValue(object target, PropertyInfo propertyInfo)
        {
            FrameworkElement frameworkElement = target as FrameworkElement;
            target.GetType();
            object obj = propertyInfo.GetValue(target, null);
            if (frameworkElement != null && (propertyInfo.Name == "Width" || propertyInfo.Name == "Height") && double.IsNaN((double)obj))
            {
                obj = propertyInfo.Name != "Width" ? frameworkElement.ActualHeight : frameworkElement.ActualWidth;
            }

            return obj;
        }

        private void ValidateAnimationPossible(Type targetType)
        {
            if (Increment)
            {
                throw new InvalidOperationException(ExceptionStringTable.ChangePropertyActionCannotIncrementAnimatedPropertyChangeExceptionMessage);
            }

            if (!typeof(DependencyObject).IsAssignableFrom(targetType))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ExceptionStringTable.ChangePropertyActionCannotAnimateTargetTypeExceptionMessage, targetType.Name));
            }
        }

        private Timeline CreateKeyFrameAnimation(object newValue, object fromValue)
        {
            ObjectAnimationUsingKeyFrames objectAnimationUsingKeyFrames = new ObjectAnimationUsingKeyFrames();
            DiscreteObjectKeyFrame discreteObjectKeyFrame = new DiscreteObjectKeyFrame();
            discreteObjectKeyFrame.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0L));
            discreteObjectKeyFrame.Value = fromValue;
            DiscreteObjectKeyFrame keyFrame = discreteObjectKeyFrame;
            DiscreteObjectKeyFrame discreteObjectKeyFrame2 = new DiscreteObjectKeyFrame();
            discreteObjectKeyFrame2.KeyTime = KeyTime.FromTimeSpan(Duration.TimeSpan);
            discreteObjectKeyFrame2.Value = newValue;
            DiscreteObjectKeyFrame keyFrame2 = discreteObjectKeyFrame2;
            objectAnimationUsingKeyFrames.KeyFrames.Add(keyFrame);
            objectAnimationUsingKeyFrames.KeyFrames.Add(keyFrame2);
            return objectAnimationUsingKeyFrames;
        }

        private Timeline CreatePointAnimation(Point fromValue, Point newValue)
        {
            PointAnimation pointAnimation = new PointAnimation();
            pointAnimation.From = fromValue;
            pointAnimation.To = newValue;
            return pointAnimation;
        }

        private Timeline CreateColorAnimation(Color fromValue, Color newValue)
        {
            ColorAnimation colorAnimation = new ColorAnimation();
            colorAnimation.From = fromValue;
            colorAnimation.To = newValue;
            return colorAnimation;
        }

        private Timeline CreateDoubleAnimation(double fromValue, double newValue)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = fromValue;
            doubleAnimation.To = newValue;
            return doubleAnimation;
        }

        private void ValidateProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, ExceptionStringTable.ChangePropertyActionCannotFindPropertyNameExceptionMessage, PropertyName, base.Target.GetType().Name));
            }

            if (!propertyInfo.CanWrite)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, ExceptionStringTable.ChangePropertyActionPropertyIsReadOnlyExceptionMessage, PropertyName, base.Target.GetType().Name));
            }
        }

        private object IncrementCurrentValue(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ExceptionStringTable.ChangePropertyActionCannotIncrementWriteOnlyPropertyExceptionMessage, propertyInfo.Name));
            }

            object value = propertyInfo.GetValue(base.Target, null);
            object obj = value;
            Type propertyType = propertyInfo.PropertyType;
            TypeConverter typeConverter = TypeConverterHelper.GetTypeConverter(propertyInfo.PropertyType);
            object obj2 = Value;
            if (obj2 == null || value == null)
            {
                return obj2;
            }

            if (typeConverter.CanConvertFrom(obj2.GetType()))
            {
                obj2 = TypeConverterHelper.DoConversionFrom(typeConverter, obj2);
            }

            if (typeof(double).IsAssignableFrom(propertyType))
            {
                return (double)value + (double)obj2;
            }

            if (typeof(int).IsAssignableFrom(propertyType))
            {
                return (int)value + (int)obj2;
            }

            if (typeof(float).IsAssignableFrom(propertyType))
            {
                return (float)value + (float)obj2;
            }

            if (typeof(string).IsAssignableFrom(propertyType))
            {
                return (string)value + (string)obj2;
            }

            return TryAddition(value, obj2);
        }

        private static object TryAddition(object currentValue, object value)
        {
            //object obj = null;
            Type type = value.GetType();
            Type type2 = currentValue.GetType();
            MethodInfo methodInfo = null;
            object obj2 = value;
            MethodInfo[] methods = type2.GetMethods();
            foreach (MethodInfo methodInfo2 in methods)
            {
                if (string.Compare(methodInfo2.Name, "op_Addition", StringComparison.Ordinal) != 0)
                {
                    continue;
                }

                ParameterInfo[] parameters = methodInfo2.GetParameters();
                Type parameterType = parameters[1].ParameterType;
                if (!parameters[0].ParameterType.IsAssignableFrom(type2))
                {
                    continue;
                }

                if (!parameterType.IsAssignableFrom(type))
                {
                    TypeConverter typeConverter = TypeConverterHelper.GetTypeConverter(parameterType);
                    if (!typeConverter.CanConvertFrom(type))
                    {
                        continue;
                    }

                    obj2 = TypeConverterHelper.DoConversionFrom(typeConverter, value);
                }

                if (methodInfo != null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, ExceptionStringTable.ChangePropertyActionAmbiguousAdditionOperationExceptionMessage, type2.Name));
                }

                methodInfo = methodInfo2;
            }

            if (methodInfo != null)
            {
                return methodInfo.Invoke(null, new object[2] { currentValue, obj2 });
            }

            return value;
        }
    }

    internal static class TypeConverterHelper
    {
        internal static object DoConversionFrom(TypeConverter converter, object value)
        {
            object result = value;
            try
            {
                if (converter != null)
                {
                    if (value != null)
                    {
                        if (converter.CanConvertFrom(value.GetType()))
                        {
                            result = converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
                            return result;
                        }

                        return result;
                    }

                    return result;
                }

                return result;
            }
            catch (Exception e)
            {
                if (!ShouldEatException(e))
                {
                    throw;
                }

                return result;
            }
        }

        private static bool ShouldEatException(Exception e)
        {
            bool flag = false;
            if (e.InnerException != null)
            {
                flag |= ShouldEatException(e.InnerException);
            }

            return flag || e is FormatException;
        }

        internal static TypeConverter GetTypeConverter(Type type)
        {
            return TypeDescriptor.GetConverter(type);
        }
    }

    [DebuggerNonUserCode]
    [CompilerGenerated]
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    internal class ExceptionStringTable
    {
        internal ExceptionStringTable()
        {
        }

        private static ResourceManager resourceMan;

        private static CultureInfo resourceCulture;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    ResourceManager resourceManager = (resourceMan = new ResourceManager("ExceptionStringTable", typeof(ExceptionStringTable).Assembly));
                }

                return resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        internal static string CallMethodActionValidMethodNotFoundExceptionMessage => ResourceManager.GetString("CallMethodActionValidMethodNotFoundExceptionMessage", resourceCulture);

        internal static string ChangePropertyActionAmbiguousAdditionOperationExceptionMessage => ResourceManager.GetString("ChangePropertyActionAmbiguousAdditionOperationExceptionMessage", resourceCulture);

        internal static string ChangePropertyActionCannotAnimateTargetTypeExceptionMessage => ResourceManager.GetString("ChangePropertyActionCannotAnimateTargetTypeExceptionMessage", resourceCulture);

        internal static string ChangePropertyActionCannotFindPropertyNameExceptionMessage => ResourceManager.GetString("ChangePropertyActionCannotFindPropertyNameExceptionMessage", resourceCulture);

        internal static string ChangePropertyActionCannotIncrementAnimatedPropertyChangeExceptionMessage => ResourceManager.GetString("ChangePropertyActionCannotIncrementAnimatedPropertyChangeExceptionMessage", resourceCulture);

        internal static string ChangePropertyActionCannotIncrementWriteOnlyPropertyExceptionMessage => ResourceManager.GetString("ChangePropertyActionCannotIncrementWriteOnlyPropertyExceptionMessage", resourceCulture);

        internal static string ChangePropertyActionCannotSetValueExceptionMessage => ResourceManager.GetString("ChangePropertyActionCannotSetValueExceptionMessage", resourceCulture);

        internal static string ChangePropertyActionPropertyIsReadOnlyExceptionMessage => ResourceManager.GetString("ChangePropertyActionPropertyIsReadOnlyExceptionMessage", resourceCulture);

        internal static string DataStateBehaviorStateNameNotFoundExceptionMessage => ResourceManager.GetString("DataStateBehaviorStateNameNotFoundExceptionMessage", resourceCulture);

        internal static string GoToStateActionTargetHasNoStateGroups => ResourceManager.GetString("GoToStateActionTargetHasNoStateGroups", resourceCulture);

        internal static string InvalidLeftOperand => ResourceManager.GetString("InvalidLeftOperand", resourceCulture);

        internal static string InvalidOperands => ResourceManager.GetString("InvalidOperands", resourceCulture);

        internal static string InvalidRightOperand => ResourceManager.GetString("InvalidRightOperand", resourceCulture);

        internal static string UnsupportedRemoveTargetExceptionMessage => ResourceManager.GetString("UnsupportedRemoveTargetExceptionMessage", resourceCulture);
    }
}