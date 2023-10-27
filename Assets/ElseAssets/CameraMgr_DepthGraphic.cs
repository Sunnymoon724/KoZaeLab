// using Kino;
// using Sirenix.OdinInspector;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace KZLib
// {
// 	public partial class CameraMgr : SingletonMB<CameraMgr>
//     {
// 		public Bokeh DepthOfField => m_DepthOfField;

// 		private Camera m_DeferredCamera = null;
// 		private RenderTexture m_RenderTargetsTexture = null;
// 		private RenderTexture m_RenderTargetsDepth = null;
// 		private bool m_DeferredInitialized = false;	

// 		public Camera DeferredCamera => m_DeferredCamera;

// 		private bool m_UseDepthGraphicOption = false;
// 		private bool m_UseDepthOfField = false;

// 		private bool m_UseDepthWater = false;

// 		private List<int> m_WaterList = new List<int>();
		

// 		private Bokeh m_DepthOfField = null;

// 		private int m_DeferredCullingMask = 0;

// 		public bool UseDepthGraphicOption => m_UseDepthGraphicOption;

		
// 		private void CreateDeferredCamera()
// 		{
// 			var deferred = new GameObject("DeferredCamera");
// 			deferred.transform.parent = transform;
// 			deferred.transform.localPosition = Vector3.zero;
// 			deferred.transform.localRotation = Quaternion.identity;
// 			deferred.hideFlags = HideFlags.DontSave;

// 			m_DeferredCamera = deferred.AddComponent<Camera>();
// 			m_DeferredCamera.allowDynamicResolution = true;
// 			m_DeferredCamera.enabled = false;
// 			m_DeferredCamera.allowMSAA = false;

// 			m_RenderTargetsTexture = new RenderTexture(Screen.width/2,Screen.height/2,0,RenderTextureFormat.Default);
// 			m_RenderTargetsDepth = new RenderTexture(Screen.width/2,Screen.height/2,24,RenderTextureFormat.Depth);

// 			m_DeferredCamera.SetTargetBuffers(m_RenderTargetsTexture.colorBuffer,m_RenderTargetsDepth.depthBuffer);
// 			// m_DeferredCullingMask = LayerMask.GetMask("Background_Terrain", "player");
// 			m_DeferredCamera.cullingMask = m_DeferredCullingMask;

// 			m_DeferredInitialized = true;
// 		}

// 		private void ReleaseDeferredCamera()
// 		{
// 			if(m_DeferredCamera)
// 			{
// 				Tools.DestroyObject(m_DeferredCamera.gameObject);

// 				m_DeferredCamera = null;
// 			}

// 			if(m_RenderTargetsTexture)
// 			{
// 				m_RenderTargetsTexture.Release();

// 				DestroyImmediate(m_RenderTargetsTexture);

// 				m_RenderTargetsTexture = null;
// 			}

// 			if(m_RenderTargetsDepth)
// 			{
// 				m_RenderTargetsDepth.Release();

// 				DestroyImmediate(m_RenderTargetsDepth);

// 				m_RenderTargetsDepth = null;
// 			}
// 		}

// 		private void DeferredPreCull(Camera _camera)
// 		{
// 			if(_camera == null || !m_DeferredInitialized || _camera.depthTextureMode != DepthTextureMode.None)
// 			{
// 				return;
// 			}
			
// 			if(m_UseDepthOfField || m_UseDepthWater)
// 			{
// 				m_DeferredCamera.CopyFrom(_camera);
// 				m_DeferredCamera.cullingMask = m_DeferredCullingMask;
// 				m_DeferredCamera.SetTargetBuffers(m_RenderTargetsTexture.colorBuffer,m_RenderTargetsDepth.depthBuffer);
// 				m_DeferredCamera.Render();
				
// 				Shader.SetGlobalTexture("_CameraDepthTexture",m_RenderTargetsDepth);
// 			}
// 		}
		
		
// 		public void SetDepthOfField(bool _enabled,Bokeh _dof)
// 		{
// 			if(!m_UseDepthGraphicOption || !_enabled)
// 			{
// 				if(_dof != null)
// 				{
// 					_dof.Release();
// 				}

// 				m_UseDepthOfField = false;

// 				return;
// 			}
			
// 			if(_dof != null)
// 			{
// 				_dof.Initialize();
// 			}

// 			m_UseDepthOfField = _enabled;
// 		}

// 		public void SetDepthGraphicOption(bool _enabled)
// 		{
// 			m_UseDepthGraphicOption = _enabled;

// 			if(!_enabled)
// 			{
// 				m_UseDepthOfField = false;
// 				m_UseDepthWater = false;
// 			}
// 		}

// 		public void SetDeferredCullingMask(int _mask)
// 		{
// 			m_DeferredCullingMask = _mask;
// 		}
		
// 		public void SetDepthOfField(Transform _target)
// 		{
// 			m_DepthOfField = MainCamera.GetComponent<Bokeh>();

// 			if(m_DepthOfField != null)
// 			{
// 				m_DepthOfField.pointOfFocus = _target;
// 			}
// 		}

// 		public void SetDepthWater(bool _enabled)
// 		{
// 			if(!m_UseDepthGraphicOption || !_enabled)
// 			{
// 				m_UseDepthWater = false;

// 				return;
// 			}

// 			m_UseDepthWater = _enabled;
// 		}

// 		public void AddWater(int _id)
// 		{
// 			m_WaterList.AddNotOverlap(_id);
// 		}
		
// 		public void RemoveWater(int _id)
// 		{
// 			var index = m_WaterList.FindIndex(x => x == _id);

// 			if(index > -1)
// 			{
// 				m_WaterList.RemoveAt(index);
// 			}

// 			if(m_WaterList.Count < 1)
// 			{
// 				SetDepthWater(false);
// 			}
// 		}
// 	}
// }