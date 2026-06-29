using R3;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Draggable <see cref="Image"/> that moves its <see cref="RectTransform"/> with pointer delta. Emits R3 drag lifecycle events.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class DragDropImage : BaseImage,IDragHandler,IBeginDragHandler,IEndDragHandler
{
	private Canvas m_canvas = null;

	private readonly Subject<Unit> m_dragStartSubject = new();
	public Observable<Unit> OnStartedDrag => m_dragStartSubject;

	private readonly Subject<Unit> m_dragChangeSubject = new();
	public Observable<Unit> OnChangedDrag => m_dragChangeSubject;

	private readonly Subject<Unit> m_dragFinishSubject = new();
	public Observable<Unit> OnFinishedDrag => m_dragFinishSubject;

	private RectTransform m_rootRect = null;

	protected override void _Initialize()
	{
		base._Initialize();

		m_canvas = GetComponentInParent<Canvas>();
		m_rootRect = GetComponent<RectTransform>();

		if(!m_canvas)
		{
			LogChannel.UI.W("DragDropImage: Canvas not found in parent hierarchy.");
		}
	}

	protected override void _Release()
	{
		m_dragStartSubject.Dispose();
		m_dragChangeSubject.Dispose();
		m_dragFinishSubject.Dispose();

		base._Release();
	}

	void IDragHandler.OnDrag(PointerEventData eventData)
	{
		if(!m_canvas || !m_rootRect)
		{
			return;
		}

		m_rootRect.anchoredPosition += eventData.delta/m_canvas.scaleFactor;

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
