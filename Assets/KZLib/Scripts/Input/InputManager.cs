using System.Collections.Generic;
using KZLib.KZUtility;

namespace KZLib
{
	public class InputManager : Singleton<InputManager>
	{
		private bool m_disposed = false;
		private bool m_block = false;

		public bool IsBlocked => m_block;

		private readonly List<InputController> m_inputConList = new();

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				m_inputConList.Clear();
			}

			m_disposed = true;

			base.Release(disposing);
		}

		public void AddInputCon(InputController controller)
		{
			if(m_inputConList.Contains(controller))
			{
				return;
			}

			m_inputConList.Add(controller);

			controller.BlockInput(IsBlocked);
		}

		public void RemoveInputCon(InputController controller)
		{
			m_inputConList.RemoveSafe(controller);
		}

		public void BlockInput(bool isBlocked)
		{
			if(isBlocked)
			{
				CommonUtility.LockInput();
			}
			else
			{
				CommonUtility.UnLockInput();
			}

			foreach(var controller in m_inputConList)
			{
				controller.BlockInput(isBlocked);
			}

			if(TouchManager.HasInstance)
			{
				TouchManager.In.BlockInput(isBlocked);
			}

			m_block = isBlocked;
		}
	}
}