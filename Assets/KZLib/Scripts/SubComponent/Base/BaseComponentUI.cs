using KZLib.KZDevelop;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class BaseComponentUI : BaseComponent
{
	protected RectTransform m_rectTransform = null;

	public RectTransform UIRectTransform
	{
		get
		{
			InitializeRectTransform();

			return m_rectTransform;
		}
	}

	protected override void Reset()
	{
		base.Reset();

		InitializeRectTransform();
	}

	private void InitializeRectTransform()
	{
		if(!m_rectTransform)
		{
			m_rectTransform = GetComponent<RectTransform>();
		}
	}

	protected class GameObjectUIPool<TComponent> : GameObjectPool<TComponent> where TComponent : Component
	{
		public GameObjectUIPool(TComponent pivot,Transform storage) : base(pivot,storage,1) { }

		protected override void SetChild(Transform parent,Transform child)
		{
			parent.SetUIChild(child);
		}
	}
}