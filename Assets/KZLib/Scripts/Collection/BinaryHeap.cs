
namespace System.Collections.Generic
{
	public abstract class BinaryHeap<TData> : IEnumerable<TData>,IEnumerable,IReadOnlyCollection<TData>,ICollection where TData : IComparable<TData>
	{
		private readonly List<TData> m_DataList = null;
		private readonly object m_SyncRoot = new();

		protected abstract int Compare(TData _first,TData _second);

		public BinaryHeap(int _capacity = 0)
		{
			if(_capacity <= 0)
			{
				LogTag.System.E($"The capacity is {_capacity}.");

				return;
			}

			m_DataList = new List<TData>(_capacity);
		}

		public BinaryHeap(IEnumerable<TData> _collection)
		{
			m_DataList = new List<TData>(_collection);

			for(var i=m_DataList.Count/2-1;i>=0;i--)
			{
				HeapifyDown(i);
			}
		}


		public void Insert(TData _data)
		{
			lock(m_SyncRoot)
			{
				m_DataList.Add(_data);
				HeapifyUp(m_DataList.Count-1);
			}
		}

		public TData ExtractTop()
		{
			lock(m_SyncRoot)
			{
				if(IsEmpty)
				{
					LogTag.System.E("Heap is empty.");

					return default;
				}

				var top = m_DataList[0];

				m_DataList[0] = m_DataList[^1];
				m_DataList.RemoveAt(m_DataList.Count-1);

				if(!IsEmpty)
				{
					HeapifyDown(0);
				}

				return top;
			}
		}

		public TData Peek()
		{
			lock(m_SyncRoot)
			{
				if(IsEmpty)
				{
					LogTag.System.E("Heap is empty.");

					return default;
				}

				return m_DataList[0];
			}
		}

		public bool Remove(TData _data)
		{
			lock(m_SyncRoot)
			{
				var index = m_DataList.IndexOf(_data);

				if(index == -1)
				{
					return false;
				}

				m_DataList[index] = m_DataList[^1];
				m_DataList.RemoveAt(m_DataList.Count-1);

				if(index < m_DataList.Count)
				{
					HeapifyDown(index);
					HeapifyUp(index);
				}

				return true;
			}
		}

		private void HeapifyUp(int _index)
		{
			while(_index > 0)
			{
				var parent = (_index-1)/2;

				if(Compare(m_DataList[_index],m_DataList[parent]) >= 0)
				{
					break;
				}

				Swap(_index,parent);

				_index = parent;
			}
		}

		private void HeapifyDown(int _index)
		{
			var last = m_DataList.Count-1;

			while(_index <= last)
			{
				var left = _index*2+1;
				var right = _index*2+2;
				var swap = _index;

				if(left <= last && Compare(m_DataList[left],m_DataList[swap]) < 0)
				{
					swap = left;
				}

				if(right <= last && Compare(m_DataList[right],m_DataList[swap]) < 0)
				{
					swap = right;
				}

				if(swap == _index)
				{
					break;
				}

				Swap(_index,swap);

				_index = swap;
			}
		}

		private void Swap(int _prev,int _next)
		{
			(m_DataList[_next],m_DataList[_prev]) = (m_DataList[_prev],m_DataList[_next]);
		}

		public IEnumerator<TData> GetEnumerator()
		{
			lock(m_SyncRoot)
			{
				return m_DataList.GetEnumerator();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
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

			lock(m_SyncRoot)
			{
				if(_array is TData[] convert)
				{
					m_DataList.CopyTo(convert,_index);
				}
				else
				{
					LogTag.System.E("Invalid array type.");

					return;
				}
			}
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
				return m_DataList.Contains(_data);
			}
		}

		public bool IsEmpty => Count == 0;

		public int Count => m_DataList.Count;

		public bool IsSynchronized => true;
		public object SyncRoot => m_SyncRoot;
	}

	public class MinHeap<TData> : BinaryHeap<TData> where TData : IComparable<TData>
	{
		protected override int Compare(TData _first,TData _second)
		{
			return _first.CompareTo(_second);
		}
	}

	public class MaxHeap<TData> : BinaryHeap<TData> where TData : IComparable<TData>
	{
		protected override int Compare(TData _first,TData _second)
		{
			return _second.CompareTo(_first);
		}
	}
}