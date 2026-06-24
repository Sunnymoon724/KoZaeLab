using UnityEngine;

/// <summary>
/// Common base for <see cref="ParticleSystem"/> wrappers.
/// </summary>
/// <remarks>
/// Caches <see cref="m_particleSystem"/> and <see cref="m_mainModule"/> for derived types that read or mutate live particles.
/// </remarks>
[RequireComponent(typeof(ParticleSystem))]
public abstract class BaseParticleSystem : BaseComponent
{
	[SerializeField]
	protected ParticleSystem m_particleSystem = null;

	protected ParticleSystem.MainModule m_mainModule = default;

#if UNITY_EDITOR
	private bool m_warnedMissingCustomSpace = false;
#endif

	/// <summary>One-time init: resolves <see cref="m_particleSystem"/> at runtime and refreshes <see cref="m_mainModule"/>.</summary>
	protected override void _Initialize()
	{
		base._Initialize();

		_EnsureParticleSystem();
	}

	/// <summary>Auto-assigns <see cref="m_particleSystem"/> when the component is added or Reset is invoked in the editor.</summary>
	protected override void Reset()
	{
		base.Reset();

		_EnsureParticleSystem();
	}

	/// <summary>Resolves and caches <see cref="m_particleSystem"/> and <see cref="m_mainModule"/>.</summary>
	protected bool _EnsureParticleSystem()
	{
		if(!m_particleSystem)
		{
			m_particleSystem = GetComponent<ParticleSystem>();
		}

		if(m_particleSystem)
		{
			m_mainModule = m_particleSystem.main;
		}

		return m_particleSystem;
	}

	/// <summary>
	/// Converts a world-space point into coordinates used by live particles (<see cref="ParticleSystem.GetParticles"/> / <see cref="ParticleSystem.SetParticles"/>).
	/// </summary>
	protected Vector3 TransformWorldToSimulationSpace(Vector3 worldPosition)
	{
		switch(m_mainModule.simulationSpace)
		{
			case ParticleSystemSimulationSpace.Local:
				return transform.InverseTransformPoint(worldPosition);

			case ParticleSystemSimulationSpace.Custom:
			{
				var customSpace = m_mainModule.customSimulationSpace;

				if(customSpace)
				{
					return customSpace.InverseTransformPoint(worldPosition);
				}

#if UNITY_EDITOR
				if(!m_warnedMissingCustomSpace)
				{
					m_warnedMissingCustomSpace = true;

					Debug.LogWarning($"{nameof(BaseParticleSystem)} on '{name}' uses Custom simulation space but customSimulationSpace is not assigned. Falling back to world space.",this);
				}
#endif

				return worldPosition;
			}

			default:
				return worldPosition;
		}
	}
}
