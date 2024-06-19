using System;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

public class MagnetImageUI : BaseImageUI
{
	[Flags]
	private enum DirectionType { None = 0, Top = 1 << 0, Left = 1 << 2, Right = 1 << 3, Bottom = 1 << 4, }

	[SerializeField,HideInInspector]
	private float m_Space = 0.0f;
	[SerializeField,HideInInspector]
	private RectTransform m_Target = null;

	[ShowInInspector,LabelText("간격")]
	private float Space
	{
		get => m_Space;
		set
		{
			m_Space = value;

			if(Target != null)
			{
				OnChangedImage(Target.GetWorldSize());
			}
		}
	}

	[SerializeField,LabelText("방향")]
	private DirectionType m_Direction = DirectionType.None;

	[ShowInInspector,LabelText("타겟")]
	private RectTransform Target
	{
		get => m_Target;
		set
		{
			if(value == this)
			{
				Log.System.W("자기 자신을 타겟으로 지정할 수 없습니다.");

				return;
			}

			m_Target = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		if(m_Direction == DirectionType.None)
		{
			return;
		}

		Observable.EveryUpdate().Select(_=>Target.GetWorldSize()).DistinctUntilChanged().Subscribe(OnChangedImage);
	}

	private void OnChangedImage(Vector2 _targetSize)
	{
		var space = Space*UIRectTransform.lossyScale.x;
		var pivotSize = UIRectTransform.GetWorldSize();
		var position = Target.position;

		if(m_Direction.HasFlag(DirectionType.Top))
		{
			position += Vector3.up*((_targetSize.y+pivotSize.y)/2.0f+space);
		}

		if(m_Direction.HasFlag(DirectionType.Bottom))
		{
			position += Vector3.down*((_targetSize.y+pivotSize.y)/2.0f+space);
		}

		if(m_Direction.HasFlag(DirectionType.Left))
		{
			position += Vector3.left*((_targetSize.x+pivotSize.x)/2.0f+space);
		}

		if(m_Direction.HasFlag(DirectionType.Right))
		{
			position += Vector3.right*((_targetSize.x+pivotSize.x)/2.0f+space);
		}

		UIRectTransform.position = position;
	}
}