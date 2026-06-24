using KZLib;
using KZLib.Sounds;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Button that plays a 2D effect clip on click. Supports direct <see cref="AudioClip"/> or resource path lookup.
/// </summary>
public class AudioButtonUI : BaseButtonUI
{
	[SerializeField]
	private bool m_pathMode = false;

	[SerializeField,ShowIf(nameof(m_pathMode))]
	private string m_audioPath = null;
	[SerializeField,HideIf(nameof(m_pathMode))]
	private AudioClip m_audioClip = null;

	protected override void _Initialize()
	{
		base._Initialize();

		if(m_pathMode)
		{
			m_audioClip = ResourceManager.In.GetAudioClip(m_audioPath);
		}
	}

	protected override void _OnClickedButton()
	{
		if(!m_audioClip)
		{
			return;
		}

		SoundManager.In.PlayEffect2D(m_audioClip);
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