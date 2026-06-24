using System.Collections.Generic;
using UnityEngine;

namespace KZLib.UI.Widgets.Debug
{
	public class RenderProfile : ImmediateOverlayBehaviour
	{
		[SerializeField]
		private List<RenderMonitor> m_renderMonitorList = new();

		public override void Refresh()
		{
			for(var i=0;i<m_renderMonitorList.Count;i++)
			{
				m_renderMonitorList[i].UpdateRender();
			}
		}
	}
}