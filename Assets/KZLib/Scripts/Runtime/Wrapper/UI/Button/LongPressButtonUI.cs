using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Distinguishes short tap vs long-press via pointer down/up. Long-press fires after <see cref="m_longPressDuration"/> seconds.
/// </summary>
public class LongPressButtonUI : BaseButtonUI,IPointerDownHandler,IPointerUpHandler
{
	protected override bool UseButtonClickEvent => false;

	[SerializeField]
	private float m_longPressDuration = 0.5f;

	private Action m_onPressedShort = null;
	private Action<bool> m_onPressedLong = null;

	private bool m_isLongPressed = false;
	private bool m_isPointerDown = false;

	public bool IsLongPressed => m_isLongPressed;

	private CancellationTokenSource m_tokenSource = null;

	protected override void OnDisable()
	{
		KZExternalKit.KillTokenSource(ref m_tokenSource);

		m_isPointerDown = false;
		m_isLongPressed = false;

		base.OnDisable();
	}

	/// <summary>Registers short-tap and long-press callbacks. Long callback receives true on hold, false on release after hold.</summary>
	public void SetClicked(Action onPressedShort,Action<bool> onPressedLong)
	{
		m_onPressedShort = onPressedShort;
		m_onPressedLong = onPressedLong;
	}

	protected override void _OnClickedButton() { }

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
	{
		if(eventData.button != PointerEventData.InputButton.Left)
		{
			return;
		}

		if(!m_button || !m_button.interactable)
		{
			return;
		}

		m_isPointerDown = true;

		KZExternalKit.RecycleTokenSource(ref m_tokenSource);

		_DetectLongTouchAsync(m_tokenSource.Token).Forget();
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
	{
		if(eventData.button != PointerEventData.InputButton.Left || !m_isPointerDown)
		{
			return;
		}

		m_isPointerDown = false;

		bool wasLongPressed = m_isLongPressed;

		KZExternalKit.KillTokenSource(ref m_tokenSource);

		m_isLongPressed = false;

		if(wasLongPressed)
		{
			m_onPressedLong?.Invoke(false);
		}
		else
		{
			m_onPressedShort?.Invoke();
		}
	}

	private async UniTaskVoid _DetectLongTouchAsync(CancellationToken token)
	{
		var duration = Mathf.Max(0.0f,m_longPressDuration);

		if(duration <= 0.0f)
		{
			return;
		}

		var cancelled = await UniTask.Delay(TimeSpan.FromSeconds(duration),cancellationToken : token).SuppressCancellationThrow();

		if(cancelled)
		{
			return;
		}

		m_isLongPressed = true;

		m_onPressedLong?.Invoke(true);
	}
}
