using System.Collections.Generic;
using UnityEngine;

namespace HudPanel
{
	public class RenderProfile : BaseComponentUI
	{
		[SerializeField]
		private List<RenderMonitor> m_RenderMonitorList = new();

		public void Refresh()
		{
			for(var i=0;i<m_RenderMonitorList.Count;i++)
			{
				m_RenderMonitorList[i].UpdateRender();
			}
		}
	}
}