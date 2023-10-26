﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OhmStudio.UI.Commands
{
    public class ViewModelBase : ObservableObject, IDataErrorInfo
    {
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