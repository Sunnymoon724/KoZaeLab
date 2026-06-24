using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.Utilities
{
	/// <summary>
	/// Base for visual lerps on renderers.
	/// <see cref="ModeType.Fade"/> builds a fade-in/hold/fade-out alpha curve and locks duration.
	/// <see cref="ModeType.Free"/> allows color/gradient or material float driven by progress.
	/// </summary>
	public abstract class StanzaLerpRender : StanzaLerp
	{
		private enum ModeType { Fade, Free }

		[BoxGroup("Visual",ShowLabel = false,Order = 0),SerializeField]
		private ModeType m_modeType = ModeType.Fade;

		protected bool IsFadeMode => m_modeType == ModeType.Fade;
		protected bool IsFreeMode => m_modeType == ModeType.Free;

		protected override bool DurationLock => IsFadeMode;

		#region Fade Mode
		[SerializeField,HideInInspector]
		private Vector3 m_fadeDuration = Vector3.zero;

		/// <summary>
		/// x = fade-in, y = hold, z = fade-out (seconds).
		/// Rebuilds <see cref="m_fadeCurve"/> and sets total <see cref="StanzaLerp.Duration"/>.
		/// </summary>
		[VerticalGroup("Visual/Fade",Order = 0),ShowInInspector,ShowIf(nameof(IsFadeMode))]
		protected Vector3 FadeDuration
		{
			get => m_fadeDuration;
			set
			{
				m_fadeDuration = value;

				Duration = m_fadeDuration.x+m_fadeDuration.y+m_fadeDuration.z;

				if(Duration.ApproximatelyZero())
				{
					m_fadeCurve = null;

					return;
				}

				var fadeIn = m_fadeDuration.x/Duration;
				var keep = (m_fadeDuration.x+m_fadeDuration.y)/Duration;
				var fadeOut = m_fadeDuration.z/Duration;

				m_fadeCurve = new AnimationCurve(
					new Keyframe(0.0f,0.0f,0.0f,fadeIn.ApproximatelyZero() ? 0.0f : +1.0f/fadeIn),
					new Keyframe(fadeIn,1.0f,fadeIn.ApproximatelyZero() ? 0.0f : +1.0f/fadeIn,0.0f),
					new Keyframe(keep,1.0f,0.0f,fadeOut.ApproximatelyZero() ? 0.0f : -1.0f/fadeOut),
					new Keyframe(1.0f,0.0f,fadeOut.ApproximatelyZero() ? 0.0f : -1.0f/fadeOut,0.0f));
			}
		}

		[SerializeField,HideInInspector]
		private AnimationCurve m_fadeCurve = null;
		#endregion Fade Mode

		#region Free Mode
		[VerticalGroup("Visual/Free",Order = 1),SerializeField,ShowIf(nameof(IsFreeMode))]
		private bool m_useMaterial = false;

		protected bool ShowMaterial => IsFreeMode && m_useMaterial;
		protected bool HideMaterial => IsFreeMode && !m_useMaterial;

		[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,ShowIf(nameof(HideMaterial))]
		private bool m_useGradient = false;

		private bool UseCurve => HideMaterial && !m_useGradient;
		private bool UseGradient => HideMaterial && m_useGradient;

		[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,ShowIf(nameof(UseCurve))]
		private bool m_useOnlyAlpha = false;

		private bool UseColor => UseCurve && !m_useOnlyAlpha;
		private bool UseAlpha => UseCurve && m_useOnlyAlpha;

		[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,ShowIf(nameof(UseColor))]
		private Color m_lowColor = Color.white;
		[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,ShowIf(nameof(UseColor))]
		private Color m_highColor = Color.white;

		[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,ShowIf(nameof(UseAlpha))]
		private float m_lowAlpha = 0.0f;
		[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,ShowIf(nameof(UseAlpha))]
		private float m_highAlpha = 1.0f;

		[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,ShowIf(nameof(UseCurve))]
		private AnimationCurve m_colorCurve = AnimationCurve.Linear(0.0f,0.0f,1.0f,1.0f);

		[VerticalGroup("Visual/Free/Hide",Order = 0),SerializeField,ShowIf(nameof(UseGradient))]
		private Gradient m_gradient = new();

		[VerticalGroup("Visual/Free/Show",Order = 0),SerializeField,ShowIf(nameof(ShowMaterial))]
		protected string m_materialName = "";
		#endregion Free Mode

		/// <summary>Maps progress through the fade curve and applies alpha only.</summary>
		protected Color _GetFadeColor(Color color,float progress)
		{
			if(m_fadeCurve == null)
			{
				return color;
			}

			return color.MaskAlpha(Mathf.Lerp(0.0f,1.0f,m_fadeCurve.Evaluate(progress)));
		}

		/// <summary>Resolves color from curve/lerp or gradient depending on free-mode settings.</summary>
		protected Color _GetColor(Color color,float progress)
		{
			if(UseCurve)
			{
				var value = m_colorCurve.Evaluate(progress);

				return UseColor ? Color.Lerp(m_lowColor,m_highColor,value) : color.MaskAlpha(Mathf.Lerp(m_lowAlpha,m_highAlpha,value));
			}
			else
			{
				return m_gradient.Evaluate(progress);
			}
		}
	}
}
