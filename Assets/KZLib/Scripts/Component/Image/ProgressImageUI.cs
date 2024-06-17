using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class ProgressImageUI : BaseImageUI
{
	[SerializeField,HideInInspector]
	private float m_MinValue = 0.0f;
	[SerializeField,HideInInspector]
	private float m_MaxValue = 1.0f;

	[SerializeField,HideInInspector]
	private float m_CurrentValue = 1.0f;

	[BoxGroup("값",Order = 0),LabelText("최솟 값"),ShowInInspector]
	public float MinValue { get => m_MinValue; private set => m_MinValue = value; }

	[BoxGroup("범위",Order = 0),LabelText("최댓 값"),ShowInInspector]
	public float MaxValue { get => m_MaxValue; private set => m_MaxValue = value; }

	[BoxGroup("값",Order = 2),ShowInInspector,LabelText("현재 값")]
	public float CurrentValue
	{
		get => m_CurrentValue;
		private set
		{
			if(m_CurrentValue == value)
			{
				return;
			}

			m_CurrentValue = Mathf.Clamp(value,MinValue,MaxValue);

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
	public float CurrentProgress => CurrentValue/(MaxValue-MinValue);

	protected override void Awake()
	{
		base.Awake();

		SetValue(MinValue);
	}

	public void SetRange(float _min,float _max)
	{
		MinValue = Mathf.Min(_min,_max);
		MaxValue = Mathf.Max(_min,_max);
	}

	public void SetValue(float _value)
	{
		CurrentValue = _value;
	}

	public void SetValueDuration(float _value,float _duration)
	{
		CommonUtility.ExecuteOverTimeAsync(CurrentValue,_value,_duration,SetValue,false).Forget();
	}

	protected override void Reset() 
	{
		base.Reset();

		m_Image.type = Image.Type.Filled;
		m_Image.fillOrigin = 0;
		m_Image.fillAmount = 0.0f;

		MinValue = 0.0f;
		MaxValue = 1.0f;

		CurrentValue = 0.0f;
	}
}