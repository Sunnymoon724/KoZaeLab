using System;
using UnityEngine;

namespace KZLib.Cam
{
	[ExecuteInEditMode]
	public class BlurCamera : MonoBehaviour
	{
		[SerializeField]
		private int m_Iteration = 7;

		[SerializeField]
		private float m_SpreadSize = 0.6f;

		[SerializeField]
		private Shader m_Shader = null;
		
		private static Material m_BlurMaterial = null;

		private Material BlurMaterial
		{
			get
			{
				if(m_BlurMaterial == null)
				{
					m_BlurMaterial = new Material(m_Shader);
					m_BlurMaterial.hideFlags = HideFlags.DontSave;
				}

				return m_BlurMaterial;
			}
		}

		private void Start()
		{
			if(m_Shader == null || m_BlurMaterial.shader.isSupported == false)
			{
				enabled = false;

				return;
			}
		}

		private void OnDisable()
		{
			if(m_BlurMaterial != null)
			{
				DestroyImmediate(m_BlurMaterial);
			}
		}

		void OnRenderImage(RenderTexture _source,RenderTexture _destination)
		{
			var width = _source.width/4;
			var height = _source.height/4;
			var buffer = RenderTexture.GetTemporary(width,height,0);

			SetDownSample(_source,buffer,1.0f);

			for(int i=0;i<m_Iteration;i++)
			{
				var buffer2 = RenderTexture.GetTemporary(width,height,0);

				SetDownSample(buffer,buffer2,0.5f+i*m_SpreadSize);

				RenderTexture.ReleaseTemporary(buffer);

				buffer = buffer2;
			}
			
			Graphics.Blit(buffer,_destination);

			RenderTexture.ReleaseTemporary(buffer);
		}

		private void SetDownSample(RenderTexture _source,RenderTexture _destination,float _offset)
		{
			Graphics.BlitMultiTap(_source,_destination,BlurMaterial,new Vector2(-_offset,-_offset),new Vector2(-_offset,+_offset),new Vector2(+_offset,+_offset),new Vector2(+_offset,-_offset));
		}
	}
}
