using KZLib.KZDevelop;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropImageUI : BaseImageUI,IDragHandler,IBeginDragHandler,IEndDragHandler
{
	private Canvas m_Canvas = null;

	public MoreAction OnBeginDragHandler { get; set; }
	public MoreAction OnDragHandler { get; set; }
	public MoreAction OnEndDragHandler { get; set; }

	protected override void Initialize()
	{
		base.Initialize();

		m_Canvas = gameObject.GetComponentInParent<Canvas>();
	}

	public void OnDrag(PointerEventData _data)
	{
		if(m_Canvas == null)
		{
			return;
		}

		UIRectTransform.anchoredPosition += _data.delta/m_Canvas.scaleFactor;

		OnDragHandler?.Invoke();
	}

	void IBeginDragHandler.OnBeginDrag(PointerEventData _data)
	{
		transform.SetAsLastSibling();

		OnBeginDragHandler?.Invoke();
	}

	public void OnEndDrag(PointerEventData _data)
	{
		OnEndDragHandler?.Invoke();
	}
}