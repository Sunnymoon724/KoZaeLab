using System.Collections.Generic;
using KZLib.KZWidget;
using UnityEngine;

namespace KZLib.KZWidget
{
	public interface ILoadingWidget
	{
		void SetLoadingProgress(float progress);
	}
}

public class LoadingPanel : BasePanel
{
	[SerializeField]
	private List<ILoadingWidget> m_loadingWidgetList = new();

    public void SetLoadingProgress(float progress)
	{
		for(var i=0;i<m_loadingWidgetList.Count;i++)
		{
			m_loadingWidgetList[i].SetLoadingProgress(progress);
		}
	}
}