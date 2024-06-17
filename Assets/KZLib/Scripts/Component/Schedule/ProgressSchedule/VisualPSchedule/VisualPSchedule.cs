using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class VisualPSchedule : ProgressSchedule
{
	private enum ModeType { Fade, Free }

	[BoxGroup("Visual",ShowLabel = false,Order = 0),SerializeField,LabelText("모드 타입"),ValueDropdown(nameof(ModeTypeList))]
	private ModeType m_ModeType = ModeType.Fade;

	protected bool IsFadeMode => m_ModeType == ModeType.Fade;
	protected bool IsFreeMode => m_ModeType == ModeType.Free;

	protected override bool DurationLock => IsFadeMode;

	#region Fade Mode
	[SerializeField,HideInInspector]
	private Vector3 m_FadeDuration = Vector3.zero;

	[VerticalGroup("Visual/Fade",Order = 0),ShowInInspector,LabelText("페이드 시간"),ShowIf(nameof(IsFadeMode))]
	private Vector3 FadeDuration
	{
		get => m_FadeDuration;
		set
		{
			m_FadeDuration = value;

			Duration = m_FadeDuration.x+m_FadeDuration.y+m_FadeDuration.z;

			var fadeIn = m_FadeDuration.x/Duration;
			var keep = (m_FadeDuration.x+m_FadeDuration.y)/Duration;
			var fadeOut = m_FadeDuration.z/Duration;

			m_FadeCurve = new AnimationCurve(
				new Keyframe(0.0f,0.0f,0.0f,+1.0f/fadeIn),
				new Keyframe(fadeIn,1.0f,+1.0f/fadeIn,0.0f),
				new Keyframe(keep,1.0f,0.0f,-1.0f/fadeOut),
				new Keyframe(1.0f,0.0f,-1.0f/fadeOut,0.0f));
		}
	}

	[SerializeField,HideInInspector]
	private AnimationCurve m_FadeCurve = null;
	#endregion Fade Mode

	#region Free Mode
	[VerticalGroup("Visual/Free",Order = 1),SerializeField,LabelText("메터리얼 사용"),ShowIf(nameof(IsFreeMode))]
	private bool m_UseMaterial = false;

	protected bool ShowMaterial => IsFreeMode && m_UseMaterial;
	protected bool HideMaterial => IsFreeMode && !m_UseMaterial;

	private enum ColorType { AnimationCurve, Gradient };

	[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,LabelText("그라데이션 사용"),ShowIf(nameof(HideMaterial))]
	private bool m_UseGradient = false;

	private bool UseCurve => HideMaterial && !m_UseGradient;
	private bool UseGradient => HideMaterial && m_UseGradient;

	[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,LabelText("알파만 사용"),ShowIf(nameof(UseCurve))]
	private bool m_UseOnlyAlpha = false;

	private bool UseColor => UseCurve && !m_UseOnlyAlpha;
	private bool UseAlpha => UseCurve && m_UseOnlyAlpha;

	[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,LabelText("시작 색상"),ShowIf(nameof(UseColor))]
	private Color m_LowColor = Color.white;
	[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,LabelText("종료 색상"),ShowIf(nameof(UseColor))]
	private Color m_HighColor = Color.white;

	[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,LabelText("시작 색상"),ShowIf(nameof(UseAlpha))]
	private float m_LowAlpha = 0.0f;
	[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,LabelText("종료 색상"),ShowIf(nameof(UseAlpha))]
	private float m_HighAlpha = 1.0f;

	[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,LabelText("진행 방법"),ShowIf(nameof(UseCurve))]
	private AnimationCurve m_ColorCurve = AnimationCurve.Linear(0.0f,0.0f,1.0f,1.0f);

	[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,LabelText("진행 색상"),ShowIf(nameof(UseGradient))]
	private Gradient m_Gradient = new();

	[VerticalGroup("Visual/Free/Show",Order = 0),SerializeField,LabelText("머터리얼 변수"),ShowIf(nameof(ShowMaterial))]
	protected string m_MaterialName = "";
	#endregion Free Mode

	protected Color GetFadeColor(Color _color,float _progress)
	{
		return _color.MaskAlpha(Mathf.Lerp(0.0f,1.0f,m_FadeCurve.Evaluate(_progress)));
	}

	protected Color GetColor(Color _color,float _progress)
	{
		if(UseCurve)
		{
			var value = m_ColorCurve.Evaluate(_progress);

			return UseColor ? Color.Lerp(m_LowColor,m_HighColor,value) : _color.MaskAlpha(Mathf.Lerp(m_LowAlpha,m_HighAlpha,value));
		}
		else
		{
			return m_Gradient.Evaluate(_progress);
		}
	}

	private static IEnumerable ModeTypeList
	{
		get
		{
			return new ValueDropdownList<ModeType>
			{
				{ "페이드 모드",ModeType.Fade },
				{ "자유 모드",ModeType.Free },
			};
		}
	}
}