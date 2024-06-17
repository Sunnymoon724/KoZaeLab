using KZLib.KZDevelop;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class BaseComponentUI : BaseComponent
{
	protected RectTransform m_RectTransform = null;

	public RectTransform UIRectTransform
	{
		get
		{
			if(m_RectTransform == null)
			{
				m_RectTransform = GetComponent<RectTransform>();
			}

			return m_RectTransform;
		}
	}

	protected override void Reset()
	{
		base.Reset();

		if(!m_RectTransform)
		{
			m_RectTransform = GetComponent<RectTransform>();
		}
	}

	protected class UIPool : GameObjectPool
	{
		public UIPool(GameObject _pivot,Transform _storage) : base(_pivot,_storage,1) { }

		protected override void SetChild(GameObject _parent,GameObject _child)
		{
			_parent.transform.SetUIChild(_child.transform);
		}
	}
}