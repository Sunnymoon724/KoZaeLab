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
		[FoldoutGroup("General",Order = 0)]
		[VerticalGroup("General/0",Order = 0),SerializeField,LabelText("Viewport")]
		private RectTransform m_Viewport = null;

		[VerticalGroup("General/0",Order = 0),SerializeField,LabelText("Slot")]
		private FocusSlotUI m_slot = null;

		[PropertySpace(10)]

		[VerticalGroup("General/0",Order = 0),SerializeField,LabelText("Circular Mode")]
		private bool m_CircularMode = true;
		[VerticalGroup("General/0",Order = 0),SerializeField,LabelText("Order Mode")]
		private bool m_OrderMode = false;
		[VerticalGroup("General/0",Order = 0),SerializeField,LabelText("Slot Space"),Range(0.01f,0.5f)]
		private float m_slotSpace = 0.1f;

		[VerticalGroup("General/0",Order = 0),SerializeField,LabelText("Is Vertical")]
		private bool m_Vertical = false;

		[VerticalGroup("General/0",Order = 0),SerializeField,LabelText("Use Drag")]
		private bool m_UseDrag = true;

		[FoldoutGroup("버튼 설정",Order = 15)]
		[VerticalGroup("버튼 설정/0",Order = 0),SerializeField,LabelText("Use Button")]
		private bool m_UseButton = false;
		[VerticalGroup("버튼 설정/0",Order = 0),SerializeField,LabelText("Button Click Duration"),ShowIf(nameof(m_UseButton))]
		private float m_ButtonClickDuration = 0.35f;

		[VerticalGroup("버튼 설정/0",Order = 0),SerializeField,LabelText("Prev Button"),ShowIf(nameof(m_UseButton))]
		private Button m_PrevButton = null;
		[VerticalGroup("버튼 설정/0",Order = 0),SerializeField,LabelText("Next Button"),ShowIf(nameof(m_UseButton))]
		private Button m_NextButton = null;

		[FoldoutGroup("Fade",Order = 17)]
		[VerticalGroup("Fade/0",Order = 0),SerializeField,LabelText("Use Fade")]
		private bool m_UseFade = false;

		[VerticalGroup("Fade/0",Order = 0),SerializeField,LabelText("FadeIn Duration (FadeOut -> Magnet Time)"),ShowIf(nameof(m_UseFade))]
		private float m_FadeInDuration = 0.3f;

		[VerticalGroup("Fade/0",Order = 0),SerializeField,LabelText("Graphic List"),ShowIf(nameof(m_UseFade))]
		private List<Graphic> m_graphicList = new();

		private readonly List<FocusSlotUI> m_slotList = new();
		private readonly List<ICellData> m_CellList = new();

		private readonly List<(float,Transform)> m_OrderList = new();

		[BoxGroup("Viewer",ShowLabel = false,Order = 99),SerializeField,KZRichText]
		private int m_FocusIndex = -1;

		private GameObjectUIPool<FocusSlotUI> m_ObjectPool = null;

		private bool m_Initialize = false;

		[BoxGroup("Viewer",ShowLabel = false,Order = 99),SerializeField,KZRichText]
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

			if(m_slot == null)
			{
				throw new NullReferenceException("No Slot.");
			}

			var slot = m_slot as SlotUI;

			if(!m_Viewport)
			{
				throw new NullReferenceException("No Viewport.");
			}

			slot.gameObject.SetActiveSelf(false);
			m_Viewport.SetUIChild(slot.transform);

			m_Viewport.pivot = new Vector2(0.0f,1.0f);
			slot.UIRectTransform.pivot = new Vector2(0.5f,0.5f);

			m_ObjectPool = new GameObjectUIPool<FocusSlotUI>(m_slot,m_Viewport);

			m_CellList.Clear();
			m_slotList.Clear();

			if(m_UseButton)
			{
				if(m_PrevButton)
				{
					m_PrevButton.AddListener(OnClickedPrevButton);
				}

				if(m_NextButton)
				{
					m_NextButton.AddListener(OnClickedNextButton);
				}
			}

			if(m_UseFade)
			{
				OnDragStart += () =>
				{
					UniTaskUtility.MergeUniTaskAsync(new Func<UniTask>[]
					{
						() => { return PlayButtonFadeAsync(1.0f,0.0f,m_FadeInDuration); },
					},default).Forget();
				};

				OnDragEnd += _ =>
				{
					UniTaskUtility.MergeUniTaskAsync(new Func<UniTask>[]
					{
						() => { return PlayButtonFadeAsync(0.0f,1.0f,m_MagnetDuration); },
					},default).Forget();
				};
			}

			m_Initialize = true;
		}

		private void OnClickedPrevButton()
		{
			OnClickedButton(m_FocusIndex-1);
		}

		private void OnClickedNextButton()
		{
			OnClickedButton(m_FocusIndex+1);
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
			m_FocusIndex = MathUtility.LoopClamp(_index,m_CellList.Count);

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
			var index = m_CircularMode ? MathUtility.LoopClamp(_index,m_CellList.Count) : Mathf.Clamp(_index,0,m_CellList.Count);

			if(!m_CellList.ContainsIndex(index) || index == m_FocusIndex)
			{
				return;
			}

			ScrollTo(index,m_ButtonClickDuration,EaseType.OutCubic);

			if(m_UseFade)
			{
				UniTaskUtility.MergeUniTaskAsync(new Func<UniTask>[]
				{
					() => { return PlayButtonFadeAsync(1.0f,0.0f,m_ButtonClickDuration/2.0f); },
					() => { return PlayButtonFadeAsync(0.0f,1.0f,m_ButtonClickDuration/2.0f); },
				},default).Forget();
			}
		}

		private async UniTask PlayButtonFadeAsync(float _start,float _finish,float _duration)
		{
			await UniTaskUtility.ExecuteOverTimeAsync(0.0f,1.0f,_duration,(progress)=>
			{
				foreach(var graphic in m_graphicList)
				{
					graphic.color = graphic.color.MaskAlpha(Mathf.Lerp(_start,_finish,progress));
				}
			});
		}

		private void UpdateLocation(float _location,bool _forceRefresh)
		{
			m_CurrentLocation = _location;

			var location = _location-0.5f/m_slotSpace;
			var firstIndex = Mathf.CeilToInt(location);
			var firstLocation = (Mathf.Ceil(location)-location)*m_slotSpace;

			if(firstLocation+m_slotList.Count*m_slotSpace < 1.0f)
			{
				ResizePool(firstLocation);
			}

			UpdateSlotList(firstLocation,firstIndex,_forceRefresh);
		}

		private void ResizePool(float _firstLocation)
		{
			var count = Mathf.CeilToInt((1.0f-_firstLocation)/m_slotSpace)-m_slotList.Count;

			for(var i=0;i<count;i++)
			{
				m_slotList.Add(m_ObjectPool.Get(m_Viewport));
			}
		}

		private void UpdateSlotList(float _firstLocation,int _firstIndex,bool _forceRefresh)
		{
			var cellCount = m_CellList.Count;
			var slotCount = m_slotList.Count;

			m_OrderList.Clear();

			for(var i=0;i<slotCount;i++)
			{
				var index = _firstIndex+i;
				var location = _firstLocation+i*m_slotSpace;
				var slot = m_slotList[MathUtility.LoopClamp(index,slotCount)];

				if(IsCircularMode)
				{
					index = MathUtility.LoopClamp(index,cellCount);
				}

				if(index < 0 || index >= cellCount || location > 1.0f)
				{
					slot.gameObject.SetActiveSelf(false);

					continue;
				}

				if(_forceRefresh || !slot.gameObject.activeSelf)
				{
					slot.gameObject.SetActiveSelf(true);
					slot.name = $"Slot_{i}";
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
					throw new NullReferenceException("No Viewport");
				}

				m_Viewport = viewport.GetComponent<RectTransform>();
			}
		}
	}
}