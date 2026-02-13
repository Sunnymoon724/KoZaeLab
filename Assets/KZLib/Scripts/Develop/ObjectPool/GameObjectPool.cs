using System;
using UnityEngine;

namespace KZLib.Development
{
	/// <summary>
	/// Manage pool of GameObjects.
	/// </summary>
	public class GameObjectPool<TComponent> : ObjectPool<TComponent> where TComponent : Component
	{
		private readonly Transform m_storage = null;

		private readonly bool m_worldPositionStays = true;

		public GameObjectPool(TComponent pivot,Transform storage,int capacity,bool worldPositionStays) : base(_CopyObject,pivot,capacity,false)
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
			m_worldPositionStays = worldPositionStays;

			_Fill(capacity);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				static void _Clear(TComponent component)
				{
					if(component != null && component.gameObject != null)
					{
						component.DestroyObject();
					}
				}

				Clear(_Clear);
			}

			base.Dispose(disposing); 
		}

		private static TComponent _CopyObject(TComponent pivot) 
		{
			return pivot.CopyObject() as TComponent;
		}

		protected void _SetChild(Transform parent,Transform child)
		{
			if(parent)
			{
				parent.SetChild(child,m_worldPositionStays);
			}
		}

		public override void Put(TComponent item)
		{
			_SetChild(m_storage,item.transform);

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
				_SetChild(parent,item.transform);
			}

			item.gameObject.EnsureActive(true);

			return item;
		}
	}
}