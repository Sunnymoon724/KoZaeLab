using KZLib;
using UnityEngine;

public class HudTestCon : MonoBehaviour
{
	private void Start()
	{
		UIMgr.In.Open<HudPanelUI>(UITag.HudPanelUI);
	}
}