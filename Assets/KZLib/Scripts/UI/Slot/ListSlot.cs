using System.Collections.Generic;
using UnityEngine;

namespace KZLib.UI
{
	/// <summary>
	/// <see cref="Slot"/> that embeds a nested <see cref="ReuseGridLayoutGroup"/> for child entries instead of a single
	/// icon or button. Expects <see cref="ListEntryInfo"/> as its entry payload.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Parent text is bound from <see cref="ListEntryInfo"/> via the base <see cref="Slot.SetEntryInfo"/>. Child cells
	/// are pooled <see cref="Slot"/> instances inside the nested grid.
	/// </para>
	/// <para>
	/// <see cref="SetEntryInfo"/> with <c>null</c> or a non-<see cref="ListEntryInfo"/> value clears the nested grid
	/// and base slot state (pool-safe). A wrong type logs a warning before clearing.
	/// </para>
	/// </remarks>
	public class ListSlot : Slot
	{
		private static readonly List<IEntryInfo> s_emptyEntryList = new();

		[SerializeField]
		private ReuseGridLayoutGroup m_gridLayout = null;

		protected override bool UseImage => false;
		protected override bool UseButton => false;

		/// <summary>
		/// Binds a <see cref="ListEntryInfo"/> to the nested grid and parent text. Pass <c>null</c> to clear everything.
		/// </summary>
		public override void SetEntryInfo(IEntryInfo entryInfo)
		{
			if(entryInfo == null)
			{
				_ClearGrid();
				base.SetEntryInfo(null);

				return;
			}

			if(entryInfo is not ListEntryInfo listEntry)
			{
				LogChannel.UI.W($"{nameof(ListSlot)} expects {nameof(ListEntryInfo)}.");

				SetEntryInfo(null);

				return;
			}

			if(!m_gridLayout)
			{
				LogChannel.UI.W($"{nameof(ListSlot)}: nested grid is not assigned.");

				base.SetEntryInfo(entryInfo);

				return;
			}

			m_gridLayout.SetEntryInfoList(listEntry.EntryInfoList);

			base.SetEntryInfo(entryInfo);
		}

		/// <summary>Recycles all nested slots via an empty entry list.</summary>
		private void _ClearGrid()
		{
			if(!m_gridLayout)
			{
				return;
			}

			m_gridLayout.SetEntryInfoList(s_emptyEntryList);
		}
	}
}