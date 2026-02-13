using System.Collections.Generic;
using KZLib.Attributes;
using KZLib.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib
{
	[SingletonConfig(AutoCreate = true,PrefabPath = "Prefab/InputManager",DontDestroy = true)]
	public class InputManager : SingletonMB<InputManager>
	{
		[SerializeField,HideInInspector]
		private bool m_blocked = false;

		[VerticalGroup("0",Order = 0),ShowInInspector,KZIsValid("Yes","No")]
		public bool IsBlocked => m_blocked;

		private readonly List<InputController> m_activeConList = new();

		// protected override void _Initialize()
		// {
		// 	base._Initialize();
		// }

		protected override void _Release()
		{
			base._Release();

			m_activeConList.Clear();
		}

		public void AddInputCon(InputController controller)
		{
			m_activeConList.AddNotOverlap(controller);

			controller.BlockInput(IsBlocked);
		}

		public void RemoveInputCon(InputController controller)
		{
			m_activeConList.RemoveSafe(controller);
		}

		// public void ActivateTopController()
		// {
		// 	static int _FindMax(InputController controller)
		// 	{
		// 		return controller.Priority;
		// 	}

		// 	var maxIndex = m_activeConList.FindMaxIndex(_FindMax);
			
		// 	if(maxIndex != Global.INVALID_INDEX)
		// 	{
		// 		ActivateOnlyOneController(m_activeConList[maxIndex]);
		// 	}
		// }

		// public void ActivateOnlyOneController(InputController controller)
		// {
		// 	if(m_activeConList.Count == 0)
		// 	{
		// 		return;
		// 	}

		// 	for(var i=0;i<m_activeConList.Count;i++)
		// 	{
		// 		var activeCon = m_activeConList[i];

		// 		activeCon.BlockInput(activeCon != controller); 
		// 	}
		// }

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