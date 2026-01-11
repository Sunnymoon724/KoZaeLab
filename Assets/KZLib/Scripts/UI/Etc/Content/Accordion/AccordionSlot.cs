using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

namespace UnityEngine.UI
{
	[RequireComponent(typeof(LayoutElement),typeof(RectTransform))]
	public class AccordionSlot : Slot
	{
		[SerializeField,ReadOnly]
		private bool m_isOn = false;
		public bool IsOn => m_isOn;

		[SerializeField]
		private RectTransform m_titleRect = null;

		[SerializeField]
		private LayoutElement m_layoutElement = null;

		private CancellationTokenSource m_tokenSource = null;

		private float m_duration;
		private bool m_vertical;
		private Action<AccordionSlot> m_onClicked;

		private float _TitleSize => m_vertical ? m_titleRect.rect.height : m_titleRect.rect.width;

		public void SetEntryInfo(IEntryInfo entryInfo,float duration,bool isVertical,Action<AccordionSlot> onClicked)
		{
			m_duration = duration;
			m_vertical = isVertical;

			m_onClicked = onClicked;

			SetEntryInfo(entryInfo);
		}

		public void SetState(bool state)
		{
			if(!m_titleRect)
			{
				LogSvc.UI.W("header is null");

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
					var headerSize = _TitleSize;
					var expandedSize = _GetExpandedSize();

					_SetLayoutSizeAsync(headerSize,expandedSize,m_tokenSource.Token).Forget();
				}
				else
				{
					var rectTransform = GetComponent<RectTransform>();

					var currentSize = m_vertical ? rectTransform.rect.height : rectTransform.rect.width;
					var headerSize = _TitleSize;

					_SetLayoutSizeAsync(currentSize,headerSize,m_tokenSource.Token).Forget();
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
					var headerSize = _TitleSize;

					_SetLayoutSize(headerSize);
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
			var rectTransform = GetComponent<RectTransform>();

			if(m_vertical)
			{
				var originalHeight = m_layoutElement.preferredHeight;

				m_layoutElement.preferredHeight = -1f;

				var expandedHeight = LayoutUtility.GetPreferredHeight(rectTransform);

				m_layoutElement.preferredHeight = originalHeight;

				return expandedHeight;
			}
			else
			{
				var originalWidth = m_layoutElement.preferredWidth;

				m_layoutElement.preferredWidth = -1f;

				var expandedWidth = LayoutUtility.GetPreferredWidth(rectTransform);

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