using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.Attributes;

#if UNITY_EDITOR

using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;

#endif

namespace KZLib.Data
{
	/// <summary>
	/// ScriptableObject catalog loaded from Resources; maps graphic-quality option names to Unity setting values.
	/// Consumed only by <see cref="GraphicManager"/>. Each entry owns one bit in the manager's quality bitmask
	/// and stores On/Off string payloads. <see cref="GetGraphicQualityInPreset"/> builds preset bitmasks
	/// (e.g. <see cref="GraphicManager.Highest"/>); the manager applies the active bitmask via
	/// <see cref="FindOptionValue{TValue}"/>.
	/// </summary>
	public class GraphicQualityOption : SerializedScriptableObject
	{
		/// <summary>Maximum number of single-bit options (bit index 0 .. 63).</summary>
		private const int c_maxOptionCount = 64;

		/// <summary>Default <see cref="GraphicQualityEntry.MinimumPreset"/> for options added through the editor UI.</summary>
		private const GraphicQualityPresetType c_addOptionDefaultMinimumPreset = GraphicQualityPresetType.QualityHighest;

#if UNITY_EDITOR
		private enum AddOptionResult { Success, DuplicateName, }

		#region Option Handler
		/// <summary>Odin popup fields for creating a new catalog entry.</summary>
		private class OptionHandler
		{
			private string m_optionName = string.Empty;
			private string m_flagOnValue = string.Empty;
			private string m_flagOffValue = string.Empty;

			private string m_errorText = string.Empty;

			[HorizontalGroup("0",Order = 0),ShowInInspector]
			private string OptionName
			{
				get => m_optionName;
				set
				{
					m_errorText = string.Empty;
					m_optionName = value;
				}
			}

			[HorizontalGroup("1",Order = 1),ShowInInspector]
			private string FlagOnValue
			{
				get => m_flagOnValue;
				set
				{
					m_errorText = string.Empty;
					m_flagOnValue = value;
				}
			}

			[HorizontalGroup("2",Order = 2),ShowInInspector]
			private string FlagOffValue
			{
				get => m_flagOffValue;
				set
				{
					m_errorText = string.Empty;
					m_flagOffValue = value;
				}
			}

			[HorizontalGroup("3",Order = 3),ShowInInspector,KZRichText,HideLabel]
			protected string ErrorText => m_errorText;

			private readonly Func<string,string,string,AddOptionResult> m_onAddOption = null;

			private bool IsValidAddOption => !OptionName.IsEmpty() && !FlagOnValue.IsEmpty() && !FlagOffValue.IsEmpty();

			private OdinEditorWindow m_optionWindow = null;

			public OptionHandler(Func<string,string,string,AddOptionResult> onAddOption)
			{
				m_onAddOption = onAddOption;
			}

			[HorizontalGroup("4",Order = 4),Button("Add",ButtonSizes.Large),EnableIf(nameof(IsValidAddOption))]
			protected void OnAddOption()
			{
				switch(m_onAddOption.Invoke(OptionName,FlagOnValue,FlagOffValue))
				{
					case AddOptionResult.Success:
					{
						m_optionWindow.Close();

						break;
					}
					case AddOptionResult.DuplicateName:
					{
						m_errorText = "<color=red>already exist.</color>";

						break;
					}
				}
			}

			public void SetWindow(OdinEditorWindow window)
			{
				m_optionWindow = window;
			}
		}
		#endregion Option Handler
#endif

		#region Graphic Quality Entry
		/// <summary>
		/// One catalog entry. When its flag bit is set in the quality bitmask, <see cref="GetValue"/> returns the On string;
		/// otherwise the Off string. Typed parsing is done by the catalog query API.
		/// </summary>
		[Serializable]
		public class GraphicQualityEntry
		{
			[SerializeField,HideInInspector]
			private string m_name = string.Empty;

			[SerializeField,HideInInspector]
			private long m_flag = 0L;

			[SerializeField,HideInInspector]
			private string m_flagOnValue = string.Empty;

			[SerializeField,HideInInspector]
			private string m_flagOffValue = string.Empty;

			/// <summary>Lowest preset tier that includes this entry when building a preset bitmask.</summary>
			[SerializeField,HideInInspector]
			private GraphicQualityPresetType m_minimumPreset = GraphicQualityPresetType.QualityLowest;

			public GraphicQualityEntry(string name,long flag,string flagOnValue,string flagOffValue,GraphicQualityPresetType minimumPreset = GraphicQualityPresetType.QualityLowest)
			{
				m_name = name;
				m_flag = flag;
				m_flagOnValue = flagOnValue;
				m_flagOffValue = flagOffValue;
				m_minimumPreset = minimumPreset;
			}

			public long Flag => m_flag;

			/// <summary>Zero-based bit index of <see cref="Flag"/>; used for inspector display.</summary>
			public int Order => m_flag.ToFlagOrder();

			public string Name => m_name;

			[HorizontalGroup("Row")]
			[ShowInInspector,HideLabel,KZRichText,HorizontalGroup("Row/Info")]
			protected string FlagName => $"[{Order}] : {m_name} [On:{m_flagOnValue}/Off:{m_flagOffValue}]";

			[ShowInInspector,HorizontalGroup("Row/Min"),LabelText("Minimum Preset")]
			public GraphicQualityPresetType MinimumPreset
			{
				get => m_minimumPreset;
				set => m_minimumPreset = value;
			}

			/// <summary>Returns the On or Off string for the active quality bitmask from <see cref="KZLib.GraphicManager"/>.</summary>
			public string GetValue(long graphicQuality)
			{
				return graphicQuality.HasAnyFlag(m_flag) ? m_flagOnValue : m_flagOffValue;
			}
		}
		#endregion Graphic Quality Entry

		[SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true,DraggableItems = false),OnValueChanged(nameof(_OnChangedOptionList))]
		private List<GraphicQualityEntry> m_qualityList = new();

#if UNITY_EDITOR
		[HorizontalGroup("Button",Order = 0),Button("Add Option",ButtonSizes.Large),DisableIf(nameof(IsOptionListFull)),Tooltip("Cannot add more options.")]
		protected void OnAddOption()
		{
			var handler = new OptionHandler(_TryAddOption);
			var window = OdinEditorWindow.InspectObject(handler);

			window.titleContent = new GUIContent("New Graphic Quality Option");
			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(350,150);

			handler.SetWindow(window);
		}

		private bool IsOptionListFull => m_qualityList.Count >= c_maxOptionCount;

		private AddOptionResult _TryAddOption(string optionName,string flagOnValue,string flagOffValue)
		{
			return _TryAddOption(optionName,flagOnValue,flagOffValue,c_addOptionDefaultMinimumPreset);
		}

		/// <summary>Allocates the next free flag bit and appends an entry. Rejects duplicate names.</summary>
		private AddOptionResult _TryAddOption(string optionName,string flagOnValue,string flagOffValue,GraphicQualityPresetType minimumPreset)
		{
			for(var i=0;i<m_qualityList.Count;i++)
			{
				var option = m_qualityList[i];

				if(string.Equals(option.Name,optionName,StringComparison.Ordinal))
				{
					return AddOptionResult.DuplicateName;
				}
			}

			var flag = _AllocateNextFlag();

			m_qualityList.Add(new GraphicQualityEntry(optionName,flag,flagOnValue,flagOffValue,minimumPreset));

			_OnChangedOptionList();

			return AddOptionResult.Success;
		}

		/// <summary>Returns the lowest unused single-bit flag, or throws when all 64 bits are taken.</summary>
		private long _AllocateNextFlag()
		{
			var usedFlags = 0L;

			for(var i = 0;i<m_qualityList.Count;i++)
			{
				usedFlags |= m_qualityList[i].Flag;
			}

			for(var i=0; i<c_maxOptionCount;i++)
			{
				var candidate = 1L << i;

				if(!usedFlags.HasAnyFlag(candidate))
				{
					return candidate;
				}
			}

			throw new InvalidOperationException("Failed to allocate next flag");
		}

		/// <summary>Seeds entries consumed by <see cref="GraphicManager"/> (QualitySettings and camera far clip).</summary>
		private void Reset()
		{
			m_qualityList.Clear();

			_TryAddOption(Global.GlobalTextureMipmapLimit,"0","1",GraphicQualityPresetType.QualityMiddle);
			_TryAddOption(Global.AnisotropicFiltering,"Enable","Disable",GraphicQualityPresetType.QualityHigh);
			_TryAddOption(Global.DisableCameraFarHalf,"1.0","0.5",GraphicQualityPresetType.QualityHighest);
			_TryAddOption(Global.LodBias,"1.0","0.5",GraphicQualityPresetType.QualityMiddle);
			_TryAddOption(Global.MaximumLODLevel,"0","1",GraphicQualityPresetType.QualityLow);
			_TryAddOption(Global.AntiAliasing,"2","0",GraphicQualityPresetType.QualityHigh);
			_TryAddOption(Global.SkinWeights,"FourBones","TwoBones",GraphicQualityPresetType.QualityHigh);
			_TryAddOption(Global.ShadowDistance,"80","40",GraphicQualityPresetType.QualityMiddle);
			_TryAddOption(Global.RealtimeReflectionProbes,"True","False",GraphicQualityPresetType.QualityHigh);
		}
#endif

		/// <summary>Keeps the inspector list ordered by flag bit index.</summary>
		private void _OnChangedOptionList()
		{
			static int _Compare(GraphicQualityEntry infoA,GraphicQualityEntry infoB)
			{
				return infoA.Flag.CompareTo(infoB.Flag);
			}

			m_qualityList.Sort(_Compare);
		}

		/// <summary>
		/// Builds a preset bitmask for <see cref="KZLib.GraphicManager"/> (e.g. <see cref="KZLib.GraphicManager.Highest"/>).
		/// Each entry whose <see cref="GraphicQualityEntry.MinimumPreset"/> is met by <paramref name="presetType"/>
		/// has its flag bit set.
		/// </summary>
		public long GetGraphicQualityInPreset(GraphicQualityPresetType presetType)
		{
			var graphicsQuality = 0L;

			for(var i=0;i<m_qualityList.Count;i++)
			{
				var option = m_qualityList[i];

				if(_IsAtLeast(presetType,option.MinimumPreset))
				{
					graphicsQuality = graphicsQuality.AddFlag(option.Flag);
				}
			}

			return graphicsQuality;
		}

		#region Query
		/// <summary>Returns the raw On/Off string for an option in the manager's quality bitmask. Throws when the option name is missing.</summary>
		public string FindValue(long graphicQuality,string optionName)
		{
			if(TryFindValue(graphicQuality,optionName,out var value))
			{
				return value;
			}

			throw new InvalidOperationException($"Graphic quality option not found. [{optionName}]");
		}

		/// <summary>Returns the raw On/Off string for an option without throwing.</summary>
		public bool TryFindValue(long graphicQuality,string optionName,out string value)
		{
			if(_TryFindOption(optionName,out var option))
			{
				value = option.GetValue(graphicQuality);

				return true;
			}

			value = string.Empty;

			return false;
		}

		private bool _TryFindOption(string optionName,out GraphicQualityEntry option)
		{
			for(var i=0;i<m_qualityList.Count;i++)
			{
				option = m_qualityList[i];

				if(string.Equals(option.Name,optionName,StringComparison.Ordinal))
				{
					return true;
				}
			}

			option = null;

			return false;
		}

		/// <summary>
		/// Returns a typed Unity setting value for <see cref="KZLib.GraphicManager"/>.
		/// Parsing follows <typeparamref name="TValue"/> (string, enum, or <see cref="Convert.ChangeType"/>).
		/// Throws on missing name, empty value, or parse failure.
		/// </summary>
		public TValue FindOptionValue<TValue>(long graphicQuality,string optionName)
		{
			if(!_TryFindOption(optionName,out var option))
			{
				throw new InvalidOperationException($"Graphic quality option not found. [{optionName}]");
			}

			var stringValue = option.GetValue(graphicQuality);

			if(stringValue.IsEmpty())
			{
				throw new InvalidOperationException($"Graphic quality option value is empty. [{optionName}]");
			}

			try
			{
				return _ParseOptionValue<TValue>(stringValue);
			}
			catch(Exception exception)
			{
				throw new FormatException($"Failed to parse graphic quality option value. [{optionName}] [{stringValue}]",exception);
			}
		}

		/// <summary>Non-throwing counterpart of <see cref="FindOptionValue{TValue}"/>; logs parse failures.</summary>
		public bool TryFindOptionValue<TValue>(long graphicQuality,string optionName,out TValue optionValue)
		{
			optionValue = default;

			if(!TryFindValue(graphicQuality,optionName,out var stringValue))
			{
				return false;
			}

			if(stringValue.IsEmpty())
			{
				return false;
			}

			try
			{
				optionValue = _ParseOptionValue<TValue>(stringValue);

				return true;
			}
			catch(Exception exception)
			{
				LogChannel.Graphic.E($"Failed to parse option value. [{optionName}] [{stringValue}] [{exception.Message}]");
			}

			return false;
		}

		/// <summary>Parses a catalog string into <typeparamref name="TValue"/>.</summary>
		private static TValue _ParseOptionValue<TValue>(string value)
		{
			var targetType = typeof(TValue);

			if(targetType == typeof(string))
			{
				return (TValue)(object)value;
			}
			if(targetType.IsEnum)
			{
				return (TValue)Enum.Parse(targetType, value, ignoreCase: true);
			}

			return (TValue)Convert.ChangeType(value, targetType);
		}
		#endregion Query

		/// <summary>Returns whether <paramref name="presetType"/> meets or exceeds <paramref name="minimumPreset"/>.</summary>
		private bool _IsAtLeast(GraphicQualityPresetType presetType,GraphicQualityPresetType minimumPreset)
		{
			return _ToPresetRank(presetType) >= _ToPresetRank(minimumPreset);
		}

		/// <summary>Explicit preset ordering; independent of enum declaration order.</summary>
		private int _ToPresetRank(GraphicQualityPresetType presetType)
		{
			return presetType switch
			{
				GraphicQualityPresetType.QualityLowest	=> 0,
				GraphicQualityPresetType.QualityLow		=> 1,
				GraphicQualityPresetType.QualityMiddle	=> 2,
				GraphicQualityPresetType.QualityHigh	=> 3,
				GraphicQualityPresetType.QualityHighest => 4,

				_ => throw new ArgumentOutOfRangeException(nameof(presetType),presetType,null),
			};
		}
	}
}
