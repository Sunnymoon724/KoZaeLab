using KZLib;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropImageUI : BaseImageUI,IDragHandler,IBeginDragHandler,IEndDragHandler
{
	private Canvas m_Canvas = null;

	public NewAction onBeginDrag = new();
	public NewAction onDrag = new();
	public NewAction onEndDrag = new();

	protected override void Initialize()
	{
		base.Initialize();

		m_Canvas = gameObject.GetComponentInParent<Canvas>();
	}

	void IDragHandler.OnDrag(PointerEventData _data)
	{
		if(m_Canvas == null)
		{
			LogTag.UI.E("Canvas is null");

			return;
		}

		UIRectTransform.anchoredPosition += _data.delta/m_Canvas.scaleFactor;

		onDrag?.Invoke();
	}

	void IBeginDragHandler.OnBeginDrag(PointerEventData _data)
	{
		transform.SetAsLastSibling();

		onBeginDrag?.Invoke();
	}

	void IEndDragHandler.OnEndDrag(PointerEventData _data)
	{
		onEndDrag?.Invoke();
	}
}