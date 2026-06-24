using System.Collections.Generic;
using KZLib.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KZLib.Inputs
{
	/// <summary>
	/// Base for Input System–driven controllers. Loads actions from <see cref="m_inputActionAsset"/>,
	/// registers with <see cref="InputManager"/>, and supports per-controller blocking.
	/// Derive and implement <see cref="SubscribeInputAction"/> / <see cref="UnsubscribeInputAction"/>.
	/// </summary>
	public abstract class InputController : MonoBehaviour
	{
		[SerializeField,HideInInspector]
		private bool m_blocked = false;

		[VerticalGroup("0",Order = 0),ShowInInspector,KZIsValid("Yes","No")]
		public bool IsBlocked => m_blocked;

		[SerializeField]
		private InputActionAsset m_inputActionAsset = null;

		/// <summary>Action name (or <c>MapName/ActionName</c> when names collide across maps) → action.</summary>
		private readonly Dictionary<string,InputAction> m_inputActionDict = new();

		protected abstract void SubscribeInputAction();
		protected abstract void UnsubscribeInputAction();

		private void Awake()
		{
			InputManager.In.AddInputCon(this);

			if(m_inputActionAsset == null)
			{
				LogChannel.Input.E($"InputActionAsset is not assigned in {gameObject.name}.");

				return;
			}

			var actionMapArray = m_inputActionAsset.actionMaps;

			for(var i=0;i<actionMapArray.Count;i++)
			{
				var actionMap = actionMapArray[i];
				var inputActionArray = actionMap.actions;

				for(var j=0;j<inputActionArray.Count;j++)
				{
					var inputAction = inputActionArray[j];

					if(m_inputActionDict.TryAdd(inputAction.name,inputAction))
					{
						continue;
					}

					var qualifiedKey = $"{actionMap.name}/{inputAction.name}";

					if(!m_inputActionDict.TryAdd(qualifiedKey,inputAction))
					{
						LogChannel.Input.W($"Duplicate input action '{qualifiedKey}' in {gameObject.name}.");
					}
				}
			}

			SubscribeInputAction();
		}

		private void OnEnable()
		{
			// Respect block state after disable/enable cycles (OnEnable alone would re-enable actions).
			_SetEnable(!m_blocked);
		}

		private void OnDisable()
		{
			_SetEnable(false);
		}

		private void OnDestroy()
		{
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

		/// <summary>When blocked, disables all actions on this controller.</summary>
		public void BlockInput(bool isBlocked)
		{
			m_blocked = isBlocked;

			_SetEnable(!isBlocked);
		}

		/// <summary>Looks up by short action name; use <c>MapName/ActionName</c> when registered with a qualified key.</summary>
		protected InputAction _TryGetInputAction(string inputActionName)
		{
			return m_inputActionDict.TryGetValue(inputActionName,out var inputAction) ? inputAction : null;
		}
	}
}
