using System;
using UnityEngine;

namespace HudPanel
{
	public class UILogSlot : SlotUI
	{
		private Color m_tagColor = Color.white;

		public override void SetCell(ICellData cellData)
		{
			m_tagColor = (cellData as LogCellData).TagColor;

			base.SetCell(cellData);
		}

		protected override void SetImage(Sprite sprite)
		{
			m_image.color = m_tagColor;
		}
	}

	public record LogCellData(Color TagColor,string Name,string Description,Action OnClicked) : CellData(Name,Description,null,null,OnClicked);
}