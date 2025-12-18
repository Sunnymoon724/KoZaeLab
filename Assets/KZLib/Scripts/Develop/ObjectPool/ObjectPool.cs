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
		private readonly TObject m_pivot = null;
		private readonly Func<TObject,TObject> m_createFunc = null;

		public ObjectPool(Func<TObject,TObject> createFunc,TObject pivot,int capacity)
		{
			m_poolQueue = new(capacity);
			m_pivot = pivot;
			m_createFunc = createFunc;

			for(var i=0;i<capacity;i++)
			{
				var item = m_createFunc(m_pivot);

				Put(item);
			}
		}

		public virtual void Put(TObject item)
		{
			m_poolQueue.Enqueue(item);
		}

		public virtual TObject GetOrCreate()
		{
			var item = m_poolQueue.Count > 0 ? m_poolQueue.Dequeue() : m_createFunc(m_pivot);

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