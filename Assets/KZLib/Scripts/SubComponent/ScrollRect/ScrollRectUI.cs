using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public partial class ScrollRectUI : BaseComponentUI
{
	private enum MoveToType { Top, Center, Bottom, }

	[SerializeField,LabelText("Scroll Rect")]
	private ScrollRect m_ScrollRect = null;

	[SerializeField,ReadOnly,LabelText("Is Vertical")]
	private bool m_IsVertical = false;

	[SerializeField,LabelText("Padding")]
	private float m_Padding = 0.0f;

	[SerializeField,LabelText("Space")]
	private float m_Space = 0.0f;

	[SerializeField,LabelText("Pivot")]
	private SlotUI m_Pivot = null;

	[SerializeField,ReadOnly,LabelText("Slot Dictionary")]
	private Dictionary<int,SlotUI> m_SlotDict = new();

	private float m_SlotSize = 0.0f;
	private float m_SlotPivot = 0.0f;

	private readonly List<ICellData> m_CellList = new();

	private GameObjectUIPool<SlotUI> m_ObjectPool = null;

	private int m_HeadIndex = 0;
	private int m_TailIndex = 0;

	private bool m_Initialize = false;

	protected override void Initialize()
	{
		if(m_Initialize)
		{
			return;
		}

		base.Initialize();

		if(!m_Pivot)
		{
			throw new NullReferenceException("Pivot is null");
		}

		m_IsVertical = m_ScrollRect.vertical;

		m_Pivot.gameObject.SetActiveSelf(false);
		m_ScrollRect.viewport.transform.SetUIChild(m_Pivot.transform);

		var content = m_ScrollRect.content;

		m_ScrollRect.content.pivot = Vector2.up;
		m_ScrollRect.content.anchoredPosition = Vector2.zero;

		m_ScrollRect.viewport.pivot = Vector2.up;

		if(m_IsVertical)
		{
			m_SlotSize = m_Pivot.UIRectTransform.rect.height;
			m_SlotPivot = m_SlotSize*m_ScrollRect.content.pivot.y-m_SlotSize-m_Padding;

			m_ScrollRect.content.anchorMin = new Vector2(content.anchorMin.x,1.0f);
			m_ScrollRect.content.anchorMax = new Vector2(content.anchorMax.x,1.0f);
		}
		else
		{
			m_SlotSize = m_Pivot.UIRectTransform.rect.width;
			m_SlotPivot = m_SlotSize*m_ScrollRect.content.pivot.x+m_Padding;

			m_ScrollRect.content.anchorMin = new Vector2(0.0f,content.anchorMin.y);
			m_ScrollRect.content.anchorMax = new Vector2(0.0f,content.anchorMax.y);
		}

		m_ObjectPool = new GameObjectUIPool<SlotUI>(m_Pivot,m_ScrollRect.viewport);

		m_CellList.Clear();
		m_SlotDict.Clear();

		m_Initialize = true;
	}

	public void Clear()
	{
		Initialize();

		m_CellList.Clear();

		foreach(var slot in new List<SlotUI>(m_SlotDict.Values))
		{
			m_ObjectPool.Put(slot);
		}

		m_SlotDict.Clear();
	}

	public void MoveToTop(ICellData _data,float _duration = 0.0f)
	{
		MoveToTop(m_CellList.FindIndex(x=>x.Equals(_data)),_duration);
	}

	public void MoveToTop(int _index,float _duration = 0.0f)
	{
		MoveTo(_index,MoveToType.Top,_duration);
	}

	public void MoveToCenter(ICellData _data,float _duration = 0.0f)
	{
		MoveToCenter(m_CellList.FindIndex(x=>x.Equals(_data)),_duration);
	}

	public void MoveToCenter(int _index,float _duration = 0.0f)
	{
		MoveTo(_index,MoveToType.Center,_duration);
	}

	public void MoveToBottom(ICellData _data,float _duration = 0.0f)
	{
		MoveToBottom(m_CellList.FindIndex(x=>x.Equals(_data)),_duration);
	}

	public void MoveToBottom(int _index,float _duration = 0.0f)
	{
		MoveTo(_index,MoveToType.Bottom,_duration);
	}

	private void MoveTo(int _index,MoveToType _type,float _duration = 0.0f)
	{
		Initialize();

		if(!m_CellList.ContainsIndex(_index))
		{
			return;
		}

		var location = FindReachLocation(_index,_type);

		if(_duration <= 0.0f || !gameObject.activeInHierarchy)
		{
			SetScrollLocation(location);

			return;
		}

		var contentLocation = GetContentLocation();

		UniTaskUtility.ExecuteOverTimeAsync(contentLocation,location,_duration,SetScrollLocation).Forget();
	}

	private float FindReachLocation(int _index,MoveToType _type)
	{
		var viewportSize = GetViewportSize();
		var contentSize = GetContentSize();
		var location = GetSizeToIndex(_index)+m_Padding;

		if(_type == MoveToType.Center)
		{
			location -= viewportSize*0.5f-m_SlotSize*0.5f;
		}
		else if(_type == MoveToType.Bottom)
		{
			location -= viewportSize-m_SlotSize;
		}

		location = Mathf.Clamp(location,0.0f,contentSize-viewportSize);

		return Math.Max(0.0f,location);
	}

	public void SetScrollLocation(float _location)
	{
		m_ScrollRect.content.anchoredPosition = m_IsVertical ? new Vector2(m_ScrollRect.content.anchoredPosition.x,_location) : new Vector2(-_location,m_ScrollRect.content.anchoredPosition.y);
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		m_ScrollRect.AddListener(OnScrollChanged);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		m_ScrollRect.RemoveListener(OnScrollChanged);
	}

	private void OnScrollChanged(Vector2 _location)
	{
		Initialize();

		UpdateLocation(false);
	}

	private void UpdateLocation(bool _force)
	{
		var headIndex = GetShowHeadIndex();
		var tailIndex = GetShowTailIndex();

		if(m_HeadIndex != headIndex)
        {
            RemoveSlotInDict(m_HeadIndex,headIndex);

            m_HeadIndex = headIndex;
        }

		if(m_TailIndex != tailIndex)
		{
			RemoveSlotInDict(tailIndex,m_TailIndex+1);

			m_TailIndex = tailIndex;
		}

		var contentLocation = GetContentLocation();
		var viewportSize = GetViewportSize();

		var slotLocation = m_Padding;
		var size = m_Space+m_SlotSize;

		for(var i=0;i<headIndex;i++)
		{
			slotLocation += size;
		}

		for(var i=headIndex;i<m_CellList.Count;i++)
		{
			if(slotLocation-contentLocation >= viewportSize)
			{
				break;
			}

			if(!m_SlotDict.ContainsKey(i))
			{
				var data = m_ObjectPool.Get(m_ScrollRect.content);

				data.gameObject.name = $"Slot_{i}";
				data.gameObject.SetActiveSelf(true);

				m_SlotDict.Add(i,data);

				_force = true;
			}

			var slot = m_SlotDict[i];

			if(_force)
			{
				slot.SetCell(m_CellList[i]);
			}

			SetSlotLocation(slot.UIRectTransform,i);

			slotLocation += size;
		}
	}

	private void RemoveSlotInDict(int _start,int _finish)
	{
		for(var i=_start;i<_finish;i++)
		{
			if(!m_SlotDict.ContainsKey(i))
			{
				continue;
			}

			m_ObjectPool.Put(m_SlotDict[i]);

			m_SlotDict.Remove(i);
		}
	}

	private float GetViewportSize()
	{
		return m_IsVertical ? m_ScrollRect.viewport.rect.height : m_ScrollRect.viewport.rect.width;
	}

	public void SetCellList(List<ICellData> _cellList,int? _index = null)
	{
		Initialize();

		var index = _index.HasValue ? Mathf.Clamp(_index.Value,0,_cellList.Count) : 0;

		m_CellList.Clear();
		m_CellList.AddRange(_cellList);

		ResizeContent();

		m_ScrollRect.StopMovement();
		m_ScrollRect.normalizedPosition = Vector2.up;

		UpdateLocation(true);

		MoveToTop(index);
	}

	public void AddCell(ICellData _cell)
	{
		Initialize();

		m_CellList.Add(_cell);

		ResizeContent();

		UpdateLocation(true);
	}

	private float GetContentLocation()
	{
		return m_IsVertical ? m_ScrollRect.content.anchoredPosition.y : -m_ScrollRect.content.anchoredPosition.x;
	}

	private int GetShowHeadIndex()
	{
		var contentLocation = GetContentLocation();
		var location = 0.0f;

		for(var i=0;i<m_CellList.Count;i++)
		{
			if(contentLocation <= location + m_SlotSize)
			{
				return i;
			}

			location += m_SlotSize + m_Space;
		}

		return m_CellList.Count;
	}

	private int GetShowTailIndex()
	{
		var contentLocation = GetContentLocation();
		var viewportSize = GetViewportSize();
		var space = m_SlotSize+m_Space;
		var location = 0.0f;

		for(var i=0;i<m_CellList.Count;i++)
		{
			if(location-contentLocation>=viewportSize)
			{
				return i;
			}

			location += space;
		}

		return m_CellList.Count;
	}

	private float GetContentSize()
	{
		return m_IsVertical ? m_ScrollRect.content.rect.height : m_ScrollRect.content.rect.width;
	}

	private void ResizeContent()
	{
		var size = m_CellList.Count == 0 ? 0.0f : m_CellList.Count*m_SlotSize+(m_CellList.Count-1)*m_Space+m_Padding*2.0f;

		m_ScrollRect.content.sizeDelta = m_IsVertical ? new Vector2(m_ScrollRect.content.sizeDelta.x,size) : new Vector2(size,m_ScrollRect.content.sizeDelta.y);
	}

	private void SetSlotLocation(RectTransform _rect,int _index)
	{
		var size = GetSizeToIndex(_index);

		_rect.anchoredPosition = m_IsVertical ? new Vector2(0.0f,m_SlotPivot-size) : new Vector2(m_SlotPivot+size,0.0f);
	}

	private float GetSizeToIndex(int _index)
	{
		return (m_SlotSize+m_Space)*_index;
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_ScrollRect)
		{
			m_ScrollRect = GetComponent<ScrollRect>();
		}
	}
}