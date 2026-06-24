using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib;
using KZLib.Utilities;
using KZLib.Data;

namespace KZLib.Effects
{
	/// <summary>Pooled <see cref="EffectClip"/> spawn/cache. Prefab roots should be inactive; <see cref="PlayEffect"/> disables before <see cref="EffectClip.Set"/> then enables.</summary>
	[SingletonConfig(AutoCreate = true,DontDestroy = true)]
	public class EffectManager : SingletonMB<EffectManager>
	{
		private string m_effectPath = null;

		[SerializeField,ReadOnly]
		private List<EffectClip> m_playingList = new();

		private CacheResolver<EffectClip> m_cacheResolver = new();

		protected override void _Initialize()
		{
			base._Initialize();

			m_effectPath = ConfigManager.In.Fetch<GameConfig>().FXPrefabPath;
		}

		protected override void _Release()
		{
			ClearAllEffectClip();

			base._Release();
		}

		/// <summary>Removes a clip from the playing list without pooling (e.g. misconfiguration destroy).</summary>
		public void UnregisterPlayingEffect(EffectClip effectClip)
		{
			if(!effectClip)
			{
				return;
			}

			m_playingList.RemoveSafe(effectClip);
		}

		/// <summary>Returns the instance to the cache. <see cref="EffectClip"/> disable reports <see cref="EffectClip.Param.OnComplete"/> false unless already completed.</summary>
		public void ReleaseEffect(EffectClip effectClip)
		{
			if(!effectClip)
			{
				return;
			}

			m_playingList.RemoveSafe(effectClip);

			_StoreEffectClip(effectClip);
		}

		/// <summary>Spawns or reuses a pooled effect. <paramref name="name"/> must match the prefab file/root name used as the cache key. Returns null when setup fails or playback already finished synchronously (<see cref="EffectClip.Param.OnComplete"/> may still run).</summary>
		public EffectClip PlayEffect(string name,Vector3 position,Transform parent = null,EffectClip.Param effectParam = null)
		{
			var effectClip = _IsEffectNamePlaying(name) ? null : _GetEffectClip(name,parent);

			if(!effectClip)
			{
				var effectPath = _GetEffectPath(name);

				effectClip = ResourceManager.In.GetObject<EffectClip>(effectPath,parent);

				if(!effectClip)
				{
					throw new InvalidOperationException($"{name} does not exist. [{effectPath}]. Name and EffectPath must be assigned.");
				}
			}

			effectClip.gameObject.name = name;
			effectClip.transform.position = position;

			m_playingList.AddIfAbsent(effectClip);

			// Ensure Set runs before OnEnable so callback/param wiring is ready (active prefabs included).
			effectClip.gameObject.EnsureActive(false);
			effectClip.Set(effectParam);
			effectClip.gameObject.EnsureActive(true);

			if(!effectClip)
			{
				UnregisterPlayingEffect(effectClip);

				return null;
			}

			// Sync natural complete during OnEnable (e.g. instant particle stop) already pooled the instance.
			if(!effectClip.gameObject.activeSelf)
			{
				UnregisterPlayingEffect(effectClip);

				return null;
			}

			return effectClip;
		}

		public EffectClip PlayUIEffect(string name,Vector3 position,RectTransform parent = null,EffectClip.Param effectParam = null)
		{
			if(!parent)
			{
				LogChannel.FX.E($"{nameof(PlayUIEffect)} parent is null.");

				return null;
			}

			var effectClip = PlayEffect(name,position,null,effectParam);

			if(!effectClip)
			{
				return null;
			}

			parent.SetChild(effectClip.transform,false);

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
			for(var i=m_playingList.Count-1;i>=0;i--)
			{
				var clip = m_playingList[i];

				if(clip)
				{
					clip.gameObject.DestroyObject();
				}
			}

			m_playingList.Clear();

			for(var i=transform.childCount-1;i>=0;i--)
			{
				var clip = transform.GetChild(i).GetComponent<EffectClip>();

				if(clip)
				{
					clip.gameObject.DestroyObject();
				}
			}

			m_cacheResolver.Dispose();
			m_cacheResolver = new CacheResolver<EffectClip>();
		}

		private void _StoreEffectClip(EffectClip effectClip)
		{
			_DestroyDuplicateCachedClips(effectClip);

			effectClip.transform.ResetTransform(transform);
			effectClip.gameObject.EnsureActive(false);

			m_cacheResolver.StoreCache(effectClip.name,effectClip,true);
		}

		private void _DestroyDuplicateCachedClips(EffectClip effectClip)
		{
			for(var i=transform.childCount-1;i>=0;i--)
			{
				var clip = transform.GetChild(i).GetComponent<EffectClip>();

				if(!clip || clip == effectClip || clip.name != effectClip.name)
				{
					continue;
				}

				if(clip.gameObject.activeSelf || m_playingList.Contains(clip))
				{
					continue;
				}

				clip.gameObject.DestroyObject();
			}
		}

		private bool _IsEffectNamePlaying(string name)
		{
			for(var i=0;i<m_playingList.Count;i++)
			{
				var clip = m_playingList[i];

				if(clip && clip.name == name)
				{
					return true;
				}
			}

			return false;
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