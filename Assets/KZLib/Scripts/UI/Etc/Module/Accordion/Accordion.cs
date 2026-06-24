using System.Collections.Generic;
using KZLib.Attributes;
using KZLib.Utilities;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.UI
{
	/// <summary>
	/// Layout-group accordion backed by a <see cref="RosterMapper{AccordionSlot,IEntryInfo}"/> over
	/// <see cref="AccordionSlot"/> cells.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Exactly one slot may be expanded at a time. Clicking the open slot collapses every slot.
	/// </para>
	/// <para>
	/// Requires a <see cref="HorizontalOrVerticalLayoutGroup"/> and <see cref="ContentSizeFitter"/> on the same
	/// GameObject. The serialized vertical flag must match the layout group's orientation in the prefab.
	/// </para>
	/// <para>
	/// <see cref="OnChangedIndex"/> emits the open roster index, or <c>-1</c> when every slot is collapsed.
	/// </para>
	/// </remarks>
	[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup),typeof(ContentSizeFitter))]
	public class Accordion : MonoBehaviour
	{
		/// <summary>Template slot cloned by the internal roster mapper.</summary>
		[SerializeField]
		private AccordionSlot m_slot = null;

		/// <summary>Last open index passed to <see cref="SetEntryInfoList"/>; <c>-1</c> means start collapsed.</summary>
		[SerializeField]
		private int m_startIndex = -1;

		/// <summary>When <c>true</c>, slots expand along height; otherwise along width.</summary>
		[SerializeField]
		private bool m_isVertical = true;

		[SerializeField]
		private bool m_useTransition = false;
		[SerializeField,ShowIf(nameof(m_useTransition))]
		private float m_transitionDuration = 0.3f;

		/// <summary>Roster index of the expanded slot, or <c>-1</c> when every slot is collapsed.</summary>
		[BoxGroup("0",ShowLabel = false,Order = 99),SerializeField,KZRichText]
		private int m_currentIndex = -1;

		private bool m_initialize = false;

		/// <summary>Pools accordion slots and binds each entry to <see cref="IEntryInfo"/>.</summary>
		private RosterMapper<AccordionSlot,IEntryInfo> m_rosterMapper = null;

		private readonly Subject<int> m_accordionSubject = new();

		/// <summary>Emits the open roster index, or <c>-1</c> when every slot is collapsed.</summary>
		public Observable<int> OnChangedIndex => m_accordionSubject;

		/// <summary>Currently expanded slot, or <c>null</c> when every slot is collapsed.</summary>
		public AccordionSlot CurrentAccordion => m_rosterMapper?.GetItemByIndex(m_currentIndex);

		private void Awake()
		{
			_EnsureInitialized();
		}

		/// <summary>Creates the roster mapper and wires slot click handling once.</summary>
		private void _EnsureInitialized()
		{
			if(m_initialize)
			{
				return;
			}

			void _OnClicked(AccordionSlot accordion)
			{
				if(accordion.IsOn)
				{
					SetAllAccordionOff(true);
				}
				else
				{
					SetAccordionOn(accordion,true);
				}
			}

			var duration = m_useTransition ? m_transitionDuration : 0.0f;

			void _BindSlot(AccordionSlot slot,IEntryInfo entryInfo)
			{
				slot.SetEntryInfo(entryInfo,duration,m_isVertical,_OnClicked);
			}

			m_rosterMapper = new RosterMapper<AccordionSlot,IEntryInfo>(m_slot,transform,_BindSlot);

			m_initialize = true;
		}

		private void OnDestroy()
		{
			m_rosterMapper?.Dispose();
			m_accordionSubject.Dispose();
		}

		/// <summary>
		/// Binds <paramref name="entryInfoList"/> and opens <paramref name="index"/> when it is not <c>-1</c>.
		/// Pass a null or empty list to <see cref="Clear"/> every slot.
		/// </summary>
		public void SetEntryInfoList(List<IEntryInfo> entryInfoList,int index = -1)
		{
			_EnsureInitialized();

			if(entryInfoList.IsNullOrEmpty())
			{
				Clear();

				return;
			}

			if(!m_rosterMapper.TrySetDataList(entryInfoList))
			{
				return;
			}

			m_startIndex = index != -1 ? Mathf.Clamp(index,0,entryInfoList.Count-1) : -1;

			if(m_startIndex >= 0)
			{
				SetAccordionOn(m_startIndex,false);
			}
			else
			{
				SetAllAccordionOff(false);
			}
		}

		/// <summary>Returns every slot to the pool, resets the open index to <c>-1</c>, and emits <see cref="OnChangedIndex"/>.</summary>
		public void Clear()
		{
			if(m_rosterMapper == null)
			{
				m_currentIndex = -1;

				return;
			}

			m_rosterMapper.Clear();
			m_currentIndex = -1;
			m_accordionSubject.OnNext(-1);
		}

		/// <summary>Expands <paramref name="target"/> and collapses every other active slot.</summary>
		/// <param name="sendCallback">When <c>true</c>, emits <see cref="OnChangedIndex"/>.</param>
		public void SetAccordionOn(AccordionSlot target,bool sendCallback = true)
		{
			if(target == null)
			{
				return;
			}

			_SetAccordion(target,sendCallback);
		}

		/// <summary>Expands the slot at <paramref name="index"/> and collapses every other active slot.</summary>
		/// <param name="sendCallback">When <c>true</c>, emits <see cref="OnChangedIndex"/>.</param>
		public void SetAccordionOn(int index,bool sendCallback = true)
		{
			if(index < 0)
			{
				SetAllAccordionOff(sendCallback);

				return;
			}

			var target = m_rosterMapper.GetItemByIndex(index);

			if(target == null)
			{
				LogChannel.UI.W($"{nameof(Accordion)}: index {index} is out of range (count={m_rosterMapper.ItemCount}).");

				return;
			}

			_SetAccordion(target,sendCallback);
		}

		/// <summary>Collapses every active slot.</summary>
		/// <param name="sendCallback">When <c>true</c>, emits <c>-1</c> on <see cref="OnChangedIndex"/>.</param>
		public void SetAllAccordionOff(bool sendCallback)
		{
			_SetAccordion(null,sendCallback);
		}

		/// <summary>Applies expand/collapse state to every active slot and optionally notifies subscribers.</summary>
		private void _SetAccordion(AccordionSlot target,bool sendCallback)
		{
			_EnsureInitialized();

			var targetIndex = target != null ? m_rosterMapper.FindIndex(target) : -1;

			foreach(var slot in m_rosterMapper.ItemGroup)
			{
				slot.SetState(target != null && slot == target);
			}

			m_currentIndex = targetIndex;

			if(sendCallback)
			{
				m_accordionSubject.OnNext(targetIndex);
			}
		}
	}
}
