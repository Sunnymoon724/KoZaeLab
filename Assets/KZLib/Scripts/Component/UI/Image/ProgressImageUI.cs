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

	[SerializeField,KZMaxClamp(nameof(m_maxValue))]
	private float m_minValue = 0.0f;

	[SerializeField,KZMinClamp(nameof(m_minValue))]
	private float m_maxValue = 1.0f;

	[BoxGroup("Value",Order = 2),ShowInInspector,PropertyRange(nameof(m_minValue),nameof(m_maxValue))]
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
	[HorizontalGroup("Option/0"),SerializeField,ToggleLeft]
	private bool m_useGradient = false;
	[HorizontalGroup("Option/0"),HideLabel,SerializeField,ShowIf(nameof(m_useGradient))]
	private Gradient m_gradientColor = null;

	public float CurrentProgress => (CurrentValue-m_minValue)/(m_maxValue-m_minValue);

	private Tween m_tween = null;

	protected override void Initialize()
	{
		base.Initialize();

		SetValue(m_minValue);
	}

	protected override void Release()
	{
		base.Release();

		_KillTween();
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		_KillTween();
	}

    public void SetRange(float minValue,float maxValue)
	{
		m_minValue = Mathf.Min(minValue,maxValue);
		m_maxValue = Mathf.Max(minValue,maxValue);

		CurrentValue = m_minValue;
	}

	public void SetValue(float value)
	{
		CurrentValue = value;
	}

	public void SetValueDuration(float value,float duration,Action onComplete = null)
	{
		_KillTween();

		var amount = value/m_maxValue;

		m_tween = CommonUtility.SetTweenProgress(CurrentValue,value,duration,null,onComplete);

		m_tween.Play();
	}

	private void _KillTween()
	{
		CommonUtility.KillTween(m_tween);
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