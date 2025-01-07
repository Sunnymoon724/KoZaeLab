using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KZLib
{
	public class InputCon : BaseComponent
	{
		private bool m_block = false;

		public bool IsBlocked => m_block;

		[SerializeField]
		private InputActionAsset m_inputActionAsset = null;
		protected Dictionary<string,InputAction> m_inputActionDict = new();

		protected override void Initialize()
		{
			base.Initialize();

			InputMgr.In.AddInputCon(this);

			foreach(var actionMap in m_inputActionAsset.actionMaps)
			{
				foreach(var inputAction in actionMap.actions)
				{
					m_inputActionDict.Add(inputAction.name,inputAction);
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

		protected void SetEnable(bool enable)
		{
			foreach(var inputAction in m_inputActionDict.Values)
			{
				if(enable)
				{
					inputAction.Enable();
				}
				else
				{
					inputAction.Disable();
				}
			}
		}

		public void BlockInput(bool isBlocked)
		{
			m_block = isBlocked;

			SetEnable(!isBlocked);
		}
	}
}