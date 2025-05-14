using System;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

public class MagnetImageUI : BaseImageUI
{
	[Flags]
	private enum DirectionType { None = 0x00, Top = 0x01, Left = 0x02, Right = 0x04, Bottom = 0x08, }

	[SerializeField,HideInInspector]
	private float m_space = 0.0f;
	[SerializeField,HideInInspector]
	private RectTransform m_target = null;

	[ShowInInspector,LabelText("Space")]
	private float Space
	{
		get => m_space;
		set
		{
			if(m_space == value)
			{
				return;
			}

			m_space = value;

			if(Target && IsValidTarget)
			{
				_OnChangedImage(Target.CalculateWorldSize());
			}
		}
	}

	[SerializeField,LabelText("Direction")]
	private DirectionType m_direction = DirectionType.None;

	[ShowInInspector,LabelText("Target")]
	private RectTransform Target
	{
		get => m_target;
		set
		{
			if(this == value)
			{
				KZLogType.UI.W("Target is not self.");

				return;
			}

			m_target = value;
		}
	}

	private bool IsValidTarget => Target && m_direction != DirectionType.None;

	protected override void Initialize()
	{
		base.Initialize();

		if(IsValidTarget)
		{
			KZLogType.UI.E($"Target is null or direction is none {Target} or {m_direction}");

			return;
		}

		Observable.EveryUpdate().Select(_=>Target.CalculateWorldSize()).DistinctUntilChanged().Subscribe(_OnChangedImage);
	}

	private void _OnChangedImage(Vector2 targetSize)
	{
		if(IsValidTarget)
		{
			KZLogType.UI.E($"Target is null or direction is none {Target} or {m_direction}");

			return;
		}

		var space = Space*UIRectTransform.lossyScale.x;
		var pivotSize = UIRectTransform.CalculateWorldSize();
		var position = Target.position;

		var size = new Vector2((targetSize.x+pivotSize.x)/2.0f+space,(targetSize.y+pivotSize.y)/2.0f+space);

		if(_CanMove(DirectionType.Top))
		{
			position += Vector3.up*size.y;
		}
		else if(_CanMove(DirectionType.Bottom))
		{
			position += Vector3.down*size.y;
		}

		if(_CanMove(DirectionType.Left))
		{
			position += Vector3.left*size.x;
		}
		else if(_CanMove(DirectionType.Right))
		{
			position += Vector3.right*size.x;
		}

		UIRectTransform.position = position;
	}

	private bool _CanMove(DirectionType directionType)
	{
		return m_direction.HasFlag(directionType) && !m_direction.HasFlag(_GetOpposite(directionType));
	}

	private DirectionType _GetOpposite(DirectionType directionType)
	{
		switch(directionType)
		{
			case DirectionType.Top:
				return DirectionType.Bottom;
			case DirectionType.Bottom:
				return DirectionType.Top;
			case DirectionType.Left:
				return DirectionType.Right;
			case DirectionType.Right:
				return DirectionType.Left;
			case DirectionType.None:
				return DirectionType.None;

			default:
				KZLogType.UI.E($"DirectionType is not exist {directionType}");
				return DirectionType.None;
		}
	}
}