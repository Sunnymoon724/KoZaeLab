using System.Collections.Generic;
using LoadingPanel;
using UnityEngine;

namespace LoadingPanel
{
	public interface ILoadingUI
	{
		void SetLoadingProgress(float _progress);
	}
}

/// <summary>
/// 로딩하는 경우
/// </summary>
public class LoadingPanelUI : WindowUI2D
{
	public override UITag Tag => UITag.LoadingPanelUI;

	[SerializeField]
	private List<ILoadingUI> m_LoadingList = new();

    public void SetLoadingProgress(float _progress)
	{
		for(var i=0;i<m_LoadingList.Count;i++)
		{
			m_LoadingList[i].SetLoadingProgress(_progress);
		}
	}
}