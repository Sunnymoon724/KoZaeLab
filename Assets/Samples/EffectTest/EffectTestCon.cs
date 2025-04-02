using System.Collections;
using System.Collections.Generic;
using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;

public class EffectTestCon : MonoBehaviour
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
				_PlayEffect(m_EffectName,worldPoint,new AnimatorEffectClip.AnimatorEffectParam("Rotate"));
			}
		}
	}

	private void _PlayEffect(EffectType _effectType,Vector3 _worldPoint,EffectClip.EffectParam _param)
	{
		EffectMgr.In.PlayEffect(_effectType.ToString(),_worldPoint,null,_param);
	}
}