using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.UI
{
	/// <summary>
	/// <see cref="Slot"/> that keeps a group of <see cref="BaseToggleUI"/> children in sync. The slot button stays
	/// interactable without an entry callback (<see cref="Slot.RequiresEntryCallback"/> is false).
	/// </summary>
	/// <remarks>
	/// <para>
	/// All list children are toggled together; this is not for independent per-child toggles.
	/// </para>
	/// <para>
	/// Click order: <see cref="BaseToggleUI.Toggle"/> on each child, then <c>base._OnClicked()</c> for the optional
	/// <see cref="IEntryInfo.OnClicked"/> delegate and effect sound. Child <see cref="Toggle.onValueChanged"/> handlers
	/// may run when <see cref="BaseToggleUI.Toggle"/> or <see cref="SetToggle"/> changes <c>isOn</c>.
	/// </para>
	/// <para>
	/// <see cref="IEntryInfo"/> does not carry toggle state. Use <see cref="SetToggle"/> for programmatic on/off.
	/// <see cref="SetEntryInfo"/> with <c>null</c> clears base slot state and resets children to off (pool-safe).
	/// </para>
	/// </remarks>
	public class ToggleSlot : Slot
	{
		protected override bool RequiresEntryCallback => false;

		/// <summary>Prefab-assigned toggles flipped together on slot click or via <see cref="SetToggle"/>.</summary>
		[FoldoutGroup("Toggle",Order = 3),SerializeField,ListDrawerSettings(DraggableItems = false)]
		private List<BaseToggleUI> m_childList = new();

		/// <summary>Toggle widgets synchronized on click and by <see cref="SetToggle"/>; null entries are skipped.</summary>
		public IEnumerable<BaseToggleUI> ChildGroup => m_childList;

		/// <summary>
		/// Sets every child to <paramref name="isOn"/>. <paramref name="isForce"/> is forwarded to
		/// <see cref="BaseToggleUI.Set"/> so unchanged values can still be reapplied.
		/// </summary>
		public virtual void SetToggle(bool isOn,bool isForce = true)
		{
			_ApplyToChildren(child => child.Set(isOn,isForce));
		}

		/// <inheritdoc/>
		public override void SetEntryInfo(IEntryInfo entryInfo)
		{
			base.SetEntryInfo(entryInfo);

			if(entryInfo == null)
			{
				SetToggle(false);
			}
		}

		/// <summary>Toggles each child, then runs the base entry callback and sound.</summary>
		protected override void _OnClicked()
		{
			_ApplyToChildren(child => child.Toggle());

			base._OnClicked();
		}

		private void _ApplyToChildren(Action<BaseToggleUI> action)
		{
			foreach(var child in ChildGroup)
			{
				if(!child)
				{
					continue;
				}

				action(child);
			}
		}
	}
}
