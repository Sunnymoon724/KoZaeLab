using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace KZLib
{
	public partial class CameraMgr : LoadSingletonMB<CameraMgr>
	{
		/// <summary>
		/// 오버레이 카메라들
		/// </summary>
		private readonly List<Camera> m_SubCameraList = new();

		public bool AddSubCamera(Camera _subCamera)
		{
			var subCameraData = _subCamera.GetUniversalAdditionalCameraData();

			if(m_SubCameraList.Contains(_subCamera) || subCameraData.renderType != CameraRenderType.Overlay)
			{
				return false;
			}

			var cameraData = MainCamera.GetUniversalAdditionalCameraData();

			cameraData.cameraStack.Add(_subCamera);

			m_SubCameraList.Add(_subCamera);

			return true;
		}

		public bool RemoveSubCamera(Camera _subCamera)
		{
			if(!m_SubCameraList.Contains(_subCamera))
			{
				return false;
			}

			var cameraData = MainCamera.GetUniversalAdditionalCameraData();

			cameraData.cameraStack.Remove(_subCamera);

			m_SubCameraList.Remove(_subCamera);

			return true;
		}
	}
}