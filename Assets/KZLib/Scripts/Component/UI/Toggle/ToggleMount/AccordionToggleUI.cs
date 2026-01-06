// using System;
// using System.Collections.Generic;
// using Sirenix.OdinInspector;
// using UnityEngine;
// using UnityEngine.UI;

// [RequireComponent(typeof(RectTransform), typeof(LayoutElement))]
// [AddComponentMenu("UI/Extensions/Accordion/Accordion Element")]
// public class AccordionToggleUI : BaseToggleUI
// {
// 	[Serializable]
// 	protected class ActiveChild : ToggleChild
// 	{
// 		[HorizontalGroup("0",Order = 0),SerializeField]
// 		private GameObject m_gameObject = null;

// 		protected override void Set()
// 		{
// 			m_gameObject.EnsureActive(IsOnNow);
// 		}
// 	}

// 	protected override IEnumerable<ToggleChild> ToggleChildGroup => null;

// 	[SerializeField] private float m_MinHeight = 18f;

// 	public float MinHeight => m_MinHeight;

// 	[SerializeField] private float m_MinWidth = 40f;

// 	public float MinWidth => m_MinWidth;

// 	private Accordion m_Accordion;

// 	[SerializeField]
// 	private LayoutElement m_layoutElement = null;

// 	protected override void Awake()
// 	{
// 		base.Awake();
// 		base.toggleTransition = ToggleTransition.None;
// 		this.m_Accordion = this.gameObject.GetComponentInParent<Accordion>();
// 		this.m_RectTransform = this.transform as RectTransform;
// 		this.m_layoutElement = this.gameObject.GetComponent<LayoutElement>();
// 		this.onValueChanged.AddListener(OnValueChanged);
// 	}

// #if UNITY_EDITOR
// 	protected override void OnValidate()
// 	{
// 		base.OnValidate();
// 		this.m_Accordion = this.gameObject.GetComponentInParent<Accordion>();

// 		if (this.group == null)
// 		{
// 			ToggleGroup tg = this.GetComponentInParent<ToggleGroup>();
			
// 			if (tg != null)
// 			{
// 				this.group = tg;
// 			}
// 		}
		
// 		LayoutElement le = this.gameObject.GetComponent<LayoutElement>();

// 		if (le != null && m_Accordion != null)
// 		{
// 			if (this.isOn)
// 			{
// 				if (m_Accordion.ExpandVerticval)
// 				{
// 					le.preferredHeight = -1f;
// 				}
// 				else
// 				{
// 					le.preferredWidth = -1f;
// 				}
// 			}
// 			else
// 			{
// 				if (m_Accordion.ExpandVerticval)
// 				{
// 					le.preferredHeight = this.m_MinHeight;
// 				}
// 				else
// 				{
// 					le.preferredWidth = this.m_MinWidth;

// 				}
// 			}
// 		}
// 	}
// #endif

// 	public void OnValueChanged(bool state)
// 	{
// 		if (this.m_layoutElement == null)
// 			return;
		
// 		Accordion.Transition transition = (this.m_Accordion != null) ? this.m_Accordion.transition : Accordion.Transition.Instant;

// 		if (transition == Accordion.Transition.Instant && m_Accordion != null)
// 		{
// 			if (state)
// 			{
// 				if (m_Accordion.ExpandVerticval)
// 				{
// 					this.m_layoutElement.preferredHeight = -1f;
// 				}
// 				else
// 				{
// 					this.m_layoutElement.preferredWidth = -1f;
// 				}
// 			}
// 			else
// 			{
// 				if (m_Accordion.ExpandVerticval)
// 				{
// 					this.m_layoutElement.preferredHeight = this.m_MinHeight;
// 				}
// 				else
// 				{
// 					this.m_layoutElement.preferredWidth = this.m_MinWidth;
// 				}
// 			}
// 		}
// 		else if (transition == Accordion.Transition.Tween)
// 		{
// 			if (state)
// 			{
// 				if (m_Accordion.ExpandVerticval)
// 				{
// 					this.StartTween(this.m_MinHeight, this.GetExpandedHeight());
// 				}
// 				else
// 				{
// 					this.StartTween(this.m_MinWidth, this.GetExpandedWidth());
// 				}
// 			}
// 			else
// 			{
// 				if (m_Accordion.ExpandVerticval)
// 				{
// 					this.StartTween(this.m_RectTransform.rect.height, this.m_MinHeight);
// 				}
// 				else
// 				{
// 					this.StartTween(this.m_RectTransform.rect.width, this.m_MinWidth);
// 				}
// 			}
// 		}
// 	}
	
// 	protected float GetExpandedHeight()
// 	{
// 		if (this.m_layoutElement == null)
// 			return this.m_MinHeight;
		
// 		float originalPrefH = this.m_layoutElement.preferredHeight;
// 		this.m_layoutElement.preferredHeight = -1f;
// 		float h = LayoutUtility.GetPreferredHeight(this.m_RectTransform);
// 		this.m_layoutElement.preferredHeight = originalPrefH;
		
// 		return h;
// 	}

// 	protected float GetExpandedWidth()
// 	{
// 		if (this.m_layoutElement == null)
// 			return this.m_MinWidth;

// 		float originalPrefW = this.m_layoutElement.preferredWidth;
// 		this.m_layoutElement.preferredWidth = -1f;
// 		float w = LayoutUtility.GetPreferredWidth(this.m_RectTransform);
// 		this.m_layoutElement.preferredWidth = originalPrefW;

// 		return w;
// 	}

// 	protected void StartTween(float startFloat, float targetFloat)
// 	{
// 		float duration = (this.m_Accordion != null) ? this.m_Accordion.transitionDuration : 0.3f;
		
// 		FloatTween info = new FloatTween
// 		{
// 			duration = duration,
// 			startFloat = startFloat,
// 			targetFloat = targetFloat
// 		};
// 		if (m_Accordion.ExpandVerticval)
// 		{
// 			info.AddOnChangedCallback(SetHeight);
// 		}
// 		else
// 		{
// 			info.AddOnChangedCallback(SetWidth);
// 		}
// 		info.ignoreTimeScale = true;
// 		this.m_FloatTweenRunner.StartTween(info);
// 	}
	
// 	protected void SetHeight(float height)
// 	{
// 		if (this.m_layoutElement == null)
// 			return;
			
// 		this.m_layoutElement.preferredHeight = height;
// 	}

// 	protected void SetWidth(float width)
// 	{
// 		if (this.m_layoutElement == null)
// 			return;

// 		this.m_layoutElement.preferredWidth = width;
// 	}
// }