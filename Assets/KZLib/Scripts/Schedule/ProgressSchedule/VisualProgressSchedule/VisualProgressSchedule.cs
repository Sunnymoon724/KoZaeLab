using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSchedule
{
	public abstract class VisualProgressSchedule : ProgressSchedule
	{
		private enum ModeType { Fade, Free }

		[FoldoutGroup("Visual Option",Order = 0),SerializeField,LabelText("Mode Type"),ValueDropdown(nameof(ModeTypeList))]
		private ModeType m_ModeType = ModeType.Fade;

		protected bool IsFadeMode => m_ModeType == ModeType.Fade;
		protected bool IsFreeMode => m_ModeType == ModeType.Free;

		protected override bool DurationLock => IsFadeMode;

		#region Fade Mode
		[SerializeField,HideInInspector]
		private Vector3 m_FadeDuration = Vector3.zero;

		[VerticalGroup("Visual Option/Fade",Order = 0),ShowInInspector,LabelText("Fade Duration"),ShowIf(nameof(IsFadeMode))]
		protected Vector3 FadeDuration
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
		[VerticalGroup("Visual Option/Free",Order = 1),SerializeField,LabelText("Use Material"),ShowIf(nameof(IsFreeMode))]
		private bool m_UseMaterial = false;

		protected bool ShowMaterial => IsFreeMode && m_UseMaterial;
		protected bool HideMaterial => IsFreeMode && !m_UseMaterial;

		private enum ColorType { AnimationCurve, Gradient };

		[VerticalGroup("Visual Option/Free",Order = 0),SerializeField,LabelText("Use Gradient"),ShowIf(nameof(HideMaterial))]
		private bool m_UseGradient = false;

		private bool UseCurve => HideMaterial && !m_UseGradient;
		private bool UseGradient => HideMaterial && m_UseGradient;

		[VerticalGroup("Visual Option/Free",Order = 0),SerializeField,LabelText("Use Only Alpha"),ShowIf(nameof(UseCurve))]
		private bool m_UseOnlyAlpha = false;

		private bool UseColor => UseCurve && !m_UseOnlyAlpha;
		private bool UseAlpha => UseCurve && m_UseOnlyAlpha;

		[VerticalGroup("Visual Option/Free",Order = 0),SerializeField,LabelText("Low Color"),ShowIf(nameof(UseColor))]
		private Color m_LowColor = Color.white;
		[VerticalGroup("Visual Option/Free",Order = 0),SerializeField,LabelText("High Color"),ShowIf(nameof(UseColor))]
		private Color m_HighColor = Color.white;

		[VerticalGroup("Visual Option/Free",Order = 0),SerializeField,LabelText("Low Alpha"),ShowIf(nameof(UseAlpha))]
		private float m_LowAlpha = 0.0f;
		[VerticalGroup("Visual Option/Free",Order = 0),SerializeField,LabelText("High Alpha"),ShowIf(nameof(UseAlpha))]
		private float m_HighAlpha = 1.0f;

		[VerticalGroup("Visual Option/Free",Order = 0),SerializeField,LabelText("Progress Curve"),ShowIf(nameof(UseCurve))]
		private AnimationCurve m_ColorCurve = AnimationCurve.Linear(0.0f,0.0f,1.0f,1.0f);

		[VerticalGroup("Visual Option/Free",Order = 0),SerializeField,LabelText("Gradient"),ShowIf(nameof(UseGradient))]
		private Gradient m_Gradient = new();

		[VerticalGroup("Visual Option/Free",Order = 0),SerializeField,LabelText("Material Name"),ShowIf(nameof(ShowMaterial))]
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
					{ "Fade Mode",ModeType.Fade },
					{ "Free Mode",ModeType.Free },
				};
			}
		}
	}
}