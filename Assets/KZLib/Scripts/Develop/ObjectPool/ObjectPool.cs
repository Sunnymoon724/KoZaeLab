using System;
using System.Collections.Generic;

namespace KZLib.Development
{
	/// <summary>
	/// Manage pool of Objects.
	/// </summary>
	public class ObjectPool<TObject> : IDisposable where TObject : class
	{
		private bool m_disposed = false;

		private readonly Queue<TObject> m_poolQueue = null;
		private readonly TObject m_pivot = null;
		private readonly Func<TObject,TObject> m_createFunc = null;

		public ObjectPool(Func<TObject,TObject> createFunc,TObject pivot,int capacity) : this(createFunc,pivot,capacity,true) { }

		protected ObjectPool(Func<TObject,TObject> createFunc,TObject pivot,int capacity,bool autoFill)
		{
			m_poolQueue = new(capacity);
			m_pivot = pivot;
			m_createFunc = createFunc;

			if(autoFill)
			{
				_Fill(capacity);
			}
		}

		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this); 
		}

		protected virtual void Dispose(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				static void _Clear(TObject @object)
				{
					if(@object is IDisposable disposable)
					{
						disposable.Dispose();
					}
				}

				Clear(_Clear);
			}

			m_disposed = true;
		}

		protected void _Fill(int capacity)
		{
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