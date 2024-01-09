using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using OhmStudio.UI.Commands;

namespace OhmStudio.UI.Messaging
{
    public class Messenger : IMessenger
    { 
        private static readonly object CreationLock = new object();

        private static IMessenger _defaultInstance;

        private readonly object _registerLock = new object();

        private Dictionary<Type, List<WeakActionAndToken>> _recipientsOfSubclassesAction;

        private Dictionary<Type, List<WeakActionAndToken>> _recipientsStrictAction;

        private bool _isCleanupRegistered;

        private struct WeakActionAndToken
        {
            public WeakAction Action;

            public object Token;
        }

        public static IMessenger Default
        {
            get
            {
                if (_defaultInstance == null)
                {
                    lock (CreationLock)
                    {
                        _defaultInstance ??= new Messenger();
                    }
                }
                return _defaultInstance;
            }
        }

        public virtual void Register<TMessage>(object recipient, Action<TMessage> action)
        {
            Register(recipient, null, false, action);
        }

        public virtual void Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action)
        {
            Register(recipient, null, receiveDerivedMessagesToo, action);
        }

        public virtual void Register<TMessage>(object recipient, object token, Action<TMessage> action)
        {
            Register(recipient, token, false, action);
        }

        public virtual void Register<TMessage>(object recipient, object token, bool receiveDerivedMessagesToo, Action<TMessage> action)
        {
            lock (_registerLock)
            {
                Type typeFromHandle = typeof(TMessage);
                Dictionary<Type, List<WeakActionAndToken>> dictionary;
                if (receiveDerivedMessagesToo)
                {
                    _recipientsOfSubclassesAction ??= new Dictionary<Type, List<WeakActionAndToken>>();
                    dictionary = _recipientsOfSubclassesAction;
                }
                else
                {
                    _recipientsStrictAction ??= new Dictionary<Type, List<WeakActionAndToken>>();
                    dictionary = _recipientsStrictAction;
                }
                lock (dictionary)
                {
                    List<WeakActionAndToken> list;
                    if (!dictionary.ContainsKey(typeFromHandle))
                    {
                        list = new List<WeakActionAndToken>();
                        dictionary.Add(typeFromHandle, list);
                    }
                    else
                    {
                        list = dictionary[typeFromHandle];
                    }
                    WeakAction<TMessage> action2 = new WeakAction<TMessage>(recipient, action);
                    WeakActionAndToken item = new WeakActionAndToken
                    {
                        Action = action2,
                        Token = token
                    };
                    list.Add(item);
                }
            }
            RequestCleanup();
        }

        public virtual void Send<TMessage>(TMessage message)
        {
            SendToTargetOrType(message, null, null);
        }

        public virtual void Send<TMessage, TTarget>(TMessage message)
        {
            SendToTargetOrType(message, typeof(TTarget), null);
        }

        public virtual void Send<TMessage>(TMessage message, object token)
        {
            SendToTargetOrType(message, null, token);
        }

        public virtual void Unregister(object recipient)
        {
            UnregisterFromLists(recipient, _recipientsOfSubclassesAction);
            UnregisterFromLists(recipient, _recipientsStrictAction);
        }

        public virtual void Unregister<TMessage>(object recipient)
        {
            Unregister<TMessage>(recipient, null, null);
        }

        public virtual void Unregister<TMessage>(object recipient, object token)
        {
            Unregister<TMessage>(recipient, token, null);
        }

        public virtual void Unregister<TMessage>(object recipient, Action<TMessage> action)
        {
            Unregister(recipient, null, action);
        }

        public virtual void Unregister<TMessage>(object recipient, object token, Action<TMessage> action)
        {
            UnregisterFromLists(recipient, token, action, _recipientsStrictAction);
            UnregisterFromLists(recipient, token, action, _recipientsOfSubclassesAction);
            RequestCleanup();
        }

        public static void OverrideDefault(IMessenger newMessenger)
        {
            _defaultInstance = newMessenger;
        }

        public static void Reset()
        {
            _defaultInstance = null;
        }

        public void ResetAll()
        {
            Reset();
        }

        private static void CleanupList(IDictionary<Type, List<WeakActionAndToken>> lists)
        {
            if (lists == null)
            {
                return;
            }
            lock (lists)
            {
                List<Type> list = new List<Type>();
                foreach (KeyValuePair<Type, List<WeakActionAndToken>> keyValuePair in lists)
                {
                    List<WeakActionAndToken> list2 = (from item in keyValuePair.Value
                                                      where item.Action == null || !item.Action.IsAlive
                                                      select item).ToList();
                    foreach (WeakActionAndToken item2 in list2)
                    {
                        keyValuePair.Value.Remove(item2);
                    }
                    if (keyValuePair.Value.Count == 0)
                    {
                        list.Add(keyValuePair.Key);
                    }
                }
                foreach (Type key in list)
                {
                    lists.Remove(key);
                }
            }
        }

        private static void SendToList<TMessage>(TMessage message, IEnumerable<WeakActionAndToken> weakActionsAndTokens, Type messageTargetType, object token)
        {
            if (weakActionsAndTokens != null)
            {
                List<WeakActionAndToken> source = weakActionsAndTokens.ToList();
                List<WeakActionAndToken> list = source.Take(source.Count).ToList();
                foreach (WeakActionAndToken weakActionAndToken in list)
                {
                    IExecuteWithObject executeWithObject = weakActionAndToken.Action as IExecuteWithObject;
                    if (executeWithObject != null && weakActionAndToken.Action.IsAlive && weakActionAndToken.Action.Target != null
                        && (messageTargetType == null || weakActionAndToken.Action.Target.GetType() == messageTargetType || messageTargetType.IsAssignableFrom(weakActionAndToken.Action.Target.GetType())) && ((weakActionAndToken.Token == null && token == null) || (weakActionAndToken.Token != null && weakActionAndToken.Token.Equals(token))))
                    {
                        executeWithObject.ExecuteWithObject(message);
                    }
                }
            }
        }

        private static void UnregisterFromLists(object recipient, Dictionary<Type, List<WeakActionAndToken>> lists)
        {
            if (recipient == null || lists == null || lists.Count == 0)
            {
                return;
            }
            lock (lists)
            {
                foreach (Type key in lists.Keys)
                {
                    foreach (WeakActionAndToken weakActionAndToken in lists[key])
                    {
                        IExecuteWithObject executeWithObject = (IExecuteWithObject)weakActionAndToken.Action;
                        if (executeWithObject != null && recipient == executeWithObject.Target)
                        {
                            executeWithObject.MarkForDeletion();
                        }
                    }
                }
            }
        }

        private static void UnregisterFromLists<TMessage>(object recipient, object token, Action<TMessage> action, Dictionary<Type, List<WeakActionAndToken>> lists)
        {
            Type typeFromHandle = typeof(TMessage);
            if (recipient == null || lists == null || lists.Count == 0 || !lists.ContainsKey(typeFromHandle))
            {
                return;
            }
            lock (lists)
            {
                foreach (WeakActionAndToken weakActionAndToken in lists[typeFromHandle])
                {
                    WeakAction<TMessage> weakAction = weakActionAndToken.Action as WeakAction<TMessage>;
                    if (weakAction != null && recipient == weakAction.Target && (action == null || action.Method.Name == weakAction.MethodName) && (token == null || token.Equals(weakActionAndToken.Token)))
                    {
                        weakActionAndToken.Action.MarkForDeletion();
                    }
                }
            }
        }

        public void RequestCleanup()
        {
            if (!_isCleanupRegistered)
            {
                Action method = new Action(Cleanup);
                Dispatcher.CurrentDispatcher.BeginInvoke(method, DispatcherPriority.ApplicationIdle, null);
                _isCleanupRegistered = true;
            }
        }

        public void Cleanup()
        {
            CleanupList(_recipientsOfSubclassesAction);
            CleanupList(_recipientsStrictAction);
            _isCleanupRegistered = false;
        }

        private void SendToTargetOrType<TMessage>(TMessage message, Type messageTargetType, object token)
        {
            Type typeFromHandle = typeof(TMessage);
            if (_recipientsOfSubclassesAction != null)
            {
                List<Type> list = _recipientsOfSubclassesAction.Keys.Take(_recipientsOfSubclassesAction.Count).ToList();
                foreach (Type type in list)
                {
                    List<WeakActionAndToken> weakActionsAndTokens = null;
                    if (typeFromHandle == type || typeFromHandle.IsSubclassOf(type) || type.IsAssignableFrom(typeFromHandle))
                    {
                        lock (_recipientsOfSubclassesAction)
                        {
                            weakActionsAndTokens = _recipientsOfSubclassesAction[type].Take(_recipientsOfSubclassesAction[type].Count).ToList();
                        }
                    }
                    SendToList(message, weakActionsAndTokens, messageTargetType, token);
                }
            }
            if (_recipientsStrictAction != null)
            {
                List<WeakActionAndToken> list2 = null;
                lock (_recipientsStrictAction)
                {
                    if (_recipientsStrictAction.ContainsKey(typeFromHandle))
                    {
                        list2 = _recipientsStrictAction[typeFromHandle].Take(_recipientsStrictAction[typeFromHandle].Count).ToList();
                    }
                }
                if (list2 != null)
                {
                    SendToList(message, list2, messageTargetType, token);
                }
            }
            RequestCleanup();
        }
    }
}