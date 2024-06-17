using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public partial class ScrollRectUI : BaseComponentUI
{
	private enum MoveToType { Top, Center, Bottom, }

	[VerticalGroup("기본",Order = 0),SerializeField]
	private ScrollRect m_ScrollRect = null;

	[VerticalGroup("기본",Order = 0),SerializeField,ReadOnly]
	private bool m_IsVertical = false;

	[VerticalGroup("기본",Order = 0),SerializeField]
	private SlotUI m_Pivot = null;

	[VerticalGroup("기본"),SerializeField,ReadOnly]
	private Dictionary<int,SlotUI> m_SlotDict = new();

	[VerticalGroup("설정",Order = 1),SerializeField]
	private float m_Padding = 0.0f;

	[VerticalGroup("설정",Order = 1),SerializeField]
	private float m_Space = 0.0f;

	private float m_SlotSize = 0.0f;
	private float m_SlotPivot = 0.0f;

	private readonly List<ICellData> m_CellList = new();

	private UIPool m_UIPool = null;

	private int m_HeadIndex = 0;
	private int m_TailIndex = 0;

	private bool m_Initialize = false;

	protected override void Awake()
	{
		base.Awake();

		Initialize();
	}

	private void Initialize()
	{
		if(m_Initialize)
		{
			return;
		}

		if(!m_Pivot)
		{
			throw new NullReferenceException("피벗이 없습니다.");
		}

		m_IsVertical = m_ScrollRect.vertical;

		m_Pivot.gameObject.SetActiveSelf(false);
		m_ScrollRect.viewport.transform.SetUIChild(m_Pivot.transform);

		var content = m_ScrollRect.content;

		m_ScrollRect.content.pivot = new Vector2(0.0f,1.0f);
		m_ScrollRect.content.anchoredPosition = Vector2.zero;

		m_ScrollRect.viewport.pivot = new Vector2(0.0f,1.0f);

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

		m_UIPool = new UIPool(m_Pivot.gameObject,m_ScrollRect.viewport);

		m_CellList.Clear();
		m_SlotDict.Clear();

		m_Initialize = true;
	}

	public void Clear()
	{
		if(!m_Initialize)
		{
			Initialize();
		}

		m_CellList.Clear();

		foreach(var pair in new Dictionary<int,SlotUI>(m_SlotDict))
		{
			CommonUtility.DestroyObject(pair.Value.gameObject);
		}

		m_SlotDict.Clear();
	}

	public void MoveToTop(int _index,float _duration = 0.0f)
	{
		MoveTo(_index,MoveToType.Top,_duration);
	}

	public void MoveToCenter(int _index,float _duration = 0.0f)
	{
		MoveTo(_index,MoveToType.Center,_duration);
	}

	public void MoveToBottom(int _index,float _duration = 0.0f)
	{
		MoveTo(_index,MoveToType.Bottom,_duration);
	}

	public void MoveToTop(ICellData _data,float _duration = 0.0f)
	{
		MoveTo(m_CellList.FindIndex(x=>x.Equals(_data)),MoveToType.Top,_duration);
	}

	public void MoveToCenter(ICellData _data,float _duration = 0.0f)
	{
		MoveTo(m_CellList.FindIndex(x=>x.Equals(_data)),MoveToType.Center,_duration);
	}

	public void MoveToBottom(ICellData _data,float _duration = 0.0f)
	{
		MoveTo(m_CellList.FindIndex(x=>x.Equals(_data)),MoveToType.Bottom,_duration);
	}

	private void MoveTo(int _index,MoveToType _type,float _duration = 0.0f)
	{
		if(!m_Initialize)
		{
			Initialize();
		}

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

		CommonUtility.ExecuteOverTimeAsync(contentLocation,location,_duration,SetScrollLocation).Forget();
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

	private void OnEnable()
	{
		m_ScrollRect.onValueChanged.AddListener(OnScrollChanged);
	}

	private void OnDisable()
	{
		m_ScrollRect.onValueChanged.RemoveListener(OnScrollChanged);
	}

	private void OnScrollChanged(Vector2 _location)
	{
		if(!m_Initialize)
		{
			Initialize();
		}

		UpdateLocation(false);
	}

	private void UpdateLocation(bool _force)
	{
		var headIndex = GetShowHeadIndex();
		var tailIndex = GetShowTailIndex();

		if(m_HeadIndex != headIndex)
		{
			for(var i=m_HeadIndex;i<headIndex;i++)
			{
				if(m_SlotDict.ContainsKey(i))
				{
					m_UIPool.Put(m_SlotDict[i].gameObject);

					m_SlotDict.Remove(i);
				}
			}

			m_HeadIndex = headIndex;
		}

		if(m_TailIndex != tailIndex)
		{
			for(var i=tailIndex+1;i<=m_TailIndex;i++)
			{
				if(m_SlotDict.ContainsKey(i))
				{
					m_UIPool.Put(m_SlotDict[i].gameObject);

					m_SlotDict.Remove(i);
				}
			}

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
				var data = m_UIPool.Get<SlotUI>(m_ScrollRect.content);

				data.gameObject.name = string.Format("Slot_{0}",i);
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

			tailIndex = i;
		}

		if(m_TailIndex != tailIndex)
		{
			for(var i=tailIndex+1;i<=m_TailIndex;i++)
			{
				if(m_SlotDict.ContainsKey(i))
				{
					m_UIPool.Put(m_SlotDict[i].gameObject);

					m_SlotDict.Remove(i);
				}
			}

			m_TailIndex = tailIndex;
		}
	}

	private float GetViewportSize()
	{
		return m_IsVertical ? m_ScrollRect.viewport.rect.height : m_ScrollRect.viewport.rect.width;
	}

	public void SetCellList(List<ICellData> _cellList,int? _index = null)
	{
		if(!m_Initialize)
		{
			Initialize();
		}

		var index = _index.HasValue ? Mathf.Clamp(_index.Value,0,_cellList.Count) : 0;

		m_CellList.Clear();
		m_CellList.AddRange(_cellList);

		ResizeContent();

		m_ScrollRect.StopMovement();
		m_ScrollRect.normalizedPosition = new Vector2(0.0f,1.0f);

		UpdateLocation(true);

		MoveToTop(index);
	}

	public void AddCell(ICellData _cell)
	{
		if(!m_Initialize)
		{
			Initialize();
		}

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
			location += m_SlotSize;

			if(contentLocation<=location)
			{
				return i;
			}

			location += m_Space;
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
		var size = m_CellList.Count*m_SlotSize+(m_CellList.Count-1)*m_Space+m_Padding*2.0f;
		var current = m_ScrollRect.content.sizeDelta;

		m_ScrollRect.content.sizeDelta = m_IsVertical ? new Vector2(current.x,size) : new Vector2(size,current.y);
	}

	private void SetSlotLocation(RectTransform _rect,int _index)
	{
		var size = GetSizeToIndex(_index);
		var location = m_IsVertical ? m_SlotPivot-size : m_SlotPivot+size;

		_rect.anchoredPosition = m_IsVertical ? new Vector2(0.0f,location) : new Vector2(location,0.0f);
	}

	private float GetSizeToIndex(int _index)
	{
		var count = Mathf.Clamp(_index,0,m_CellList.Count-1);
		var result = m_SlotSize*count;

		if(count > 0)
		{
			var space = count;

			if(count == (m_CellList.Count == 0 ? 0 : m_CellList.Count-1))
			{
				space--;
			}

			result += m_Space*space;
		}

		return result;
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