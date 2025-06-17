using System;
using System.Reflection;
using KZLib;
using KZLib.KZData;
using KZLib.KZDevelop;
using UnityEngine;

namespace ConfigData
{
	/// <summary>
	/// OptionConfig is only used to store by PlayerPrefs.
	/// </summary>
	public class OptionConfig : IConfig
	{
		private SoundVolume m_masterVolume = SoundVolume.max;
		public SoundVolume MasterVolume => m_masterVolume;

		private SoundVolume m_musicVolume = SoundVolume.max;
		public SoundVolume MusicVolume => m_musicVolume;

		private SoundVolume m_effectVolume = SoundVolume.max;
		public SoundVolume EffectVolume => m_effectVolume;

		private ScreenResolution m_resolution = new(Global.BASE_WIDTH,Global.BASE_HEIGHT,true);
		public ScreenResolution Resolution => m_resolution;

		private int m_frameRate = Global.FRAME_RATE_60;
		public int FrameRate => m_frameRate;

		private long m_graphicQuality = GraphicQualityOption.In.GetGraphicQualityInPreset(GraphicQualityPresetType.QualityHighest);
		public long GraphicQuality => m_graphicQuality;

		private bool m_useVibration = true;
		public bool UseVibration => m_useVibration;

#if UNITY_EDITOR
		public SystemLanguage m_language = SystemLanguage.English;
#else
		public SystemLanguage m_language = Application.systemLanguage;
#endif
		public SystemLanguage Language => m_language;

#if KZLIB_PLAY_FAB
		private PlayFabLogInOptionType m_playFabLogInType = PlayFabLogInOptionType.None;
		public PlayFabLogInOptionType PlayFabLogInType => m_playFabLogInType;
#endif
		public event Action<SoundVolume,SoundVolume,SoundVolume> OnSoundVolumeChanged = null;

		public event Action<ScreenResolution> OnResolutionChanged = null;
		public event Action<int> OnFrameRateChanged = null;
		public event Action<long> OnGraphicQualityChanged = null;

		public event Action<bool> OnUseVibrationChanged = null;

		public event Action<SystemLanguage> OnLanguageChanged = null;

#if KZLIB_PLAY_FAB
		public event Action<PlayFabLogInOptionType> OnPlayFabLogInTypeChanged = null;
#endif
		public OptionConfig()
		{
			if(!TryReload())
			{
				return;
			}
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
			foreach(var fieldInfo in typeof(OptionConfig).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
			{
				var fieldType = fieldInfo.FieldType;
				var nameKey = fieldInfo.Name;

				if(!PlayerPrefsMgr.In.TryGetString(nameKey,out var text))
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
					LogSvc.System.E($"Not supported propertyType ({type}), name ({name})");

					return false;
			}
		}

		private bool _TryParseStringValue(string name,string text,out object value)
		{
			if(text.IsEmpty())
			{
				value = string.Empty;

				LogSvc.System.E($"{name} parse failed from {text}.");

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

				LogSvc.System.E($"{name} parse failed from {text}.");

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

				LogSvc.System.E($"{name} parse failed from {text}.");

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

				LogSvc.System.E($"{name} parse failed from {text}.");

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

				LogSvc.System.E($"{name} parse failed from {text}.");

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

				LogSvc.System.E($"{name} parse failed from {text}.");

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

				LogSvc.System.E($"{name} parse failed from {text}.");

				return false;
			}

			value = resolution;

			return true;
		}

		private void _SetStringPlayerPrefs(string key,string value)
		{
			PlayerPrefsMgr.In.SetString(key,value);
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
			_SetValue(ref oldValue,newValue,nameKey,()=>
			{
				OnSoundVolumeChanged?.Invoke(m_masterVolume,m_musicVolume,m_effectVolume);
			});
		}
		#endregion Sound

		#region Graphic
		public void SetScreenResolution(ScreenResolution newResolution)
		{
			_SetValue(ref m_resolution,newResolution,nameof(m_resolution),()=>
			{
				Screen.SetResolution(m_resolution.width,m_resolution.height,m_resolution.fullscreen);

				OnResolutionChanged?.Invoke(m_resolution);
			});
		}

		public void SetFrameRate(int newFrameRate)
		{
			_SetValue(ref m_frameRate,newFrameRate,nameof(m_frameRate),()=>
			{
				Application.targetFrameRate = m_frameRate;

				OnFrameRateChanged?.Invoke(m_frameRate);
			});
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
			_SetValue(ref m_graphicQuality,newGraphicQuality,nameof(m_graphicQuality),()=>
			{
				_CheckGraphicQuality();

				OnGraphicQualityChanged?.Invoke(m_graphicQuality);
			});
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
			_SetValue(ref m_useVibration,newUseVibration,nameof(m_useVibration),()=>
			{
				_CheckGraphicQuality();

				OnUseVibrationChanged.Invoke(m_useVibration);
			});
		}
		#endregion Native

		#region Language
		public void SetLanguage(SystemLanguage newLanguage)
		{
			_SetValue(ref m_language,newLanguage,nameof(m_language),()=>
			{
				OnLanguageChanged.Invoke(m_language);
			});
		}
		#endregion Language

#if KZLIB_PLAY_FAB
		#region PlayFab LogIn Type
		public void SetPlayFabLogInType(PlayFabLogInOptionType newPlayFabLogInType)
		{
			_SetValue(ref m_playFabLogInType,newPlayFabLogInType,nameof(m_playFabLogInType),()=>
			{
				OnPlayFabLogInTypeChanged.Invoke(m_playFabLogInType);
			});
		}
		#endregion PlayFab LogIn Type
#endif

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