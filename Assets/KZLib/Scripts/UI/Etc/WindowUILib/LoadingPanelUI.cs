using System.Collections.Generic;
using LoadingPanel;
using UnityEngine;

namespace LoadingPanel
{
	public interface ILoadingUI
	{
		void SetLoadingProgress(float progress);
	}
}

public class LoadingPanelUI : WindowUI2D
{
	[SerializeField]
	private List<ILoadingUI> m_loadingList = new();

    public void SetLoadingProgress(float progress)
	{
		for(var i=0;i<m_loadingList.Count;i++)
		{
			m_loadingList[i].SetLoadingProgress(progress);
		}
	}
}