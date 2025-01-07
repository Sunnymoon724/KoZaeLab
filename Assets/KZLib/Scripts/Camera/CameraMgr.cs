using System.Collections.Generic;
using MetaData;
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZUtility;
using System;

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

		private float m_farFactor = 1.0f;

		protected override void Initialize()
		{
			base.Initialize();

			if(!m_mainCamera)
			{
				throw new NullReferenceException("Main camera is missing.");
			}

			var camera = m_mainCamera.GetComponent<Camera>();

			camera.allowDynamicResolution = true;

			SetCameraBackgroundColor(Color.black);

			EventMgr.In.EnableListener(EventTag.ChangeGraphicOption,OnChangeFarClipPlane);

			OnChangeFarClipPlane();
		}

		protected override void Release()
		{
			base.Release();

			EventMgr.In.DisableListener(EventTag.ChangeGraphicOption,OnChangeFarClipPlane);
		}

		public void SetCameraData(CameraData cameraData)
		{
			if(!cameraData.IsExist)
			{
				LogTag.System.E("Camera data is not exist.");

				return;
			}

			CurrentCamera.nearClipPlane = cameraData.NearClipPlane;
			CurrentCamera.farClipPlane = m_farFactor*cameraData.FarClipPlane;

			CurrentCamera.orthographic = cameraData.Orthographic;
			CurrentCamera.orthographicSize = cameraData.FieldOfView;
			CurrentCamera.fieldOfView = cameraData.FieldOfView;

			var position = m_target ? m_target.position : Vector3.zero;

			CurrentCamera.transform.SetPositionAndRotation(position +cameraData.Position,Quaternion.Euler(cameraData.Rotation));

			SetSubCameraDict();
		}

		public void SetCamera(Camera overrideCamera)
		{
			var onCamera = overrideCamera != null;

			m_overrideCamera = overrideCamera;

			if(m_overrideCamera != null)
			{
				m_overrideCamera.gameObject.SetActiveIfDifferent(onCamera);
			}

			m_mainCamera.gameObject.SetActiveIfDifferent(!onCamera);
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

		private void OnChangeFarClipPlane()
		{
			var option = GameDataMgr.In.Access<GameData.GraphicOption>();
			var flag = option.IsIncludeGraphicQuality(GraphicQualityType.CameraFarHalf);

			m_farFactor = flag ? 0.5f : 1.0f;
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

		private void SetSubCameraDict()
		{
			var main = CurrentCamera;

			foreach(var pair in m_subCameraDict)
			{
				if(!pair.Value)
				{
					continue;
				}

				var camera = pair.Key;

				camera.nearClipPlane = main.nearClipPlane;
				camera.farClipPlane = main.farClipPlane;

				camera.orthographic = main.orthographic;
				camera.orthographicSize = main.orthographicSize;
				camera.fieldOfView = main.fieldOfView;
			}
		}

		private void SetCameraBackgroundColor(Color color)
		{
			m_mainCamera.backgroundColor = color;
		}
	}
}