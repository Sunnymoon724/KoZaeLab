﻿using System.Collections.Generic;
using UnityEngine;

namespace KZLib
{
	/// <summary>
	/// Manage pool of GameObjects.
	/// </summary>
	public class GameObjectPool<TComponent> where TComponent : Component
	{
		private readonly TComponent m_Pivot = null;
		private readonly Queue<TComponent> m_PoolQueue = null;
		private readonly Transform m_Storage = null;

		public GameObjectPool(TComponent _pivot,Transform _storage,int _capacity)
		{
			if(!_pivot)
			{
				LogTag.System.E("Pivot is null.");

				return;
			}

			if(!_storage)
			{
				LogTag.System.E("Storage is null.");

				return;
			}

			m_Pivot = _pivot;
			m_PoolQueue = new(_capacity);

			m_Storage = _storage;
		}

		protected virtual void SetChild(Transform _parent,Transform _child)
		{
			_parent.SetChild(_child);
		}

		public void Put(TComponent _data)
		{
			SetChild(m_Storage,_data.transform);

			_data.name = m_Pivot.name;
			_data.gameObject.SetActiveSelf(false);

			m_PoolQueue.Enqueue(_data);
		}

		public TComponent Get(Transform _parent = null)
		{
			var data = m_PoolQueue.Count > 0 ? m_PoolQueue.Dequeue() : CommonUtility.CopyObject(m_Pivot);

			if(_parent)
			{
				SetChild(_parent,data.transform);
			}

			data.gameObject.SetActiveSelf(true);

			return data;
		}
	}
}