using System.Collections.Generic;
using KZLib.Attributes;
using KZLib.Development;
using R3;
using Sirenix.OdinInspector;

namespace UnityEngine.UI
{
	[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup),typeof(ContentSizeFitter))]
	public class Accordion : BaseComponent
	{
		[SerializeField]
		private AccordionSlot m_slot = null;

		[SerializeField]
		private int m_startIndex = -1;

		[SerializeField]
		private bool m_isVertical = true;

		[SerializeField]
		private bool m_useTransition = false;
		[SerializeField,ShowIf(nameof(m_useTransition))]
		private float m_transitionDuration = 0.3f;

		[BoxGroup("0",ShowLabel = false,Order = 99),SerializeField,KZRichText]
		private int m_currentIndex = 0;

		private bool m_initialize = false;
		private GameObjectPoolBinder<AccordionSlot,IEntryInfo> m_poolBinder = null;

		private readonly Subject<int> m_accordionSubject = new();
		public Observable<int> OnChangedIndex => m_accordionSubject;

		public AccordionSlot CurrentAccordion => m_poolBinder.GetItemByIndex(m_currentIndex);

		protected override void _Initialize()
		{
			base._Initialize();

			_EnsureInitialized();
		}

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

			m_poolBinder = new GameObjectPoolBinder<AccordionSlot,IEntryInfo>(m_slot,transform,_BindSlot);

			m_initialize = true;
		}

		protected override void _Release()
		{
			base._Release();

			m_poolBinder?.Dispose();
		}

		public void SetEntryInfoList(List<IEntryInfo> entryInfoList,int index = -1)
		{
			_EnsureInitialized();

			if(!m_poolBinder.TrySetDataList(entryInfoList))
			{
				return;
			}

			m_startIndex = index != -1 ? Mathf.Clamp(index,0,entryInfoList.Count) : -1;

			SetAccordionOn(m_startIndex,false);
		}

		public void Clear()
		{
			m_poolBinder.Clear();
		}

		public void SetAccordionOn(AccordionSlot target,bool sendCallback = true)
		{
			if(target == null)
			{
				return;
			}

			_SetAccordion(target,sendCallback);
		}

		public void SetAccordionOn(int index,bool sendCallback = true)
		{
			var target = m_poolBinder.GetItemByIndex(index);

			if(target == null)
			{
				return;
			}

			_SetAccordion(target,sendCallback);
		}

		public void SetAllAccordionOff(bool sendCallback)
		{
			_SetAccordion(null,sendCallback);
		}

		private void _SetAccordion(AccordionSlot target,bool sendCallback)
		{
			_EnsureInitialized();

			var targetIndex = -1;
			var currentIndex = 0;

			foreach(var slot in m_poolBinder.ItemGroup)
			{
				slot.SetState(target != null && slot == target);

				if(slot == target)
				{
					targetIndex = currentIndex;
				}

				currentIndex++;
			}

			m_currentIndex = targetIndex;

			if(sendCallback)
			{
				m_accordionSubject.OnNext(targetIndex);
			}
		}
	}
}