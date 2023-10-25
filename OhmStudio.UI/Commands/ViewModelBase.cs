using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OhmStudio.UI.Commands
{
    public class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged<T>(ref T property, T newValue, string propertyName)
        {
            property = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged<T>(ref T property, T newValue, Expression<Func<T>> propertyExpression)
        {
            property = newValue;
            OnPropertyChanged(propertyExpression);
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                string propertyName = GetPropertyName(propertyExpression);
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
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

            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("Argument is not a property", nameof(memberExpression.Member));
            }

            return propertyInfo.Name;
        }

        /// <summary>
        /// 表单验证错误集合。
        /// </summary>
        private Dictionary<string, string> _dataErrors = new Dictionary<string, string>();

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                try
                {
                    var validationContext = new ValidationContext(this) { MemberName = columnName };
                    var validationResults = new List<ValidationResult>();
                    Validator.TryValidateProperty(GetType().GetProperty(columnName).GetValue(this), validationContext, validationResults);
                    if (validationResults.Count > 0)
                    {
                        if (!_dataErrors.ContainsKey(validationContext.MemberName))
                        {
                            _dataErrors.Add(validationContext.MemberName, string.Empty);
                        }
                        return string.Join(Environment.NewLine, validationResults.Select(x => x.ErrorMessage));
                    }
                    _dataErrors.Remove(validationContext.MemberName);
                    return string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }
}