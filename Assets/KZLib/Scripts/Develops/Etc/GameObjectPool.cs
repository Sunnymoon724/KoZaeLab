using System;
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
				throw new NullReferenceException("Pivot is null.");
			}

			if(!storage)
			{
				throw new NullReferenceException("Storage is null.");
			}

			m_pivot = pivot;
			m_poolQueue = new(capacity);

			m_storage = storage;
		}

		protected virtual void SetChild(Transform parent,Transform child)
		{
			parent.SetChild(child);
		}

		public void Put(TComponent item)
		{
			SetChild(m_storage,item.transform);

			item.name = m_pivot.name;
			item.gameObject.SetActiveIfDifferent(false);

			m_poolQueue.Enqueue(item);
		}

		public TComponent GetOrCreate(Transform parent = null)
		{
			var item = m_poolQueue.Count > 0 ? m_poolQueue.Dequeue() : m_pivot.CopyObject() as TComponent;

			if(parent)
			{
				SetChild(parent,item.transform);
			}

			item.gameObject.SetActiveIfDifferent(true);

			return item;
		}
	}
}