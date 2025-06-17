using System.Collections.Generic;
using UnityEngine;

namespace KZLib.KZDevelop
{
	/// <summary>
	/// Manage pool of GameObjects.
	/// </summary>
	public class GameObjectPool<TComponent> where TComponent : Component
	{
		private readonly TComponent m_pivot = null;
		private readonly Queue<TComponent> m_poolQueue = null;
		private readonly Transform m_storage = null;

		public GameObjectPool(TComponent pivot,Transform storage,int capacity)
		{
			if(!pivot)
			{
				Logger.System.E("Pivot is null");

				return;
			}

			if(!storage)
			{
				Logger.System.E("Storage is null");

				return;
			}

			m_pivot = pivot;
			m_poolQueue = new(capacity);

			m_storage = storage;

			for(var i=0;i<capacity;i++)
			{
				var item = m_pivot.CopyObject() as TComponent;

				Put(item);
			}
		}

		protected virtual void SetChild(Transform parent,Transform child)
		{
			parent.SetChild(child);
		}

		public void Put(TComponent item)
		{
			SetChild(m_storage,item.transform);

			item.name = m_pivot.name;
			item.gameObject.EnsureActive(false);

			m_poolQueue.Enqueue(item);
		}

		public TComponent GetOrCreate(Transform parent = null)
		{
			var item = m_poolQueue.Count < 1 ? m_poolQueue.Dequeue() : m_pivot.CopyObject() as TComponent;

			if(parent)
			{
				SetChild(parent,item.transform);
			}

			item.gameObject.EnsureActive(true);

			return item;
		}
	}

	public class GameObjectUIPool<TComponent> : GameObjectPool<TComponent> where TComponent : Component
	{
		public GameObjectUIPool(TComponent pivot,Transform storage,int capacity) : base(pivot,storage,capacity) { }

		protected override void SetChild(Transform parent,Transform child)
		{
			parent.SetUIChild(child);
		}
	}
}