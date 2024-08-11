using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace OhmStudio.UI.Mvvm
{
    public class ObservableObject : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanging<T>(Expression<Func<T>> propertyExpression)
        {
            if (PropertyChanging != null)
            {
                string propertyName = GetPropertyName(propertyExpression);
                PropertyChanging.Invoke(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (PropertyChanged != null)
            {
                string propertyName = GetPropertyName(propertyExpression);
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
            OnPropertyChanging(propertyName);
            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty<T>(ref T field, T newValue, Expression<Func<T>> propertyExpression)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
            OnPropertyChanging(propertyExpression);
            field = newValue;
            OnPropertyChanged(propertyExpression);
            return true;
        }

        public void VerifyPropertyName(string propertyName)
        {
            Type type = GetType();
            if (!string.IsNullOrEmpty(propertyName) && type.GetProperty(propertyName) == null)
            {
                if (this is ICustomTypeDescriptor customTypeDescriptor)
                {
                    if (customTypeDescriptor.GetProperties().Cast<PropertyDescriptor>().Any((PropertyDescriptor property) => property.Name == propertyName))
                    {
                        return;
                    }
                }
                throw new ArgumentException("Property not found", propertyName);
            }
        }

        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            if (propertyExpression.Body is not MemberExpression memberExpression)
            {
                throw new ArgumentException("Invalid argument", nameof(propertyExpression.Body));
            }

            if (memberExpression.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException("Argument is not a property", nameof(memberExpression.Member));
            }

            return propertyInfo.Name;
        }
    }
}