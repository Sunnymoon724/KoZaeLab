
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public abstract class BaseParticleSystem : BaseComponent
{
	[SerializeField]
	protected ParticleSystem m_particleSystem = null;

	protected ParticleSystem.MainModule m_mainModule = default;

	protected override void Reset()
	{
		base.Reset();

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