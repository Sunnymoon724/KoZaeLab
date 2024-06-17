using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace KZLib
{
	public partial class SoundMgr : LoadSingletonMB<SoundMgr>
	{
		private const string MIXER_MASTER = "Mixer_Master";
		private const string MIXER_MUSIC = "Mixer_Music";
		private const string MIXER_EFFECT = "Mixer_Effect";

		[SerializeField] private AudioMixer m_Mixer = null;

		public bool AudioTransitioned { get; private set; }

		private readonly Dictionary<string,AudioMixerGroup> m_MixerDict = new();

		private AudioMixerGroup GetAudioMixerGroup(string _key)
		{
			return m_MixerDict.TryGetValue(_key,out var data) ? data : null;
		}

		public void SetAudioSlow(bool _isOn,float _duration)
		{
			var snapshotName = _isOn ? "Slow" : "Snapshot";

			TransitionTo(snapshotName,_duration);
			AudioTransitioned = _isOn;
		}

		public void SetAudioFast(bool _isOn,float _duration)
		{
			var snapshotName = _isOn ? "Fast" : "Snapshot";

			TransitionTo(snapshotName,_duration);
			AudioTransitioned = _isOn;
		}
		
		private void TransitionTo(string _snapshotName,float _duration)
		{
			var snapshot = GetAudioMixerGroup(MIXER_MASTER).audioMixer.FindSnapshot(_snapshotName);

			if(snapshot)
			{
				snapshot.TransitionTo(_duration);
			}
		}
	}
}