using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GraphicToggleUI : BaseToggleUI
{
	[Serializable]
	protected class GraphicChild : ToggleChild
	{
		[Flags] 
		private enum ModeCategory { None = 0, Color = 1<<0, Material = 1<<1, All = -1 }

		[BoxGroup("0",ShowLabel = false,Order = 0),SerializeField,LabelText("그래픽")]
		private Graphic m_Child = null;

		[VerticalGroup("0/2",Order = 10),SerializeField,LabelText("그래픽 모드")]
		private ModeCategory m_Mode = ModeCategory.None;

		[PropertySpace(10)]
		[VerticalGroup("0/2",Order = 10),SerializeField,LabelText("활성화 색상"),ShowIf(nameof(IsColor))]
		private Color m_EnableColor = Color.white;
		[VerticalGroup("0/2",Order = 10),SerializeField,LabelText("비활성화 색상"),ShowIf(nameof(IsColor))]
		private Color m_DisableColor = Color.gray;

		[PropertySpace(10)]
		[VerticalGroup("0/2",Order = 10),SerializeField,LabelText("활성화 매터리얼"),ShowIf(nameof(IsMaterial))]
		private Material m_EnableMaterial = null;
		[VerticalGroup("0/2",Order = 10),SerializeField,LabelText("비활성화 매터리얼"),ShowIf(nameof(IsMaterial))]
		private Material m_DisableMaterial = null;

		private bool IsColor => m_Mode.HasFlag(ModeCategory.Color);
		private bool IsMaterial => m_Mode.HasFlag(ModeCategory.Material);

		protected override void Set()
		{
			if(IsColor)
			{
				m_Child.color = IsOnNow ? m_EnableColor : m_DisableColor;
			}

			if(IsMaterial)
			{
				m_Child.material = IsOnNow ? m_EnableMaterial : m_DisableMaterial;
			}
		}
		
	}

	[VerticalGroup("1",Order = 1),SerializeField,LabelText("자식 리스트"),ListDrawerSettings(DraggableItems = false)]
	private List<GraphicChild> m_ChildList = new();

	protected override IEnumerable<ToggleChild> ChildGroup => m_ChildList;
}