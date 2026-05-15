
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public abstract class BaseParticleSystem : MonoBehaviour
{
	[SerializeField]
	protected ParticleSystem m_particleSystem = null;

	protected ParticleSystem.MainModule m_mainModule = default;

	private void Reset()
	{
		if(!m_particleSystem)
		{
			m_particleSystem = GetComponent<ParticleSystem>();

			if(m_particleSystem)
			{
				m_mainModule = m_particleSystem.main;
			}
		}
	}
}