using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropImageUI : BaseImageUI,IDragHandler,IBeginDragHandler,IEndDragHandler
{
	private Canvas m_canvas = null;

	public event Action OnDragStart = null;
	public event Action OnDragUpdate = null;
	public event Action OnDragEnd = null;

	protected override void Initialize()
	{
		base.Initialize();

		m_canvas = gameObject.GetComponentInParent<Canvas>();
	}

	void IDragHandler.OnDrag(PointerEventData eventData)
	{
		if(m_canvas == null)
		{
			Logger.UI.E("Canvas is null");

			return;
		}

		UIRectTransform.anchoredPosition += eventData.delta/m_canvas.scaleFactor;

		OnDragUpdate?.Invoke();
	}

	void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
	{
		transform.SetAsLastSibling();

		OnDragStart?.Invoke();
	}

	void IEndDragHandler.OnEndDrag(PointerEventData eventData)
	{
		OnDragEnd?.Invoke();
	}
}