using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using KZLib.KZAttribute;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace KZLib.KZDevelop
{
	public partial class FocusScroller : BaseComponentUI,IPointerUpHandler,IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler,IScrollHandler
	{
		[FoldoutGroup("기본 설정",Order = 0)]
		[VerticalGroup("기본 설정/0",Order = 0),SerializeField,LabelText("뷰 포트")]
		private RectTransform m_Viewport = null;

		[VerticalGroup("기본 설정/0",Order = 0),SerializeField,LabelText("슬롯")]
		private FocusSlotUI m_Slot = null;

		[PropertySpace(10)]

		[VerticalGroup("기본 설정/0",Order = 0),SerializeField,LabelText("순환 모드")]
		private bool m_CircularMode = true;
		[VerticalGroup("기본 설정/0",Order = 0),SerializeField,LabelText("정렬 모드")]
		private bool m_OrderMode = false;
		[VerticalGroup("기본 설정/0",Order = 0),SerializeField,LabelText("슬롯 간격"),Range(0.01f,0.5f)]
		private float m_SlotSpace = 0.1f;

		[VerticalGroup("기본 설정/0",Order = 0),SerializeField,LabelText("세로 모드")]
		private bool m_Vertical = false;

		[VerticalGroup("기본 설정/0",Order = 0),SerializeField,LabelText("드래그 사용")]
		private bool m_UseDrag = true;

		[FoldoutGroup("버튼 설정",Order = 15)]
		[VerticalGroup("버튼 설정/0",Order = 0),SerializeField,LabelText("버튼 사용")]
		private bool m_UseButton = false;
		[VerticalGroup("버튼 설정/0",Order = 0),SerializeField,LabelText("버튼 진행 시간"),ShowIf(nameof(m_UseButton))]
		private float m_ButtonClickDuration = 0.35f;

		[VerticalGroup("버튼 설정/0",Order = 0),SerializeField,LabelText("이전 버튼"),ShowIf(nameof(m_UseButton))]
		private Button m_PrevButton = null;
		[VerticalGroup("버튼 설정/0",Order = 0),SerializeField,LabelText("이후 버튼"),ShowIf(nameof(m_UseButton))]
		private Button m_NextButton = null;

		[FoldoutGroup("페이드 설정",Order = 17)]
		[VerticalGroup("페이드 설정/0",Order = 0),SerializeField,LabelText("페이드 사용")]
		private bool m_UseFade = false;

		[VerticalGroup("페이드 설정/0",Order = 0),SerializeField,LabelText("페이드 인 시간 (아웃은 마그넷 시간을 따라감)"),ShowIf(nameof(m_UseFade))]
		private float m_FadeInDuration = 0.3f;

		[VerticalGroup("페이드 설정/0",Order = 0),SerializeField,LabelText("그래픽 리스트"),ShowIf(nameof(m_UseFade))]
		private List<Graphic> m_GraphicList = new();

		private readonly List<FocusSlotUI> m_SlotList = new();
		private readonly List<ICellData> m_CellList = new();

		private readonly List<(float,Transform)> m_OrderList = new();

		[BoxGroup("뷰어",ShowLabel = false,Order = 99),SerializeField,KZRichText]
		private int m_FocusIndex = -1;

		private GameObjectUIPool m_ObjectPool = null;

		private bool m_Initialize = false;

		[BoxGroup("뷰어",ShowLabel = false,Order = 99),SerializeField,KZRichText]
		private float m_CurrentLocation = 0.0f;

        private bool IsCircularMode => m_CircularMode && m_CellList.Count != 1;

        private Action<ICellData> m_OnSetFocus = null;

		public event Action<ICellData> OnSetFocus
		{
			add { m_OnSetFocus -= value; m_OnSetFocus += value; }
			remove { m_OnSetFocus -= value; }
		}

		public ICellData FocusCellData => m_CellList.TryGetValueByIndex(m_FocusIndex,out var data) ? data : null;

		protected override void Initialize()
		{
			if(m_Initialize)
			{
				return;
			}

			base.Initialize();

			if(m_Slot == null)
			{
				throw new NullReferenceException("슬롯이 없습니다.");
			}

			var slot = m_Slot as SlotUI;

			if(!m_Viewport)
			{
				throw new NullReferenceException("뷰포트가 없습니다.");
			}

			slot.gameObject.SetActiveSelf(false);
			m_Viewport.SetUIChild(slot.transform);

			m_Viewport.pivot = new Vector2(0.0f,1.0f);
			slot.UIRectTransform.pivot = new Vector2(0.5f,0.5f);

			m_ObjectPool = new GameObjectUIPool(slot.gameObject,m_Viewport);

			m_CellList.Clear();
			m_SlotList.Clear();

			if(m_UseButton)
			{
				if(m_PrevButton)
				{
					m_PrevButton.SetOnClickListener(()=> { OnClickedButton(m_FocusIndex-1); });
				}

				if(m_NextButton)
				{
					m_NextButton.SetOnClickListener(()=> { OnClickedButton(m_FocusIndex+1); });
				}
			}

			if(m_UseFade)
			{
				OnDragStart += () =>
				{
					CommonUtility.MergeUniTaskAsync(new Func<UniTask>[]
					{
						() => { return PlayButtonFadeAsync(1.0f,0.0f,m_FadeInDuration); },
					},default).Forget();
				};

				OnDragEnd += _ =>
				{
					CommonUtility.MergeUniTaskAsync(new Func<UniTask>[]
					{
						() => { return PlayButtonFadeAsync(0.0f,1.0f,m_MagnetDuration); },
					},default).Forget();
				};
			}

			m_Initialize = true;
		}

		public void SetCellList(List<ICellData> _cellList,int? _index = null)
		{
			Initialize();

			var index = _index.HasValue ? Mathf.Clamp(_index.Value,0,_cellList.Count) : 0;

			m_CellList.Clear();
			m_CellList.AddRange(_cellList);

			UpdateLocation(index,true);
			UpdateIndex(index);
		}

		public void UpdateIndex(int _index)
		{
			m_FocusIndex = CommonUtility.LoopClamp(_index,m_CellList.Count);

			m_OnSetFocus?.Invoke(m_CellList[m_FocusIndex]);

			if(!m_UseButton)
			{
				return;
			}

			if(m_PrevButton)
			{
				m_PrevButton.gameObject.SetActiveSelf(m_CircularMode || m_FocusIndex != 0);
			}

			if(m_NextButton)
			{
				m_NextButton.gameObject.SetActiveSelf(m_CircularMode || m_FocusIndex != m_CellList.Count-1);
			}
		}

		private void OnClickedButton(int _index)
		{
			var index = m_CircularMode ? CommonUtility.LoopClamp(_index,m_CellList.Count) : Mathf.Clamp(_index,0,m_CellList.Count);

			if(!m_CellList.ContainsIndex(index) || index == m_FocusIndex)
			{
				return;
			}

			ScrollTo(index,m_ButtonClickDuration,EaseType.OutCubic);

			if(m_UseFade)
			{
				CommonUtility.MergeUniTaskAsync(new Func<UniTask>[]
				{
					() => { return PlayButtonFadeAsync(1.0f,0.0f,m_ButtonClickDuration/2.0f); },
					() => { return PlayButtonFadeAsync(0.0f,1.0f,m_ButtonClickDuration/2.0f); },
				},default).Forget();
			}
		}

		private async UniTask PlayButtonFadeAsync(float _start,float _finish,float _duration)
		{
			await CommonUtility.ExecuteOverTimeAsync(0.0f,1.0f,_duration,(progress)=>
			{
				foreach(var graphic in m_GraphicList)
				{
					graphic.color = graphic.color.MaskAlpha(Mathf.Lerp(_start,_finish,progress));
				}
			});
		}

		private void UpdateLocation(float _location,bool _forceRefresh)
		{
			m_CurrentLocation = _location;

			var location = _location-0.5f/m_SlotSpace;
			var firstIndex = Mathf.CeilToInt(location);
			var firstLocation = (Mathf.Ceil(location)-location)*m_SlotSpace;

			if(firstLocation+m_SlotList.Count*m_SlotSpace < 1.0f)
			{
				ResizePool(firstLocation);
			}

			UpdateSlotList(firstLocation,firstIndex,_forceRefresh);
		}

		private void ResizePool(float _firstLocation)
		{
			var count = Mathf.CeilToInt((1.0f-_firstLocation)/m_SlotSpace)-m_SlotList.Count;

			for(var i=0;i<count;i++)
			{
				m_SlotList.Add(m_ObjectPool.Get<FocusSlotUI>(m_Viewport));
			}
		}

		private void UpdateSlotList(float _firstLocation,int _firstIndex,bool _forceRefresh)
		{
			var cellCount = m_CellList.Count;
			var slotCount = m_SlotList.Count;

			m_OrderList.Clear();

			for(var i=0;i<slotCount;i++)
			{
				var index = _firstIndex+i;
				var location = _firstLocation+i*m_SlotSpace;
				var slot = m_SlotList[CommonUtility.LoopClamp(index,slotCount)];

				if(IsCircularMode)
				{
					index = CommonUtility.LoopClamp(index,cellCount);
				}

				if(index < 0 || index >= cellCount || location > 1.0f)
				{
					slot.gameObject.SetActiveSelf(false);

					continue;
				}

				if(_forceRefresh || !slot.gameObject.activeSelf)
				{
					slot.gameObject.SetActiveSelf(true);
					slot.name = string.Format("Slot_{0}",i);
					slot.SetCell(m_CellList[index]);
				}

				slot.UpdateLocation(location);

				if(m_OrderMode)
				{
					m_OrderList.Add((location,slot.transform));
				}
			}

			if(m_OrderMode)
			{
				foreach(var pair in m_OrderList.OrderByDescending(x=>Math.Abs(x.Item1-0.5f)))
				{
					pair.Item2.SetAsLastSibling();
				}
			}
		}

		protected override void Reset()
		{
			base.Reset();

			if(!m_Viewport)
			{
				var viewport = transform.Find("Viewport");

				if(!viewport)
				{
					throw new NullReferenceException("뷰포트가 없습니다.");
				}

				m_Viewport = viewport.GetComponent<RectTransform>();
			}
		}
	}
}