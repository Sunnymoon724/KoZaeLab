using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace KZLib.Samples.UI
{
	public class CarouselController : BaseComponent
	{
		[VerticalGroup("0",Order = 0),SerializeField]
		private List<Carousel> m_carouselList = new();

		[SerializeField,HideInInspector]
		private int m_entryInfoCount = 3;
		[SerializeField,HideInInspector]
		private int m_startIndex = 0;

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

				if(Application.isPlaying)
				{
					_SetEntryInfoList();
				}
			}
		}

		protected int MaxCenter => EntryInfoCount-1;

		[VerticalGroup("1",Order = 1),ShowInInspector,PropertyRange(-1,nameof(MaxCenter))]
		private int StartIndex
		{
			get => m_startIndex;
			set
			{
				if(m_startIndex == value)
				{
					return;
				}

				m_startIndex = value;

				if(Application.isPlaying)
				{
					_SetEntryInfoList();
				}
			}
		}

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

			for(var i=0;i<count;i++)
			{
				m_entryInfoList.Add(new EntryInfo($"The Carousel {i}",null,null,null,null));
			}
			
			for(var i=0;i<m_carouselList.Count;i++)
			{
				m_carouselList[i].SetEntryInfoList(m_entryInfoList,StartIndex);
			}
		}
	}
}

