using System.Collections.Generic;
using UnityEngine;
using KZLib.KZUtility;
using KZLib.KZDevelop;
using KZLib.KZData;
using UnityEngine.Rendering.Universal;

namespace KZLib
{
	public class CameraManager : Singleton<CameraManager>
	{
		private record CameraStoreInfo(float Depth,CameraClearFlags ClearFlag);

		private Camera m_mainCamera = null;
		public Camera CurrentCamera => m_mainCamera;

		private readonly Dictionary<Camera,CameraStoreInfo> m_subCameraDict = new();

		private bool m_disposed = false;

		private float m_farFactor = 1.0f;

		protected override void Initialize()
		{
			base.Initialize();

			var optionCfg = ConfigManager.In.Access<OptionConfig>();

			optionCfg.OnGraphicQualityChanged += _OnChangeFarClipPlane;

			_OnChangeFarClipPlane(optionCfg.GraphicQuality);
		}

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				if(ConfigManager.HasInstance)
				{
					var optionCfg = ConfigManager.In.Access<OptionConfig>();

					optionCfg.OnGraphicQualityChanged -= _OnChangeFarClipPlane;
				}
			}

			m_disposed = true;

			base.Release(disposing);
		}

		public void AttachCamera(Camera newCamera)
		{
			if(newCamera == null)
			{
				return;
			}

			m_mainCamera = newCamera;
			m_mainCamera.farClipPlane *= m_farFactor;

			var mainCameraData = m_mainCamera.GetUniversalAdditionalCameraData();
			var cameraList = mainCameraData.cameraStack;

			foreach(var subCamera in m_subCameraDict.Keys)
			{
				if(cameraList.Contains(subCamera))
				{
					continue;
				}

				cameraList.Add(subCamera);
			}

			if(UIManager.HasInstance)
			{
				UIManager.In.Set3DCamera(newCamera);
			}
		}

		public void DetachCamera()
		{
			m_mainCamera = null;

			if(UIManager.HasInstance)
			{
				UIManager.In.Set3DCamera(null);
			}
		}

		private void _OnChangeFarClipPlane(long graphicQuality)
		{
			var value = GraphicQualityOption.In.FindValue(graphicQuality,Global.DISABLE_CAMERA_FAR_HALF);

			if(!float.TryParse(value,out var factor))
			{
				return;
			}

			m_farFactor = factor;

			if (m_mainCamera != null)
			{
				m_mainCamera.farClipPlane *= m_farFactor;
			}
		}

		public void AddSubCamera(Camera subCamera)
		{
			if(m_mainCamera == null || subCamera == null || m_subCameraDict.ContainsKey(subCamera))
			{
				return;
			}

			m_subCameraDict.Add(subCamera,new CameraStoreInfo(subCamera.depth,subCamera.clearFlags));

			subCamera.depth = m_mainCamera.depth+1;
			subCamera.clearFlags = CameraClearFlags.Nothing;

			var mainCameraData = m_mainCamera.GetUniversalAdditionalCameraData();

			mainCameraData.cameraStack.AddNotOverlap(subCamera);
		}

		public void RemoveSubCamera(Camera subCamera)
		{
			if(subCamera == null || m_mainCamera == null || !m_subCameraDict.ContainsKey(subCamera))
			{
				return;
			}

			m_subCameraDict.RemoveOut(subCamera,out var cameraInfo);

			subCamera.depth = cameraInfo.Depth;
			subCamera.clearFlags = cameraInfo.ClearFlag;

			var mainCameraData = m_mainCamera.GetUniversalAdditionalCameraData();

			mainCameraData.cameraStack.RemoveSafe(subCamera);
		}

		public void ClearAllSubCameras()
		{
			if(m_mainCamera == null)
			{
				return;
			}

			var mainCameraData = m_mainCamera.GetUniversalAdditionalCameraData();

			foreach(var pair in m_subCameraDict)
			{
				var cam = pair.Key;
				var info = pair.Value;

				cam.depth = info.Depth;
				cam.clearFlags = info.ClearFlag;

				mainCameraData.cameraStack.RemoveSafe(cam);
			}

			m_subCameraDict.Clear();
		}
	}
}