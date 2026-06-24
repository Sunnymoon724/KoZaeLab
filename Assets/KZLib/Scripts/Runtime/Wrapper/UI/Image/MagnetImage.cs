using System;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Positions this image adjacent to a target <see cref="RectTransform"/> on selected sides, tracking target size each frame.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class MagnetImage : BaseImage
{
	[Flags]
	private enum DirectionType { None = 0x00, Top = 0x01, Left = 0x02, Right = 0x04, Bottom = 0x08, }

	[SerializeField,HideInInspector]
	private float m_space = 0.0f;
	[SerializeField,HideInInspector]
	private RectTransform m_target = null;

	[ShowInInspector]
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
				_OnChangedImage(_CalculateSize(Unit.Default));
			}
		}
	}

	[SerializeField]
	private DirectionType m_direction = DirectionType.None;

	[ShowInInspector]
	private RectTransform Target
	{
		get => m_target;
		set
		{
			var self = m_rootRect ? m_rootRect : GetComponent<RectTransform>();

			if(value && value == self)
			{
				LogChannel.UI.W("Target cannot be self.");

				return;
			}

			m_target = value;
		}
	}

	private bool IsValidTarget => Target && m_direction != DirectionType.None;

	private RectTransform m_rootRect = null;

	protected override void _Initialize()
	{
		base._Initialize();

		if(!IsValidTarget)
		{
			LogChannel.UI.W("MagnetImage: Target or Direction is not assigned.");

			return;
		}

		m_rootRect = GetComponent<RectTransform>();

		Observable.EveryUpdate().Select(_CalculateSize).DistinctUntilChanged().Subscribe(_OnChangedImage).RegisterTo(destroyCancellationToken);
	}
	
	private Vector2 _CalculateSize(Unit _)
	{
		return Target.CalculateWorldSize();
	}

	private void _OnChangedImage(Vector2 targetSize)
	{
		if(!IsValidTarget)
		{
			return;
		}

		var space = Space*m_rootRect.lossyScale.x;
		var pivotSize = m_rootRect.CalculateWorldSize();
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

		m_rootRect.position = position;
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
				LogChannel.UI.E($"{directionType} is not DirectionType.");
				return DirectionType.None;
		}
	}
}