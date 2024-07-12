using System;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

public class MagnetImageUI : BaseImageUI
{
	[Flags]
	private enum DirectionType { None = 0x00, Top = 0x01, Left = 0x02, Right = 0x04, Bottom = 0x08, }

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
			if(m_Space == value)
			{
				return;
			}

			m_Space = value;

			if(Target && IsValid)
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
			if(this == value)
			{
				LogTag.UI.W("자기 자신을 타겟으로 지정할 수 없습니다.");

				return;
			}

			m_Target = value;
		}
	}

	private bool IsValid => Target && m_Direction != DirectionType.None;

	protected override void Initialize()
	{
		base.Initialize();

		if(IsValid)
		{
			return;
		}

		Observable.EveryUpdate().Select(_=>Target.GetWorldSize()).DistinctUntilChanged().Subscribe(OnChangedImage);
	}

	private void OnChangedImage(Vector2 _targetSize)
	{
		if(IsValid)
		{
			return;
		}

		var space = Space*UIRectTransform.lossyScale.x;
		var pivotSize = UIRectTransform.GetWorldSize();
		var position = Target.position;

		var size = new Vector2((_targetSize.x+pivotSize.x)/2.0f+space,(_targetSize.y+pivotSize.y)/2.0f+space);

		if(m_Direction.HasFlag(DirectionType.Top) && !m_Direction.HasFlag(DirectionType.Bottom))
		{
			position += Vector3.up*size.y;
		}
		else if(m_Direction.HasFlag(DirectionType.Bottom) && !m_Direction.HasFlag(DirectionType.Top))
		{
			position += Vector3.down*size.y;
		}

		if(m_Direction.HasFlag(DirectionType.Left) && !m_Direction.HasFlag(DirectionType.Right))
		{
			position += Vector3.left*size.x;
		}
		else if(m_Direction.HasFlag(DirectionType.Right) && !m_Direction.HasFlag(DirectionType.Left))
		{
			position += Vector3.right*size.x;
		}

		UIRectTransform.position = position;
	}
}