using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System;

public class GraphicToggleUI : ToggleUI
{
	[Flags] 
	private enum ModeCategory { None = 0, Color = 1<<0, Material = 1<<1, All = -1 }

	[VerticalGroup("2",Order = 2),SerializeField,LabelText("그래픽")]
	private Graphic m_Graphic = null;

	[FoldoutGroup("그래픽 설정",Order = 3),SerializeField,LabelText("그래픽 모드")]
	private ModeCategory m_Mode = ModeCategory.None;

	[PropertySpace(10)]
	[FoldoutGroup("그래픽 설정",Order = 3),SerializeField,LabelText("비활성화 색상"),ShowIf("IsColor")]
	private Color m_DisableColor = Color.gray;

	[FoldoutGroup("그래픽 설정",Order = 3),SerializeField,LabelText("활성화 색상"),ShowIf("IsColor")]
	private Color m_EnableColor = Color.white;

	[PropertySpace(10)]
	[FoldoutGroup("그래픽 설정",Order = 3),SerializeField,LabelText("비활성화 매터리얼"),ShowIf("IsMaterial")]
	private Material m_DisableMaterial = null;

	[FoldoutGroup("그래픽 설정",Order = 3),SerializeField,LabelText("활성화 매터리얼"),ShowIf("IsMaterial")]
	private Material m_EnableMaterial = null;

	private bool IsColor => m_Mode.HasFlag(ModeCategory.Color);
	private bool IsMaterial => m_Mode.HasFlag(ModeCategory.Material);

	protected override void Reset()
	{
		base.Reset();

		if(!m_Graphic)
		{
			m_Graphic = GetComponent<Graphic>();
		}
	}

	protected override void PlayToggle()
	{
		if(IsColor)
		{
			m_Graphic.color = IsOnNow ? m_EnableColor : m_DisableColor;
		}

		if(IsMaterial)
		{
			m_Graphic.material = IsOnNow ? m_EnableMaterial : m_DisableMaterial;
		}
	}

	public void SetColor(Color? _enable,Color? _disable)
	{
		if(_enable.HasValue)
		{
			m_EnableColor = _enable.Value;
		}

		if(_disable.HasValue)
		{
			m_DisableColor = _disable.Value;
		}

		PlayToggle();
	}
}