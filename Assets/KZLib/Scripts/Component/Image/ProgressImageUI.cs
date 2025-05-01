using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using KZLib.KZAttribute;
using DG.Tweening;
using System;

public class ProgressImageUI : BaseImageUI
{
	[SerializeField,HideInInspector]
	private float m_currentValue = 1.0f;

	[LabelText("Min Value"),SerializeField,KZMaxClamp(nameof(m_maxValue))]
	private float m_minValue = 0.0f;

	[LabelText("Max Value"),SerializeField,KZMinClamp(nameof(m_minValue))]
	private float m_maxValue = 1.0f;

	[BoxGroup("Value",Order = 2),ShowInInspector,LabelText("Current Value"),PropertyRange(nameof(m_minValue),nameof(m_maxValue))]
	public float CurrentValue
	{
		get => m_currentValue;
		private set
		{
			if(m_currentValue == value)
			{
				return;
			}

			m_currentValue = Mathf.Clamp(value,m_minValue,m_maxValue);

			if(m_image)
			{
				m_image.fillAmount = CurrentProgress;
			}

			if(m_useGradient)
			{
				m_image.color = m_gradientColor.Evaluate(CurrentProgress);
			}
		}
	}

	[BoxGroup("Option",Order = 1)]
	[HorizontalGroup("Option/0"),LabelText("Use Gradient"),SerializeField,ToggleLeft]
	private bool m_useGradient = false;
	[HorizontalGroup("Option/0"),HideLabel,SerializeField,ShowIf(nameof(m_useGradient))]
	private Gradient m_gradientColor = null;

	public float CurrentProgress => (CurrentValue-m_minValue)/(m_maxValue-m_minValue);

	private Tween m_Tween = null;

	protected override void Initialize()
	{
		base.Initialize();

		SetValue(m_minValue);
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

    public void SetRange(float minValue,float maxValue)
	{
		m_minValue = Mathf.Min(minValue,maxValue);
		m_maxValue = Mathf.Max(minValue,maxValue);

		CurrentValue = m_minValue;
	}

	public void SetValue(float _value)
	{
		CurrentValue = _value;
	}

	public void SetValueDuration(float value,float duration,Action onComplete = null)
	{
		CommonUtility.KillTween(m_Tween);

		var amount = value/m_maxValue;

		m_Tween = CommonUtility.SetTweenProgress(CurrentValue,value,duration,null,onComplete);

		m_Tween.Play();
	}

	protected override void Reset() 
	{
		base.Reset();

		m_image.type = Image.Type.Filled;
		m_image.fillMethod = Image.FillMethod.Horizontal;
		m_image.fillOrigin = 0;
		m_image.fillAmount = 0.0f;

		m_minValue = 0.0f;
		m_maxValue = 1.0f;

		CurrentValue = 0.0f;
	}
}