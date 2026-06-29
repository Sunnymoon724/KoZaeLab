using System.Collections.Generic;
using UnityEngine;
using KZLib.Utilities;
using UnityEngine.Rendering.Universal;

namespace KZLib
{
	/// <summary>
	/// Registers the active main camera, manages URP overlay sub-cameras on its <c>cameraStack</c>,
	/// and applies <see cref="GraphicManager"/> far-clip scaling from graphic quality settings.
	/// Call <see cref="AttachCamera"/> / <see cref="DetachCamera"/> from game code when the scene main camera changes.
	/// </summary>
	public class CameraManager : Singleton<CameraManager>
	{
		/// <summary>Original sub-camera state captured at registration; restored on remove / detach / clear.</summary>
		private record CameraStoreInfo(float Depth,CameraClearFlags ClearFlag,int StackOrder,CameraRenderType RenderType);

		private Camera m_mainCamera = null;
		public Camera CurrentCamera => m_mainCamera;

		private readonly Dictionary<Camera,CameraStoreInfo> m_subCameraDict = new();
		/// <summary>Monotonic registration index; drives stack render order and sub-camera depth offset.</summary>
		private int m_nextStackOrder = 0;

		private bool m_hasOriginalFarClipPlane = false;
		private float m_originalFarClipPlane = 0.0f;

		private bool m_hasOriginalMainRenderType = false;
		private CameraRenderType m_originalMainRenderType = CameraRenderType.Base;

		private CameraManager() { }

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				ClearAllSubCameras();
			}

			base._Release(disposing);
		}

		/// <summary>
		/// Binds <paramref name="newCamera"/> as the main camera, restores the previous main if any,
		/// and applies all registered sub-cameras to the new URP stack.
		/// </summary>
		public void AttachCamera(Camera newCamera)
		{
			if(newCamera == null)
			{
				return;
			}

			if(m_mainCamera == newCamera)
			{
				RefreshSubCameraDepth();
				return;
			}

			_RestoreFarClipPlane();
			_RestoreMainRenderType();

			if(m_mainCamera != null)
			{
				_RemoveSubCamerasFromStack(m_mainCamera);
			}

			m_mainCamera = newCamera;

			_CaptureFarClipPlane(newCamera);
			_CaptureMainRenderType(newCamera);

			if(GraphicManager.HasInstance && GraphicManager.In.TryFindGraphicQualityOptionValue<float>(Global.DisableCameraFarHalf,out var factor))
			{
				ApplyFarClipScale(factor);
			}

			_ApplySubCamerasToMain();
		}

		/// <summary>
		/// Detaches the main camera, removes sub-cameras from the URP stack, and restores their stored state.
		/// Sub-camera entries remain in the registry so a later <see cref="AttachCamera"/> can re-apply them.
		/// </summary>
		public void DetachCamera()
		{
			_RestoreFarClipPlane();
			_RestoreMainRenderType();

			if(m_mainCamera != null)
			{
				_RemoveSubCamerasFromStack(m_mainCamera);
				_RestoreSubCamerasToStoredState();
			}

			m_mainCamera = null;

			_ResetFarClipTracking();
			_ResetMainRenderTypeTracking();
		}

		private void _CaptureFarClipPlane(Camera camera)
		{
			m_originalFarClipPlane = camera.farClipPlane;
			m_hasOriginalFarClipPlane = true;
		}

		private void _ResetFarClipTracking()
		{
			m_hasOriginalFarClipPlane = false;
			m_originalFarClipPlane = 0.0f;
		}

		private void _RestoreFarClipPlane()
		{
			if(m_mainCamera != null && m_hasOriginalFarClipPlane)
			{
				m_mainCamera.farClipPlane = m_originalFarClipPlane;
			}
		}

		private void _CaptureMainRenderType(Camera camera)
		{
			m_originalMainRenderType = _GetCameraRenderType(camera);
			m_hasOriginalMainRenderType = true;
		}

		private void _ResetMainRenderTypeTracking()
		{
			m_hasOriginalMainRenderType = false;
			m_originalMainRenderType = CameraRenderType.Base;
		}

		private void _RestoreMainRenderType()
		{
			if(m_mainCamera != null && m_hasOriginalMainRenderType)
			{
				_SetCameraRenderType(m_mainCamera,m_originalMainRenderType);
			}
		}

		/// <summary>Scales main far clip from the baseline captured at <see cref="AttachCamera"/>.</summary>
		public void ApplyFarClipScale(float factor)
		{
			if(m_mainCamera != null && m_hasOriginalFarClipPlane)
			{
				m_mainCamera.farClipPlane = m_originalFarClipPlane*factor;
			}
		}

		/// <summary>
		/// Registers an overlay sub-camera. When no main camera is attached yet, only the dict entry is stored (pending).
		/// </summary>
		public void AddSubCamera(Camera subCamera)
		{
			if(subCamera == null || subCamera == m_mainCamera || m_subCameraDict.ContainsKey(subCamera))
			{
				return;
			}

			var stackOrder = m_nextStackOrder++;

			m_subCameraDict.Add(subCamera,new CameraStoreInfo(subCamera.depth,subCamera.clearFlags,stackOrder,_GetCameraRenderType(subCamera)));

			if(m_mainCamera != null && _TryGetCameraStack(m_mainCamera,out var cameraStack))
			{
				_PrepareSubCameraForStack(subCamera,stackOrder);
				_InsertSubCameraIntoStack(subCamera,stackOrder,cameraStack);
			}
		}

		/// <summary>Unregisters a sub-camera and restores depth, clear flags, and URP render type.</summary>
		public void RemoveSubCamera(Camera subCamera)
		{
			if(subCamera == null || !m_subCameraDict.ContainsKey(subCamera))
			{
				return;
			}

			m_subCameraDict.TryRemove(subCamera,out var cameraInfo);

			_RestoreSubCameraState(subCamera,cameraInfo);

			if(m_mainCamera != null)
			{
				_RemoveSubCameraFromStack(m_mainCamera,subCamera);
			}
		}

		/// <summary>Recomputes sub-camera depth after the main camera depth changes at runtime.</summary>
		public void RefreshSubCameraDepth()
		{
			if(m_mainCamera == null)
			{
				return;
			}

			foreach(var pair in m_subCameraDict)
			{
				_SetSubCameraDepth(pair.Key,pair.Value.StackOrder);
			}
		}

		/// <summary>Removes every sub-camera from the stack, restores original state, and clears the registry.</summary>
		public void ClearAllSubCameras()
		{
			if(m_mainCamera != null && _TryGetCameraStack(m_mainCamera,out var cameraStack))
			{
				foreach(var pair in m_subCameraDict)
				{
					cameraStack.RemoveSafe(pair.Key);
				}
			}

			_RestoreSubCamerasToStoredState();

			m_subCameraDict.Clear();
			m_nextStackOrder = 0;
		}

		/// <summary>Prepares main/sub cameras and rebuilds the full URP stack (used on <see cref="AttachCamera"/>).</summary>
		private void _ApplySubCamerasToMain()
		{
			if(m_mainCamera == null)
			{
				return;
			}

			if(!_TryGetCameraStack(m_mainCamera,out var cameraStack))
			{
				return;
			}

			_PrepareMainCameraForStack(m_mainCamera);

			foreach(var pair in m_subCameraDict)
			{
				_PrepareSubCameraForStack(pair.Key,pair.Value.StackOrder);
			}

			_RebuildCameraStack(cameraStack);
		}

		private void _PrepareSubCameraForStack(Camera subCamera,int stackOrder)
		{
			if(m_mainCamera == null || subCamera == null)
			{
				return;
			}

			_SetSubCameraDepth(subCamera,stackOrder);
			subCamera.clearFlags = CameraClearFlags.Nothing;
			_SetCameraRenderType(subCamera,CameraRenderType.Overlay);
		}

		/// <summary>Removes managed sub-cameras from the stack and re-adds them sorted by <see cref="CameraStoreInfo.StackOrder"/>.</summary>
		private void _RebuildCameraStack(IList<Camera> cameraStack)
		{
			foreach(var pair in m_subCameraDict)
			{
				cameraStack.RemoveSafe(pair.Key);
			}

			var sortedSubCameraList = new List<KeyValuePair<Camera,CameraStoreInfo>>(m_subCameraDict);

			sortedSubCameraList.Sort((left,right) => left.Value.StackOrder.CompareTo(right.Value.StackOrder));

			for(var i=0;i<sortedSubCameraList.Count;i++)
			{
				cameraStack.AddIfAbsent(sortedSubCameraList[i].Key);
			}
		}

		/// <summary>Inserts one sub-camera at the stack index that preserves <paramref name="stackOrder"/> among managed cameras.</summary>
		private void _InsertSubCameraIntoStack(Camera subCamera,int stackOrder,IList<Camera> cameraStack)
		{
			cameraStack.RemoveSafe(subCamera);

			var insertIndex = cameraStack.Count;

			for(var i=0;i<cameraStack.Count;i++)
			{
				if(!m_subCameraDict.TryGetValue(cameraStack[i],out var info))
				{
					continue;
				}

				if(stackOrder < info.StackOrder)
				{
					insertIndex = i;
					break;
				}
			}

			if(cameraStack is List<Camera> cameraList)
			{
				cameraList.Insert(insertIndex,subCamera);
				return;
			}

			_RebuildCameraStack(cameraStack);
		}

		private void _PrepareMainCameraForStack(Camera mainCamera)
		{
			_SetCameraRenderType(mainCamera,CameraRenderType.Base);
		}

		private void _RestoreSubCamerasToStoredState()
		{
			foreach(var pair in m_subCameraDict)
			{
				_RestoreSubCameraState(pair.Key,pair.Value);
			}
		}

		private static void _RestoreSubCameraState(Camera subCamera,CameraStoreInfo cameraInfo)
		{
			subCamera.depth = cameraInfo.Depth;
			subCamera.clearFlags = cameraInfo.ClearFlag;
			_SetCameraRenderType(subCamera,cameraInfo.RenderType);
		}

		private static CameraRenderType _GetCameraRenderType(Camera camera)
		{
			if(!camera)
			{
				return CameraRenderType.Base;
			}

			return camera.GetUniversalAdditionalCameraData().renderType;
		}

		private static void _SetCameraRenderType(Camera camera,CameraRenderType renderType)
		{
			if(!camera)
			{
				return;
			}

			camera.GetUniversalAdditionalCameraData().renderType = renderType;
		}

		private void _SetSubCameraDepth(Camera subCamera,int stackOrder)
		{
			if(m_mainCamera == null || subCamera == null)
			{
				return;
			}

			subCamera.depth = m_mainCamera.depth+1+stackOrder;
		}

		private void _RemoveSubCamerasFromStack(Camera mainCamera)
		{
			if(!_TryGetCameraStack(mainCamera,out var cameraStack))
			{
				return;
			}

			foreach(var pair in m_subCameraDict)
			{
				cameraStack.RemoveSafe(pair.Key);
			}
		}

		private void _RemoveSubCameraFromStack(Camera mainCamera,Camera subCamera)
		{
			if(_TryGetCameraStack(mainCamera,out var cameraStack))
			{
				cameraStack.RemoveSafe(subCamera);
			}
		}

		private bool _TryGetCameraStack(Camera camera,out IList<Camera> cameraStack)
		{
			cameraStack = null;

			if(!camera)
			{
				return false;
			}

			cameraStack = camera.GetUniversalAdditionalCameraData().cameraStack;

			return cameraStack != null;
		}
	}
}
