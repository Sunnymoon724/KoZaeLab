using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib
{
	public partial class CameraMgr : LoadSingletonMB<CameraMgr>
	{
		[SerializeField,LabelText("메인 카메라")]
		private Camera m_MainCamera = null;

		private Camera m_OverrideCamera = null;
		public Camera MainCamera => !m_OverrideCamera ? m_MainCamera : m_OverrideCamera;

		private bool m_PlayingCutScene;
		public bool IsPlayingCutScene => m_PlayingCutScene;

		public void SetPlayingCutScene(bool _isPlay)
		{
			m_PlayingCutScene = _isPlay;
		}

		public bool IsActiveMainCamera()
		{
			return MainCamera.gameObject.activeSelf;
		}

		protected override void Initialize()
		{
			if(!m_MainCamera)
			{
				throw new NullReferenceException("메인 카메라가 없습니다.");
			}
		}

		protected override void Release()
		{
			
		}

		public void SetCamera(Camera _overrideCamera)
		{
			m_OverrideCamera = _overrideCamera;

			m_OverrideCamera.gameObject.SetActiveSelf(_overrideCamera);
			m_MainCamera.gameObject.SetActiveSelf(!_overrideCamera);
		}

		public void SetOrthographicCamera(float _nearClipPlane,float _farClipPlane,float _size)
		{
			MainCamera.orthographic = true;

			MainCamera.nearClipPlane = _nearClipPlane;
			MainCamera.farClipPlane = _farClipPlane;

			MainCamera.orthographicSize = _size;
		}

		public void SetPerspectiveCamera(float _nearClipPlane,float _farClipPlane,float _fieldOfView)
		{
			MainCamera.orthographic = false;

			MainCamera.nearClipPlane = _nearClipPlane;
			MainCamera.farClipPlane = _farClipPlane;
			MainCamera.fieldOfView = _fieldOfView;
		}

		private void SetCameraBackgroundColor(Color _color)
		{
			MainCamera.backgroundColor = _color;
		}

		public void SetEnableCamera(bool _enabled)
		{			
			MainCamera.enabled = _enabled;
		}

		public void SetUpCamera(float _width = 16.0f,float _height = 9.0f)
		{
			MainCamera.aspect = _width/_height;

			var ratio = new Vector2(Screen.width/_width, Screen.height/_height);
			var added = new Vector2(((ratio.x/(ratio.y/100))-100)/200,((ratio.y/(ratio.x/100))-100)/200);

			if(ratio.y > ratio.x)
			{
				added.x = 0.0f;
			}
			else
			{
				added.y = 0.0f;
			}

			var rect = MainCamera.rect;

			MainCamera.rect = new Rect(rect.x+Mathf.Abs(added.x),rect.y+Mathf.Abs(added.y),rect.width+(added.x*2.0f),rect.height+(added.y*2.0f));
		}
	}
}