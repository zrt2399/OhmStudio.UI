using System;
using System.Windows.Input;

namespace OhmStudio.UI.Commands
{
    public class RelayCommand : ICommand
    {
        public RelayCommand(Action execute) : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            _execute = new WeakAction(execute);
            if (canExecute != null)
            {
                _canExecute = new WeakFunc<bool>(canExecute);
            }
        }

        private readonly WeakAction _execute;

        private readonly WeakFunc<bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || ((_canExecute.IsStatic || _canExecute.IsAlive) && _canExecute.Execute());
        }

        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter) && _execute != null && (_execute.IsStatic || _execute.IsAlive))
            {
                _execute.Execute();
            }
        }
    }

    public class RelayCommand<T> : ICommand
    {
        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            _execute = new WeakAction<T>(execute);
            if (canExecute != null)
            {
                _canExecute = new WeakFunc<T, bool>(canExecute);
            }
        }

        private readonly WeakAction<T> _execute;

        private readonly WeakFunc<T, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }
            if (!_canExecute.IsStatic && !_canExecute.IsAlive)
            {
                return false;
            }
            if (parameter == null && typeof(T).IsValueType)
            {
                return _canExecute.Execute(default);
            }
            return _canExecute.Execute((T)parameter);
        }

        public virtual void Execute(object parameter)
        {
            object obj = parameter;
            if (parameter != null && parameter.GetType() != typeof(T) && parameter is IConvertible)
            {
                obj = Convert.ChangeType(parameter, typeof(T), null);
            }
            if (CanExecute(obj) && _execute != null && (_execute.IsStatic || _execute.IsAlive))
            {
                if (obj == null)
                {
                    if (typeof(T).IsValueType)
                    {
                        _execute.Execute(default);
                        return;
                    }
                    _execute.Execute((T)obj);
                    return;
                }
                else
                {
                    _execute.Execute((T)obj);
                }
            }
        }
    }
}