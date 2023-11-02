using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace OhmStudio.UI.Converters
{
    public class EnumDescriptionTypeConverter : EnumConverter
    {
        public EnumDescriptionTypeConverter(Type type) : base(type) { }

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
                    var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attributes.Length > 0 && !string.IsNullOrEmpty(attributes[0].Description))
                    {
                        return attributes[0].Description;
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