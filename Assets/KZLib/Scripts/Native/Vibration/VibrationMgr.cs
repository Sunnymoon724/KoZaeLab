#if UNITY_IOS && !UNITY_EDITOR

using System.Runtime.InteropServices;

#elif UNITY_ANDROID && !UNITY_EDITOR

using System;
using UnityEngine;

#endif

using UnityEngine;
using KZLib.KZUtility;

namespace KZLib
{
	public class VibrationMgr : Singleton<VibrationMgr>
	{
		private bool m_disposed = false;

		private bool m_useVibration = true;

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
		private AndroidJavaObject m_vibration = null;
		private AndroidJavaObject m_activity = null;
#endif

		protected override void Initialize()
		{
			var optionConfig = ConfigMgr.In.Access<ConfigData.OptionConfig>();

			optionConfig.OnUseVibrationChange += OnChangeUseVibration;

			OnChangeUseVibration(optionConfig.UseVibration);

#if UNITY_IOS && !UNITY_EDITOR
			VibrationInitialize();
#elif UNITY_ANDROID && !UNITY_EDITOR
			m_vibration = new AndroidJavaObject("com.shf.vibrator.Vibration");

			using var pluginClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			m_activity = pluginClass.GetStatic<AndroidJavaObject>("currentActivity");
#endif
		}

		protected override void Release(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				var optionConfig = ConfigMgr.In.Access<ConfigData.OptionConfig>();

				optionConfig.OnUseVibrationChange -= OnChangeUseVibration;
			}

			m_disposed = true;

			base.Release(disposing);
		}

		private void OnChangeUseVibration(bool useVibration)
		{
			m_useVibration = useVibration;
		}

		/// <param name="_amplitude">0.0 ~ 10.0</param>
		public void Play(float amplitude,float millisecond)
		{
			if(amplitude <= 0.0f || millisecond <= 0.0f)
			{
				return;
			}

			var newAmplitude = Mathf.Clamp(amplitude,0.0f,10.0f);

			if(!m_useVibration)
			{
				return;
			}

#if UNITY_IOS && !UNITY_EDITOR
			PlayVibration(millisecond,newAmplitude/10.0f,1.0f,true);
#elif UNITY_ANDROID && !UNITY_EDITOR
			m_vibration.Call("AmplitudeVibrate",m_activity,Convert.ToInt64(millisecond*1000L),Mathf.RoundToInt(newAmplitude*25.5f));
#endif
		}
	}
}