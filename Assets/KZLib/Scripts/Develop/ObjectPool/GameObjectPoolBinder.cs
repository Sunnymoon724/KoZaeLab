using System;
using System.Collections.Generic;
using UnityEngine;

namespace KZLib.KZDevelop
{
	public class GameObjectPoolBinder<TItem,UData> : IDisposable where TItem : Component
	{
		private bool m_disposed = false;

		private readonly Transform m_content = null;
		private readonly Action<TItem,UData> m_onBindData = null;

		private readonly List<TItem> m_itemList = new();
		private readonly GameObjectPool<TItem> m_objectPool = null;

		public IEnumerable<TItem> ItemGroup => m_itemList;
		public int ItemCount => m_itemList.Count;

		public GameObjectPoolBinder(TItem pivot,Transform content,Action<TItem,UData> onBindData,Transform storage = null,int capacity = 1)
		{
			m_content = content;
			m_onBindData = onBindData ?? throw new ArgumentNullException("Binding action must not be null.");

			var currentStorage = storage == null ? UIManager.In.GetStorage(false) : storage;

			m_objectPool = new GameObjectPool<TItem>(pivot,currentStorage,capacity,false);

			pivot.gameObject.EnsureActive(false);
			currentStorage.SetChild(pivot.transform);
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
				m_objectPool?.Dispose();
			}

			m_disposed = true;
		}

		public bool TrySetData(UData data)
		{
			return TrySetDataList(new List<UData>() { data } );
		}

		public bool TrySetDataList(List<UData> dataList)
		{
			if(dataList.IsNullOrEmpty())
			{
				Clear();

				return false;
			}

			var cellToCreate = dataList.Count-m_itemList.Count;

			if(cellToCreate > 0)
			{
				for(var i=0;i<cellToCreate;i++)
				{
					m_itemList.Add(m_objectPool.GetOrCreate(m_content));
				}
			}

			var index = 0;

			for(var i=0;i<dataList.Count;i++)
			{
				var data = dataList[i];
				var item = m_itemList[index];

				m_onBindData.Invoke(item,data);

				index++;
			}

			_PutRemainingObject(index);

			return true;
		}

		public void Clear()
		{
			_PutRemainingObject(0);
		}

		private void _PutRemainingObject(int index)
		{
			for(var i=m_itemList.Count-1;i>=index;i--)
			{
				var item = m_itemList[i];

				m_objectPool.Put(item);
				m_itemList.RemoveAt(i);
			}
		}

		public TItem GetItemByIndex(int index)
		{
			return m_itemList.ContainsIndex(index) ? m_itemList[index] : null;
		}

		public TItem GetNearItem(TItem target,int count,bool canLoop)
		{
			if(!m_itemList.Contains(target))
			{
				return null;
			}

			if(count == 0)
			{
				return target;
			}

			return m_itemList.FindNext(target,count,canLoop);
		}

		public int FindIndex(TItem target)
		{
			if(!m_itemList.Contains(target))
			{
				return -1;
			}

			return m_itemList.IndexOf(target);
		}
	}
}