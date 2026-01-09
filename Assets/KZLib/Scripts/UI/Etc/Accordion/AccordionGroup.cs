using System.Collections.Generic;
using KZLib.KZAttribute;
using KZLib.KZDevelop;
using R3;
using Sirenix.OdinInspector;

namespace UnityEngine.UI
{
	[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup),typeof(ContentSizeFitter))]
	public class AccordionGroup : BaseComponentUI
	{
		[SerializeField]
		private Accordion m_accordion = null;

		[SerializeField]
		private Transform m_storage = null;

		[SerializeField]
		private int m_startIndex = -1;

		[SerializeField]
		private bool m_isVertical = true;

		[SerializeField]
		private bool m_useTransition = false;
		[SerializeField,ShowIf(nameof(m_useTransition))]
		private float m_transitionDuration = 0.3f;

		[Space(10)]
		[BoxGroup("0",ShowLabel = false,Order = 99),SerializeField,KZRichText]
		private int m_currentIndex = 0;

		private readonly List<Accordion> m_accordionList = new();
		private readonly List<IEntryInfo> m_entryInfoList = new();

		private GameObjectPool<Accordion> m_accordionPool = null;

		private readonly Subject<int> m_accordionSubject = new();
		public Observable<int> OnChangedIndex => m_accordionSubject;

		public Accordion FocusAccordion => m_currentIndex == -1 ? null : m_accordionList[m_currentIndex];

		protected override void Initialize()
		{
			base.Initialize();

			m_accordionPool = new GameObjectPool<Accordion>(m_accordion,m_storage,1,false);

			m_accordion.gameObject.EnsureActive(false);
			m_storage.SetChild(m_accordion.transform);

			m_accordion.CurrentRect.pivot = new Vector2(0.5f,0.5f);

			m_entryInfoList.Clear();
			m_accordionList.Clear();
		}

		public void SetEntryInfoList(List<IEntryInfo> entryInfoList,int index = -1)
		{
			if(entryInfoList.IsNullOrEmpty())
			{
				_PutLeftAccordion(m_accordionList.Count);

				return;
			}

			m_startIndex = index != -1 ? Mathf.Clamp(index,0,entryInfoList.Count) : -1;

			var cellToCreate = entryInfoList.Count - m_accordionList.Count;

			if(cellToCreate > 0)
			{
				for(var i=0;i<cellToCreate;i++)
				{
					m_accordionList.Add(m_accordionPool.GetOrCreate(transform));
				}
			}

			void _OnClicked(Accordion accordion)
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

			var leftIndex = 0;
			var duration = m_useTransition ? m_transitionDuration : 0.0f;

			for(var i=0;i<entryInfoList.Count;i++)
			{
				var entryInfo = entryInfoList[i];
				var accordion = m_accordionList[leftIndex];

				accordion.SetEntryInfo(entryInfo,duration,m_isVertical,_OnClicked);

				leftIndex++;
			}

			_PutLeftAccordion(leftIndex);

			_SetAccordion(m_startIndex,false);
		}

		private void _PutLeftAccordion(int index)
		{
			for(var i=m_accordionList.Count-1;i>=index;i-- )
			{
				var accordion = m_accordionList[i];

				m_accordionPool.Put(accordion);
				m_accordionList.Remove(accordion);
			}
		}

		public void SetAccordionOn(Accordion target,bool sendCallback = true)
		{
			if(target == null)
			{
				return;
			}

			bool _IsMatch(Accordion accordion)
			{
				return accordion.Equals(target);
			}

			var index = m_accordionList.FindIndex(_IsMatch);

			if(index != -1)
			{
				SetAccordionOn(index,sendCallback);
			}
		}

		public void SetAccordionOn(int index,bool sendCallback = true)
		{
			if(!m_accordionList.ContainsIndex(index))
			{
				return;
			}

			_SetAccordion(index,sendCallback);
		}

		public void SetAllAccordionOff(bool sendCallback)
		{
			_SetAccordion(-1,sendCallback);
		}

		private void _SetAccordion(int index,bool sendCallback)
		{
			for(var i=0;i<m_accordionList.Count;i++)
			{
				m_accordionList[i].SetState(i == index);
			}

			m_currentIndex = index;

			if(sendCallback)
			{
				m_accordionSubject.OnNext(index);
			}
		}
	}
}