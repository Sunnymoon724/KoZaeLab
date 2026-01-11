	using UnityEngine;

	[ExecuteInEditMode]
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

		protected override void _Initialize()
		{
			base._Initialize();

			m_mainModule = m_particleSystem.main;
			m_startTime = Time.time;
		}

		protected override void Start()
		{
			base.Start();

			m_startTime = Time.time;
		}

		private void LateUpdate()
		{
			if(!m_target)
			{
				return;
			}

			var maxParticles = m_mainModule.maxParticles;

			if(m_particleArray == null || m_particleArray.Length < maxParticles)
			{
				m_particleArray = new ParticleSystem.Particle[maxParticles];
			}

			var count = m_particleSystem.GetParticles(m_particleArray);
			var force = m_forceCurve.Evaluate((Time.time-m_startTime)/m_forceLifeTime)*Time.deltaTime*m_force;
		
			var position = Vector3.zero;

			if(m_mainModule.simulationSpace == ParticleSystemSimulationSpace.Local)
			{
				position = transform.InverseTransformPoint(m_target.position);
			}
			else if(m_mainModule.simulationSpace == ParticleSystemSimulationSpace.World)
			{
				position = m_target.position;
			}
		
			for(var i=0;i<count;i++)
			{				
				m_particleArray[i].velocity += Vector3.Normalize(position-m_particleArray[i].position)*force;
			}

			m_particleSystem.SetParticles(m_particleArray,count);
		}
	}
