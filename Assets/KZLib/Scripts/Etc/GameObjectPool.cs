﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace KZLib
{
	/// <summary>
	/// 유니티 게임 오브젝트를 관리하는 객체 풀 클래스입니다.
	/// </summary>
	public class GameObjectPool
	{
		private readonly GameObject m_Pivot = null;
		private readonly Queue<GameObject> m_ObjectQueue = null;
		private readonly Transform m_Storage = null;

		public GameObjectPool(GameObject _pivot,Transform _storage,int _capacity)
		{
			if(!_pivot)
			{
				throw new ArgumentNullException("피봇이 null 입니다.");
			}

			if(!_storage)
			{
				throw new ArgumentNullException("스토리지가 null입니다.");
			}

			m_Pivot = _pivot;
			m_ObjectQueue = new(_capacity);

			m_Storage = _storage;
		}

		/// <summary>
		/// 부모와 자식 게임 오브젝트를 설정합니다.
		/// </summary>
		protected virtual void SetChild(Transform _parent,Transform _child)
		{
			_parent.SetChild(_child);
		}

		/// <summary>
		/// 풀에 반환하고 비활성화합니다.
		/// </summary>
		public void Put(GameObject _data)
		{
			SetChild(m_Storage,_data.transform);

			_data.name = m_Pivot.name;
			_data.SetActiveSelf(false);

			m_ObjectQueue.Enqueue(_data);
		}

		/// <summary>
		/// 풀에서 인스턴스를 하나 꺼낸다.
		/// 만약 풀에 여유분이 없으면 새로 생성한다.
		/// </summary>
		public GameObject Get(Transform _parent = null)
		{
			var data = m_ObjectQueue.Count > 0 ? m_ObjectQueue.Dequeue() : UnityUtility.CopyObject(m_Pivot);

			if(_parent)
			{
				SetChild(_parent,data.transform);
			}

			data.SetActiveSelf(true);

			return data;
		}

		/// <summary>
		/// 풀에서 인스턴스를 하나 꺼낸다.
		/// 만약 풀에 여유분이 없으면 새로 생성한다.
		/// </summary>
		public TComponent Get<TComponent>(Transform _parent = null) where TComponent : Component
		{
			var data = Get(_parent);

			return data.GetComponent<TComponent>();
		}
	}
}