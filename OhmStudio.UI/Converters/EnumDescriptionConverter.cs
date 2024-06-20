using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace OhmStudio.UI.Converters
{
    public class EnumDescriptionConverter : EnumConverter
    {
        public EnumDescriptionConverter(Type type) : base(type) { }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return GetEnumDesc(value);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public static string GetEnumDesc(object obj)
        {
            if (obj != null)
            {
                FieldInfo fieldInfo = obj.GetType().GetField(obj.ToString());
                if (fieldInfo != null)
                { 
                    if (fieldInfo.GetCustomAttribute<DescriptionAttribute>(false) is DescriptionAttribute descriptionAttribute && !string.IsNullOrEmpty(descriptionAttribute.Description))
                    {
                        return descriptionAttribute.Description;
                    }
                    else
                    {
                        return obj.ToString();
                    }
                }
            }
            return string.Empty;
        }
    }
} 