using System;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class MagnetImageUI : BaseImageUI
{
	[Flags]
	private enum DirectionCategory { None = 0, Top = 1 << 0, Left = 1 << 2, Right = 1 << 3, Bottom = 1 << 4, }

	[SerializeField,HideInInspector]
	private float m_Space = 0.0f;

	[ShowInInspector,LabelText("간격")]
	private float Space
	{
		get => m_Space;
		set
		{
			m_Space = value;
			OnChangedImage(m_Target.GetWorldSize());
		}
	}

	[SerializeField,LabelText("방향")]
	private DirectionCategory m_Direction = DirectionCategory.None;

	[SerializeField,LabelText("타겟")]
	private RectTransform m_Target = null;

	protected override void Awake()
	{
		base.Awake();

		if(m_Direction == DirectionCategory.None)
		{
			return;
		}

		this.ObserveEveryValueChanged(_=>m_Target.GetWorldSize()).Subscribe(OnChangedImage);
	}

	private void OnChangedImage(Vector2 _targetSize)
	{
		var space = Space*UIRectTransform.lossyScale.x;
		var pivotSize = UIRectTransform.GetWorldSize();
		var position = m_Target.position;

		if(m_Direction.HasFlag(DirectionCategory.Top))
		{
			position += Vector3.up*((_targetSize.y+pivotSize.y)/2.0f+space);
		}

		if(m_Direction.HasFlag(DirectionCategory.Bottom))
		{
			position += Vector3.down*((_targetSize.y+pivotSize.y)/2.0f+space);
		}

		if(m_Direction.HasFlag(DirectionCategory.Left))
		{
			position += Vector3.left*((_targetSize.x+pivotSize.x)/2.0f+space);
		}

		if(m_Direction.HasFlag(DirectionCategory.Right))
		{
			position += Vector3.right*((_targetSize.x+pivotSize.x)/2.0f+space);
		}

		UIRectTransform.position = position;
	}
}