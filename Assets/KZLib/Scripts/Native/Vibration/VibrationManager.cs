#if UNITY_IOS && !UNITY_EDITOR

using System.Runtime.InteropServices;

#elif UNITY_ANDROID && !UNITY_EDITOR

using System;
using UnityEngine;

#endif

using UnityEngine;
using KZLib.KZUtility;
using KZLib.KZData;
using R3;

namespace KZLib
{
	public class VibrationManager : Singleton<VibrationManager>
	{
		private readonly CompositeDisposable m_disposable = new();

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

		private VibrationManager() { }

		protected override void _Initialize()
		{
			base._Initialize();

			var optionCfg = ConfigManager.In.Access<OptionConfig>();

			optionCfg.OnChangedUseVibration.Subscribe(_OnChangeUseVibration).AddTo(m_disposable);

			_OnChangeUseVibration(optionCfg.UseVibration);

#if UNITY_IOS && !UNITY_EDITOR
			VibrationInitialize();
#elif UNITY_ANDROID && !UNITY_EDITOR
			m_vibration = new AndroidJavaObject("com.shf.vibrator.Vibration");

			using var pluginClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			m_activity = pluginClass.GetStatic<AndroidJavaObject>("currentActivity");
#endif
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_disposable.Dispose();
			}

			base._Release(disposing);
		}

		private void _OnChangeUseVibration(bool useVibration)
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