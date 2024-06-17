using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 설정한 시간만큼 카운트 한다.
/// </summary>
public class TimerTextUI : BaseTextUI
{
	[SerializeField,LabelText("진행 시간"),MinValue(0.01f)]
	private float m_Duration = 0.01f;
	[SerializeField]
	private bool m_IsCountUp = false;
	[SerializeField]
	private bool m_IsIntOnly = false;

	private CancellationTokenSource m_Source = null;

	private void OnEnable()
	{
		if(m_Source != null)
		{
			m_Source.Cancel();
			m_Source.Dispose();
		}

		m_Source = new();

		PlayTimerText();
	}

	private void OnDisable()
	{
		m_Source?.Cancel();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if(m_Source != null)
		{
			m_Source.Cancel();
			m_Source.Dispose();
		}
	}

	public void PlayTimerText()
	{
		PlayTimerTextAsync().Forget();
	}

	private async UniTask PlayTimerTextAsync()
	{
		var duration = m_IsIntOnly ? Mathf.FloorToInt(m_Duration) : m_Duration;
		var pivot = m_IsIntOnly ? 1.0f : 0.02f;

		for(var i=0.0f;i<duration;i+=pivot)
		{
			m_Text.SetSafeTextMeshPro(string.Format("{0}",m_IsCountUp ? i : duration-i));

			await UniTask.WaitForSeconds(pivot,false,cancellationToken : m_Source.Token);

			if(m_Source.IsCancellationRequested)
			{
				return;
			}
		}
	}
}