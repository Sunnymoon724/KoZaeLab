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

	private void Awake()
	{
		if(m_pathMode)
		{
			m_audioClip = ResourceManager.In.GetAudioClip(m_audioPath);
		}
	}

	protected override void _OnClickedButton()
	{
		SoundManager.In.PlaySFX(m_audioClip);
	}

	public void SetAudio(AudioClip audioClip)
	{
		m_audioClip = audioClip;

		m_pathMode = false;
	}

	public void SetAudio(string audioPath)
	{
		m_audioPath = audioPath;

		m_audioClip = ResourceManager.In.GetAudioClip(m_audioPath);

		m_pathMode = true;
	}
}