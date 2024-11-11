using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib
{
	public partial class EffectMgr : AutoSingletonMB<EffectMgr>
	{
		private const float POOL_INTERVAL = 30.0f;   // 30s

		private readonly Dictionary<string,List<PoolData>> m_PoolDataDict = new();

		private string m_EffectPath = null;

		[SerializeField,ReadOnly]
		private List<EffectClip> m_PlayingList = new();

		private readonly List<string> m_RemoveList = new();

		protected override void Initialize()
		{
			m_EffectPath = GameSettings.In.FXPrefabPath;
		}

		private async void Start()
		{
			while(true)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(POOL_INTERVAL),true);

				foreach(var key in new List<string>(m_PoolDataDict.Keys))
				{
					var poolList = m_PoolDataDict[key];

					poolList.RemoveAll(data => data.IsOverdue() && data.Release());

					if(poolList.Count == 0)
					{
						m_RemoveList.Add(key);
					}
				}

				foreach(var key in m_RemoveList)
				{
					m_PoolDataDict.Remove(key);
				}

				m_RemoveList.Clear();
			}
		}

		protected override void Release()
		{
			base.Release();

			DestroyAllEffects();
		}

		public void ReleaseEffect(EffectClip _data)
		{
			m_PlayingList.RemoveSafe(_data);

			PutClip(_data);
		}

		public EffectClip PlayEffect(string _name,Vector3 _position,Transform _parent = null,EffectClip.EffectParam _param = null)
		{
			var clip = GetClip(_name,_parent);

			if(!clip)
			{
				var path = GetEffectPath(_name);

				clip = ResMgr.In.GetObject<EffectClip>(path,_parent);

				if(!clip)
				{
					throw new NullReferenceException($"{_name} is not exist. [{path}]");
				}
			}

			clip.transform.position = _position;
			clip.gameObject.SetActiveSelf(true);

			clip.SetEffect(_param);

			m_PlayingList.AddNotOverlap(clip);

			return clip;
		}

		public EffectClip PlayUIEffect(string _name,Vector3 _position,SortingLayerBase _parent,EffectClip.EffectParam _param = null)
		{
			var clip = PlayEffect(_name,_position,null,_param);

			if(!clip)
			{
				return null;
			}

			_parent.transform.SetUIChild(clip.transform);

			var sortingLayer = clip.GetComponent<SortingLayerBase>();

			if(!sortingLayer)
			{
				throw new NullReferenceException($"SortingLayerBase is not exist in {_name}");
			}

			sortingLayer.SetSortingLayerOrder(_parent.SortingLayerOrder+1);

			return clip;
		}

		private string GetEffectPath(string _name)
		{
			return $"{m_EffectPath}/{_name}.prefab";
		}

		public void StopAllEffects()
		{
			foreach(var clip in new List<EffectClip>(m_PlayingList))
			{
				if(clip)
				{
					ReleaseEffect(clip);
				}
			}

			m_PlayingList.Clear();
		}

		public void DestroyAllEffects()
		{
			foreach(var clip in new List<EffectClip>(m_PlayingList))
			{
				if(clip)
				{
					CommonUtility.DestroyObject(clip.gameObject);
				}
			}

			foreach(var pair in m_PoolDataDict)
			{
				foreach(var data in new List<PoolData>(pair.Value))
				{
					data.Release();
					pair.Value.Remove(data);
				}
			}

			m_PlayingList.Clear();
			m_PoolDataDict.Clear();
		}

		private void PutClip(EffectClip _effect)
		{
			_effect.transform.ResetTransform(transform);
			_effect.gameObject.SetActiveSelf(false);

			m_PoolDataDict.AddOrCreate(_effect.name,new PoolData(_effect));
		}

		private EffectClip GetClip(string _name,Transform _parent)
		{
			if(m_PoolDataDict.TryGetValue(_name, out var dataList) && dataList.Count > 0)
			{
				var data = dataList[0];

				dataList.RemoveAt(0);
				data.Clip.transform.parent = _parent;

				return data.Clip;
			}

			return null;
		}

		private record PoolData
		{
			private const float DEFAULT_DELETE_TIME = 60.0f;   // 60s
			public EffectClip Clip { get; }

			private readonly long m_Duration = 0L;

			public PoolData(EffectClip _clip)
			{
				m_Duration = DateTime.Now.AddSeconds(DEFAULT_DELETE_TIME).Ticks;
				Clip = _clip;
			}

			public bool IsOverdue()
			{
				return m_Duration < DateTime.Now.Ticks;
			}

			public bool Release()
			{
				if(Clip)
				{
					CommonUtility.DestroyObject(Clip.gameObject);

					return true;
				}

				return false;
			}
		}
	}
}