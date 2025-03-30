using System.Collections.Generic;
using UnityEngine;
using KZLib.KZUtility;
using System;
using KZLib.KZDevelop;

namespace KZLib
{
	public class CameraMgr : LoadSingletonMB<CameraMgr>
	{
		[SerializeField]
		private Camera m_mainCamera = null;
		private Camera m_overrideCamera = null;
		public Camera CurrentCamera => m_overrideCamera == null ? m_mainCamera : m_overrideCamera;

		private readonly Dictionary<Camera,bool> m_subCameraDict = new();

		private WeakReference<ConfigData.OptionConfig> m_optionRef = null;

		private float m_farFactor = 1.0f;

		protected override void Initialize()
		{
			base.Initialize();

			var optionCfg = ConfigMgr.In.Access<ConfigData.OptionConfig>();

			optionCfg.OnGraphicQualityChange += OnChangeFarClipPlane;

			m_optionRef = new WeakReference<ConfigData.OptionConfig>(optionCfg);

			OnChangeFarClipPlane(optionCfg.GraphicQuality);
		}

		protected override void Release()
		{
			base.Release();

			if(m_optionRef.TryGetTarget(out var optionCfg))
			{
				optionCfg.OnGraphicQualityChange -= OnChangeFarClipPlane;
			}

			m_optionRef = null;
		}

		public void SetCamera(Camera newCamera)
		{
			m_mainCamera = newCamera;

			m_mainCamera.farClipPlane *= m_farFactor;
		}

		public void SetOverrideCamera(Camera overrideCamera)
		{
			var onCamera = overrideCamera != null;

			m_overrideCamera = overrideCamera;

			if(m_overrideCamera != null)
			{
				m_overrideCamera.gameObject.EnsureActive(onCamera);
			}

			m_mainCamera.gameObject.EnsureActive(!onCamera);
		}

		public void SetEnableCamera(bool enable)
		{
			m_mainCamera.enabled = enable;
		}

		private void OnChangeFarClipPlane(long graphicQuality)
		{
			m_farFactor = float.Parse(GraphicQualityOption.In.FindValue(graphicQuality,Global.DISABLE_CAMERA_FAR_HALF));
		}

		public void AddSubCamera(Camera camera,bool dependency = true)
		{
			if(!m_subCameraDict.ContainsKey(camera))
			{
				m_subCameraDict.Add(camera,dependency);
			}

			camera.depth = m_mainCamera.depth+1;
			camera.clearFlags = CameraClearFlags.Nothing;
		}

		public void RemoveSubCamera(Camera camera)
		{
			if(m_subCameraDict.ContainsKey(camera))
			{
				return;
			}

			m_subCameraDict.Remove(camera);

			camera.depth = -1;
			camera.clearFlags = CameraClearFlags.Color;
		}
	}
}