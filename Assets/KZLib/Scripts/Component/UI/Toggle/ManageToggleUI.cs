using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Toggle))]
public class ManageToggleUI : BaseToggleUI
{
	[SerializeField]
	private List<BaseToggleMount> m_toggleMountList = new();

	protected override void _OnClickedToggle(bool isToggle)
	{
		for(var i=0;i<m_toggleMountList.Count;i++)
		{
			m_toggleMountList[i].IsOn = isToggle;
		}
	}
}