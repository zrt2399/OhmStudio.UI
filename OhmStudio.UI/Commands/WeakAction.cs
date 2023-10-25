using System;
using System.Reflection;

namespace OhmStudio.UI.Commands
{
    public class WeakAction
    {
        protected WeakAction()
        {
        }

        public WeakAction(Action action) : this(action?.Target, action)
        {
        }

        public WeakAction(object target, Action action)
        {
            if (action.Method.IsStatic)
            {
                _staticAction = action;
                if (target != null)
                {
                    Reference = new WeakReference(target);
                }
                return;
            }
            Method = action.Method;
            ActionReference = new WeakReference(action.Target);
            Reference = new WeakReference(target);
        }

        protected MethodInfo Method { get; set; }

        public virtual string MethodName
        {
            get
            {
                if (_staticAction != null)
                {
                    return _staticAction.Method.Name;
                }
                return Method.Name;
            }
        }

        private Action _staticAction;

        protected WeakReference ActionReference { get; set; }

        protected WeakReference Reference { get; set; }

        public bool IsStatic => _staticAction != null;

        public virtual bool IsAlive
        {
            get
            {
                if (_staticAction == null && Reference == null)
                {
                    return false;
                }
                if (_staticAction != null)
                {
                    return Reference == null || Reference.IsAlive;
                }
                return Reference.IsAlive;
            }
        }

        public object Target
        {
            get
            {
                if (Reference == null)
                {
                    return null;
                }
                return Reference.Target;
            }
        }

        protected object ActionTarget
        {
            get
            {
                if (ActionReference == null)
                {
                    return null;
                }
                return ActionReference.Target;
            }
        }

        public void Execute()
        {
            if (_staticAction != null)
            {
                _staticAction();
                return;
            }
            object actionTarget = ActionTarget;
            if (IsAlive && Method != null && ActionReference != null && actionTarget != null)
            {
                Method.Invoke(actionTarget, null);
            }
        }

        public void MarkForDeletion()
        {
            Reference = null;
            ActionReference = null;
            Method = null;
            _staticAction = null;
        }
    }

    public interface IExecuteWithObject
    {
        object Target { get; }

        void ExecuteWithObject(object parameter);

        void MarkForDeletion();
    }

    public interface IExecuteWithObjectAndResult
    {
        object ExecuteWithObject(object parameter);
    }

    public class WeakAction<T> : WeakAction, IExecuteWithObject
    {
        public WeakAction(Action<T> action) : this(action?.Target, action)
        {
        }

        public WeakAction(object target, Action<T> action)
        {
            if (action.Method.IsStatic)
            {
                _staticAction = action;
                if (target != null)
                {
                    Reference = new WeakReference(target);
                }
                return;
            }
            Method = action.Method;
            ActionReference = new WeakReference(action.Target);
            Reference = new WeakReference(target);
        }

        private Action<T> _staticAction;

        public override string MethodName
        {
            get
            {
                if (_staticAction != null)
                {
                    return _staticAction.Method.Name;
                }
                return Method.Name;
            }
        }

        public override bool IsAlive
        {
            get
            {
                if (_staticAction == null && Reference == null)
                {
                    return false;
                }
                if (_staticAction != null)
                {
                    return Reference == null || Reference.IsAlive;
                }
                return Reference.IsAlive;
            }
        }

        public new void Execute()
        {
            Execute(default);
        }

        public void Execute(T parameter)
        {
            if (_staticAction != null)
            {
                _staticAction(parameter);
                return;
            }
            object actionTarget = ActionTarget;
            if (IsAlive && Method != null && ActionReference != null && actionTarget != null)
            {
                Method.Invoke(actionTarget, new object[] { parameter });
            }
        }

        public void ExecuteWithObject(object parameter)
        {
            Execute((T)parameter);
        }

        public new void MarkForDeletion()
        {
            _staticAction = null;
            base.MarkForDeletion();
        }
    }

    public class WeakFunc<TResult>
    {
        protected WeakFunc()
        {
        }

        public WeakFunc(Func<TResult> func) : this(func?.Target, func)
        {
        }

        public WeakFunc(object target, Func<TResult> func)
        {
            if (func.Method.IsStatic)
            {
                _staticFunc = func;
                if (target != null)
                {
                    Reference = new WeakReference(target);
                }
                return;
            }
            Method = func.Method;
            FuncReference = new WeakReference(func.Target);
            Reference = new WeakReference(target);
        }

        private Func<TResult> _staticFunc;

        protected MethodInfo Method { get; set; }

        public bool IsStatic => _staticFunc != null;

        public virtual string MethodName
        {
            get
            {
                if (_staticFunc != null)
                {
                    return _staticFunc.Method.Name;
                }
                return Method.Name;
            }
        }

        protected WeakReference FuncReference { get; set; }

        protected WeakReference Reference { get; set; }

        public virtual bool IsAlive
        {
            get
            {
                if (_staticFunc == null && Reference == null)
                {
                    return false;
                }
                if (_staticFunc != null)
                {
                    return Reference == null || Reference.IsAlive;
                }
                return Reference.IsAlive;
            }
        }

        public object Target
        {
            get
            {
                if (Reference == null)
                {
                    return null;
                }
                return Reference.Target;
            }
        }

        protected object FuncTarget
        {
            get
            {
                if (FuncReference == null)
                {
                    return null;
                }
                return FuncReference.Target;
            }
        }

        public TResult Execute()
        {
            if (_staticFunc != null)
            {
                return _staticFunc();
            }
            object funcTarget = FuncTarget;
            if (IsAlive && Method != null && FuncReference != null && funcTarget != null)
            {
                return (TResult)Method.Invoke(funcTarget, null);
            }
            return default;
        }

        public void MarkForDeletion()
        {
            Reference = null;
            FuncReference = null;
            Method = null;
            _staticFunc = null;
        }
    }

    public class WeakFunc<T, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult
    {
        public WeakFunc(Func<T, TResult> func) : this(func?.Target, func)
        {
        }

        public WeakFunc(object target, Func<T, TResult> func)
        {
            if (func.Method.IsStatic)
            {
                _staticFunc = func;
                if (target != null)
                {
                    Reference = new WeakReference(target);
                }
                return;
            }
            Method = func.Method;
            FuncReference = new WeakReference(func.Target);
            Reference = new WeakReference(target);
        }

        private Func<T, TResult> _staticFunc;

        public override string MethodName
        {
            get
            {
                if (_staticFunc != null)
                {
                    return _staticFunc.Method.Name;
                }
                return Method.Name;
            }
        }

        public override bool IsAlive
        {
            get
            {
                if (_staticFunc == null && Reference == null)
                {
                    return false;
                }
                if (_staticFunc != null)
                {
                    return Reference == null || Reference.IsAlive;
                }
                return Reference.IsAlive;
            }
        }

        public new TResult Execute()
        {
            return Execute(default);
        }

        public TResult Execute(T parameter)
        {
            if (_staticFunc != null)
            {
                return _staticFunc(parameter);
            }
            object funcTarget = FuncTarget;
            if (IsAlive && Method != null && FuncReference != null && funcTarget != null)
            {
                return (TResult)Method.Invoke(funcTarget, new object[] { parameter });
            }
            return default;
        }

        public object ExecuteWithObject(object parameter)
        {
            return Execute((T)parameter);
        }

        public new void MarkForDeletion()
        {
            _staticFunc = null;
            base.MarkForDeletion();
        }
    }
}