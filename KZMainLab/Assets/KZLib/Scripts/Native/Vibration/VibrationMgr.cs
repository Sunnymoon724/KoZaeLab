#if UNITY_IOS && !UNITY_EDITOR

using System.Runtime.InteropServices;

#elif UNITY_ANDROID && !UNITY_EDITOR

using System;
using UnityEngine;

#endif

using UnityEngine;

namespace KZLib
{
	public class VibrationMgr : Singleton<VibrationMgr>
	{
		private bool m_Disposed = false;

		private bool m_UseVibration = true;

#if UNITY_IOS && !UNITY_EDITOR
		[DllImport("__Internal")]
		static extern void VibrationInitialize();

		[DllImport("__Internal")]
		static extern void VibrationTerminate();

		[DllImport("__Internal")]
		static extern void PlayVibration(float duration,float intensity,float sharpness,bool sustained);

		[DllImport("__Internal")]
		static extern void StopVibration();
#elif UNITY_ANDROID && !UNITY_EDITOR
		private AndroidJavaObject m_Vibration = null;
		private AndroidJavaObject m_Activity = null;
#endif

		protected override void Initialize()
		{
			OnChangeNativeOption();

			Broadcaster.EnableListener(EventTag.ChangeNativeOption,OnChangeNativeOption);

#if UNITY_IOS && !UNITY_EDITOR
			VibrationInitialize();
#elif UNITY_ANDROID && !UNITY_EDITOR
			m_Vibration = new AndroidJavaObject("com.shf.vibrator.Vibration");

			using var pluginClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			m_Activity = pluginClass.GetStatic<AndroidJavaObject>("currentActivity");
#endif
		}

		protected override void Release(bool _disposing)
		{
			if(m_Disposed)
			{
				return;
			}

			if(_disposing)
			{
				Broadcaster.DisableListener(EventTag.ChangeNativeOption,OnChangeNativeOption);
			}

			m_Disposed = true;

			base.Release(_disposing);
		}

		private void OnChangeNativeOption()
		{
			var option = GameDataMgr.In.Access<GameData.NativeOption>();

			m_UseVibration = option.UseVibration;
		}

		/// <param name="_amplitude">0.0 ~ 10.0</param>
		public void Play(float _amplitude,float _millisecond)
		{
			if(_amplitude <= 0.0f || _millisecond <= 0.0f)
			{
				return;
			}

			var amplitude = Mathf.Clamp(_amplitude,0.0f,10.0f);

			if(!m_UseVibration)
			{
				return;
			}

#if UNITY_IOS && !UNITY_EDITOR
			PlayVibration(_millisecond,amplitude/10.0f,1.0f,true);
#elif UNITY_ANDROID && !UNITY_EDITOR
			m_Vibration.Call("AmplitudeVibrate",m_Activity,Convert.ToInt64(_millisecond*1000L),Mathf.RoundToInt(amplitude*25.5f));
#endif
		}
	}
}