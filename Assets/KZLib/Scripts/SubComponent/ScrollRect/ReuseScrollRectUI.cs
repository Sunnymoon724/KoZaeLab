using System;
using System.Collections.Generic;
using DG.Tweening;
using KZLib.KZAttribute;
using KZLib.KZDevelop;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ReuseScrollRectUI : BaseComponentUI
{
	private enum ScrollToType { Top, Center, Bottom, }

	[SerializeField]
	private ScrollRect m_scrollRect = null;

	[SerializeField,ReadOnly]
	private bool m_isVertical = false;

	[SerializeField]
	private float m_padding = 0.0f;

	[SerializeField]
	private float m_space = 0.0f;

	[SerializeField,KZMinClamp(1)]
	private int m_poolCapacity = 1;

	[SerializeField]
	private SlotUI m_pivot = null;

	[SerializeField,ReadOnly]
	private Dictionary<int,SlotUI> m_slotDict = new();

	private float m_slotSize = 0.0f;
	private float m_slotPivot = 0.0f;

	private readonly List<ICellData> m_cellDataList = new();

	private GameObjectUIPool<SlotUI> m_slotUIPool = null;

	private int m_headIndex = 0;
	private int m_tailIndex = 0;

	private Tween m_tween = null;

	protected override void Initialize()
	{
		base.Initialize();

		if(!m_pivot)
		{
			LogTag.System.E("Pivot is null");

			return;
		}

		m_isVertical = m_scrollRect.vertical;

		m_pivot.gameObject.SetActiveIfDifferent(false);
		m_scrollRect.viewport.transform.SetUIChild(m_pivot.transform);

		var content = m_scrollRect.content;

		m_scrollRect.content.pivot = Vector2.up;
		m_scrollRect.content.anchoredPosition = Vector2.zero;

		m_scrollRect.viewport.pivot = Vector2.up;

		if(m_isVertical)
		{
			m_slotSize = m_pivot.UIRectTransform.rect.height;
			m_slotPivot = m_slotSize*m_scrollRect.content.pivot.y-m_slotSize-m_padding;

			m_scrollRect.content.anchorMin = new Vector2(content.anchorMin.x,1.0f);
			m_scrollRect.content.anchorMax = new Vector2(content.anchorMax.x,1.0f);
		}
		else
		{
			m_slotSize = m_pivot.UIRectTransform.rect.width;
			m_slotPivot = m_slotSize*m_scrollRect.content.pivot.x+m_padding;

			m_scrollRect.content.anchorMin = new Vector2(0.0f,content.anchorMin.y);
			m_scrollRect.content.anchorMax = new Vector2(0.0f,content.anchorMax.y);
		}

		m_slotUIPool = new GameObjectUIPool<SlotUI>(m_pivot,m_scrollRect.viewport,m_poolCapacity);

		m_cellDataList.Clear();
		m_slotDict.Clear();
	}

	public void Clear()
	{
		m_cellDataList.Clear();

		foreach(var slot in new List<SlotUI>(m_slotDict.Values))
		{
			m_slotUIPool.Put(slot);
		}

		m_slotDict.Clear();
	}

	public void ScrollToTop(ICellData cellData,float duration = 0.0f)
	{
		ScrollToTop(m_cellDataList.FindIndex(x=>x.Equals(cellData)),duration);
	}

	public void ScrollToTop(int index,float duration = 0.0f)
	{
		ScrollTo(index,ScrollToType.Top,duration);
	}

	public void ScrollToCenter(ICellData cellData,float duration = 0.0f)
	{
		ScrollToCenter(m_cellDataList.FindIndex(x=>x.Equals(cellData)),duration);
	}

	public void ScrollToCenter(int index,float duration = 0.0f)
	{
		ScrollTo(index,ScrollToType.Center,duration);
	}

	public void ScrollToBottom(ICellData cellData,float duration = 0.0f)
	{
		ScrollToBottom(m_cellDataList.FindIndex(x=>x.Equals(cellData)),duration);
	}

	public void ScrollToBottom(int index,float duration = 0.0f)
	{
		ScrollTo(index,ScrollToType.Bottom,duration);
	}

	private void ScrollTo(int index,ScrollToType ScrollToType,float duration = 0.0f,Action onComplete = null)
	{
		if(!m_cellDataList.ContainsIndex(index))
		{
			return;
		}

		var location = CalculateTargetLocation(index,ScrollToType);

		if(duration <= 0.0f || !gameObject.activeInHierarchy)
		{
			SetContentLocation(location);

			onComplete?.Invoke();

			return;
		}

		var current = GetContentLocation();

		CommonUtility.KillTween(m_tween);

		m_tween = CommonUtility.SetTweenProgress(current,location,duration,SetContentLocation,onComplete);

		m_tween.Play();
	}

	private float CalculateTargetLocation(int targetIndex,ScrollToType ScrollToType)
	{
		var viewportSize = GetViewportSize();
		var contentSize = GetContentSize();
		var location = GetSizeToIndex(targetIndex)+m_padding;

		if(ScrollToType == ScrollToType.Center)
		{
			location -= viewportSize*0.5f-m_slotSize*0.5f;
		}
		else if(ScrollToType == ScrollToType.Bottom)
		{
			location -= viewportSize-m_slotSize;
		}

		location = Mathf.Clamp(location,0.0f,contentSize-viewportSize);

		return Math.Max(0.0f,location);
	}

	public void SetContentLocation(float location)
	{
		m_scrollRect.content.anchoredPosition = m_isVertical ? new Vector2(m_scrollRect.content.anchoredPosition.x,location) : new Vector2(-location,m_scrollRect.content.anchoredPosition.y);
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_scrollRect.onValueChanged.AddAction(OnScrollChanged);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		m_scrollRect.onValueChanged.RemoveAction(OnScrollChanged);

		CommonUtility.KillTween(m_tween);
	}

	protected override void Release()
	{
		base.Release();

		CommonUtility.KillTween(m_tween);

		Clear();
	}

	private void OnScrollChanged(Vector2 location)
	{
		UpdateVisibleSlot(false);
	}

	private void UpdateVisibleSlot(bool isForce)
	{
		var headIndex = FindShowHeadIndex();
		var tailIndex = FindShowTailIndex();

		if(m_headIndex != headIndex)
        {
            RemoveInvisibleSlot(m_headIndex,headIndex);

            m_headIndex = headIndex;
        }

		if(m_tailIndex != tailIndex)
		{
			RemoveInvisibleSlot(tailIndex,m_tailIndex+1);

			m_tailIndex = tailIndex;
		}

		var contentLocation = GetContentLocation();
		var viewportSize = GetViewportSize();

		var slotLocation = m_padding;
		var size = m_space+m_slotSize;

		for(var i=0;i<headIndex;i++)
		{
			slotLocation += size;
		}

		for(var i=headIndex;i<m_cellDataList.Count;i++)
		{
			if(slotLocation-contentLocation >= viewportSize)
			{
				break;
			}

			if(!m_slotDict.ContainsKey(i))
			{
				var data = m_slotUIPool.GetOrCreate(m_scrollRect.content);

				data.gameObject.name = $"Slot_{i}";
				data.gameObject.SetActiveIfDifferent(true);

				m_slotDict.Add(i,data);

				isForce = true;
			}

			var slot = m_slotDict[i];

			if(isForce)
			{
				slot.SetCell(m_cellDataList[i]);
			}

			SetSlotLocation(slot.UIRectTransform,i);

			slotLocation += size;
		}
	}

	private void RemoveInvisibleSlot(int start,int end)
	{
		for(var i=start;i<end;i++)
		{
			if(!m_slotDict.ContainsKey(i))
			{
				continue;
			}

			m_slotUIPool.Put(m_slotDict[i]);

			m_slotDict.Remove(i);
		}
	}

	private float GetViewportSize()
	{
		return m_isVertical ? m_scrollRect.viewport.rect.height : m_scrollRect.viewport.rect.width;
	}

	public void SetCellList(List<ICellData> cellDataList,int? index = null)
	{
		var cellIndex = index.HasValue ? Mathf.Clamp(index.Value,0,cellDataList.Count) : 0;

		m_cellDataList.Clear();
		m_cellDataList.AddRange(cellDataList);

		ResizeContent();

		m_scrollRect.StopMovement();
		m_scrollRect.normalizedPosition = Vector2.up;

		UpdateVisibleSlot(true);

		ScrollToTop(cellIndex);
	}

	public void AddCell(ICellData cellData)
	{
		m_cellDataList.Add(cellData);

		ResizeContent();

		UpdateVisibleSlot(true);
	}

	private float GetContentLocation()
	{
		return m_isVertical ? m_scrollRect.content.anchoredPosition.y : -m_scrollRect.content.anchoredPosition.x;
	}

	private int FindShowHeadIndex()
	{
		var contentLocation = GetContentLocation();
		var location = 0.0f;

		for(var i=0;i<m_cellDataList.Count;i++)
		{
			if(contentLocation <= location + m_slotSize)
			{
				return i;
			}

			location += m_slotSize + m_space;
		}

		return m_cellDataList.Count;
	}

	private int FindShowTailIndex()
	{
		var contentLocation = GetContentLocation();
		var viewportSize = GetViewportSize();
		var space = m_slotSize+m_space;
		var location = 0.0f;

		for(var i=0;i<m_cellDataList.Count;i++)
		{
			if(location-contentLocation>=viewportSize)
			{
				return i;
			}

			location += space;
		}

		return m_cellDataList.Count;
	}

	private float GetContentSize()
	{
		return m_isVertical ? m_scrollRect.content.rect.height : m_scrollRect.content.rect.width;
	}

	private void ResizeContent()
	{
		var size = m_cellDataList.Count == 0 ? 0.0f : m_cellDataList.Count*m_slotSize+(m_cellDataList.Count-1)*m_space+m_padding*2.0f;

		m_scrollRect.content.sizeDelta = m_isVertical ? new Vector2(m_scrollRect.content.sizeDelta.x,size) : new Vector2(size,m_scrollRect.content.sizeDelta.y);
	}

	private void SetSlotLocation(RectTransform rectTransform,int index)
	{
		var size = GetSizeToIndex(index);

		rectTransform.anchoredPosition = m_isVertical ? new Vector2(0.0f,m_slotPivot-size) : new Vector2(m_slotPivot+size,0.0f);
	}

	private float GetSizeToIndex(int index)
	{
		return (m_slotSize+m_space)*index;
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_scrollRect)
		{
			m_scrollRect = GetComponent<ScrollRect>();
		}
	}
}