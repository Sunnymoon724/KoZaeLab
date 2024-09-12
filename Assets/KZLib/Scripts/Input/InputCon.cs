using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KZLib
{
	public class InputCon : BaseComponent
	{
		private readonly bool m_InputBlock = false;

		public bool IsInputBlock => m_InputBlock;

		[SerializeField]
		private InputActionAsset m_InputActionAsset = null;
		protected Dictionary<string,InputAction> m_InputActionDictionary = new();

		protected override void Initialize()
		{
			base.Initialize();

			InputMgr.In.AddInputCon(this);

			foreach(var actionMap in m_InputActionAsset.actionMaps)
			{
				foreach(var inputAction in actionMap.actions)
				{
					m_InputActionDictionary.Add(inputAction.name,inputAction);
				}
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			SetEnable(true);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			SetEnable(false);
		}

		protected override void Release()
		{
			base.Release();

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
			SetEnable(!_block);
		}
	}
}