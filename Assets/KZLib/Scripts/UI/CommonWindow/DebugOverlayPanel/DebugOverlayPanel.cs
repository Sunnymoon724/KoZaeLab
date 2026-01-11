using System.Collections.Generic;
using KZLib.KZWidget.Debug;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZWidget.Debug
{
	public interface IImmediateOverlay
	{
		bool IsActive { get; }
		void Refresh();
	}

	public interface IPeriodicOverlay
	{
		bool IsActive { get; }
		void Refresh(int frameRate,float frameTime);
	}
}

public class DebugOverlayPanel : BasePanel
{
	[SerializeField,ListDrawerSettings(ShowFoldout = false)]
	private List<IImmediateOverlay> m_immediateList = new();

	[SerializeField,ListDrawerSettings(ShowFoldout = false)]
	private List<IPeriodicOverlay> m_periodicList = new();

	public void RefreshImmediate()
	{
		for(var i=0;i<m_immediateList.Count;i++)
		{
			var overlay = m_immediateList[i];

			if(overlay.IsActive)
			{
				overlay.Refresh();
			}
		}
	}

	public void RefreshPeriodic(int frameRate,float frameTime)
	{
		for(var i=0;i<m_periodicList.Count;i++)
		{
			var overlay = m_periodicList[i];

			if(overlay.IsActive)
			{
				overlay.Refresh(frameRate,frameTime);
			}
		}
	}
}