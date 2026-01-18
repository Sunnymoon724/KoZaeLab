using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

namespace UnityEngine.UI
{
	[RequireComponent(typeof(LayoutElement))]
	public class AccordionSlot : Slot
	{
		[SerializeField,ReadOnly]
		private bool m_isOn = false;
		public bool IsOn => m_isOn;

		[SerializeField]
		private RectTransform m_titleRectTrans = null;

		[SerializeField]
		private LayoutElement m_layoutElement = null;

		private CancellationTokenSource m_tokenSource = null;

		private float m_duration = 0.0f;
		private bool m_vertical = false;
		private Action<AccordionSlot> m_onClicked = null;

		private float _TitleSize => m_vertical ? m_titleRectTrans.rect.height : m_titleRectTrans.rect.width;

		public void SetEntryInfo(IEntryInfo entryInfo,float duration,bool isVertical,Action<AccordionSlot> onClicked)
		{
			m_duration = duration;
			m_vertical = isVertical;

			m_onClicked = onClicked;

			SetEntryInfo(entryInfo);
		}

		public void SetState(bool state)
		{
			if(!m_titleRectTrans)
			{
				LogChannel.UI.W("title is null");

				return;
			}

			if(m_isOn == state)
			{
				return;
			}

			m_isOn = state;

			if(m_duration != 0.0f)
			{
				CommonUtility.RecycleTokenSource(ref m_tokenSource);

				if(m_isOn)
				{
					var titleSize = _TitleSize;
					var expandedSize = _GetExpandedSize();

					_SetLayoutSizeAsync(titleSize,expandedSize,m_tokenSource.Token).Forget();
				}
				else
				{
					var currentSize = GetSlotSize(m_vertical);
					var titleSize = _TitleSize;

					_SetLayoutSizeAsync(currentSize,titleSize,m_tokenSource.Token).Forget();
				}
			}
			else
			{
				if(m_isOn)
				{
					_SetLayoutSize(-1.0f);
				}
				else
				{
					var titleSize = _TitleSize;

					_SetLayoutSize(titleSize);
				}
			}
		}

		private void _SetLayoutSize(float size)
		{
			if(m_vertical)
			{
				m_layoutElement.preferredHeight = size;
			}
			else
			{
				m_layoutElement.preferredWidth = size;
			}
		}

		private float _GetExpandedSize()
		{
			if(m_vertical)
			{
				var originalHeight = m_layoutElement.preferredHeight;

				m_layoutElement.preferredHeight = -1f;

				var expandedHeight = LayoutUtility.GetPreferredHeight(RectTrans);

				m_layoutElement.preferredHeight = originalHeight;

				return expandedHeight;
			}
			else
			{
				var originalWidth = m_layoutElement.preferredWidth;

				m_layoutElement.preferredWidth = -1f;

				var expandedWidth = LayoutUtility.GetPreferredWidth(RectTrans);

				m_layoutElement.preferredWidth = originalWidth;

				return expandedWidth;
			}
		}

		private async UniTask _SetLayoutSizeAsync(float start,float finish,CancellationToken token)
		{
			if(m_vertical)
			{
				void _ProgressHeight(float height)
				{
					m_layoutElement.preferredHeight = height;
				}

				await CommonUtility.ExecuteProgressAsync(start,finish,m_duration,_ProgressHeight,false,null,token);
			}
			else
			{
				void _ProgressWidth(float width)
				{
					m_layoutElement.preferredWidth = width;
				}

				await CommonUtility.ExecuteProgressAsync(start,finish,m_duration,_ProgressWidth,false,null,token);
			}
		}
		
		protected override void _OnClicked()
		{
			base._OnClicked();

			m_onClicked?.Invoke(this);
		}

		protected override void Reset()
		{
			base.Reset();

			if(!m_layoutElement)
			{
				m_layoutElement = GetComponent<LayoutElement>();
			}
		}
	}
}