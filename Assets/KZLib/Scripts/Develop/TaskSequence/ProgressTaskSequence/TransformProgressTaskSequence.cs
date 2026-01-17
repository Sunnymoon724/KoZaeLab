using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.KZDevelop
{
	public class TransformProgressTaskSequence : ProgressTaskSequence
	{
		[Flags]
		private enum ModeType { None = 0, Position = 1<<0, Rotation = 1<<1, Scale = 1<<2, All = -1 }

		[SerializeField]
		private ModeType m_modeType = ModeType.None;

		private bool IsPosition => m_modeType.HasFlag(ModeType.Position);
		private bool IsRotation => m_modeType.HasFlag(ModeType.Rotation);
		private bool IsScale => m_modeType.HasFlag(ModeType.Scale);

		[BoxGroup("Position"),SerializeField,ShowIf(nameof(IsPosition))]
		private Vector3 m_lowPosition = Vector3.zero;
		[BoxGroup("Position"),SerializeField,ShowIf(nameof(IsPosition))]
		private Vector3 m_highPosition = Vector3.zero;
		[BoxGroup("Position"),SerializeField,ShowIf(nameof(IsPosition))]
		private AnimationCurve m_positionCurve = AnimationCurve.Linear(0.0f,0.0f,1.0f,1.0f);

		[BoxGroup("Rotation"),SerializeField,ShowIf(nameof(IsRotation))]
		private Vector3 m_lowRotation = Vector3.zero;
		[BoxGroup("Rotation"),SerializeField,ShowIf(nameof(IsRotation))]
		private Vector3 m_highRotation = Vector3.zero;
		[BoxGroup("Rotation"),SerializeField,ShowIf(nameof(IsRotation))]
		private AnimationCurve m_rotationCurve = AnimationCurve.Linear(0.0f,0.0f,1.0f,1.0f);

		[BoxGroup("Scale"),SerializeField,ShowIf(nameof(IsScale))]
		private Vector3 m_lowScale = Vector3.zero;
		[BoxGroup("Scale"),SerializeField,ShowIf(nameof(IsScale))]
		private Vector3 m_highScale = Vector3.zero;
		[BoxGroup("Scale"),SerializeField,ShowIf(nameof(IsScale))]
		private AnimationCurve m_scaleCurve = AnimationCurve.Linear(0.0f,0.0f,1.0f,1.0f);

		protected override void _SetProgress(float progress)
		{
			if(IsPosition)
			{
				if(transform.TryGetComponent<RectTransform>(out var rectTrans))
				{
					rectTrans.anchoredPosition = Vector3.Lerp(m_lowPosition,m_highPosition,m_positionCurve.Evaluate(progress));
				}
				else
				{
					transform.position = Vector3.Lerp(m_lowPosition,m_highPosition,m_positionCurve.Evaluate(progress));
				}
			}

			if(IsRotation)
			{
				transform.localRotation = Quaternion.Euler(Vector3.Lerp(m_lowRotation,m_highRotation,m_rotationCurve.Evaluate(progress)));
			}

			if(IsScale)
			{
				transform.localScale = Vector3.Lerp(m_lowScale,m_highScale,m_scaleCurve.Evaluate(progress));
			}
		}
	}
}