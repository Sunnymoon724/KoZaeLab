﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZUtility;
using System.Threading;
using KZLib.KZData;

namespace KZLib
{
	public class EffectMgr : AutoSingletonMB<EffectMgr>
	{
		private const float c_poolInterval = 30.0f;	// 30s

		private readonly Dictionary<string,List<PoolData>> m_poolDataDict = new();

		private string m_effectPath = null;

		[SerializeField,ReadOnly]
		private List<EffectClip> m_playingList = new();

		private readonly List<string> m_removeList = new();

		private CancellationTokenSource m_tokenSource = null;

		protected override void Initialize()
		{
			m_effectPath = ConfigMgr.In.Access<ConfigData.GameConfig>().FXPrefabPath;
		}

		private async UniTaskVoid CheckExpiredEffectAsync()
		{
			while(!m_tokenSource.Token.IsCancellationRequested)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(c_poolInterval),true);

				foreach(var (key,poolDataList) in m_poolDataDict)
				{
					poolDataList.RemoveAll(x => x.IsOverdue() && x.Release());

					if(poolDataList.Count == 0)
					{
						m_removeList.Add(key);
					}
				}

				foreach(var key in m_removeList)
				{
					m_poolDataDict.Remove(key);
				}

				m_removeList.Clear();

				if(m_poolDataDict.Count == 0)
				{
					break;
				}
			}
		}

		protected override void Release()
		{
			base.Release();

			DestroyAllEffectClip();
		}

		public void ReleaseEffect(EffectClip effectClip)
		{
			m_playingList.RemoveSafe(effectClip);

			_PutEffectClip(effectClip);
		}

		public EffectClip PlayEffect(string name,Vector3 position,Transform parent = null,EffectClip.EffectParam effectParam = null)
		{
			var effectClip = _GetEffectClip(name,parent);

			if(!effectClip)
			{
				var effectPath = _GetEffectPath(name);

				effectClip = ResMgr.In.GetObject<EffectClip>(effectPath,parent);

				if(!effectClip)
				{
					throw new NullReferenceException($"{name} is not exist. [{effectPath}]");
				}
			}

			effectClip.transform.position = position;
			effectClip.gameObject.EnsureActive(true);

			effectClip.SetEffect(effectParam);

			m_playingList.AddNotOverlap(effectClip);

			return effectClip;
		}

		public EffectClip PlayUIEffect(string name,Vector3 position,RectTransform parent = null,EffectClip.EffectParam effectParam = null)
		{
			var effectClip = PlayEffect(name,position,null,effectParam);

			parent.SetUIChild(effectClip.transform);

			return effectClip;
		}

		private string _GetEffectPath(string name)
		{
			return $"{m_effectPath}/{name}.prefab";
		}

		public void StopAllEffectClip()
		{
			for(var i=m_playingList.Count-1;i>=0;i--)
			{
				var clip = m_playingList[i];

				if(clip)
				{
					ReleaseEffect(clip);
				}
			}

			m_playingList.Clear();
		}

		public void DestroyAllEffectClip()
		{
			CommonUtility.KillTokenSource(ref m_tokenSource);

			for(var i=0;i<m_playingList.Count;i++)
			{
				var clip = m_playingList[i];

				if(clip)
				{
					clip.gameObject.DestroyObject();
				}
			}

            foreach(var (_,poolList) in m_poolDataDict)
			{
				for(var i=poolList.Count-1;i>=0;i--)
				{
					var data = poolList[i];

					data.Release();
					poolList.Remove(data);
				}
			}

			m_playingList.Clear();
			m_poolDataDict.Clear();
		}

		private void _PutEffectClip(EffectClip effectClip)
		{
			effectClip.transform.ResetTransform(transform);
			effectClip.gameObject.EnsureActive(false);

			m_poolDataDict.AddOrCreate(effectClip.name,new PoolData(effectClip));

			if(m_poolDataDict.Count == 1)
			{
				CommonUtility.RecycleTokenSource(ref m_tokenSource);

				CheckExpiredEffectAsync().Forget();
			}
		}

		private EffectClip _GetEffectClip(string name,Transform parent)
		{
			if(m_poolDataDict.TryGetValue(name, out var poolDataList) && poolDataList.Count > 0)
			{
				var poolData = poolDataList.PopFront();

				poolData.SetParent(parent);

				return poolData.GetEffectClip();
			}

			return null;
		}

		private record PoolData
		{
			private const float c_deleteTime = 60.0f;	// 60s
			public readonly EffectClip m_effectClip = null;
			private readonly long m_duration = 0L;

			public PoolData(EffectClip effectClip)
			{
				m_duration = DateTime.Now.AddSeconds(c_deleteTime).Ticks;
				m_effectClip = effectClip;
			}

			public bool IsOverdue()
			{
				return m_duration < DateTime.Now.Ticks;
			}

			public bool Release()
			{
				if(m_effectClip)
				{
					m_effectClip.gameObject.DestroyObject();

					return true;
				}

				return false;
			}

			public void SetParent(Transform parent)
			{
				m_effectClip.transform.parent = parent;
			}

			public EffectClip GetEffectClip()
			{
				return m_effectClip;
			}
		}
	}
}