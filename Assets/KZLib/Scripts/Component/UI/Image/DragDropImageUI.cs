using System;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropImageUI : BaseImageUI,IDragHandler,IBeginDragHandler,IEndDragHandler
{
	private Canvas m_canvas = null;

	private readonly Subject<Unit> m_dragStartSubject = new();
	public Observable<Unit> OnStartedDrag => m_dragStartSubject;

	private readonly Subject<Unit> m_dragChangeSubject = new();
	public Observable<Unit> OnChangedDrag => m_dragChangeSubject;

	private readonly Subject<Unit> m_dragFinishSubject = new();
	public Observable<Unit> OnFinishedDrag => m_dragFinishSubject;

	protected override void Initialize()
	{
		base.Initialize();

		m_canvas = GetComponentInParent<Canvas>();
	}

	void IDragHandler.OnDrag(PointerEventData eventData)
	{
		if(!m_canvas)
		{
			throw new NullReferenceException("Canvas is null");
		}

		UIRectTransform.anchoredPosition += eventData.delta/m_canvas.scaleFactor;

		m_dragChangeSubject.OnNext(Unit.Default);
	}

	void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
	{
		transform.SetAsLastSibling();

		m_dragStartSubject.OnNext(Unit.Default);
	}

	void IEndDragHandler.OnEndDrag(PointerEventData eventData)
	{
		m_dragFinishSubject.OnNext(Unit.Default);
	}
}