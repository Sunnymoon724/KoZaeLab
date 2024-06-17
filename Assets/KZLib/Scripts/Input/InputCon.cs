using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KZLib
{
	public abstract class InputCon : BaseComponent
	{
		private readonly bool m_InputBlock = false;

		public bool IsInputBlock => m_InputBlock;

		[SerializeField]
		private InputActionAsset m_InputActionAsset = null;
		protected Dictionary<string,InputAction> m_InputActionDictionary = new();

		protected override void Awake()
		{
			base.Awake();

			InputMgr.In.AddInputCon(this);

			foreach(var actionMap in m_InputActionAsset.actionMaps)
			{
				foreach(var inputAction in actionMap.actions)
				{
					m_InputActionDictionary.Add(inputAction.name,inputAction);
				}
			}
		}

		protected virtual void OnEnable()
		{
			SetEnable(true);
		}

		protected virtual void OnDisable()
		{
			SetEnable(false);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if(InputMgr.HasInstance)
			{
				InputMgr.In.RemoveInputCon(this);
			}
		}

		protected void SetEnable(bool _enable)
		{
			foreach(var inputAction in m_InputActionDictionary.Values)
			{
				if(_enable)
				{
					inputAction.Enable();
				}
				else
				{
					inputAction.Disable();
				}
			}
		}

		public void BlockInput(bool _block)
		{
			SetEnable(_block);
		}
	}
}