using System.Collections.Generic;
using KZLib.KZDevelop;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public partial class FocusScroller : BaseComponentUI,IPointerUpHandler,IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler,IScrollHandler
	{
		[Space(10)]
		[SerializeField]
		private bool m_useNavigator = false;

		[SerializeField,ShowIf(nameof(m_useNavigator))]
		private CarouselNavigator m_navigator = null;

		private GameObjectPoolBinder<CarouselNavigator,bool> m_poolBinder = null;
		private readonly List<bool> m_navigatorStateList = new();

		private void _InitializeNavigator()
		{
			if(!m_useNavigator)
			{
				return;
			}

			static void _SetNavigator(CarouselNavigator navigator,bool selected)
			{
				navigator.SetNavigator(selected);
			}

			m_poolBinder = new GameObjectPoolBinder<CarouselNavigator,bool>(m_navigator,transform,_SetNavigator);
			m_navigatorStateList.Clear();
		}

		private void _AddNavigatorState(int count)
		{
			if(!m_useNavigator)
			{
				return;
			}

			m_navigatorStateList.Clear();

			if(count == 0 || count == 1)
			{
				m_poolBinder.Clear();

				return;
			}

			for(var i=0;i<count;i++)
			{
				m_navigatorStateList.Add(false);
			}
		}

		private void _RefreshNavigatorState()
		{
			if(!m_useNavigator)
			{
				return;
			}

			for(var i=0;i<m_navigatorStateList.Count;i++)
			{
				m_navigatorStateList.Add(i == m_focusIndex);
			}

			m_poolBinder.TrySetDataList(m_navigatorStateList);
		}
	}
}