using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace OhmStudio.UI.PublicMethods
{
    public class CallMethodAction : TriggerAction<DependencyObject>
    {
        private class MethodDescriptor
        {
            public MethodInfo MethodInfo { get; private set; }

            public bool HasParameters => Parameters.Length > 0;

            public int ParameterCount => Parameters.Length;

            public ParameterInfo[] Parameters { get; private set; }

            public Type SecondParameterType
            {
                get
                {
                    if (Parameters.Length >= 2)
                    {
                        return Parameters[1].ParameterType;
                    }

                    return null;
                }
            }

            public MethodDescriptor(MethodInfo methodInfo, ParameterInfo[] methodParams)
            {
                MethodInfo = methodInfo;
                Parameters = methodParams;
            }
        }

        private List<MethodDescriptor> methodDescriptors;

        public static readonly DependencyProperty TargetObjectProperty = DependencyProperty.Register("TargetObject", typeof(object), typeof(CallMethodAction), new PropertyMetadata(OnTargetObjectChanged));

        public static readonly DependencyProperty MethodNameProperty = DependencyProperty.Register("MethodName", typeof(string), typeof(CallMethodAction), new PropertyMetadata(OnMethodNameChanged));

        public object TargetObject
        {
            get
            {
                return GetValue(TargetObjectProperty);
            }
            set
            {
                SetValue(TargetObjectProperty, value);
            }
        }

        public string MethodName
        {
            get
            {
                return (string)GetValue(MethodNameProperty);
            }
            set
            {
                SetValue(MethodNameProperty, value);
            }
        }

        private object Target => TargetObject ?? base.AssociatedObject;

        public CallMethodAction()
        {
            methodDescriptors = new List<MethodDescriptor>();
        }

        protected override void Invoke(object parameter)
        {
            if (AssociatedObject == null)
            {
                return;
            }

            MethodDescriptor methodDescriptor = FindBestMethod(parameter);
            if (methodDescriptor != null)
            {
                ParameterInfo[] parameters = methodDescriptor.Parameters;
                if (parameters.Length == 0)
                {
                    methodDescriptor.MethodInfo.Invoke(Target, null);
                }
                else if (parameters.Length == 2 && base.AssociatedObject != null && parameter != null && parameters[0].ParameterType.IsAssignableFrom(base.AssociatedObject.GetType()) && parameters[1].ParameterType.IsAssignableFrom(parameter.GetType()))
                {
                    methodDescriptor.MethodInfo.Invoke(Target, new object[2] { base.AssociatedObject, parameter });
                }
            }
            else if (TargetObject != null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, ExceptionStringTable.CallMethodActionValidMethodNotFoundExceptionMessage, MethodName, TargetObject.GetType().Name));
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateMethodInfo();
        }

        protected override void OnDetaching()
        {
            methodDescriptors.Clear();
            base.OnDetaching();
        }

        private MethodDescriptor FindBestMethod(object parameter)
        {
            parameter?.GetType();

            return methodDescriptors.FirstOrDefault((MethodDescriptor methodDescriptor) => !methodDescriptor.HasParameters || (parameter != null && methodDescriptor.SecondParameterType.IsAssignableFrom(parameter.GetType())));
        }

        private void UpdateMethodInfo()
        {
            methodDescriptors.Clear();
            if (Target == null || string.IsNullOrEmpty(MethodName))
            {
                return;
            }

            Type type = Target.GetType();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (MethodInfo methodInfo in methods)
            {
                if (IsMethodValid(methodInfo))
                {
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    if (AreMethodParamsValid(parameters))
                    {
                        methodDescriptors.Add(new MethodDescriptor(methodInfo, parameters));
                    }
                }
            }

            methodDescriptors = methodDescriptors.OrderByDescending(delegate (MethodDescriptor methodDescriptor)
            {
                int num = 0;
                if (methodDescriptor.HasParameters)
                {
                    Type type2 = methodDescriptor.SecondParameterType;
                    while (type2 != typeof(EventArgs))
                    {
                        num++;
                        type2 = type2.BaseType;
                    }
                }

                return methodDescriptor.ParameterCount + num;
            }).ToList();
        }

        private bool IsMethodValid(MethodInfo method)
        {
            if (!string.Equals(method.Name, MethodName, StringComparison.Ordinal))
            {
                return false;
            }

            if (method.ReturnType != typeof(void))
            {
                return false;
            }

            return true;
        }

        private static bool AreMethodParamsValid(ParameterInfo[] methodParams)
        {
            if (methodParams.Length == 2)
            {
                if (methodParams[0].ParameterType != typeof(object))
                {
                    return false;
                }

                if (!typeof(EventArgs).IsAssignableFrom(methodParams[1].ParameterType))
                {
                    return false;
                }
            }
            else if (methodParams.Length != 0)
            {
                return false;
            }

            return true;
        }

        private static void OnMethodNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            CallMethodAction callMethodAction = (CallMethodAction)sender;
            callMethodAction.UpdateMethodInfo();
        }

        private static void OnTargetObjectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            CallMethodAction callMethodAction = (CallMethodAction)sender;
            callMethodAction.UpdateMethodInfo();
        }
    }
}