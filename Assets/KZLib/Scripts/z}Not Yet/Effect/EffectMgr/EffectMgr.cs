using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib
{
	public partial class EffectMgr : AutoSingletonMB<EffectMgr>
	{
		private const float POOL_INTERVAL = 30.0f;   // 30초

		private string m_EffectPath = null;

		[SerializeField,ReadOnly]
		private List<EffectClip> m_PlayingList = new();

		protected override void Initialize()
		{
			m_EffectPath = GameSettings.In.EffectPrefabPath;
		}

		private readonly List<string> m_RemoveList = new();

		private async void Start()
		{
			while(true)
			{
				await UniTask.WaitForSeconds(POOL_INTERVAL,true);

				foreach(var pair in m_PoolDataDict)
				{
					foreach(var data in new List<PoolData>(pair.Value))
					{
						//? 기한이 지난 것들
						if(data.IsOverdue())
						{
							data.Release();
							pair.Value.Remove(data);
						}
					}

					if(pair.Value.Count <= 0)
					{
						m_RemoveList.Add(pair.Key);
					}
				}

				foreach(var remove in m_RemoveList)
				{
					m_PoolDataDict.Remove(remove);
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
			// 옵션 체크 해서 현재 옵션상 사용 불가하면 null
			// if (CommonFunc.GetCurrentEffectState() == Data.SystemOption.OPTION_STATE.eAllOff)
			// {
			// 	return null;
			// }

			var data = GetClip(_name,_parent);

			if(!data)
			{
				data = ResMgr.In.GetObject<EffectClip>(GetEfxPath(_name),_parent);

				if(!data)
				{
					throw new Exception("해당 이펙트에 이펙트 클립이 없습니다.");
				}
			}

			data.transform.position = _position;
			data.gameObject.SetActiveSelf(true);

			data.Initialize(_param);

			m_PlayingList.AddNotOverlap(data);

			return data;
		}

		public EffectClip PlayUIEffect(string _name,Vector3 _position,SortingLayerBase _parent,EffectClip.EffectParam _param = null)
		{
			var data = PlayEffect(_name,_position,null,_param);

			if(!data)
			{
				return null;
			}

			_parent.transform.SetUIChild(data.transform);

			// var scale = data.transform.localScale;

			// data.transform.position = _position;
			// data.transform.localScale = scale;

			var renderer = data.GetComponent<SortingLayerBase>();

			if(!renderer)
			{
				throw new NullReferenceException(string.Format("{0}에 SortingLayerBase가 없습니다.",_name));
			}

			renderer.SetSortingLayerOrder(_parent.SortingLayerOrder+1);
			
			return data;
		}

		private string GetEfxPath(string _name)
		{
			return string.Format("{0}/{1}/{1}.prefab",m_EffectPath,_name);
		}

		public void StopAllEffects()
		{
			foreach(var data in new List<EffectClip>(m_PlayingList))
			{
				if(!data)
				{
					continue;
				}

				ReleaseEffect(data);
			}

			m_PlayingList.Clear();
		}

		public void DestroyAllEffects()
		{
			foreach(var data in new List<EffectClip>(m_PlayingList))
			{
				if(!data)
				{
					continue;
				}

				CommonUtility.DestroyObject(data.gameObject);
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
	}
}