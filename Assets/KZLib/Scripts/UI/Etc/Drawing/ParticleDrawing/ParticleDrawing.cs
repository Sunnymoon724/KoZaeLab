using KZLib;

namespace UnityEngine.UI
{
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticleDrawing : GraphicDrawing
	{
		private const int c_maxParticleCount = c_maxVertexCount/4;

		[SerializeField]
		private bool m_useLateUpdate = true;

		[SerializeField]
		private ParticleSystem m_particleSystem = null;

		[SerializeField]
		private Material m_particleMaterial = null;

		private ParticleSystem.MainModule m_mainModule = default;
		private ParticleSystem.TextureSheetAnimationModule m_textureSheetAnimation = default;
		private int m_textureSheetAnimationFrames = 0;
		private Vector2 m_textureSheetAnimationFrameSize = Vector2.zero;



		private ParticleSystem.Particle[] m_particleArray = null;
		private readonly UIVertex[] m_quadVertexArray = new UIVertex[4];

		

		private ParticleSystem.Particle[] ParticleArray
		{
			get
			{
				return m_particleArray ??= new ParticleSystem.Particle[m_mainModule.maxParticles];
			}
		}

		private bool m_initialize = false;

		protected override void Awake()
		{
			base.Awake();

			_EnsureInitialized();
		}

		private void _EnsureInitialized()
		{
			if(m_initialize)
			{
				return;
			}

			if(m_mainModule.maxParticles > c_maxParticleCount)
			{
				m_mainModule.maxParticles = c_maxParticleCount;
			}

			if(m_particleSystem.TryGetComponent<ParticleSystemRenderer>(out var renderer))
			{
				renderer.enabled = false;
			}

			if(!m_particleMaterial)
			{
				m_particleMaterial = ShaderManager.In.GetMaterial("UI/Unlit/Transparent");
			}

			m_mainModule.scalingMode = ParticleSystemScalingMode.Hierarchy;

			if(m_textureSheetAnimation.enabled)
			{
				var tilesX = m_textureSheetAnimation.numTilesX;
				var tilesY = m_textureSheetAnimation.numTilesY;

				m_textureSheetAnimationFrames = tilesX*tilesY;
				m_textureSheetAnimationFrameSize = new Vector2(1.0f/tilesX,1.0f/tilesY);
			}

			for(var i=0;i<m_quadVertexArray.Length;i++)
			{
				m_quadVertexArray[i] = UIVertex.simpleVert;
			}

			m_initialize = true;
		}

		protected override void _PopulateMesh(VertexHelper vertexHelper)
		{
			_EnsureInitialized();

			if(!gameObject.activeInHierarchy)
			{
				return;
			}

			var count = m_particleSystem.GetParticles(ParticleArray);
			var defaultUV = new Vector4(0,0,1,1);

			for(var i=0;i<count;i++)
			{
				var particle = ParticleArray[i];

				var position = m_mainModule.simulationSpace == ParticleSystemSimulationSpace.Local ? particle.position : transform.InverseTransformPoint(particle.position);

				var color = particle.GetCurrentColor(m_particleSystem);
				var size = particle.GetCurrentSize(m_particleSystem)*0.5f;

				if(m_mainModule.scalingMode == ParticleSystemScalingMode.Shape)
				{
					position /= canvas.scaleFactor;
				}

				var uv = defaultUV;

				if(m_textureSheetAnimation.enabled)
				{
					var frameProgress = 1.0f - (particle.remainingLifetime/particle.startLifetime);

					if(m_textureSheetAnimation.frameOverTime.curveMin != null)
					{
						frameProgress = m_textureSheetAnimation.frameOverTime.curveMin.Evaluate(1 - (particle.remainingLifetime / particle.startLifetime));
					}
					else if(m_textureSheetAnimation.frameOverTime.curve != null)
					{
						frameProgress = m_textureSheetAnimation.frameOverTime.curve.Evaluate(1 - (particle.remainingLifetime / particle.startLifetime));
					}
					else if(m_textureSheetAnimation.frameOverTime.constant > 0)
					{
						frameProgress = m_textureSheetAnimation.frameOverTime.constant - (particle.remainingLifetime / particle.startLifetime);
					}

					frameProgress = Mathf.Repeat(frameProgress*m_textureSheetAnimation.cycleCount,1);

					var frame = 0;

					switch(m_textureSheetAnimation.animation)
					{
						case ParticleSystemAnimationType.WholeSheet:
							frame = Mathf.FloorToInt(frameProgress * m_textureSheetAnimationFrames);
							break;

						case ParticleSystemAnimationType.SingleRow:
							frame = Mathf.FloorToInt(frameProgress * m_textureSheetAnimation.numTilesX);

							int row = m_textureSheetAnimation.rowIndex;

							if(m_textureSheetAnimation.rowMode == ParticleSystemAnimationRowMode.Random)
							{ // FIXME - is this handled internally by rowIndex?
								row = Mathf.Abs((int)particle.randomSeed % m_textureSheetAnimation.numTilesY);
							}
							frame += row * m_textureSheetAnimation.numTilesX;
							break;

					}

					frame %= m_textureSheetAnimationFrames;

					uv.x = frame % m_textureSheetAnimation.numTilesX * m_textureSheetAnimationFrameSize.x;
					uv.y = 1.0f - ((frame / m_textureSheetAnimation.numTilesX) + 1) * m_textureSheetAnimationFrameSize.y;
					uv.z = uv.x+m_textureSheetAnimationFrameSize.x;
					uv.w = uv.y+m_textureSheetAnimationFrameSize.y;
				}

				for(var j=0;j<m_quadVertexArray.Length;j++)
				{
					m_quadVertexArray[j].color = color;
				}

				m_quadVertexArray[0].uv0.x = uv.x;
				m_quadVertexArray[0].uv0.y = uv.y;
				m_quadVertexArray[1].uv0.x = uv.x;
				m_quadVertexArray[1].uv0.y = uv.w;
				m_quadVertexArray[2].uv0.x = uv.z;
				m_quadVertexArray[2].uv0.y = uv.w;
				m_quadVertexArray[3].uv0.x = uv.z;
				m_quadVertexArray[3].uv0.y = uv.y;

				if(particle.rotation == 0.0f)
				{
					var xMin = position.x-size;
					var xMax = position.x+size;
					var yMin = position.y-size;
					var yMax = position.y+size;

					m_quadVertexArray[0].position.x = xMin;
					m_quadVertexArray[0].position.y = yMin;
					m_quadVertexArray[1].position.x = xMin;
					m_quadVertexArray[1].position.y = yMax;
					m_quadVertexArray[2].position.x = xMax;
					m_quadVertexArray[2].position.y = yMax;
					m_quadVertexArray[3].position.x = xMax;
					m_quadVertexArray[3].position.y = yMin;
				}
				else
				{
					var radian = -particle.rotation*Mathf.Deg2Rad;
					var cos = Mathf.Cos(radian)*size;
					var sin = Mathf.Sin(radian)*size;

					m_quadVertexArray[0].position.x = position.x - cos + sin;
					m_quadVertexArray[0].position.y = position.y - sin - cos;
					m_quadVertexArray[1].position.x = position.x - cos - sin;
					m_quadVertexArray[1].position.y = position.y - sin + cos;
					m_quadVertexArray[2].position.x = position.x + cos - sin;
					m_quadVertexArray[2].position.y = position.y + sin + cos;
					m_quadVertexArray[3].position.x = position.x + cos + sin;
					m_quadVertexArray[3].position.y = position.y + sin - cos;
				}

				vertexHelper.AddUIVertexQuad(m_quadVertexArray);
			}
		}

		private void Update()
		{
			if(!m_useLateUpdate && Application.isPlaying)
			{
				m_particleSystem.Simulate(Time.unscaledDeltaTime,false,false,true);

				SetAllDirty();

				if ((currentMaterial != null && currentTexture != currentMaterial.mainTexture) || (material != null && currentMaterial != null && material.shader != currentMaterial.shader))
				{
					m_particleSystem = null;

					Initialize();
				}
			}
		}

		private void LateUpdate()
		{
			if (!Application.isPlaying)
			{
				SetAllDirty();
			}
			else
			{
				if(m_useLateUpdate)
				{
					m_particleSystem.Simulate(Time.unscaledDeltaTime,false,false,true);

					SetAllDirty();

					if((currentMaterial != null && currentTexture != currentMaterial.mainTexture) ||
						(material != null && currentMaterial != null && material.shader != currentMaterial.shader))
					{
						m_particleSystem = null;
						Initialize();
					}
				}
			}

			if(material == currentMaterial) { return; }

			m_particleSystem = null;

			Initialize();
		}

		protected override void OnDestroy()
		{
			m_particleMaterial.DestroyObject();
			m_particleMaterial = null;

			base.OnDestroy();
		}

		protected override int _GetTotalExpectedVertices()
		{
			throw new System.NotImplementedException();
		}
		
		protected override void Reset()
		{
			base.Reset();

			if(!m_particleSystem)
			{
				m_particleSystem = GetComponent<ParticleSystem>();

				if(m_particleSystem)
				{
					m_mainModule = m_particleSystem.main;
					m_textureSheetAnimation = m_particleSystem.textureSheetAnimation;
				}
			}
		}
	}
}