using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;

public class AudioButtonUI : BaseButtonUI
{
	[SerializeField,LabelText("Path Mode")]
	private bool m_PathMode = false;
	[SerializeField,LabelText("Audio Path"),ShowIf(nameof(m_PathMode))]
	private string m_AudioPath = null;
	[SerializeField,LabelText("Audio Clip"),HideIf(nameof(m_PathMode))]
	private AudioClip m_AudioClip = null;

	protected override void Initialize()
	{
		if(m_PathMode)
		{
			m_AudioClip = ResMgr.In.GetAudioClip(m_AudioPath);
		}

		base.Initialize();
	}

	protected override void OnClickedButton()
	{
		SoundMgr.In.PlayUIShot(m_AudioClip);
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