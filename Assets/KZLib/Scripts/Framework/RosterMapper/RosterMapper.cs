using System;
using System.Collections.Generic;
using UnityEngine;

namespace KZLib.Utilities
{
	/// <summary>
	/// Maps an ordered data roster to pooled <typeparamref name="TItem"/> views.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="GameObjectPawnPool{TComponent}"/> owns clone, activate, and storage lifecycle.
	/// This type owns roster length, display order, and the per-item binding callback.
	/// </para>
	/// <para>
	/// <see cref="TrySetDataList"/> grows or shrinks the active view count to match the input list,
	/// invokes <see cref="Action{T1,T2}"/> for each pair in order, and returns surplus views to the pool.
	/// </para>
	/// <para>
	/// Used by non-virtualized list UIs such as <c>Accordion</c>, <c>Carousel</c>, and <c>CarouselNavigator</c>.
	/// For virtualized scrolling, use <c>ReuseScrollRect</c> or <c>ReuseGridLayoutGroup</c> instead.
	/// </para>
	/// </remarks>
	/// <typeparam name="TItem">Pooled view component; must extend <see cref="GameObjectPawn"/>.</typeparam>
	/// <typeparam name="UData">Data entry bound to each active view.</typeparam>
	public class RosterMapper<TItem,UData> : IDisposable where TItem : GameObjectPawn
	{
		/// <summary>Tracks whether <see cref="Dispose"/> has already run.</summary>
		private bool m_disposed = false;

		/// <summary>Parent transform for active views checked out from the pool.</summary>
		private readonly Transform m_content = null;

		/// <summary>Called once per active view when roster data is applied.</summary>
		private readonly Action<TItem,UData> m_onBindData = null;

		/// <summary>Active views in roster display order.</summary>
		private readonly List<TItem> m_itemList = new();

		/// <summary>Reused buffer for <see cref="TrySetData"/> to avoid per-call list allocation.</summary>
		private readonly List<UData> m_singleDataList = new(1);

		/// <summary>Internal pool that supplies and stores inactive views.</summary>
		private readonly GameObjectPawnPool<TItem> m_objectPool = null;

		/// <summary>All currently active bound views in roster order.</summary>
		public IEnumerable<TItem> ItemGroup => m_itemList;

		/// <summary>Number of active bound views.</summary>
		public int ItemCount => m_itemList.Count;

		/// <summary>
		/// Creates a mapper backed by a new <see cref="GameObjectPawnPool{TComponent}"/>.
		/// The pivot template is deactivated and moved to storage immediately.
		/// </summary>
		/// <param name="pivot">Template component cloned by the internal pool.</param>
		/// <param name="content">Parent transform for active views.</param>
		/// <param name="onBindData">Called once per visible view when data is applied.</param>
		/// <param name="storage">Inactive pool storage; defaults to <see cref="UIManager.In"/> storage when null.</param>
		/// <param name="capacity">Initial pool capacity passed to <see cref="GameObjectPawnPool{TComponent}"/>.</param>
		/// <param name="deleteTime">Seconds after <see cref="GameObjectPawnPool{TComponent}.Put"/> before inactive instances are purged.</param>
		/// <exception cref="ArgumentNullException"><paramref name="pivot"/>, <paramref name="content"/>, or <paramref name="onBindData"/> is null.</exception>
		public RosterMapper(TItem pivot,Transform content,Action<TItem,UData> onBindData,Transform storage = null,int capacity = 1,float deleteTime = 60.0f)
		{
			if(!pivot)
			{
				throw new ArgumentNullException(nameof(pivot));
			}

			if(!content)
			{
				throw new ArgumentNullException(nameof(content));
			}

			m_content = content;
			m_onBindData = onBindData ?? throw new ArgumentNullException(nameof(onBindData));

			var currentStorage = storage == null ? UIManager.In.GetStorage(false) : storage;

			m_objectPool = new GameObjectPawnPool<TItem>(pivot,currentStorage,capacity,false,deleteTime);

			pivot.gameObject.EnsureActive(false);
			currentStorage.SetChild(pivot.transform);
		}

		/// <summary>Returns every active view to the pool and disposes the internal pool.</summary>
		public void Dispose()
		{
			_Dispose(true);
			GC.SuppressFinalize(this); 
		}

		/// <summary>Releases managed resources when <paramref name="disposing"/> is true.</summary>
		protected virtual void _Dispose(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				Clear();
				m_objectPool?.Dispose();
			}

			m_disposed = true;
		}

		/// <summary>Convenience wrapper for binding a single roster entry.</summary>
		/// <returns><c>true</c> when binding succeeds; <c>false</c> when data is cleared.</returns>
		/// <exception cref="ObjectDisposedException">This mapper has been disposed.</exception>
		public bool TrySetData(UData data)
		{
			_ThrowIfDisposed();

			m_singleDataList.Clear();
			m_singleDataList.Add(data);

			return TrySetDataList(m_singleDataList);
		}

		/// <summary>
		/// Syncs active views to <paramref name="dataList"/> count, binds data in order, and returns surplus views to the pool.
		/// </summary>
		/// <returns><c>false</c> and clears active views when <paramref name="dataList"/> is null or empty; otherwise <c>true</c>.</returns>
		/// <exception cref="ObjectDisposedException">This mapper has been disposed.</exception>
		public bool TrySetDataList(List<UData> dataList)
		{
			_ThrowIfDisposed();

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

		/// <summary>Returns every active view to the pool and clears the active list.</summary>
		public void Clear()
		{
			if(m_disposed)
			{
				return;
			}

			_PutRemainingObject(0);
		}

		/// <summary>Puts views from <paramref name="index"/> through the end back into the pool and removes them from the active list.</summary>
		private void _PutRemainingObject(int index)
		{
			for(var i=m_itemList.Count-1;i>=index;i--)
			{
				var item = m_itemList[i];

				m_objectPool.Put(item);
				m_itemList.RemoveAt(i);
			}
		}

		/// <summary>Returns the active view at <paramref name="index"/>.</summary>
		/// <returns>The view at the index, or null when out of range or disposed.</returns>
		public TItem GetItemByIndex(int index)
		{
			if(m_disposed)
			{
				return null;
			}

			return m_itemList.ContainsIndex(index) ? m_itemList[index] : null;
		}

		/// <summary>Steps <paramref name="count"/> views from <paramref name="target"/> in roster order.</summary>
		/// <param name="canLoop">When true, stepping past either end wraps around the roster.</param>
		/// <returns>The stepped view, <paramref name="target"/> when <paramref name="count"/> is zero, or null when not found or disposed.</returns>
		public TItem GetNearItem(TItem target,int count,bool canLoop)
		{
			if(m_disposed)
			{
				return null;
			}

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

		/// <summary>Returns the roster index of <paramref name="target"/>.</summary>
		/// <returns>The zero-based index, or <c>-1</c> when not found or disposed.</returns>
		public int FindIndex(TItem target)
		{
			if(m_disposed)
			{
				return -1;
			}

			if(!m_itemList.Contains(target))
			{
				return -1;
			}

			return m_itemList.IndexOf(target);
		}

		/// <summary>Throws <see cref="ObjectDisposedException"/> when this mapper has been disposed.</summary>
		private void _ThrowIfDisposed()
		{
			if(m_disposed)
			{
				throw new ObjectDisposedException(nameof(RosterMapper<TItem,UData>));
			}
		}
	}
}