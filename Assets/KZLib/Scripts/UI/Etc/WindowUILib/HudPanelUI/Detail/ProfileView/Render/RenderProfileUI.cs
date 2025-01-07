using System.Collections.Generic;
using UnityEngine;

namespace HudPanel
{
	public class RenderProfileUI : BaseComponentUI
	{
		[SerializeField]
		private List<RenderMonitorUI> m_renderMonitorUIList = new();

		public void Refresh()
		{
			for(var i=0;i<m_renderMonitorUIList.Count;i++)
			{
				m_renderMonitorUIList[i].UpdateRender();
			}
		}
	}
}