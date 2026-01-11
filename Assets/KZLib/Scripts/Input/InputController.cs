using System.Collections.Generic;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KZLib
{
	public abstract class InputController : BaseComponent
	{
		[SerializeField,HideInInspector]
		private bool m_blocked = false;

		[VerticalGroup("0",Order = 0),ShowInInspector,KZIsValid("Yes","No")]
		public bool IsBlocked => m_blocked;

		[SerializeField]
		private InputActionAsset m_inputActionAsset = null;

		[SerializeField]
		private int m_priority = 0;
		public int Priority => m_priority;

		private readonly Dictionary<string,InputAction> m_inputActionDict = new();

		protected abstract void SubscribeInputAction();
		protected abstract void UnsubscribeInputAction();

		protected override void _Initialize()
		{
			base._Initialize();

			InputManager.In.AddInputCon(this);
			
			var actionMapArray = m_inputActionAsset.actionMaps;
			
			for(var i=0;i<actionMapArray.Count;i++)
			{
				var inputActionArray = actionMapArray[i].actions;

				for(var j=0;j<inputActionArray.Count;j++)
				{
					var inputAction = inputActionArray[j];

					m_inputActionDict.Add(inputAction.name,inputAction);
				}
			}

			SubscribeInputAction();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			_SetEnable(true);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			_SetEnable(false);
		}

		protected override void _Release()
		{
			base._Release();

			UnsubscribeInputAction();

			if(InputManager.HasInstance)
			{
				InputManager.In.RemoveInputCon(this);
			}
		}

		private void _SetEnable(bool enable)
		{
			foreach(var pair in m_inputActionDict)
			{
				if(enable)
				{
					pair.Value.Enable();
				}
				else
				{
					pair.Value.Disable();
				}
			}
		}

		public void BlockInput(bool isBlocked)
		{
			m_blocked = isBlocked;

			_SetEnable(!isBlocked);
		}

		protected InputAction _TryGetInputAction(string inputActionName)
		{
			return m_inputActionDict.TryGetValue(inputActionName,out var inputAction) ? inputAction : null;
		}
	}
}