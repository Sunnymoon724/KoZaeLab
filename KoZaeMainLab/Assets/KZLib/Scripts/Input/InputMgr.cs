using System.Collections.Generic;

namespace KZLib
{
	public class InputMgr : Singleton<InputMgr>
	{
		private bool m_Disposed = false;
		private bool m_InputBlock = false;

		public bool IsInputBlock => m_InputBlock;

		private readonly List<InputCon> m_InputConList = new();

		protected override void Release(bool _disposing)
		{
			if(m_Disposed)
			{
				return;
			}

			if(_disposing)
			{
				m_InputConList.Clear();
			}

			m_Disposed = true;

			base.Release(_disposing);
		}

		public void AddInputCon(InputCon _controller)
		{
			if(m_InputConList.Contains(_controller))
			{
				return;
			}

			m_InputConList.Add(_controller);

			_controller.BlockInput(m_InputBlock);
		}

		public void RemoveInputCon(InputCon _controller)
		{
			m_InputConList.RemoveSafe(_controller);
		}

		public void BlockInput(bool _block)
		{
			if(UIMgr.HasInstance)
			{
				UIMgr.In.BlockInput(_block);
			}

			foreach(var controller in m_InputConList)
			{
				controller.BlockInput(_block);
			}

			if(TouchMgr.HasInstance)
			{
				TouchMgr.In.BlockInput(_block);
			}

			m_InputBlock = _block;
		}
	}
}