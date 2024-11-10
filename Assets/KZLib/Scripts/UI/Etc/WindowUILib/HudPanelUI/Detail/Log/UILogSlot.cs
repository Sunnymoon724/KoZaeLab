using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace HudPanel
{
	public class UILogSlot : SlotUI
	{
		[FoldoutGroup("Image",Order = SlotOrder.IMAGE),SerializeField,ShowIf(nameof(UseImage))]
		protected Image m_TagImage = null;

		public override void SetCell(ICellData _cell)
		{
			m_TagImage.color = (_cell as LogCellData).TagColor;

			base.SetCell(_cell);
		}
	}

	public record LogCellData(Color TagColor,string Name,string Description,Action OnClicked) : CellData(Name,Description,null,null,OnClicked);
}