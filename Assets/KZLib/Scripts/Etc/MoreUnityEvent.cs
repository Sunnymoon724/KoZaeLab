using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace KZLib
{
    #region MoreUnityEvent
    public class MoreUnityEvent : UnityEvent
    {
        private readonly List<UnityAction> m_MethodList = new();

        public int Count => m_MethodList.Count;

        public void AddListener(UnityAction _onAction,bool _overlap = false)
        {
            if(_onAction == null)
            {
                return;
            }

            if(_overlap)
            {
                m_MethodList.Add(_onAction);
            }
            else
            {
                RemoveListener(_onAction);
                m_MethodList.AddNotOverlap(_onAction);
            }

            base.AddListener(_onAction);
        }

        public void SetListener(UnityAction _onAction)
        {
            if(_onAction == null)
            {
                return;
            }

            RemoveAllListeners();

            base.AddListener(_onAction);

            m_MethodList.Add(_onAction);
        }

        public void AddListenerAtOnce(UnityAction _onAction)
        {
            UnityAction onAction = null;

            onAction = () =>
            {
                _onAction?.Invoke();

                RemoveListener(onAction);
            };

            AddListener(onAction);
        }

        public new void RemoveListener(UnityAction _onAction)
        {
            if(_onAction == null)
            {
                return;
            }

            base.RemoveListener(_onAction);

            m_MethodList.Remove(_onAction);
        }

        public new void RemoveAllListeners()
        {
            base.RemoveAllListeners();

            m_MethodList.Clear();
        }

        public static MoreUnityEvent operator +(MoreUnityEvent _delegate,UnityAction _onAction)
		{
            _delegate.AddListener(_onAction);

			return _delegate;
		}

        public static MoreUnityEvent operator -(MoreUnityEvent _delegate,UnityAction _onAction)
		{
			_delegate.RemoveListener(_onAction);

			return _delegate;
		}

        public List<string> GetAllListenerName()
        {
            return m_MethodList.Select(x => GetUnityActionName(x)).ToList();
        }

        private string GetUnityActionName(UnityAction _onAction)
        {
            return _onAction.Method.Name;
        }
    }
    #endregion MoreUnityEvent

    // -----------------------------------------------------------------------------------------------

    #region MoreUnityEvent<TObject>
    public class MoreUnityEvent<TObject> : UnityEvent<TObject>
    {
        private readonly List<UnityAction<TObject>> m_MethodList = new();

        public int Count => m_MethodList.Count;

        public void AddListener(UnityAction<TObject> _onAction,bool _overlap = false)
        {
            if(_onAction == null)
            {
                return;
            }

            if(_overlap)
            {
                m_MethodList.Add(_onAction);
            }
            else
            {
                RemoveListener(_onAction);
                m_MethodList.AddNotOverlap(_onAction);
            }

            base.AddListener(_onAction);
        }

        public void SetListener(UnityAction<TObject> _onAction)
        {
            if(_onAction == null)
            {
                return;
            }

            RemoveAllListeners();

            base.AddListener(_onAction);

            m_MethodList.Add(_onAction);
        }

        public void AddListenerAtOnce(UnityAction<TObject> _onAction,TObject _argument)
        {
            UnityAction<TObject> onAction = null;

            onAction = (_argument) =>
            {
                _onAction?.Invoke(_argument);

                RemoveListener(onAction);
            };

            AddListener(onAction);
        }

        public new void RemoveListener(UnityAction<TObject> _onAction)
        {
            if(_onAction == null)
            {
                return;
            }

            base.RemoveListener(_onAction);

            m_MethodList.Remove(_onAction);
        }

        public new void RemoveAllListeners()
        {
            base.RemoveAllListeners();

            m_MethodList.Clear();
        }

        public static MoreUnityEvent<TObject> operator +(MoreUnityEvent<TObject> _delegate,UnityAction<TObject> _onAction)
		{
            _delegate.AddListener(_onAction);

			return _delegate;
		}

        public static MoreUnityEvent<TObject> operator -(MoreUnityEvent<TObject> _delegate,UnityAction<TObject> _onAction)
		{
			_delegate.RemoveListener(_onAction);

			return _delegate;
		}

        public List<string> GetAllListenerName()
        {
            return m_MethodList.Select(x => GetUnityActionName(x)).ToList();
        }

        private string GetUnityActionName(UnityAction<TObject> _onAction)
        {
            return _onAction.Method.Name;
        }
    }
    #endregion MoreUnityEvent<TObject>

    // -----------------------------------------------------------------------------------------------

    #region MoreUnityEvent<TObject,UObject>
    public class MoreUnityEvent<TObject,UObject> : UnityEvent<TObject,UObject>
    {
        private readonly List<UnityAction<TObject,UObject>> m_MethodList = new();

        public int Count => m_MethodList.Count;

        public void AddListener(UnityAction<TObject,UObject> _onAction,bool _overlap = false)
        {
            if(_onAction == null)
            {
                return;
            }

            if(_overlap)
            {
                m_MethodList.Add(_onAction);
            }
            else
            {
                RemoveListener(_onAction);
                m_MethodList.AddNotOverlap(_onAction);
            }

            base.AddListener(_onAction);
        }

        public void SetListener(UnityAction<TObject,UObject> _onAction)
        {
            if(_onAction == null)
            {
                return;
            }

            RemoveAllListeners();

            base.AddListener(_onAction);

            m_MethodList.Add(_onAction);
        }

        public void AddListenerAtOnce(UnityAction<TObject,UObject> _onAction,TObject _argument1,UObject _argument2)
        {
            UnityAction<TObject,UObject> onAction = null;

            onAction = (_argument1,_argument2) =>
            {
                _onAction?.Invoke(_argument1,_argument2);

                RemoveListener(onAction);
            };

            AddListener(onAction);
        }

        public new void RemoveListener(UnityAction<TObject,UObject> _onAction)
        {
            if(_onAction == null)
            {
                return;
            }

            base.RemoveListener(_onAction);

            m_MethodList.Remove(_onAction);
        }

        public new void RemoveAllListeners()
        {
            base.RemoveAllListeners();

            m_MethodList.Clear();
        }

        public static MoreUnityEvent<TObject,UObject> operator +(MoreUnityEvent<TObject,UObject> _delegate,UnityAction<TObject,UObject> _onAction)
		{
            _delegate.AddListener(_onAction);

			return _delegate;
		}

        public static MoreUnityEvent<TObject,UObject> operator -(MoreUnityEvent<TObject,UObject> _delegate,UnityAction<TObject,UObject> _onAction)
		{
			_delegate.RemoveListener(_onAction);

			return _delegate;
		}

        public List<string> GetAllListenerName()
        {
            return m_MethodList.Select(x => GetUnityActionName(x)).ToList();
        }

        private string GetUnityActionName(UnityAction<TObject,UObject> _onAction)
        {
            return _onAction.Method.Name;
        }
    }
    #endregion MoreUnityEvent<TObject>

    // -----------------------------------------------------------------------------------------------

    

    // -----------------------------------------------------------------------------------------------
}
