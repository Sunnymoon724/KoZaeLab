using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZUtility;
using System;
using KZLib.KZDevelop;
using KZLib.KZData;

namespace KZLib
{
	public class CameraMgr : LoadSingletonMB<CameraMgr>
	{
		[SerializeField,LabelText("Main Camera")]
		private Camera m_mainCamera = null;
		private Camera m_overrideCamera = null;
		public Camera CurrentCamera => m_overrideCamera == null ? m_mainCamera : m_overrideCamera;

		[ShowInInspector,ReadOnly,LabelText("Camera Target")]
		private Transform m_target = null;

		[SerializeField,LabelText("Lock X Rotate")]
		private bool m_lockRotateX = false;

		private readonly Dictionary<Camera,bool> m_subCameraDict = new();

		private WeakReference<ConfigData.OptionConfig> m_optionRef = null;

		private float m_farFactor = 1.0f;

		protected override void Initialize()
		{
			base.Initialize();

			if(!m_mainCamera)
			{
				LogTag.System.E("Main camera is missing.");

				return;
			}

			var camera = m_mainCamera.GetComponent<Camera>();

			camera.allowDynamicResolution = true;

			SetCameraBackgroundColor(Color.black);

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

		public void SetCameraProto(CameraProto cameraPrt)
		{
			if(cameraPrt == null)
			{
				LogTag.System.E("Camera proto is not exist.");

				return;
			}

			CurrentCamera.nearClipPlane = cameraPrt.NearClipPlane;
			CurrentCamera.farClipPlane = m_farFactor*cameraPrt.FarClipPlane;

			CurrentCamera.orthographic = cameraPrt.Orthographic;
			CurrentCamera.orthographicSize = cameraPrt.FieldOfView;
			CurrentCamera.fieldOfView = cameraPrt.FieldOfView;

			var position = m_target ? m_target.position : Vector3.zero;

			CurrentCamera.transform.SetPositionAndRotation(position+cameraPrt.Position,Quaternion.Euler(cameraPrt.Rotation));

			OnSyncSubCamera();
		}

		public void SetCamera(Camera overrideCamera)
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

		public void SetTarget(Transform target)
		{
			m_target = target;
		}

		public void LookTarget(Transform target,float duration = 0.0f)
		{
			SetTarget(target);

			if(target)
			{
				return;
			}

			var pivot = (target.position-transform.position).normalized;
			var rotation = Quaternion.LookRotation(pivot).eulerAngles;

			if(m_lockRotateX)
			{
				rotation.x = transform.rotation.eulerAngles.x;
			}

			rotation.z = transform.rotation.eulerAngles.z;

			transform.rotation = Quaternion.Euler(rotation);

			// TODO duration 부분 수정하기
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

		private void SetCameraBackgroundColor(Color color)
		{
			m_mainCamera.backgroundColor = color;
		}

		[Button("Sync Sub Cameras")]
		private void OnSyncSubCamera()
		{
			var main = CurrentCamera;

			foreach(var pair in m_subCameraDict)
			{
				if(!pair.Value)
				{
					continue;
				}

				var camera = pair.Key;

				camera.nearClipPlane	= main.nearClipPlane;
				camera.farClipPlane		= main.farClipPlane;

				camera.orthographic		= main.orthographic;
				camera.orthographicSize = main.orthographicSize;
				camera.fieldOfView		= main.fieldOfView;
			}
		}
	}
}