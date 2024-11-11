using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using KZLib.KZAttribute;
using DG.Tweening;
using System;

public class ProgressImageUI : BaseImageUI
{
	[SerializeField,HideInInspector]
	private float m_CurrentValue = 1.0f;

	[LabelText("Min Value"),SerializeField,KZMaxClamp(nameof(m_MaxValue))]
	private float m_MinValue = 0.0f;

	[LabelText("Max Value"),SerializeField,KZMinClamp(nameof(m_MinValue))]
	private float m_MaxValue = 1.0f;

	[BoxGroup("Value",Order = 2),ShowInInspector,LabelText("Current Value"),PropertyRange(nameof(m_MinValue),nameof(m_MaxValue))]
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

			if(m_UseGradient)
			{
				m_Image.color = m_GradientColor.Evaluate(CurrentProgress);
			}
		}
	}

	[BoxGroup("Option",Order = 1)]
	[HorizontalGroup("Option/0"),LabelText("Use Gradient"),SerializeField,ToggleLeft]
	private bool m_UseGradient = false;
	[HorizontalGroup("Option/0"),HideLabel,SerializeField,ShowIf(nameof(m_UseGradient))]
	private Gradient m_GradientColor = null;

	public float CurrentProgress => (CurrentValue-m_MinValue)/(m_MaxValue-m_MinValue);

	private Tween m_Tween = null;

	protected override void Initialize()
	{
		base.Initialize();

		SetValue(m_MinValue);
	}

	protected override void Release()
	{
		base.Release();

		CommonUtility.KillTween(m_Tween);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		CommonUtility.KillTween(m_Tween);
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

	public void SetValueDuration(float _value,float _duration,Action _onComplete = null)
	{
		CommonUtility.KillTween(m_Tween);

		var amount = _value/m_MaxValue;

		m_Tween = CommonUtility.SetTweenProgress(CurrentValue,_value,_duration,null,_onComplete);

		m_Tween.Play();
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