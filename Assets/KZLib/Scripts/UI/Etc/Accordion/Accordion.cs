using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public class Accordion : BaseComponentUI,IPointerClickHandler
	{
		[SerializeField,ReadOnly]
		private bool m_isOn = false;
		public bool IsOn => m_isOn;

		[SerializeField]
		private RectTransform m_titleRect;
		[SerializeField]
		private TMP_Text m_titleText = null;
		[SerializeField]
		private TMP_Text m_descriptionText = null;

		[SerializeField]
		private LayoutElement m_layoutElement = null;

		private CancellationTokenSource m_tokenSource = null;

		private float m_duration;
		private bool m_vertical;
		private Action<Accordion> m_onClicked;

		private float _TitleSize => m_vertical ? m_titleRect.rect.height : m_titleRect.rect.width;
		private float _CurrentSize => m_vertical ? CurrentRect.rect.height : CurrentRect.rect.width;

		public void SetEntryInfo(IEntryInfo entryInfo,float duration,bool isVertical,Action<Accordion> onClicked)
		{
			if(m_titleText)
			{
				m_titleText.SetSafeTextMeshPro(entryInfo.Name);
			}

			if(m_descriptionText)
			{
				m_descriptionText.SetSafeTextMeshPro(entryInfo.Description);
			}

			m_duration = duration;
			m_vertical = isVertical;

			m_onClicked = onClicked;
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
					var currentSize = _CurrentSize;
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
			if(m_vertical)
			{
				var originalHeight = m_layoutElement.preferredHeight;

				m_layoutElement.preferredHeight = -1f;

				var expandedHeight = LayoutUtility.GetPreferredHeight(CurrentRect);

				m_layoutElement.preferredHeight = originalHeight;

				return expandedHeight;
			}
			else
			{
				var originalWidth = m_layoutElement.preferredWidth;

				m_layoutElement.preferredWidth = -1f;

				var expandedWidth = LayoutUtility.GetPreferredWidth(CurrentRect);

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

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if(eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			if(!isActiveAndEnabled)
			{
				return;
			}

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