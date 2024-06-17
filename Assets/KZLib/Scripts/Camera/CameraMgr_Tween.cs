// using DG.Tweening;
// using UnityEngine;

// namespace KZLib
// {
// 	public partial class CameraMgr : LoadSingletonMB<CameraMgr>
// 	{
// 		private Tween m_ShakeTween = null;
// 		private Tween m_ZoomTween = null;

// 		private void ReleaseTween()
// 		{
// 			CommonUtility.KillTween(m_ShakeTween,false);
// 			CommonUtility.KillTween(m_ZoomTween,false);
// 		}

// 		public void RestShake()
// 		{
// 			CommonUtility.KillTween(m_ShakeTween,false);

// 			MainCamera.transform.localRotation = Quaternion.identity;
// 		}

// 		public void ShakeRotate(float _duration,float _sensitivity,int _interval)
// 		{
// 			CommonUtility.KillTween(m_ShakeTween,false);

// 			var duration = _duration/_interval;

// 			m_ShakeTween = MainCamera.transform.DOShakeRotation(duration,_sensitivity).SetLoops(_interval);
// 		}

// 		public void SetZoom(float _endValue,float _duration,bool _return)
// 		{
// 			CommonUtility.KillTween(m_ZoomTween,false);

// 			var fov = MainCamera.fieldOfView;

// 			if(fov.Approximately(_endValue))
// 			{
// 				return;
// 			}

// 			m_ZoomTween = MainCamera.DOFieldOfView(_endValue,_duration);

// 			if(_return)
// 			{
// 				m_ZoomTween.OnComplete(()=>
// 				{
// 					MainCamera.fieldOfView = fov;
// 				});
// 			}
// 		}
// 	}
// }