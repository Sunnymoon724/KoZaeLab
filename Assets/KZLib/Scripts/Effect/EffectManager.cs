using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZUtility;
using KZLib.KZData;
using KZLib.KZDevelop;

namespace KZLib
{
	public class EffectManager : AutoSingletonMB<EffectManager>
	{
		private string m_effectPath = null;

		[SerializeField,ReadOnly]
		private List<EffectClip> m_playingList = new();

		private readonly CacheResolver<EffectClip> m_cacheResolver = new();

		protected override void Initialize()
		{
			m_effectPath = ConfigManager.In.Access<GameConfig>().FXPrefabPath;
		}

		protected override void Release()
		{
			base.Release();

			ClearAllEffectClip();
		}

		public void ReleaseEffect(EffectClip effectClip)
		{
			m_playingList.RemoveSafe(effectClip);

			_StoreEffectClip(effectClip);
		}

		public EffectClip PlayEffect(string name,Vector3 position,Transform parent = null,EffectClip.Param effectParam = null)
		{
			var effectClip = _GetEffectClip(name,parent);

			if(!effectClip)
			{
				var effectPath = _GetEffectPath(name);

				effectClip = ResourceManager.In.GetObject<EffectClip>(effectPath,parent);

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

		public EffectClip PlayUIEffect(string name,Vector3 position,RectTransform parent = null,EffectClip.Param effectParam = null)
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

		public void ClearAllEffectClip()
		{
			for(var i=0;i<m_playingList.Count;i++)
			{
				var clip = m_playingList[i];

				if(clip)
				{
					clip.gameObject.DestroyObject();
				}
			}

			m_playingList.Clear();
			m_cacheResolver.Dispose();
		}

		private void _StoreEffectClip(EffectClip effectClip)
		{
			effectClip.transform.ResetTransform(transform);
			effectClip.gameObject.EnsureActive(false);

			m_cacheResolver.StoreCache(effectClip.name,effectClip,true);
		}

		private EffectClip _GetEffectClip(string name,Transform parent)
		{
			if(m_cacheResolver.TryGetCache(name,out var cache))
			{
				parent.SetChild(cache.transform);

				return cache;
			}

			return null;
		}
	}
}