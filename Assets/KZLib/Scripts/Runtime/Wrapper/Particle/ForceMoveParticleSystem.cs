using UnityEngine;

/// <summary>
/// Applies a force that moves live particles toward or away from a target <see cref="Transform"/>.
/// </summary>
/// <remarks>
/// Force magnitude follows <see cref="m_forceCurve"/> over <see cref="m_forceLifeTime"/> seconds from each <see cref="OnEnable"/>.
/// Positive force pulls particles in; negative force pushes them away.
/// Respects the parent <see cref="ParticleSystem"/>'s simulation space (Local, World, Custom).
/// </remarks>
public class ForceMoveParticleSystem : BaseParticleSystem
{
	[SerializeField]
	private Transform m_target = null;
	[SerializeField]
	private float m_force = 1.0f;
	[SerializeField]
	private AnimationCurve m_forceCurve = AnimationCurve.EaseInOut(0.0f,1.0f,1.0f,1.0f);
	[SerializeField]
	private float m_forceLifeTime = 1.0f;

	private float m_startTime = 0.0f;
	private ParticleSystem.Particle[] m_particleArray = null;

	/// <summary>Resets curve timing whenever the component is enabled (supports pooling and re-activation).</summary>
	protected override void OnEnable()
	{
		base.OnEnable();

		m_startTime = Time.time;
	}

	private void LateUpdate()
	{
		if(!m_target || !_EnsureParticleSystem())
		{
			return;
		}

		if(!m_particleSystem.isPlaying)
		{
			return;
		}

		var maxParticles = m_mainModule.maxParticles;

		if(m_particleArray == null || m_particleArray.Length < maxParticles)
		{
			m_particleArray = new ParticleSystem.Particle[maxParticles];
		}

		var count = m_particleSystem.GetParticles(m_particleArray);

		if(count <= 0)
		{
			return;
		}

		var normalizedTime = m_forceLifeTime > 0.0f ? Mathf.Clamp01((Time.time-m_startTime)/m_forceLifeTime) : 1.0f;
		var force = m_forceCurve.Evaluate(normalizedTime)*Time.deltaTime*m_force;

		if(Mathf.Approximately(force,0.0f))
		{
			return;
		}

		var position = TransformWorldToSimulationSpace(m_target.position);

		for(var i=0;i<count;i++)
		{
			var delta = position-m_particleArray[i].position;
			var sqrDistance = delta.sqrMagnitude;

			if(sqrDistance <= Mathf.Epsilon)
			{
				continue;
			}

			m_particleArray[i].velocity += delta*(force/Mathf.Sqrt(sqrDistance));
		}

		m_particleSystem.SetParticles(m_particleArray,count);
	}
}
