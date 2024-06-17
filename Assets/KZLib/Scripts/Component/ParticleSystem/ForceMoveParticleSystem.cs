	using UnityEngine;

	[ExecuteInEditMode]
	public class ForceMoveParticleSystem : BaseParticleSystem
	{
		[SerializeField]
		private Transform m_Target = null;
		[SerializeField]
		private float m_Force = 1.0f;
		[SerializeField]
		private AnimationCurve m_ForceCurve = AnimationCurve.EaseInOut(0.0f,1.0f,1.0f,1.0f);
		[SerializeField]
		private float m_ForceLifeTime = 1.0f;

		private float m_StartTime = 0.0f;
		private ParticleSystem.Particle[] m_ParticleArray = null;

		protected override void Awake()
		{
			base.Awake();

			m_MainModule = m_ParticleSystem.main;
			m_StartTime = Time.time;
		}

		private void Start()
		{
			m_StartTime = Time.time;			
		}

		private void LateUpdate()
		{
			if(!m_Target)
			{
				return;
			}

			var maxParticles = m_MainModule.maxParticles;

			if(m_ParticleArray == null || m_ParticleArray.Length < maxParticles)
			{
				m_ParticleArray = new ParticleSystem.Particle[maxParticles];
			}

			var count = m_ParticleSystem.GetParticles(m_ParticleArray);
			var force = m_ForceCurve.Evaluate((Time.time-m_StartTime)/m_ForceLifeTime)*Time.deltaTime*m_Force;
		
			var position = Vector3.zero;

			if(m_MainModule.simulationSpace == ParticleSystemSimulationSpace.Local)
			{
				position = transform.InverseTransformPoint(m_Target.position);
			}
			else if(m_MainModule.simulationSpace == ParticleSystemSimulationSpace.World)
			{
				position = m_Target.position;
			}
		
			for(var i=0;i<count;i++)
			{				
				m_ParticleArray[i].velocity += Vector3.Normalize(position-m_ParticleArray[i].position)*force;
			}

			m_ParticleSystem.SetParticles(m_ParticleArray,count);
		}
	}
