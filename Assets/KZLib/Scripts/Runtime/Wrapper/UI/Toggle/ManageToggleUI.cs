using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Toggle that propagates its on-state to a list of <see cref="BaseToggleMount"/> visual handlers.
/// </summary>
[RequireComponent(typeof(Toggle))]
public class ManageToggleUI : BaseToggleUI
{
	[SerializeField]
	private List<BaseToggleMount> m_toggleMountList = new();

	protected override void _OnClickedToggle(bool isToggle)
	{
		for(var i=0;i<m_toggleMountList.Count;i++)
		{
			if(m_toggleMountList[i] == null)
			{
				continue;
			}

			m_toggleMountList[i].IsOn = isToggle;
		}
	}
}