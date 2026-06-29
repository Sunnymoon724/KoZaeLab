#if UNITY_IOS && !UNITY_EDITOR

using System.Runtime.InteropServices;

#elif UNITY_ANDROID && !UNITY_EDITOR

using System;
using UnityEngine;

#endif

using UnityEngine;
using KZLib.Utilities;
using R3;

namespace KZLib.Natives
{
	public class VibrationManager : Singleton<VibrationManager>
	{
		private ReactivePrefs<bool> m_useVibration = null;
		public Observable<bool> OnChangedVibration => m_useVibration.OnChanged;
		public bool UseVibration
		{
			get => m_useVibration.Value;
			set => _ApplyVibration(value);
		}

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

		private string _PrefsKey(string name) => $"[{nameof(VibrationManager)}] {name}";

		protected override void _Initialize()
		{
			base._Initialize();

			m_useVibration = new ReactivePrefs<bool>(_PrefsKey(nameof(m_useVibration)),bool.TryParse,true);

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
				m_useVibration?.Dispose();
			}

			base._Release(disposing);
		}

		private void _ApplyVibration(bool useVibration)
		{
			m_useVibration.TrySetValue(useVibration);
		}

		/// <param name="_amplitude">0.0 ~ 10.0</param>
		public void Play(float amplitude,float millisecond)
		{
			if(amplitude <= 0.0f || millisecond <= 0.0f)
			{
				return;
			}

			var newAmplitude = Mathf.Clamp(amplitude,0.0f,10.0f);

			if(!UseVibration)
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