using System;
using System.Collections.Generic;
using MetaData;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib
{
	public class CameraMgr : LoadSingletonMB<CameraMgr>
	{
		[SerializeField,LabelText("Main Camera")]
		private Camera m_MainCamera = null;
		private Camera m_OverrideCamera = null;
		public Camera CurrentCamera => m_OverrideCamera == null ? m_MainCamera : m_OverrideCamera;

		[ShowInInspector,ReadOnly,LabelText("Camera Target")]
		private Transform m_Target = null;

		[SerializeField,LabelText("Lock X Rotate")]
		private bool m_LockRotateX = false;

		private readonly Dictionary<Camera,bool> m_SubCameraDict = new();

		private float m_FarFactor = 1.0f;

		protected override void Initialize()
		{
			base.Initialize();

			if(!m_MainCamera)
			{
				throw new NullReferenceException("Main camera is missing.");
			}

			var camera = m_MainCamera.GetComponent<Camera>();

			camera.allowDynamicResolution = true;

			SetCameraBackgroundColor(Color.black);

			Broadcaster.EnableListener(EventTag.ChangeGraphicOption,OnChangeFarClipPlane);

			OnChangeFarClipPlane();
		}

		protected override void Release()
		{
			base.Release();

			Broadcaster.DisableListener(EventTag.ChangeGraphicOption,OnChangeFarClipPlane);
		}

		public void SetCameraData(CameraData _cameraData)
		{
			if(!_cameraData.IsExist)
			{
				throw new NullReferenceException("Camera data is missing.");
			}

			CurrentCamera.nearClipPlane = _cameraData.NearClipPlane;
			CurrentCamera.farClipPlane = m_FarFactor*_cameraData.FarClipPlane;

			CurrentCamera.orthographic = _cameraData.Orthographic;
			CurrentCamera.orthographicSize = _cameraData.FieldOfView;
			CurrentCamera.fieldOfView = _cameraData.FieldOfView;

			var position = m_Target ? m_Target.position : Vector3.zero;

			CurrentCamera.transform.SetPositionAndRotation(position +_cameraData.Position,Quaternion.Euler(_cameraData.Rotation));

			SetSubCameraDict();
		}

		public void SetCamera(Camera _overrideCamera)
		{
			var onCamera = _overrideCamera != null;

			m_OverrideCamera = _overrideCamera;

			if(m_OverrideCamera != null)
			{
				m_OverrideCamera.gameObject.SetActiveSelf(onCamera);
			}

			m_MainCamera.gameObject.SetActiveSelf(!onCamera);
		}

		public void SetEnableCamera(bool _enable)
		{
			m_MainCamera.enabled = _enable;
		}

		public void SetTarget(Transform _target)
		{
			m_Target = _target;
		}

		public void LookTarget(Transform _target,float _duration = 0.0f)
		{
			SetTarget(_target);

			if(_target)
			{
				return;
			}

			var pivot = (_target.position-transform.position).normalized;
			var rotation = Quaternion.LookRotation(pivot).eulerAngles;

			if(m_LockRotateX)
			{
				rotation.x = transform.rotation.eulerAngles.x;
			}

			rotation.z = transform.rotation.eulerAngles.z;

			transform.rotation = Quaternion.Euler(rotation);

			// TODO _duration 부분 수정하기
		}

		private void OnChangeFarClipPlane()
		{
			var option = GameDataMgr.In.Access<GameData.GraphicOption>();
			var flag = option.IsIncludeGraphicQualityOption(GraphicQualityTag.CameraFarHalf);

			m_FarFactor = flag ? 0.5f : 1.0f;
		}

		public void AddSubCamera(Camera _camera,bool _dependency = true)
		{
			if(!m_SubCameraDict.ContainsKey(_camera))
			{
				m_SubCameraDict.Add(_camera,_dependency);
			}

			_camera.depth = m_MainCamera.depth+1;
			_camera.clearFlags = CameraClearFlags.Nothing;
		}

		public void RemoveSubCamera(Camera _camera)
		{
			if(m_SubCameraDict.ContainsKey(_camera))
			{
				return;
			}

			m_SubCameraDict.Remove(_camera);

			_camera.depth = -1;
			_camera.clearFlags = CameraClearFlags.Color;
		}

		private void SetSubCameraDict()
		{
			var main = CurrentCamera;

			foreach(var pair in m_SubCameraDict)
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

		private void SetCameraBackgroundColor(Color _color)
		{
			m_MainCamera.backgroundColor = _color;
		}
	}
}