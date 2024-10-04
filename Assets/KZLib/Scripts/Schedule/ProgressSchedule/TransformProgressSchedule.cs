using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZSchedule
{
	public class TransformProgressSchedule : ProgressSchedule
	{
		[Flags]
		private enum ModeType { None = 0, Position = 1<<0, Rotation = 1<<1, Scale = 1<<2, All = -1 }

		[SerializeField,LabelText("Current Mode")]
		private ModeType m_Mode = ModeType.None;

		private bool IsPosition => m_Mode.HasFlag(ModeType.Position);
		private bool IsRotation => m_Mode.HasFlag(ModeType.Rotation);
		private bool IsScale => m_Mode.HasFlag(ModeType.Scale);

		[BoxGroup("Position"),SerializeField,LabelText("Low Point"),ShowIf(nameof(IsPosition))]
		private Vector3 m_LowPosition = Vector3.zero;
		[BoxGroup("Position"),SerializeField,LabelText("High Point"),ShowIf(nameof(IsPosition))]
		private Vector3 m_HighPosition = Vector3.zero;
		[BoxGroup("Position"),SerializeField,LabelText("Progress Curve"),ShowIf(nameof(IsPosition))]
		private AnimationCurve m_PositionCurve = AnimationCurve.Linear(0.0f,0.0f,1.0f,1.0f);

		[BoxGroup("Rotation"),SerializeField,LabelText("Low Point"),ShowIf(nameof(IsRotation))]
		private Vector3 m_LowRotation = Vector3.zero;
		[BoxGroup("Rotation"),SerializeField,LabelText("High Point"),ShowIf(nameof(IsRotation))]
		private Vector3 m_HighRotation = Vector3.zero;
		[BoxGroup("Rotation"),SerializeField,LabelText("Progress Curve"),ShowIf(nameof(IsRotation))]
		private AnimationCurve m_RotationCurve = AnimationCurve.Linear(0.0f,0.0f,1.0f,1.0f);

		[BoxGroup("Scale"),SerializeField,LabelText("Low Point"),ShowIf(nameof(IsScale))]
		private Vector3 m_LowScale = Vector3.zero;
		[BoxGroup("Scale"),SerializeField,LabelText("High Point"),ShowIf(nameof(IsScale))]
		private Vector3 m_HighScale = Vector3.zero;
		[BoxGroup("Scale"),SerializeField,LabelText("Progress Curve"),ShowIf(nameof(IsScale))]
		private AnimationCurve m_ScaleCurve = AnimationCurve.Linear(0.0f,0.0f,1.0f,1.0f);

		protected override void SetProgress(float _progress)
		{
			if(IsPosition)
			{
				var newPosition = Vector3.Lerp(m_LowPosition, m_HighPosition, m_PositionCurve.Evaluate(_progress));

				if(transform.TryGetComponent<RectTransform>(out var rect))
				{
					rect.anchoredPosition = newPosition;
				}
				else
				{
					transform.position = newPosition;
				}
			}

			if(IsRotation)
			{
				var newRotation = Vector3.Lerp(m_LowRotation, m_HighRotation, m_RotationCurve.Evaluate(_progress));

				transform.localRotation = Quaternion.Euler(newRotation);
			}

			if(IsScale)
			{
				var newScale = Vector3.Lerp(m_LowScale, m_HighScale, m_ScaleCurve.Evaluate(_progress));

				transform.localScale = newScale;
			}
		}
	}
}