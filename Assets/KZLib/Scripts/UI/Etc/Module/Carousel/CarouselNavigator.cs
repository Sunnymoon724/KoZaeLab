using System.Collections.Generic;
using R3;
using System;
using UnityEngine;
using KZLib.Utilities;

namespace KZLib.UI
{
	/// <summary>
	/// Page-indicator dots that mirror the focused index of a paired <see cref="Carousel"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Uses <see cref="RosterMapper{CarouselDot,bool}"/> to pool <see cref="CarouselDot"/> views.
	/// Each dot receives <c>true</c> when its roster index matches the carousel focus.
	/// </para>
	/// <para>
	/// Dot count tracks <see cref="Carousel.ItemCount"/> automatically; call <see cref="SetNavigator"/>
	/// only when a manual resize is needed. Index changes from <see cref="Carousel.OnChangedIndex"/>
	/// refresh selection state.
	/// </para>
	/// <para>
	/// When <see cref="m_useDotClick"/> is enabled, dot clicks call <see cref="Carousel.SetTargetIndex"/>.
	/// <see cref="_SyncFromCarousel"/> runs on enable so a late-enabled navigator still matches carousel state.
	/// </para>
	/// </remarks>
	public class CarouselNavigator : MonoBehaviour
	{
		[SerializeField]
		private Carousel m_carousel = null;

		/// <summary>Template dot cloned by the internal roster mapper.</summary>
		[SerializeField]
		private CarouselDot m_dot = null;

		/// <summary>When <c>true</c>, pooled dots request focus on the paired carousel.</summary>
		[SerializeField]
		private bool m_useDotClick = true;

		/// <summary>Snap duration passed to <see cref="Carousel.SetTargetIndex"/>. Non-positive values snap immediately.</summary>
		[SerializeField]
		private float m_dotSnapDuration = -1.0f;

		/// <summary>Selection flag per dot index; passed to the roster mapper on refresh.</summary>
		private readonly List<bool> m_stateList = new();
		private RosterMapper<CarouselDot,bool> m_rosterMapper = null;

		private IDisposable m_subscription = null;

		private void Awake()
		{
			static void _BindDot(CarouselDot dot,bool selected)
			{
				dot.SetDot(selected);
			}

			m_rosterMapper = new RosterMapper<CarouselDot,bool>(m_dot,transform,_BindDot);
		}
		
		private void OnEnable()
		{
			if(!m_carousel)
			{
				return;
			}

			m_subscription = m_carousel.OnChangedIndex.Subscribe(_OnChangedIndex);

			_SyncFromCarousel();
		}

		/// <summary>Re-applies dot count and selection from the paired carousel without waiting for the next event.</summary>
		private void _SyncFromCarousel()
		{
			_OnChangedIndex(m_carousel.CenterIndex);
		}

		private void OnDisable()
		{
			m_subscription?.Dispose();
		}

		private void OnDestroy()
		{
			m_rosterMapper?.Dispose();
		}

		/// <summary>Resizes the dot roster to <paramref name="count"/> and wires click handlers.</summary>
		public void SetNavigator(int count)
		{
			m_stateList.Clear();

			for(var i=0;i<count;i++)
			{
				m_stateList.Add(false);
			}

			m_rosterMapper.TrySetDataList(m_stateList);

			_RefreshDotBindings();
		}

		/// <summary>Assigns roster indices and optional click callbacks on every active dot.</summary>
		private void _RefreshDotBindings()
		{
			for(var i=0;i<m_stateList.Count;i++)
			{
				m_rosterMapper.GetItemByIndex(i)?.SetNavigatorIndex(i,m_useDotClick ? _OnDotClicked : null);
			}
		}

		private void _OnDotClicked(int index)
		{
			if(!m_carousel)
			{
				return;
			}

			m_carousel.SetTargetIndex(index,m_dotSnapDuration);
		}

		/// <summary>Resizes dots when needed and updates selection when the carousel focus index changes.</summary>
		private void _OnChangedIndex(int index)
		{
			if(!m_carousel)
			{
				return;
			}

			var count = m_carousel.ItemCount;

			if(count != m_stateList.Count)
			{
				SetNavigator(count);
			}

			if(index < 0)
			{
				return;
			}

			for(var i=0;i<m_stateList.Count;i++)
			{
				var selected = i == index;

				if(m_stateList[i] == selected)
				{
					continue;
				}

				m_stateList[i] = selected;

				if(m_rosterMapper != null)
				{
					var dot = m_rosterMapper.GetItemByIndex(i);

					if(dot != null)
					{
						dot.SetDot(selected);
					}
				}
			}
		}
	}
}