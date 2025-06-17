using KZLib.KZData;
using KZLib.KZUtility;
using System.Collections.Generic;
using UnityEngine;

namespace KZLib
{
	public partial class SoundMgr : LoadSingletonMB<SoundMgr>
	{
		private const int c_sfxCount = 10;

		[SerializeField]
		private Transform m_sfxBox = null;

		private readonly List<AudioSource> m_sfxList = new(c_sfxCount);

		private SoundVolume m_sfxVolume = SoundVolume.max;

		public AudioSource PlaySFX(string audioPath,bool ignoreTime = false)
		{
			if(audioPath.IsEmpty())
			{
				LogSvc.System.E("Audio path is empty");

				return null;
			}

			return PlaySFX(ResMgr.In.GetAudioClip(audioPath),ignoreTime);
		}

		public AudioSource PlaySFX(AudioClip audioClip,bool ignoreTime = false)
		{
			if(!audioClip)
			{
				LogSvc.System.E("Audio clip is null");

				return null;
			}

			var audioSource = _SetSFX(audioClip);

			audioSource.ignoreListenerPause = ignoreTime;

			audioSource.PlayOneShot(audioClip,m_sfxVolume.level);

			_PlaySound(audioSource);

			return audioSource;
		}

		public void StopAllSFX()
		{
			for(var i=0;i<m_sfxList.Count;i++)
			{
				StopSFX(m_sfxList[i]);
			}
		}

		public void StopSFX(string sfxName)
		{
			var effect = m_sfxList.Find(x=>x.clip.name.IsEqual(sfxName));

			if(effect)
			{
				StopSFX(effect);
			}
		}

		public void StopEffect(AudioClip _clip)
		{
			var effect = m_sfxList.Find(x=>x.clip.Equals(_clip));

			if(effect)
			{
				StopSFX(effect);
			}
		}

		public void StopSFX(AudioSource audioSource)
		{
			_StopSound(audioSource,true);
		}

		private AudioSource _SetSFX(AudioClip audioClip)
		{
			var source = FindEmptySFX();

			if(!source)
			{
				source = CreateSFX(m_sfxList.Count);
			}

			_SetAudioSource(source,audioClip,string.Format("[Effect] {0}",audioClip.name),false,m_sfxVolume);

			return source;
		}

		private AudioSource FindEmptySFX()
		{
			for(var i=0;i<m_sfxList.Count;i++)
			{
				if(!m_sfxList[i].isPlaying)
				{
					return m_sfxList[i];
				}
			}

			return null;
		}

		private AudioSource CreateSFX(int order)
		{
			var child = m_sfxBox.AddChild($"SoundEffect_{order}");
			var source = child.gameObject.AddComponent<AudioSource>();

			source.loop = false;
			source.playOnAwake = false;

			return source;
		}
	}
}