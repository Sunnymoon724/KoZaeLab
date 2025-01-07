using System.Collections.Generic;
using KZLib.KZUtility;

namespace KZLib
{
	public class InputMgr : Singleton<InputMgr>
	{
		private bool m_disposed = false;
		private bool m_block = false;

		public bool IsBlocked => m_block;

		private readonly List<InputCon> m_inputConList = new();

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

		public void AddInputCon(InputCon controller)
		{
			if(m_inputConList.Contains(controller))
			{
				return;
			}

			m_inputConList.Add(controller);

			controller.BlockInput(IsBlocked);
		}

		public void RemoveInputCon(InputCon controller)
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

			if(TouchMgr.HasInstance)
			{
				TouchMgr.In.BlockInput(isBlocked);
			}

			m_block = isBlocked;
		}
	}
}