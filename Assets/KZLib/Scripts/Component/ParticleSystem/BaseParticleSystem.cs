
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(ParticleSystem))]
public abstract class BaseParticleSystem : BaseComponent
{
	[SerializeField]
	protected ParticleSystem m_ParticleSystem = null;

	protected ParticleSystem.MainModule m_MainModule;
	
	protected override void Reset()
	{
		base.Reset();

		if(!m_ParticleSystem)
		{
			m_ParticleSystem = GetComponent<ParticleSystem>();

			if(m_ParticleSystem)
			{
				m_MainModule = m_ParticleSystem.main;
			}
		}
	}
}