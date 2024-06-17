using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;

public class AudioButtonUI : BaseButtonUI
{
	[SerializeField]
	private bool m_PathMode = false;
	[SerializeField,ShowIf(nameof(m_PathMode))]
	private string m_AudioPath = null;
	[SerializeField,HideIf(nameof(m_PathMode))]
	private AudioClip m_AudioClip = null;

	protected override void Awake()
	{
		base.Awake();

		if(m_PathMode)
		{
			m_AudioClip = ResMgr.In.GetAudioClip(m_AudioPath);
		}

		m_Button.SetOnClickListener(()=>
		{
			SoundMgr.In.PlayEffect(m_AudioClip,true);
		});
	}

	public void SetAudio(AudioClip _clip)
	{
		m_AudioClip = _clip;

		m_PathMode = false;
	}

	public void SetAudio(string _audioPath)
	{
		m_AudioPath = _audioPath;

		m_AudioClip = ResMgr.In.GetAudioClip(m_AudioPath);

		m_PathMode = true;
	}
}