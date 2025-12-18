using System;
using UnityEngine;

namespace KZLib.KZDevelop
{
	/// <summary>
	/// Manage pool of GameObjects.
	/// </summary>
	public class GameObjectPool<TComponent> : ObjectPool<TComponent> where TComponent : Component
	{
		private readonly Transform m_storage = null;

		public GameObjectPool(TComponent pivot,Transform storage,int capacity) : base(_CopyObject,pivot,capacity)
		{
			if(!pivot)
			{
				throw new NullReferenceException("Pivot is null.");
			}

			if(!storage)
			{
				throw new NullReferenceException("Storage is null.");
			}

			m_storage = storage;
		}

		private static TComponent _CopyObject(TComponent pivot) 
		{
			return pivot.CopyObject() as TComponent;
		}

		protected virtual void SetChild(Transform parent,Transform child)
		{
			parent.SetChild(child);
		}

		public override void Put(TComponent item)
		{
			SetChild(m_storage,item.transform);

			item.gameObject.EnsureActive(false);

			base.Put(item);
		}

		public override TComponent GetOrCreate()
		{
			return GetOrCreate(null);
		}

		public TComponent GetOrCreate(Transform parent)
		{
			var item = base.GetOrCreate();

			if(parent)
			{
				SetChild(parent,item.transform);
			}

			item.gameObject.EnsureActive(true);

			return item;
		}
	}
}