using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongPressButtonUI : BaseButtonUI,IPointerDownHandler,IPointerUpHandler
{
	[SerializeField]
	private float m_longPressDuration = 0.5f;

	private Action m_onPressedShort = null;
	private Action<bool> m_onPressedLong = null;

	private bool m_isLongPressed = false;

	public bool IsLongPressed => m_isLongPressed;

	private CancellationTokenSource m_tokenSource = null;

	protected override void OnDisable()
	{
		base.OnDisable();

		CommonUtility.KillTokenSource(ref m_tokenSource); 
	}

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

		if(!m_button.interactable)
		{
			return;
		}

		CommonUtility.RecycleTokenSource(ref m_tokenSource);

		_DetectLongTouchAsync().Forget();
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
	{
		if(eventData.button != PointerEventData.InputButton.Left)
		{
			return;
		}

		bool wasLongPressed = m_isLongPressed;

		CommonUtility.KillTokenSource(ref m_tokenSource);

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

	private async UniTaskVoid _DetectLongTouchAsync()
	{
		try
		{
			await UniTask.Delay(TimeSpan.FromSeconds(m_longPressDuration), cancellationToken : m_tokenSource.Token );

			m_isLongPressed = true;

			m_onPressedLong?.Invoke(true);
		}
		catch(OperationCanceledException) { }
	}
}