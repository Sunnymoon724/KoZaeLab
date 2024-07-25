using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;

public class AudioButtonUI : BaseButtonUI
{
	[SerializeField,LabelText("패스 모드")]
	private bool m_PathMode = false;
	[SerializeField,LabelText("오디오 경로"),ShowIf(nameof(m_PathMode))]
	private string m_AudioPath = null;
	[SerializeField,LabelText("오디오 클립"),HideIf(nameof(m_PathMode))]
	private AudioClip m_AudioClip = null;

	protected override void Initialize()
	{
		if(m_PathMode)
		{
			m_AudioClip = ResMgr.In.GetAudioClip(m_AudioPath);
		}

		base.Initialize();
	}

	protected override void OnClickButton()
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