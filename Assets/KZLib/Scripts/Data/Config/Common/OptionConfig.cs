using System;
using KZLib.KZData;
using KZLib.KZDevelop;
using UnityEngine;
using UnityEngine.Events;

namespace ConfigData
{
	public class OptionConfig : IConfig
	{
		public SoundVolume m_masterVolume = SoundVolume.max;
		public SoundVolume m_musicVolume = SoundVolume.max;
		public SoundVolume m_effectVolume = SoundVolume.max;
		public SoundVolume MasterVolume { get => m_masterVolume; private set => m_masterVolume = value; }
		public SoundVolume MusicVolume { get => m_musicVolume; private set => m_musicVolume = value; }
		public SoundVolume EffectVolume { get => m_effectVolume; private set => m_effectVolume = value; }


		public ScreenResolution Resolution { get; private set; } = new(Global.BASE_WIDTH,Global.BASE_HEIGHT,true);
		public int FrameRate { get; private set; } = Global.FRAME_RATE_60;
		public long GraphicQuality { get; private set; } = GraphicQualityOption.In.GetGraphicQualityInPreset(GraphicQualityPresetType.QualityHighest);

		public bool UseVibration { get; private set; } = true;

#if UNITY_EDITOR
		public SystemLanguage Language { get; private set; } = SystemLanguage.English;
#else
		public SystemLanguage Language { get; private set; } = Application.systemLanguage;
#endif

		public event UnityAction<SoundVolume,SoundVolume,SoundVolume> OnSoundVolumeChange = null;

		public event UnityAction<ScreenResolution> OnResolutionChange = null;
		public event UnityAction<int> OnFrameRateChange = null;
		public event UnityAction<long> OnGraphicQualityChange = null;

		public event UnityAction<bool> OnUseVibrationChange = null;

		public event UnityAction<SystemLanguage> OnLanguageChange = null;

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
			foreach(var propertyInfo in typeof(OptionConfig).GetProperties())
			{
				var propertyType = propertyInfo.PropertyType;
				var nameKey = propertyInfo.Name;

				if(!PlayerPrefs.HasKey(nameKey))
				{
					continue;
				}

				var text = PlayerPrefs.GetString(nameKey,string.Empty);

				if(!_TryParseValue(nameKey,propertyType,text,out var value))
				{
					return false;
				}

				propertyInfo.SetValue(this,value);
			}

			return true;
		}

		private bool _TryParseValue(string propertyName,Type propertyType,string text,out object value)
		{
			value = null;

			switch(Type.GetTypeCode(propertyType))
			{
				case TypeCode.String:
					return _TryParseStringValue(propertyName,text,out value);
				case TypeCode.Int32:
					return _TryParseIntValue(propertyName,text,out value);
				case TypeCode.Boolean:
					return _TryParseBoolValue(propertyName,text,out value);
				case TypeCode.Single:
					return _TryParseFloatValue(propertyName,text,out value);
				case TypeCode.Object when propertyType == typeof(Enum):
					return _TryParseEnumValue(propertyName,text,propertyType,out value);
				case TypeCode.Object when propertyType == typeof(SoundVolume):
					return _TryParseSoundVolumeValue(propertyName,text,out value);
				case TypeCode.Object when propertyType == typeof(ScreenResolution):
					return _TryParseScreenResolutionValue(propertyName,text,out value);
				default:
					LogTag.System.E($"Not supported propertyType ({propertyType}), name ({propertyName})");

					return false;
			}
		}

		private bool _TryParseStringValue(string propertyName,string text,out object value)
		{
			if(text.IsEmpty())
			{
				value = string.Empty;

				LogTag.System.E($"{propertyName} parse failed from {text}.");

				return false;
			}

			value = text;

			return true;
		}

		private bool _TryParseIntValue(string propertyName,string text,out object value)
		{
			if(!int.TryParse(text,out var number))
			{
				value = 0;

				LogTag.System.E($"{propertyName} parse failed from {text}.");

				return false;
			}

			value = number;

			return true;
		}

		private bool _TryParseBoolValue(string propertyName,string text,out object value)
		{
			if(!bool.TryParse(text,out var boolean))
			{
				value = false;

				LogTag.System.E($"{propertyName} parse failed from {text}.");

				return false;
			}

			value = boolean;

			return true;
		}

		private bool _TryParseFloatValue(string propertyName,string text,out object value)
		{
			if(!float.TryParse(text,out var number))
			{
				value = 0.0f;

				LogTag.System.E($"{propertyName} parse failed from {text}.");

				return false;
			}

			value = number;

			return true;
		}

		private bool _TryParseEnumValue(string propertyName,string text,Type enumType,out object value)
		{
			if(!Enum.TryParse(enumType,text,out var result))
			{
				value = default;

				LogTag.System.E($"{propertyName} parse failed from {text}.");

				return false;
			}

			value = result;

			return true;
		}

		private bool _TryParseSoundVolumeValue(string propertyName,string text,out object value)
		{
			if(!SoundVolume.TryParse(text,out var volume))
			{
				value = SoundVolume.zero;

				LogTag.System.E($"{propertyName} parse failed from {text}.");

				return false;
			}

			value = volume;

			return true;
		}

		private bool _TryParseScreenResolutionValue(string propertyName,string text,out object value)
		{
			if(!ScreenResolution.TryParse(text,out var resolution))
			{
				value = ScreenResolution.hd;

				LogTag.System.E($"{propertyName} parse failed from {text}.");

				return false;
			}

			value = resolution;

			return true;
		}

		private void _SetStringPlayerPrefs(string key,string value)
		{
			try
			{
				PlayerPrefs.SetString(key,value);
			}
			catch(Exception exception)
			{
				LogTag.System.E($"Set playerPrefs failed. [{key}/{value} - {exception.Message}]");
			}
		}

		#region Sound
		public void SetMasterVolume(float level = 1.0f,bool isMute = false)
		{
			_SetVolumeOption(ref m_masterVolume,new SoundVolume(level,isMute),nameof(MasterVolume));
		}

		public void SetMusicVolume(float level = 1.0f,bool isMute = false)
		{
			_SetVolumeOption(ref m_musicVolume,new SoundVolume(level,isMute),nameof(MusicVolume));
		}

		public void SetEffectVolume(float level = 1.0f,bool isMute = false)
		{
			_SetVolumeOption(ref m_effectVolume,new SoundVolume(level,isMute),nameof(EffectVolume));
		}

		private void _SetVolumeOption(ref SoundVolume refValue,SoundVolume newValue,string nameKey)
		{
			if(refValue == newValue)
			{
				return;
			}

			refValue = newValue;

			_SetStringPlayerPrefs(nameKey,newValue.ToString());

			OnSoundVolumeChange?.Invoke(MasterVolume,MusicVolume,EffectVolume);
		}
		#endregion Sound

		#region Graphic
		public void SetScreenResolution(ScreenResolution newResolution)
		{
			if(Resolution == newResolution)
			{
				return;
			}

			Resolution = newResolution;

			Screen.SetResolution(Resolution.width,Resolution.height,Resolution.fullscreen);

			_SetStringPlayerPrefs(nameof(Resolution),newResolution.ToString());

			OnResolutionChange?.Invoke(newResolution);
		}

		public void SetFrameRate(int newFrameRate)
		{
			if(FrameRate == newFrameRate)
			{
				return;
			}

			FrameRate = newFrameRate;

			Application.targetFrameRate = newFrameRate;

			_SetStringPlayerPrefs(nameof(FrameRate),newFrameRate.ToString());

			OnFrameRateChange?.Invoke(newFrameRate);
		}

		public void AddGraphicQuality(long quality)
		{
			_SetGraphicQuality(GraphicQuality.AddFlag(quality));
		}

		public void RemoveGraphicQuality(long quality)
		{
			_SetGraphicQuality(GraphicQuality.RemoveFlag(quality));
		}

		private void _SetGraphicQuality(long quality)
		{
			if(GraphicQuality == quality)
			{
				return;
			}

			GraphicQuality = quality;

			_CheckGraphicQuality();

			OnGraphicQualityChange?.Invoke(quality);
		}

		private void _CheckGraphicQuality()
		{
			QualitySettings.globalTextureMipmapLimit = int.Parse(GraphicQualityOption.In.FindValue(GraphicQuality,Global.GLOBAL_TEXTURE_MIPMAP_LIMIT));
			QualitySettings.anisotropicFiltering = (AnisotropicFiltering) Enum.Parse(typeof(AnisotropicFiltering),GraphicQualityOption.In.FindValue(GraphicQuality,Global.ANISOTROPIC_FILTERING));
			QualitySettings.vSyncCount = int.Parse(GraphicQualityOption.In.FindValue(GraphicQuality,Global.VERTICAL_SYNC_COUNT));
		}
		#endregion Graphic

		#region Native
		public void SetUseVibration(bool newUseVibration)
		{
			if(UseVibration == newUseVibration)
			{
				return;
			}

			UseVibration = newUseVibration;

			_SetStringPlayerPrefs(nameof(UseVibration),newUseVibration.ToString());

			OnUseVibrationChange.Invoke(newUseVibration);
		}
		#endregion Native

		#region Language
		public void SetLanguage(SystemLanguage newLanguage)
		{
			if(Language == newLanguage)
			{
				return;
			}

			Language = newLanguage;

			_SetStringPlayerPrefs(nameof(Language),newLanguage.ToString());

			OnLanguageChange.Invoke(newLanguage);
		}
		#endregion Language
	}
}