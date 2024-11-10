
namespace System.Collections.Generic
{
	public class CircularQueue<TData> : IEnumerable<TData>,IEnumerable,IReadOnlyCollection<TData>,ICollection
	{
		private readonly TData[] m_DataArray = null;
		private readonly int m_Capacity = 0;
		private readonly object m_SyncRoot = new();

		private int m_Front = -1;
		private int m_Rear = -1;

		public int Size => m_DataArray.Length;
		public int Capacity => m_Capacity;

		public CircularQueue(int _capacity)
		{
			if(_capacity <= 0)
			{
				LogTag.System.E($"The capacity is {_capacity}.");

				return;
			}

			m_Capacity = _capacity;
			m_DataArray = new TData[_capacity];
		}

		public CircularQueue(IEnumerable<TData> _collection)
		{
			if(_collection is ICollection<TData> collection)
			{
				m_Capacity = collection.Count;

				if(m_Capacity <= 0)
				{
					LogTag.System.E($"The capacity is {m_Capacity}.");

					return;
				}

				m_DataArray = new TData[m_Capacity];

				collection.CopyTo(m_DataArray,0);
			}
			else
			{
				var tempList = new List<TData>();

				foreach(var item in _collection)
				{
					tempList.Add(item);
				}

				m_Capacity = tempList.Count;

				if(m_Capacity <= 0)
				{
					LogTag.System.E($"The capacity is {m_Capacity}.");

					return;
				}

				m_DataArray = new TData[m_Capacity];

				tempList.CopyTo(m_DataArray,0);
			}

			m_Rear = m_Capacity-1;
		}

		public void Enqueue(TData _data)
		{
			if(_data == null)
			{
				LogTag.System.E("Data cannot be null.");

				return;
			}

			lock(m_SyncRoot)
			{
				if(IsFull)
				{
					m_Front = (m_Front+1)%m_Capacity;
				}
				else if(IsEmpty)
				{
					m_Front = 0;
				}

				m_Rear = (m_Rear+1)%m_Capacity;
				m_DataArray[m_Rear] = _data;
			}
		}

		public TData Peek()
		{
			lock(m_SyncRoot)
			{
				if(IsEmpty)
				{
					LogTag.System.E("Queue is empty.");

					return default;
				}

				return m_DataArray[m_Front];
			}
		}

		public TData Dequeue()
		{
			lock(m_SyncRoot)
			{
				if(IsEmpty)
				{
					LogTag.System.E("Queue is empty.");

					return default;
				}

				var data = m_DataArray[m_Front];

				if(m_Front == m_Rear)
				{
					m_Front = -1;
					m_Rear = -1;
				}
				else
				{
					m_Front = (m_Front+1)%m_Capacity;
				}

				return data;
			}
		}

		public int Count
		{
			get
			{
				lock(m_SyncRoot)
				{
					return IsEmpty ? 0 : (m_Rear >= m_Front ? m_Rear-m_Front+1 : m_Capacity-m_Front+m_Rear+1);
				}
			}
		}

		public bool IsEmpty => m_Front == -1;
		public bool IsFull => (m_Rear+1)%m_Capacity == m_Front;

		public void Clear()
		{
			lock(m_SyncRoot)
			{
				m_Front = -1;
				m_Rear = -1;

				Array.Clear(m_DataArray,0,m_Capacity);
			}
		}

		public IEnumerator<TData> GetEnumerator()
		{
			lock(m_SyncRoot)
			{
				if(IsEmpty)
				{
					yield break;
				}

				var index = m_Front;

				while(index != m_Rear)
				{
					yield return m_DataArray[index];

					index = (index+1)%m_Capacity;
				}

				yield return m_DataArray[index];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Contains(TData _data)
		{
			if(_data == null)
			{
				LogTag.System.E("Data is null.");

				return false;
			}

			lock(m_SyncRoot)
			{
				var index = m_Front;

				for(var i=0;i<Count;i++)
				{
					if(EqualityComparer<TData>.Default.Equals(m_DataArray[index],_data))
					{
						return true;
					}

					index = (index+1)%m_Capacity;
				}
			}

			return false;
		}

		public void CopyTo(Array _array,int _index)
		{
			if(_array == null)
			{
				LogTag.System.E("Array is null.");

				return;
			}

			if(_index < 0 || _index >= _array.Length)
			{
				LogTag.System.E($"Index {_index} is out of bounds for the array.");

				return;
			}

			if(Count > 0)
			{
				lock(m_SyncRoot)
				{
					var index = m_Front;

					for(var i=0;i<Count;i++)
					{
						_array.SetValue(m_DataArray[index],_index+i);
						index = (index+1)%m_Capacity;
					}
				}
			}
		}

		public bool IsSynchronized => true;
		public object SyncRoot => m_SyncRoot;
	}
}