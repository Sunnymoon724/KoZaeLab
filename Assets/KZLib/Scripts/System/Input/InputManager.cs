using System.Collections.Generic;
using KZLib.Attributes;
using KZLib.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Inputs
{
	/// <summary>
	/// Singleton hub that registers <see cref="InputController"/> instances and applies a global input block.
	/// Game code should lock via <see cref="KZInputKit"/>, which reference-counts and also blocks <see cref="UIManager.BlockUI"/>.
	/// </summary>
	[SingletonConfig(AutoCreate = true,PrefabPath = "Prefab/InputManager",DontDestroy = true)]
	public class InputManager : SingletonMB<InputManager>
	{
		[SerializeField,HideInInspector]
		private bool m_blocked = false;

		[VerticalGroup("0",Order = 0),ShowInInspector,KZIsValid("Yes","No")]
		public bool IsBlocked => m_blocked;

		private readonly List<InputController> m_activeConList = new();

		protected override void _Release()
		{
			base._Release();

			m_activeConList.Clear();
		}

		/// <summary>Called from <see cref="InputController"/> on <c>Awake</c>. Syncs the controller with the current block state.</summary>
		public void AddInputCon(InputController controller)
		{
			m_activeConList.AddIfAbsent(controller);

			controller.BlockInput(IsBlocked);
		}

		/// <summary>Called from <see cref="InputController"/> on <c>OnDestroy</c>.</summary>
		public void RemoveInputCon(InputController controller)
		{
			m_activeConList.RemoveSafe(controller);
		}

		/// <summary>Disables every registered controller's <see cref="UnityEngine.InputSystem.InputAction"/> set. Called from <see cref="KZInputKit"/> only.</summary>
		public void BlockInput(bool isBlocked)
		{
			m_blocked = isBlocked;

			for(var i=0;i<m_activeConList.Count;i++)
			{
				m_activeConList[i].BlockInput(isBlocked);
			}
		}
	}
}