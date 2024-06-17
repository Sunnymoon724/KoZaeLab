using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropEventUI : BaseEventUI,IDragHandler,IBeginDragHandler,IEndDragHandler
{
	private Canvas m_Canvas = null;

	private bool IsError => m_Canvas == null;

	private Action m_OnStartDrag = null;
	public event Action OnStartDrag
	{
		add { m_OnStartDrag -= value; m_OnStartDrag += value; }
		remove { m_OnStartDrag -= value; }
	}

	private Action m_OnPlayDrag = null;
	public event Action OnPlayDrag
	{
		add { m_OnPlayDrag -= value; m_OnPlayDrag += value; }
		remove { m_OnPlayDrag -= value; }
	}

	private Action m_OnFinishDrag = null;
	public event Action OnFinishDrag
	{
		add { m_OnFinishDrag -= value; m_OnFinishDrag += value; }
		remove { m_OnFinishDrag -= value; }
	}

	protected override void Awake()
	{
		base.Awake();

		m_Canvas = gameObject.GetComponentInParent<Canvas>();
	}

	public void OnDrag(PointerEventData _data)
	{
		if(IsError)
		{
			return;
		}

		UIRectTransform.anchoredPosition += _data.delta/m_Canvas.scaleFactor;

		m_OnPlayDrag?.Invoke();
	}

	public void OnBeginDrag(PointerEventData _data)
	{
		transform.SetAsLastSibling();

		m_OnStartDrag?.Invoke();
	}

	public void OnEndDrag(PointerEventData _data)
	{
		m_OnFinishDrag?.Invoke();
	}
}