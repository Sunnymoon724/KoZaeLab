using KZLib;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropImageUI : BaseImageUI,IDragHandler,IBeginDragHandler,IEndDragHandler
{
	private Canvas m_Canvas = null;

	public NewAction onBeginDragHandler = new();
	public NewAction onDragHandler = new();
	public NewAction onEndDragHandler = new();

	protected override void Initialize()
	{
		base.Initialize();

		m_Canvas = gameObject.GetComponentInParent<Canvas>();
	}

	void IDragHandler.OnDrag(PointerEventData _data)
	{
		if(m_Canvas == null)
		{
			return;
		}

		UIRectTransform.anchoredPosition += _data.delta/m_Canvas.scaleFactor;

		onDragHandler?.Invoke();
	}

	void IBeginDragHandler.OnBeginDrag(PointerEventData _data)
	{
		transform.SetAsLastSibling();

		onBeginDragHandler?.Invoke();
	}

	void IEndDragHandler.OnEndDrag(PointerEventData _data)
	{
		onEndDragHandler?.Invoke();
	}
}