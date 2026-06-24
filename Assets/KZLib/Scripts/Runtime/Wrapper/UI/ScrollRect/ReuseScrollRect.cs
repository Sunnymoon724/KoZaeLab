using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using KZLib.Attributes;
using KZLib.UI;
using KZLib.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Virtualized scroll list that pools <see cref="Slot"/> views for <see cref="IEntryInfo"/> entries. Supports smooth scroll-to APIs.
/// </summary>
public class ReuseScrollRect : BaseScrollRect
{
	private enum ScrollToType { Top, Center, Bottom, }

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
	private GameObjectPawnPool<Slot> m_slotPool = null;

	private int m_headIndex = 0;
	private int m_tailIndex = 0;

	private CancellationTokenSource m_tokenSource = null;

	private bool m_initialize = false;

	protected override void _Initialize()
	{
		base._Initialize();

		_EnsureInitialized();
	}

	private bool _EnsureInitialized()
	{
		if(m_initialize)
		{
			return m_slotPool != null;
		}

		if(!m_pivot || !m_scrollRect || !m_scrollRect.viewport || !m_scrollRect.content)
		{
			LogChannel.UI.W("ReuseScrollRect: pivot or scrollRect references are not assigned.");

			return false;
		}

		m_slotPool = new GameObjectPawnPool<Slot>(m_pivot,m_scrollRect.viewport,m_poolCapacity,false);

		var content = m_scrollRect.content;
		var viewport = m_scrollRect.viewport;

		m_pivot.gameObject.EnsureActive(false);
		viewport.transform.SetChild(m_pivot.transform,false);

		viewport.pivot = new Vector2(0.0f,1.0f);

		m_slotSize = m_pivot.GetSlotSize(IsVertical);

		content.pivot = new Vector2(0.0f,1.0f);
		content.anchoredPosition = new Vector2(0.0f,0.0f);

		if(IsVertical)
		{
			m_slotPivot = m_slotSize*content.pivot.y-m_slotSize-m_padding;

			content.anchorMin = new Vector2(content.anchorMin.x,1.0f);
			content.anchorMax = new Vector2(content.anchorMax.x,1.0f);
		}
		else
		{
			m_slotPivot = m_slotSize*content.pivot.x+m_padding;

			content.anchorMin = new Vector2(0.0f,content.anchorMin.y);
			content.anchorMax = new Vector2(0.0f,content.anchorMax.y);
		}

		m_entryInfoList.Clear();
		m_slotDict.Clear();

		m_initialize = true;

		return true;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		if(m_scrollRect)
		{
			m_scrollRect.onValueChanged.AddAction(_OnScrollChanged);
		}
	}

	protected override void OnDisable()
	{
		if(m_scrollRect)
		{
			m_scrollRect.onValueChanged.RemoveAction(_OnScrollChanged);
		}

		KZExternalKit.RecycleTokenSource(ref m_tokenSource);

		base.OnDisable();
	}

	protected override void _Release()
	{
		KZExternalKit.RecycleTokenSource(ref m_tokenSource);
		Clear();

		base._Release();
	}

	/// <summary>Returns pooled slots to the pool and clears entry data.</summary>
	public void Clear()
	{
		m_entryInfoList.Clear();

		if(m_slotPool == null)
		{
			m_slotDict.Clear();

			return;
		}

		foreach(var pair in m_slotDict)
		{
			m_slotPool.Put(pair.Value);
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
		bool _IsMatch(IEntryInfo info) => info.Equals(entryInfo);

		return m_entryInfoList.FindIndex(_IsMatch);
	}

	public void ScrollToBottom(int index,float duration = 0.0f)
	{
		_ScrollTo(index,ScrollToType.Bottom,duration);
	}

	private void _ScrollTo(int index,ScrollToType ScrollToType,float duration = 0.0f,Action onComplete = null)
	{
		if(!_EnsureInitialized())
		{
			return;
		}

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

		KZExternalKit.RecycleTokenSource(ref m_tokenSource);
		_SmoothMoveAsync(current, location, duration, onComplete, m_tokenSource.Token).Forget();
	}

	private async UniTaskVoid _SmoothMoveAsync(float from, float to, float duration, Action onComplete, CancellationToken token)
	{
		float time = 0f;
		while (time < duration)
		{
			time += Time.deltaTime;
			float t = Mathf.Clamp01(time / duration);
			float value = Mathf.Lerp(from, to, t);
			SetContentLocation(value);
			await UniTask.Yield(token);
		}
		SetContentLocation(to);
		onComplete?.Invoke();
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

		location = Mathf.Clamp(location,0.0f,Mathf.Max(0.0f,contentSize-viewportSize));

		return Math.Max(0.0f,location);
	}

	public void SetContentLocation(float location)
	{
		if(!_EnsureInitialized())
		{
			return;
		}

		var content = m_scrollRect.content;

		content.anchoredPosition = IsVertical ? new Vector2(content.anchoredPosition.x,location) : new Vector2(-location,content.anchoredPosition.y);
	}

	private void _OnScrollChanged(Vector2 location)
	{
		_UpdateVisibleSlot(false);
	}

	private void _UpdateVisibleSlot(bool isForce)
	{
		if(!_EnsureInitialized())
		{
			return;
		}

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
				var newSlot = m_slotPool.GetOrCreate(m_scrollRect.content);

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

			var rectTransform = currentSlot.GetComponent<RectTransform>();

			rectTransform.anchoredPosition = _GetSlotLocation(i);

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

			m_slotPool.Put(m_slotDict[i]);

			m_slotDict.Remove(i);
		}
	}

	private float _GetViewportSize()
	{
		var rect = m_scrollRect.viewport.rect;

		return IsVertical ? rect.height : rect.width;
	}

	/// <summary>Replaces all entries, resizes content, and scrolls to <paramref name="index"/> (default top).</summary>
	public void SetEntryInfoList(List<IEntryInfo> entryInfoList,int? index = null)
	{
		if(!_EnsureInitialized() || entryInfoList == null)
		{
			return;
		}

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
		if(!_EnsureInitialized())
		{
			return;
		}

		m_entryInfoList.Add(entryInfo);

		_ResizeContent();

		_UpdateVisibleSlot(true);
	}

	private float _GetContentLocation()
	{
		var anchoredPosition = m_scrollRect.content.anchoredPosition;

		return IsVertical ? anchoredPosition.y : -anchoredPosition.x;
	}

	private int _FindShowHeadIndex()
	{
		var infoCount = m_entryInfoList.Count;
		var contentLocation = _GetContentLocation();
		var location = 0.0f;

		for(var i=0;i<infoCount;i++)
		{
			if(contentLocation <= location + m_slotSize)
			{
				return i;
			}

			location += m_slotSize + m_space;
		}

		return infoCount;
	}

	private int _FindShowTailIndex()
	{
		var infoCount = m_entryInfoList.Count;
		var contentLocation = _GetContentLocation();
		var viewportSize = _GetViewportSize();
		var space = m_slotSize+m_space;
		var location = 0.0f;

		for(var i=0;i<infoCount;i++)
		{
			if(location-contentLocation>=viewportSize)
			{
				return i;
			}

			location += space;
		}

		return infoCount;
	}

	private float _GetContentSize()
	{
		var rect = m_scrollRect.content.rect;

		return IsVertical ? rect.height : rect.width;
	}

	private void _ResizeContent()
	{
		var infoCount = m_entryInfoList.Count;
		var contentSize = infoCount == 0 ? 0.0f : infoCount*m_slotSize+(infoCount-1)*m_space+m_padding*2.0f;
		var content = m_scrollRect.content;

		content.sizeDelta = IsVertical ? new Vector2(content.sizeDelta.x,contentSize) : new Vector2(contentSize,content.sizeDelta.y);
	}

	private Vector2 _GetSlotLocation(int index)
	{
		var size = _GetSizeToIndex(index);

		return IsVertical ? new Vector2(0.0f,m_slotPivot-size) : new Vector2(m_slotPivot+size,0.0f);
	}

	private float _GetSizeToIndex(int index)
	{
		return (m_slotSize+m_space)*index;
	}
}