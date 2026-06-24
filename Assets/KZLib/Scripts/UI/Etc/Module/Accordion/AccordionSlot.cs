using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace KZLib.UI
{
	/// <summary>
	/// <see cref="Slot"/> that expands and collapses along one axis via <see cref="LayoutElement"/> preferred size.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Collapsed size is measured from the title <see cref="RectTransform"/>; expanded size uses the full slot root.
	/// The slot button stays interactable without an entry callback (<see cref="Slot.RequiresEntryCallback"/> is false).
	/// </para>
	/// <para>
	/// Click order: optional entry click callback and sound via <c>base._OnClicked()</c>, then the
	/// accordion group callback supplied by <see cref="Accordion"/>.
	/// </para>
	/// <para>
	/// <see cref="SetEntryInfo"/> with <c>null</c> clears base slot state and resets layout (pool-safe).
	/// </para>
	/// </remarks>
	[RequireComponent(typeof(LayoutElement))]
	public class AccordionSlot : Slot
	{
		private const string c_titleChildName = "Title";

		protected override bool RequiresEntryCallback => false;

		[SerializeField,ReadOnly]
		private bool m_isOn = false;
		public bool IsOn => m_isOn;

		[SerializeField,FormerlySerializedAs("m_titleRectTrans")]
		private RectTransform m_titleRootRect = null;

		[SerializeField]
		private LayoutElement m_layoutElement = null;

		private CancellationTokenSource m_tokenSource = null;

		private float m_duration = 0.0f;
		private bool m_vertical = false;
		private Action<AccordionSlot> m_onClicked = null;

		private void Awake()
		{
			_EnsureTitleRootRect();
		}

		/// <summary>Binds entry data and accordion-specific expand settings used by <see cref="Accordion"/>.</summary>
		public void SetEntryInfo(IEntryInfo entryInfo,float duration,bool isVertical,Action<AccordionSlot> onClicked)
		{
			m_duration = duration;
			m_vertical = isVertical;
			m_onClicked = onClicked;

			SetEntryInfo(entryInfo);
		}

		/// <inheritdoc/>
		public override void SetEntryInfo(IEntryInfo entryInfo)
		{
			base.SetEntryInfo(entryInfo);

			if(entryInfo == null)
			{
				_ResetAccordionState();
			}
		}

		public void SetState(bool state)
		{
			if(!_EnsureTitleRootRect())
			{
				LogChannel.UI.W($"{nameof(AccordionSlot)}: title root is not assigned.");

				return;
			}

			if(m_isOn == state)
			{
				return;
			}

			m_isOn = state;

			if(m_duration != 0.0f)
			{
				KZExternalKit.RecycleTokenSource(ref m_tokenSource);

				if(m_isOn)
				{
					var titleSize = _GetTitleSize();
					var expandedSize = _GetExpandedSize();

					_SetLayoutSizeAsync(titleSize,expandedSize,m_tokenSource.Token).Forget();
				}
				else
				{
					var currentSize = GetSlotSize(m_vertical);
					var titleSize = _GetTitleSize();

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
					_SetLayoutSize(_GetTitleSize());
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

		private float _GetTitleSize()
		{
			if(!m_titleRootRect)
			{
				return 0.0f;
			}

			_ForceRebuildLayout(m_titleRootRect);

			var size = m_vertical ? LayoutUtility.GetPreferredHeight(m_titleRootRect) : LayoutUtility.GetPreferredWidth(m_titleRootRect);

			if(size > 0.0f)
			{
				return size;
			}

			var rect = m_titleRootRect.rect;

			return m_vertical ? rect.height : rect.width;
		}

		private float _GetExpandedSize()
		{
			_ForceRebuildLayout(RootRect);

			if(m_vertical)
			{
				var originalHeight = m_layoutElement.preferredHeight;

				m_layoutElement.preferredHeight = -1f;

				var expandedHeight = LayoutUtility.GetPreferredHeight(RootRect);

				m_layoutElement.preferredHeight = originalHeight;

				return expandedHeight;
			}
			else
			{
				var originalWidth = m_layoutElement.preferredWidth;

				m_layoutElement.preferredWidth = -1f;

				var expandedWidth = LayoutUtility.GetPreferredWidth(RootRect);

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

				await KZExternalKit.ExecuteProgressAsync(start,finish,m_duration,_ProgressHeight,false,null,token);
			}
			else
			{
				void _ProgressWidth(float width)
				{
					m_layoutElement.preferredWidth = width;
				}

				await KZExternalKit.ExecuteProgressAsync(start,finish,m_duration,_ProgressWidth,false,null,token);
			}
		}

		protected override void _OnClicked()
		{
			base._OnClicked();

			m_onClicked?.Invoke(this);
		}

		private void _ResetAccordionState()
		{
			KZExternalKit.RecycleTokenSource(ref m_tokenSource);

			m_isOn = false;
			m_onClicked = null;

			if(!m_layoutElement || !_EnsureTitleRootRect())
			{
				return;
			}

			_SetLayoutSize(_GetTitleSize());
		}

		private bool _EnsureTitleRootRect()
		{
			if(m_titleRootRect)
			{
				return true;
			}

			var title = transform.Find(c_titleChildName) as RectTransform;

			if(title)
			{
				m_titleRootRect = title;
			}

			return m_titleRootRect;
		}

		private static void _ForceRebuildLayout(RectTransform rectTrans)
		{
			if(!rectTrans)
			{
				return;
			}

			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTrans);
		}

		private void Reset()
		{
			if(!m_layoutElement)
			{
				m_layoutElement = GetComponent<LayoutElement>();
			}

			_EnsureTitleRootRect();
		}
	}
}