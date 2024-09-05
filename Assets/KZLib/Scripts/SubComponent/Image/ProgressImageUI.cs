using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using KZLib.KZAttribute;
using System.Threading;

public class ProgressImageUI : BaseImageUI
{
	[SerializeField,HideInInspector]
	private float m_CurrentValue = 1.0f;

	[LabelText("최솟 값"),SerializeField,KZMaxClamp(nameof(m_MaxValue))]
	private float m_MinValue = 0.0f;

	[LabelText("최댓 값"),SerializeField,KZMinClamp(nameof(m_MinValue))]
	private float m_MaxValue = 1.0f;

	[BoxGroup("값",Order = 2),ShowInInspector,LabelText("현재 값"),PropertyRange(nameof(m_MinValue),nameof(m_MaxValue))]
	public float CurrentValue
	{
		get => m_CurrentValue;
		private set
		{
			if(m_CurrentValue == value)
			{
				return;
			}

			m_CurrentValue = Mathf.Clamp(value,m_MinValue,m_MaxValue);

			if(m_Image)
			{
				m_Image.fillAmount = CurrentProgress;
			}

			if(m_UseColor)
			{
				m_Image.color = m_GradientColor.Evaluate(CurrentProgress);
			}
		}
	}

	[BoxGroup("옵션",Order = 1)]
	[HorizontalGroup("옵션/0"),LabelText("그라데이션 사용"),SerializeField,ToggleLeft]
	private bool m_UseColor = false;
	[HorizontalGroup("옵션/0"),HideLabel,SerializeField,ShowIf(nameof(m_UseColor))]
	private Gradient m_GradientColor = null;

	public float CurrentProgress => (CurrentValue-m_MinValue)/(m_MaxValue-m_MinValue);

	private CancellationTokenSource m_TokenSource = null;

	protected override void Initialize()
	{
		base.Initialize();

		SetValue(m_MinValue);
	}

	protected override void Release()
	{
		base.Release();

		UniTaskUtility.KillTokenSource(ref m_TokenSource);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		UniTaskUtility.KillTokenSource(ref m_TokenSource);
	}

    public void SetRange(float _min,float _max)
	{
		m_MinValue = Mathf.Min(_min,_max);
		m_MaxValue = Mathf.Max(_min,_max);

		CurrentValue = m_MinValue;
	}

	public void SetValue(float _value)
	{
		CurrentValue = _value;
	}

	public void SetValueDuration(float _value,float _duration)
	{
		UniTaskUtility.RecycleTokenSource(ref m_TokenSource);

		UniTaskUtility.ExecuteOverTimeAsync(CurrentValue,_value,_duration,SetValue,false,null,m_TokenSource.Token).Forget();
	}

	protected override void Reset() 
	{
		base.Reset();

		m_Image.type = Image.Type.Filled;
		m_Image.fillMethod = Image.FillMethod.Horizontal;
		m_Image.fillOrigin = 0;
		m_Image.fillAmount = 0.0f;

		m_MinValue = 0.0f;
		m_MaxValue = 1.0f;

		CurrentValue = 0.0f;
	}
}