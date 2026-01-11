using System;
using System.Reflection;
using KZLib.KZDevelop;
using R3;
using UnityEngine;

namespace KZLib.KZData
{
	/// <summary>
	/// OptionConfig is only used to store by PlayerPrefs.
	/// </summary>
	public class OptionConfig : IConfig,IDisposable
	{
		private SoundVolume m_masterVolume = SoundVolume.max;
		private SoundVolume m_musicVolume = SoundVolume.max;
		private SoundVolume m_effectVolume = SoundVolume.max;
		public SoundProfile CurrentSound => new(m_masterVolume,m_musicVolume,m_effectVolume);

		private ScreenResolution m_resolution = new(Global.BASE_WIDTH,Global.BASE_HEIGHT,true);
		public ScreenResolution CurrentResolution => m_resolution;

		private int m_frameRate = Global.FRAME_RATE_60;
		public int CurrentFrameRate => m_frameRate;

		private long m_graphicQuality = GraphicQualityOption.In.GetGraphicQualityInPreset(GraphicQualityPresetType.QualityHighest);
		public long CurrentGraphicQuality => m_graphicQuality;

		private bool m_useVibration = true;
		public bool UseVibration => m_useVibration;

#if UNITY_EDITOR
		public SystemLanguage m_language = SystemLanguage.English;
#else
		public SystemLanguage m_language = Application.systemLanguage;
#endif
		public SystemLanguage CurrentLanguage => m_language;

		private readonly Subject<SoundProfile> m_soundProfileSubject = new();
		public Observable<SoundProfile> OnChangedSoundVolume => m_soundProfileSubject;

		private readonly Subject<ScreenResolution> m_screenResolutionSubject = new();
		public Observable<ScreenResolution> OnChangedResolution => m_screenResolutionSubject;

		private readonly Subject<int> m_frameRateSubject = new();
		public Observable<int> OnChangedFrameRate => m_frameRateSubject;

		private readonly Subject<long> m_graphicQualitySubject = new();
		public Observable<long> OnChangedGraphicQuality => m_graphicQualitySubject;

		private readonly Subject<bool> m_useVibrationSubject = new();
		public Observable<bool> OnChangedUseVibration => m_useVibrationSubject;

		private readonly Subject<SystemLanguage> m_languageSubject = new();
		public Observable<SystemLanguage> OnChangedLanguage => m_languageSubject;

		private bool m_disposed = false;

		public OptionConfig()
		{
			if(!TryReload())
			{
				return;
			}
		}

		~OptionConfig()
		{
			_Dispose(false);
		}

		public void Dispose()
		{
			_Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void _Dispose(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				m_soundProfileSubject.Dispose();
				m_screenResolutionSubject.Dispose();
				m_frameRateSubject.Dispose();
				m_graphicQualitySubject.Dispose();
				m_useVibrationSubject.Dispose();
				m_languageSubject.Dispose();
			}

			m_disposed = true;
		}

		public bool TryReload()
		{
			if(!TryLoadPlayerPrefs())
			{
				return false;
			}

			return true;
		}

		public bool TryLoadPlayerPrefs()
		{
			var fieldInfoArray = typeof(OptionConfig).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

			for(var i=0;i<fieldInfoArray.Length;i++)
			{
				var fieldInfo = fieldInfoArray[i];
				var fieldType = fieldInfo.FieldType;
				var nameKey = fieldInfo.Name;

				if(!PlayerPrefsManager.In.TryGetString(nameKey,out var text))
				{
					continue;
				}

				if(!_TryParseValue(nameKey,fieldType,text,out var value))
				{
					return false;
				}

				fieldInfo.SetValue(this,value);
			}

			return true;
		}

		private bool _TryParseValue(string name,Type type,string text,out object value)
		{
			value = null;

			switch(Type.GetTypeCode(type))
			{
				case TypeCode.String:
					return _TryParseStringValue(name,text,out value);
				case TypeCode.Int32:
					return _TryParseIntValue(name,text,out value);
				case TypeCode.Boolean:
					return _TryParseBoolValue(name,text,out value);
				case TypeCode.Single:
					return _TryParseFloatValue(name,text,out value);
				case TypeCode.Object when type == typeof(Enum):
					return _TryParseEnumValue(name,text,type,out value);
				case TypeCode.Object when type == typeof(SoundVolume):
					return _TryParseSoundVolumeValue(name,text,out value);
				case TypeCode.Object when type == typeof(ScreenResolution):
					return _TryParseScreenResolutionValue(name,text,out value);
				default:
					LogChannel.System.E($"Not supported propertyType ({type}), name ({name})");

					return false;
			}
		}

		private bool _TryParseStringValue(string name,string text,out object value)
		{
			if(text.IsEmpty())
			{
				value = string.Empty;

				LogChannel.System.E($"{name} parse failed from {text}.");

				return false;
			}

			value = text;

			return true;
		}

		private bool _TryParseIntValue(string name,string text,out object value)
		{
			if(!int.TryParse(text,out var number))
			{
				value = 0;

				LogChannel.System.E($"{name} parse failed from {text}.");

				return false;
			}

			value = number;

			return true;
		}

		private bool _TryParseBoolValue(string name,string text,out object value)
		{
			if(!bool.TryParse(text,out var boolean))
			{
				value = false;

				LogChannel.System.E($"{name} parse failed from {text}.");

				return false;
			}

			value = boolean;

			return true;
		}

		private bool _TryParseFloatValue(string name,string text,out object value)
		{
			if(!float.TryParse(text,out var number))
			{
				value = 0.0f;

				LogChannel.System.E($"{name} parse failed from {text}.");

				return false;
			}

			value = number;

			return true;
		}

		private bool _TryParseEnumValue(string name,string text,Type enumType,out object value)
		{
			if(!Enum.TryParse(enumType,text,out var result))
			{
				value = default;

				LogChannel.System.E($"{name} parse failed from {text}.");

				return false;
			}

			value = result;

			return true;
		}

		private bool _TryParseSoundVolumeValue(string name,string text,out object value)
		{
			if(!SoundVolume.TryParse(text,out var volume))
			{
				value = SoundVolume.zero;

				LogChannel.System.E($"{name} parse failed from {text}.");

				return false;
			}

			value = volume;

			return true;
		}

		private bool _TryParseScreenResolutionValue(string name,string text,out object value)
		{
			if(!ScreenResolution.TryParse(text,out var resolution))
			{
				value = ScreenResolution.hd;

				LogChannel.System.E($"{name} parse failed from {text}.");

				return false;
			}

			value = resolution;

			return true;
		}

		private void _SetStringPlayerPrefs(string key,string value)
		{
			PlayerPrefsManager.In.SetString(key,value);
		}

		#region Sound
		public void SetMasterVolume(float level = 1.0f,bool isMute = false)
		{
			_SetVolumeOption(ref m_masterVolume,new SoundVolume(level,isMute),nameof(m_masterVolume));
		}

		public void SetMusicVolume(float level = 1.0f,bool isMute = false)
		{
			_SetVolumeOption(ref m_musicVolume,new SoundVolume(level,isMute),nameof(m_musicVolume));
		}

		public void SetEffectVolume(float level = 1.0f,bool isMute = false)
		{
			_SetVolumeOption(ref m_effectVolume,new SoundVolume(level,isMute),nameof(m_effectVolume));
		}

		private void _SetVolumeOption(ref SoundVolume oldValue,SoundVolume newValue,string nameKey)
		{
			void _ChangeVolume()
			{
				m_soundProfileSubject.OnNext(CurrentSound);
			}

			_SetValue(ref oldValue,newValue,nameKey,_ChangeVolume);
		}
		#endregion Sound

		#region Graphic
		public void SetScreenResolution(ScreenResolution newResolution)
		{
			void _ChangeResolution()
			{
				Screen.SetResolution(m_resolution.width,m_resolution.height,m_resolution.fullscreen);

				m_screenResolutionSubject.OnNext(CurrentResolution);
			}

			_SetValue(ref m_resolution,newResolution,nameof(m_resolution),_ChangeResolution);
		}

		public void SetFrameRate(int newFrameRate)
		{
			void _ChangeFrameRate()
			{
				Application.targetFrameRate = m_frameRate;

				m_frameRateSubject.OnNext(CurrentFrameRate);
			}

			_SetValue(ref m_frameRate,newFrameRate,nameof(m_frameRate),_ChangeFrameRate);
		}

		public void AddGraphicQuality(long graphicQuality)
		{
			_SetGraphicQuality(m_graphicQuality.AddFlag(graphicQuality));
		}

		public void RemoveGraphicQuality(long graphicQuality)
		{
			_SetGraphicQuality(m_graphicQuality.RemoveFlag(graphicQuality));
		}

		private void _SetGraphicQuality(long newGraphicQuality)
		{
			void _ChangeGraphicQuality()
			{
				_CheckGraphicQuality();

				m_graphicQualitySubject.OnNext(CurrentGraphicQuality);
			}

			_SetValue(ref m_graphicQuality,newGraphicQuality,nameof(m_graphicQuality),_ChangeGraphicQuality);
		}

		private void _CheckGraphicQuality()
		{
			QualitySettings.globalTextureMipmapLimit = int.Parse(GraphicQualityOption.In.FindValue(m_graphicQuality,Global.GLOBAL_TEXTURE_MIPMAP_LIMIT));
			QualitySettings.anisotropicFiltering = (AnisotropicFiltering) Enum.Parse(typeof(AnisotropicFiltering),GraphicQualityOption.In.FindValue(m_graphicQuality,Global.ANISOTROPIC_FILTERING));
			QualitySettings.vSyncCount = int.Parse(GraphicQualityOption.In.FindValue(m_graphicQuality,Global.VERTICAL_SYNC_COUNT));
		}
		#endregion Graphic

		#region Native
		public void SetUseVibration(bool newUseVibration)
		{
			void _ChangeVibration()
			{
				m_useVibrationSubject.OnNext(UseVibration);
			}

			_SetValue(ref m_useVibration,newUseVibration,nameof(m_useVibration),_ChangeVibration);
		}
		#endregion Native

		#region Language
		public void SetLanguage(SystemLanguage newLanguage)
		{
			void _ChangeLanguage()
			{
				m_languageSubject.OnNext(CurrentLanguage);
			}

			_SetValue(ref m_language,newLanguage,nameof(m_language),_ChangeLanguage);
		}
		#endregion Language

		private void _SetValue<TValue>(ref TValue oldValue,TValue newValue,string nameKey,Action onValueChanged)
		{
			if(oldValue.Equals(newValue))
			{
				return;
			}

			oldValue = newValue;

			_SetStringPlayerPrefs(nameKey,newValue.ToString());

			onValueChanged?.Invoke();
		}
	}
}