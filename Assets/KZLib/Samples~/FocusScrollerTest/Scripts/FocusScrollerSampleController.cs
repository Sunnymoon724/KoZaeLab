using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.KZSample
{
	public class FocusScrollerSampleController : BaseComponent
	{
		private record SampleEntryInfo : EntryInfo
		{
			public SampleEntryInfo(int index) : base($"{index}") { }
		}

		[VerticalGroup("0",Order = 0),SerializeField]
		private FocusScroller m_focusScroller = null;

		[SerializeField,HideInInspector]
		private int m_entryInfoCount = 3;

		[SerializeField,HideInInspector]
		private int m_centerIndex = 0;

		[VerticalGroup("1",Order = 1),ShowInInspector]
		private int EntryInfoCount
		{
			get => m_entryInfoCount;
			set
			{
				if(m_entryInfoCount == value)
				{
					return;
				}

				m_entryInfoCount = value;
				m_centerIndex = 0;

				if(Application.isPlaying)
				{
					_SetEntryInfoList();
				}
			}
		}

		protected int MaxCenter => EntryInfoCount-1;

		[VerticalGroup("1",Order = 1),ShowInInspector,PropertyRange(0,nameof(MaxCenter))]
		private int CenterIndex
		{
			get => m_centerIndex;
			set
			{
				if(m_centerIndex == value)
				{
					return;
				}

				m_centerIndex = value;

				if(Application.isPlaying)
				{
					_SetEntryInfoList();
				}
			}
		}

		private readonly List<int> m_orderList = new();

		private readonly List<IEntryInfo> m_entryInfoList = new();

		protected override void Start()
		{
			base.Start();

			_SetEntryInfoList();
		}

		private void _SetEntryInfoList()
		{
			var count = EntryInfoCount;

			m_entryInfoList.Clear();
			m_orderList.Clear();

			for(var i=0;i<count;i++)
			{
				m_entryInfoList.Add(new SampleEntryInfo(i));
				m_orderList.Add(i);
			}

			m_focusScroller.SetEntryInfoList(m_entryInfoList,CenterIndex);
		}
	}
}