using KZLib;
using UnityEngine;

public class EffectTestController : MonoBehaviour
{
	private enum EffectType
	{
		RedFX,
		GreenFX,
		OstrichFX,
		BlueFX,
	}

	[SerializeField]
	private EffectType m_EffectName = EffectType.RedFX;

	private void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition).ToVector2();

			if(m_EffectName == EffectType.RedFX)
			{
				_PlayEffect(m_EffectName,worldPoint,null);
			}
			else if(m_EffectName == EffectType.OstrichFX)
			{
				_PlayEffect(m_EffectName,worldPoint,new AnimatorEffectClip.Param("Rotate"));
			}
		}
	}

	private void _PlayEffect(EffectType effectType,Vector3 worldPoint,EffectClip.Param effectParam)
	{
		EffectManager.In.PlayEffect(effectType.ToString(),worldPoint,null,effectParam);
	}
}