using KZLib;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class BaseComponentUI : BaseComponent
{
	protected RectTransform m_RectTransform = null;

	public RectTransform UIRectTransform
	{
		get
		{
			InitializeRectTransform();

			return m_RectTransform;
		}
	}

	protected override void Reset()
	{
		base.Reset();

		InitializeRectTransform();
	}

	private void InitializeRectTransform()
	{
		if(!m_RectTransform)
		{
			m_RectTransform = GetComponent<RectTransform>();
		}
	}

	protected class GameObjectUIPool : GameObjectPool
	{
		public GameObjectUIPool(GameObject _pivot,Transform _storage) : base(_pivot,_storage,1) { }

		protected override void SetChild(Transform _parent,Transform _child)
		{
			_parent.SetUIChild(_child);
		}
	}
}