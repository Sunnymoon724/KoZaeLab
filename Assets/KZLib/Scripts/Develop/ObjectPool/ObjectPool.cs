using System;
using System.Collections.Generic;

namespace KZLib.KZDevelop
{
	/// <summary>
	/// Manage pool of Objects.
	/// </summary>
	public class ObjectPool<TObject>where TObject : class
	{
		private readonly Queue<TObject> m_poolQueue = null;

		private readonly Func<TObject> m_createFunc;

		public ObjectPool(Func<TObject> createFunc,int capacity)
		{
			m_poolQueue = new(capacity);

			m_createFunc = createFunc;

			for(var i=0;i<capacity;i++)
			{
				var item = m_createFunc();

				Put(item);
			}
		}

		public virtual void Put(TObject item)
		{
			m_poolQueue.Enqueue(item);
		}

		public virtual TObject GetOrCreate()
		{
			var item = m_poolQueue.Count > 0 ? m_poolQueue.Dequeue() : m_createFunc();

			return item;
		}

		public void Clear(Action<TObject> onDestroy = null)
		{
			while(m_poolQueue.Count > 0)
			{
				var item = m_poolQueue.Dequeue();

				onDestroy?.Invoke(item);
			}
		}
	}
}