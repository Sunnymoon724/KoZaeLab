using System;
using System.Collections.Generic;
using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.UI
{
	/// <summary>
	/// Contract for a UI window managed by <see cref="UIManager"/>.
	/// </summary>
	public interface IWindow
	{
		void Open(object param);
		void Close();
		bool IsOpen { get; }

		void Hide(bool isHidden);
		bool IsHidden { get; }

		void BlockUI(bool isBlocked);
		bool IsBlocked { get; }

		bool Is3D { get; }

		WindowPrefabType WindowType { get; }
		WindowPriorityType PriorityType { get; }

		CommonUINameTag NameTag { get; }

		bool IsPooling { get; }

		bool IsIgnoreHide { get; }
	}

	/// <summary>
	/// Base behaviour for all UI windows.
	/// Use <see cref="_SelfClose"/> or <see cref="UIManager.Close"/> to close; calling <see cref="Close"/> directly
	/// only deactivates the GameObject and does not remove it from <see cref="Repository"/>.
	/// </summary>
	[RequireComponent(typeof(CanvasGroup))]
	public abstract class Window : MonoBehaviour,IWindow
	{
		[InfoBox("CanvasGroup is null",InfoMessageType.Error,nameof(IsExistCanvasGroup))]
		[VerticalGroup("CanvasGroup",Order = -25),SerializeField]
		protected CanvasGroup m_canvasGroup = null;

		private bool IsExistCanvasGroup => m_canvasGroup == null;

		// Assigned by Repository when the window is opened.
		protected Canvas m_canvas = null;

		public bool IsOpen => gameObject.activeSelf;

		/// <summary>Hidden when invisible and input is blocked (see <see cref="Window2D"/>).</summary>
		public virtual bool IsHidden => m_canvasGroup.alpha == 0 && IsBlocked;

		public abstract bool IsBlocked { get; }

		// When true, Repository hide operations skip this window.
		public bool IsIgnoreHide { get; protected set; }

		public abstract bool IsPooling  { get; }

		public abstract WindowPrefabType WindowType { get; }
		public abstract WindowPriorityType PriorityType { get; }

		public abstract void BlockUI(bool isBlocked);

		private CommonUINameTag m_nameTag = null;

		/// <summary>Resolved once from the concrete class name via <c>CommonUINameTag</c>.</summary>
		public CommonUINameTag NameTag
		{
			get
			{
				if(m_nameTag == null)
				{
					var typeName = GetType().Name;

					if(typeName.TryToCustomTag<CommonUINameTag>(out var nameTag))
					{
						m_nameTag = nameTag;
					}
					else
					{
						throw new InvalidOperationException($"{typeName} is not defined UINameTag");
					}
				}

				return m_nameTag;
			}
		}

		public abstract bool Is3D { get; }

		private readonly HashSet<Window2D> m_linkedWindowHashSet = new();

		private Action<int> m_onClose = null;
		protected int m_result = -1;

		public void SetCanvas(Canvas canvas)
		{
			m_canvas = canvas;
		}

		private void Awake()
		{
			_SetCanvasGroupState(1,true,true);

			_Initialize();
		}

		protected virtual void _Initialize() { }

		private void OnEnable()
		{
			_OnEnable();
		}

		protected virtual void _OnEnable() { }

		private void OnDisable()
		{
			_OnDisable();
		}

		protected virtual void _OnDisable() { }

		/// <summary>Activates the window. Subclasses override to consume <paramref name="param"/>.</summary>
		public virtual void Open(object param)
		{
			LogChannel.UI.I($"{NameTag} is opened");

			gameObject.EnsureActive(true);
		}

		/// <summary>
		/// Deactivates the window and closes linked children.
		/// Prefer <see cref="_SelfClose"/> from UI callbacks so <see cref="UIManager"/> updates the repository.
		/// </summary>
		public virtual void Close()
		{
			foreach(var window in m_linkedWindowHashSet)
			{
				UIManager.In.Close(window.NameTag);
			}

			m_linkedWindowHashSet.Clear();

			m_onClose?.Invoke(m_result);

			gameObject.EnsureActive(false);

			LogChannel.UI.I($"{NameTag} is closed");
		}

		private void OnDestroy()
		{
			_Release();
		}

		protected virtual void _Release() { }

		/// <summary>Default hide implementation for 3D windows. Overridden by <see cref="Window2D"/>.</summary>
		public virtual void Hide(bool isHidden)
		{
			if(isHidden)
			{
				LogChannel.UI.I($"{NameTag} is hidden");

				_SetCanvasGroupState(0,false,false);
			}
			else
			{
				LogChannel.UI.I($"{NameTag} is shown");

				_SetCanvasGroupState(1,true,true);
			}
		}

		protected void _SetCanvasGroupState(int alpha,bool interactable,bool blocksRaycasts)
		{
			m_canvasGroup.alpha = alpha;
			m_canvasGroup.interactable = interactable;
			m_canvasGroup.blocksRaycasts = blocksRaycasts;
		}

		/// <summary>Closes this window through <see cref="UIManager"/>.</summary>
		protected virtual void _SelfClose()
		{
			UIManager.In.Close(NameTag);
		}

		/// <summary>
		/// Logs the message and closes this window through <see cref="UIManager"/>.
		/// </summary>
		protected void _FailOpen(string message)
		{
			LogChannel.UI.E(message);

			_SelfClose();
		}

		/// <summary>
		/// Validates the open param type. On failure, logs and closes via <see cref="_FailOpen"/>.
		/// </summary>
		/// <param name="isRequired">When false, a null param is accepted.</param>
		protected bool _TryGetOpenParam<TParam>(object param,out TParam validParam,bool isRequired = true) where TParam : class
		{
			if(param is TParam typedParam)
			{
				validParam = typedParam;

				return true;
			}

			if(!isRequired && param == null)
			{
				validParam = null;

				return true;
			}

			_FailOpen($"{NameTag} requires {(isRequired ? typeof(TParam).Name : $"null or {typeof(TParam).Name}")}.");

			validParam = null;

			return false;
		}

		/// <summary>
		/// Closes the linked window when this window closes.
		/// NOTE: Only 2D windows are supported; 3D linked windows were not considered.
		/// </summary>
		public void AddLink(Window2D window)
		{
			m_linkedWindowHashSet.Add(window);
		}

		/// <summary>Closes and clears all windows registered via <see cref="AddLink"/>.</summary>
		protected void _CloseLinkedWindows()
		{
			foreach(var window in m_linkedWindowHashSet)
			{
				if(window)
				{
					UIManager.In.Close(window.NameTag);
				}
			}

			m_linkedWindowHashSet.Clear();
		}

		/// <summary>Reserved for a future close-result callback.</summary>
		public void SetOnClose(Action<int> onClose)
		{
			m_onClose = onClose;
		}

		protected virtual void Reset()
		{
			if(!m_canvasGroup)
			{
				m_canvasGroup = GetComponent<CanvasGroup>();
			}
		}
	}
}