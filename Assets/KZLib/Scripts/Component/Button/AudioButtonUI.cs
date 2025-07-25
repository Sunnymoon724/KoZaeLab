using KZLib;
using Sirenix.OdinInspector;
using UnityEngine;

public class AudioButtonUI : BaseButtonUI
{
	[SerializeField]
	private bool m_pathMode = false;

	[SerializeField,ShowIf(nameof(m_pathMode))]
	private string m_audioPath = null;
	[SerializeField,HideIf(nameof(m_pathMode))]
	private AudioClip m_audioClip = null;

	protected override void Initialize()
	{
		if(m_pathMode)
		{
			m_audioClip = ResMgr.In.GetAudioClip(m_audioPath);
		}

		base.Initialize();
	}

	protected override void OnClickedButton()
	{
		SoundMgr.In.PlaySFX(m_audioClip);
	}

	public void SetAudio(AudioClip audioClip)
	{
		m_audioClip = audioClip;

		m_pathMode = false;
	}

	public void SetAudio(string audioPath)
	{
		m_audioPath = audioPath;

		m_audioClip = ResMgr.In.GetAudioClip(m_audioPath);

		m_pathMode = true;
	}
}