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

	[ShowInInspector,ReadOnly]
	private bool IsVertical => m_scrollRect != null && m_scrollRect.vertical;

	[SerializeField]
	private float m_padding = 0.0f;

	[SerializeField]
	private float m_space = 0.0f;

	[SerializeField,KZMinClamp(1)]
	private int m_poolCapacity = 1;

	[SerializeField]
	private Slot m_pivot = null;

	[SerializeField,ReadOnly]
	private Dictionary<int,Slot> m_slotDict = new();

	private float m_slotSize = 0.0f;
	private float m_slotPivot = 0.0f;

	private readonly List<IEntryInfo> m_entryInfoList = new();

	private GameObjectUIPool<Slot> m_slotUIPool = null;

	private int m_headIndex = 0;
	private int m_tailIndex = 0;

	private Tween m_tween = null;

	private bool m_initialize = false;

	protected override void Initialize()
	{
		base.Initialize();

		_EnsureInitialized();
	}

	private void _EnsureInitialized()
	{
		if(m_initialize)
		{
			return;
		}

		m_slotUIPool = new GameObjectUIPool<Slot>(m_pivot,m_scrollRect.viewport,m_poolCapacity);

		m_pivot.gameObject.EnsureActive(false);
		m_scrollRect.viewport.transform.SetUIChild(m_pivot.transform);

		var content = m_scrollRect.content;

		m_scrollRect.content.pivot = Vector2.up;
		m_scrollRect.content.anchoredPosition = Vector2.zero;

		m_scrollRect.viewport.pivot = Vector2.up;

		if(IsVertical)
		{
			m_slotSize = m_pivot.CurrentRect.rect.height;
			m_slotPivot = m_slotSize*m_scrollRect.content.pivot.y-m_slotSize-m_padding;

			m_scrollRect.content.anchorMin = new Vector2(content.anchorMin.x,1.0f);
			m_scrollRect.content.anchorMax = new Vector2(content.anchorMax.x,1.0f);
		}
		else
		{
			m_slotSize = m_pivot.CurrentRect.rect.width;
			m_slotPivot = m_slotSize*m_scrollRect.content.pivot.x+m_padding;

			m_scrollRect.content.anchorMin = new Vector2(0.0f,content.anchorMin.y);
			m_scrollRect.content.anchorMax = new Vector2(0.0f,content.anchorMax.y);
		}

		m_entryInfoList.Clear();
		m_slotDict.Clear();

		m_initialize = true;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_scrollRect.onValueChanged.AddAction(_OnScrollChanged);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		m_scrollRect.onValueChanged.RemoveAction(_OnScrollChanged);

		CommonUtility.KillTween(m_tween);
	}

	protected override void Release()
	{
		base.Release();

		CommonUtility.KillTween(m_tween);

		Clear();
	}

	public void Clear()
	{
		m_entryInfoList.Clear();

		foreach(var pair in m_slotDict)
		{
			m_slotUIPool.Put(pair.Value);
		}

		m_slotDict.Clear();
	}

	public void ScrollToTop(IEntryInfo entryInfo,float duration = 0.0f)
	{
		ScrollToTop(_FindIndex(entryInfo),duration);
	}

	public void ScrollToTop(int index,float duration = 0.0f)
	{
		_ScrollTo(index,ScrollToType.Top,duration);
	}

	public void ScrollToCenter(IEntryInfo entryInfo,float duration = 0.0f)
	{
		ScrollToCenter(_FindIndex(entryInfo),duration);
	}

	public void ScrollToCenter(int index,float duration = 0.0f)
	{
		_ScrollTo(index,ScrollToType.Center,duration);
	}

	public void ScrollToBottom(IEntryInfo entryInfo,float duration = 0.0f)
	{
		ScrollToBottom(_FindIndex(entryInfo),duration);
	}

	private int _FindIndex(IEntryInfo entryInfo)
	{
		bool _IsMatch(IEntryInfo info)
		{
			return info.Equals(entryInfo);
		}

		return m_entryInfoList.FindIndex(_IsMatch);
	}

	public void ScrollToBottom(int index,float duration = 0.0f)
	{
		_ScrollTo(index,ScrollToType.Bottom,duration);
	}

	private void _ScrollTo(int index,ScrollToType ScrollToType,float duration = 0.0f,Action onComplete = null)
	{
		_EnsureInitialized();

		if(!m_entryInfoList.ContainsIndex(index))
		{
			return;
		}

		var location = _CalculateTargetLocation(index,ScrollToType);

		if(duration <= 0.0f || !gameObject.activeInHierarchy)
		{
			SetContentLocation(location);

			onComplete?.Invoke();

			return;
		}

		var current = _GetContentLocation();

		CommonUtility.KillTween(m_tween);

		m_tween = CommonUtility.SetTweenProgress(current,location,duration,SetContentLocation,onComplete);

		m_tween.Play();
	}

	private float _CalculateTargetLocation(int targetIndex,ScrollToType ScrollToType)
	{
		var viewportSize = _GetViewportSize();
		var contentSize = _GetContentSize();
		var location = _GetSizeToIndex(targetIndex)+m_padding;

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
		_EnsureInitialized();

		m_scrollRect.content.anchoredPosition = IsVertical ? new Vector2(m_scrollRect.content.anchoredPosition.x,location) : new Vector2(-location,m_scrollRect.content.anchoredPosition.y);
	}

	private void _OnScrollChanged(Vector2 location)
	{
		_UpdateVisibleSlot(false);
	}

	private void _UpdateVisibleSlot(bool isForce)
	{
		var headIndex = _FindShowHeadIndex();
		var tailIndex = _FindShowTailIndex();

		if(m_headIndex != headIndex)
        {
            _RemoveInvisibleSlot(m_headIndex,headIndex);

            m_headIndex = headIndex;
        }

		if(m_tailIndex != tailIndex)
		{
			_RemoveInvisibleSlot(tailIndex,m_tailIndex+1);

			m_tailIndex = tailIndex;
		}

		var contentLocation = _GetContentLocation();
		var viewportSize = _GetViewportSize();

		var slotLocation = m_padding;
		var size = m_space+m_slotSize;

		for(var i=0;i<headIndex;i++)
		{
			slotLocation += size;
		}

		for(var i=headIndex;i<m_entryInfoList.Count;i++)
		{
			if(slotLocation-contentLocation >= viewportSize)
			{
				break;
			}

			if(!m_slotDict.ContainsKey(i))
			{
				var newSlot = m_slotUIPool.GetOrCreate(m_scrollRect.content);

				newSlot.gameObject.name = $"Slot_{i}";
				newSlot.gameObject.EnsureActive(true);

				m_slotDict.Add(i,newSlot);

				isForce = true;
			}

			var currentSlot = m_slotDict[i];

			if(isForce)
			{
				currentSlot.SetEntryInfo(m_entryInfoList[i]);
			}

			_SetSlotLocation(currentSlot.CurrentRect,i);

			slotLocation += size;
		}
	}

	private void _RemoveInvisibleSlot(int start,int end)
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

	private float _GetViewportSize()
	{
		return IsVertical ? m_scrollRect.viewport.rect.height : m_scrollRect.viewport.rect.width;
	}

	public void SetEntryInfoList(List<IEntryInfo> entryInfoList,int? index = null)
	{
		_EnsureInitialized();

		var entryInfoIndex = index.HasValue ? Mathf.Clamp(index.Value,0,entryInfoList.Count) : 0;

		m_entryInfoList.Clear();
		m_entryInfoList.AddRange(entryInfoList);

		_ResizeContent();

		m_scrollRect.StopMovement();
		m_scrollRect.normalizedPosition = Vector2.up;

		_UpdateVisibleSlot(true);

		ScrollToTop(entryInfoIndex);
	}

	public void AddEntry(IEntryInfo entryInfo)
	{
		_EnsureInitialized();

		m_entryInfoList.Add(entryInfo);

		_ResizeContent();

		_UpdateVisibleSlot(true);
	}

	private float _GetContentLocation()
	{
		return IsVertical ? m_scrollRect.content.anchoredPosition.y : -m_scrollRect.content.anchoredPosition.x;
	}

	private int _FindShowHeadIndex()
	{
		var contentLocation = _GetContentLocation();
		var location = 0.0f;

		for(var i=0;i<m_entryInfoList.Count;i++)
		{
			if(contentLocation <= location + m_slotSize)
			{
				return i;
			}

			location += m_slotSize + m_space;
		}

		return m_entryInfoList.Count;
	}

	private int _FindShowTailIndex()
	{
		var contentLocation = _GetContentLocation();
		var viewportSize = _GetViewportSize();
		var space = m_slotSize+m_space;
		var location = 0.0f;

		for(var i=0;i<m_entryInfoList.Count;i++)
		{
			if(location-contentLocation>=viewportSize)
			{
				return i;
			}

			location += space;
		}

		return m_entryInfoList.Count;
	}

	private float _GetContentSize()
	{
		return IsVertical ? m_scrollRect.content.rect.height : m_scrollRect.content.rect.width;
	}

	private void _ResizeContent()
	{
		var size = m_entryInfoList.Count == 0 ? 0.0f : m_entryInfoList.Count*m_slotSize+(m_entryInfoList.Count-1)*m_space+m_padding*2.0f;

		m_scrollRect.content.sizeDelta = IsVertical ? new Vector2(m_scrollRect.content.sizeDelta.x,size) : new Vector2(size,m_scrollRect.content.sizeDelta.y);
	}

	private void _SetSlotLocation(RectTransform rectTransform,int index)
	{
		var size = _GetSizeToIndex(index);

		rectTransform.anchoredPosition = IsVertical ? new Vector2(0.0f,m_slotPivot-size) : new Vector2(m_slotPivot+size,0.0f);
	}

	private float _GetSizeToIndex(int index)
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