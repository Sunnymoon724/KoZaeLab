using System.Linq;

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
				throw new ArgumentOutOfRangeException($"용량이 {_capacity} 입니다."); 
			}

			m_Capacity = _capacity;
			m_DataArray = new TData[_capacity];
		}

		public CircularQueue(IEnumerable<TData> _collection) : this(_collection.Count())
		{
			if(m_Capacity > 0)
			{
				Array.Copy(_collection.ToArray(),m_DataArray,m_Capacity);
				m_Rear = m_Capacity-1;
			}
		}

		public void Enqueue(TData _data)
		{
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
					throw new InvalidOperationException("큐가 비어 있습니다.");
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
					throw new InvalidOperationException("큐가 비어 있습니다.");
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

		public int Count => IsEmpty ? 0 : (m_Rear >= m_Front ? m_Rear-m_Front+1 : m_Capacity-m_Front+m_Rear+1);
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

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Contains(TData _data)
		{
			if(_data == null)
			{
				throw new ArgumentNullException("null은 포함될 수 없습니다.");
			}

			return m_DataArray.Any(x=>x != null && x.Equals(_data));
		}

		public void CopyTo(Array _array,int _index)
		{
			if(_array == null)
			{
				throw new ArgumentNullException("배열이 null입니다.");
			}

			if(_index < 0 || _index >= _array.Length)
            {
                throw new ArgumentOutOfRangeException($"인덱스 {_index}는 배열의 범위를 초과합니다.");
            }

			if(Count > 0)
			{
				var index = m_Front;

				for(var i=0;i<Count;i++)
				{
					_array.SetValue(m_DataArray[index],_index+i);
					index = (index+1)%m_Capacity;
				}
			}
		}

		bool ICollection.IsSynchronized => true;
		object ICollection.SyncRoot => m_SyncRoot;
	}
}