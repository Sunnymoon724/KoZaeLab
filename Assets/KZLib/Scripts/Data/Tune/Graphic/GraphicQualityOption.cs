using System;
using System.Collections.Generic;
using KZLib.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.Attributes;

#if UNITY_EDITOR

using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

#endif

namespace KZLib.Data
{
	/// <summary>
	/// ScriptableObject catalog for graphic quality options.
	/// Each option owns a single bit in a graphicQuality long and stores On/Off string payloads.
	/// Preset helpers (e.g. Highest) OR together flags whose MinimumPreset is met.
	/// Consumed by GraphicManager.
	/// </summary>
	public class GraphicQualityOption : SingletonSO<GraphicQualityOption>
	{
		/// <summary>Maximum number of single-bit options (bit index 0 .. 63).</summary>
		private const int c_maxOptionCount = 64;
		// UI "Add Option" path: new options start at Highest only. Adjust MinimumPreset in the list afterward.
		private const GraphicQualityPresetType c_addOptionDefaultMinimumPreset = GraphicQualityPresetType.QualityHighest;

#if UNITY_EDITOR
		private enum AddOptionResult
		{
			Success,
			DuplicateName,
			MaxCountReached,
		}

		#region Option Handler
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

			[HorizontalGroup("1",Order = 1),ShowInInspector,LabelText("Flag On Value")]
			private string FlagOnValue
			{
				get => m_flagOnValue;
				set
				{
					m_errorText = string.Empty;
					m_flagOnValue = value;
				}
			}

			[HorizontalGroup("2",Order = 2),ShowInInspector,LabelText("Flag Off Value")]
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
					case AddOptionResult.MaxCountReached:
					{
						EditorUtility.DisplayDialog("Info",$"Cannot create more options. (Max {c_maxOptionCount})","Ok");
						m_errorText = "<color=red>max options reached.</color>";

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

		#region Graphic Quality Info
		/// <summary>
		/// One graphic quality entry. Flag bit is ON when the preset includes this option;
		/// OFF otherwise. On/Off values are strings parsed by FindOptionValue.
		/// </summary>
		[Serializable]
		public class GraphicQualityInfo
		{
			[SerializeField,HideInInspector]
			private string m_name = string.Empty;

			[SerializeField,HideInInspector]
			private long m_flag = 0L;

			[SerializeField,HideInInspector]
			private string m_flagOnValue = string.Empty;

			[SerializeField,HideInInspector]
			private string m_flagOffValue = string.Empty;

			/// Lowest preset rank that turns this option's flag ON.
			[SerializeField,HideInInspector]
			private GraphicQualityPresetType m_minimumPreset = GraphicQualityPresetType.QualityLowest;

			public GraphicQualityInfo(string name,long flag,string flagOnValue,string flagOffValue,GraphicQualityPresetType minimumPreset = GraphicQualityPresetType.QualityLowest)
			{
				m_name = name;
				m_flag = flag;
				m_flagOnValue = flagOnValue;
				m_flagOffValue = flagOffValue;
				m_minimumPreset = minimumPreset;
			}

			public long Flag => m_flag;
			public int Order => m_flag.ToFlagOrder();
			public string Name => m_name;

			[HorizontalGroup("Row")]
			[ShowInInspector,HideLabel,KZRichText,HorizontalGroup("Row/Info")]
			protected string FlagName => $"{Order} : {m_name} [On:{m_flagOnValue}/Off:{m_flagOffValue}]";

			[ShowInInspector,HorizontalGroup("Row/Min"),LabelText("Minimum Preset")]
			public GraphicQualityPresetType MinimumPreset
			{
				get => m_minimumPreset;
				set => m_minimumPreset = value;
			}

			public string GetValue(long graphicQuality)
			{
				return graphicQuality.HasAnyFlag(m_flag) ? m_flagOnValue : m_flagOffValue;
			}

#if UNITY_EDITOR
			internal void _EditorSetFlag(long flag)
			{
				m_flag = flag;
			}
#endif
		}
		#endregion Graphic Quality Info

		// Preset bitmask shortcuts. Values follow the current option list in this asset.
		public static long Highest => In.GetGraphicQualityInPreset(GraphicQualityPresetType.QualityHighest);
		public static long High => In.GetGraphicQualityInPreset(GraphicQualityPresetType.QualityHigh);
		public static long Middle => In.GetGraphicQualityInPreset(GraphicQualityPresetType.QualityMiddle);
		public static long Low => In.GetGraphicQualityInPreset(GraphicQualityPresetType.QualityLow);
		public static long Lowest => In.GetGraphicQualityInPreset(GraphicQualityPresetType.QualityLowest);


#if UNITY_EDITOR
		[HorizontalGroup("Button",Order = 0),Button("Add Option",ButtonSizes.Large)]
		protected void OnAddOption()
		{
			var handler = new OptionHandler(_TryAddOption);
			var window = OdinEditorWindow.InspectObject(handler);

			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(250,150);

			handler.SetWindow(window);
		}

		private AddOptionResult _TryAddOption(string optionName,string flagOnValue,string flagOffValue)
		{
			return _TryAddOption(optionName,flagOnValue,flagOffValue,c_addOptionDefaultMinimumPreset);
		}

		private AddOptionResult _TryAddOption(string optionName,string flagOnValue,string flagOffValue,GraphicQualityPresetType minimumPreset)
		{
			var result = _TryGetOptionFlag(optionName,out var flag);

			if(result != AddOptionResult.Success)
			{
				return result;
			}

			m_optionList.Add(new GraphicQualityInfo(optionName,flag,flagOnValue,flagOffValue,minimumPreset));

			_OnChangedOptionList();

			return AddOptionResult.Success;
		}

		private AddOptionResult _TryGetOptionFlag(string name,out long flag)
		{
			var usedOrders = new HashSet<int>();

			for(var i=0;i<m_optionList.Count;i++)
			{
				var option = m_optionList[i];

				if(string.Equals(option.Name,name,StringComparison.Ordinal))
				{
					flag = -1L;

					return AddOptionResult.DuplicateName;
				}

				_CollectUsedOrder(option,usedOrders);
			}

			if(!_TryAllocateNextFlag(usedOrders,out flag))
			{
				return AddOptionResult.MaxCountReached;
			}

			return AddOptionResult.Success;
		}

		private static void _CollectUsedOrder(GraphicQualityInfo option,HashSet<int> usedOrders)
		{
			if(!option.Flag.IsSingleBitFlag())
			{
				return;
			}

			var order = option.Order;

			if(order >= 0 && order < c_maxOptionCount)
			{
				usedOrders.Add(order);
			}
		}

		private static bool _TryAllocateNextFlag(HashSet<int> usedOrders,out long flag)
		{
			// Scan bit orders and assign the first unused single-bit flag (1L << order).
			for(var order=0;order<c_maxOptionCount;order++)
			{
				if(usedOrders.Add(order))
				{
					flag = 1L << order;

					return true;
				}
			}

			flag = -1L;

			return false;
		}

		private bool m_isValidatingOptionList = false;

		private void OnValidate()
		{
			_ValidateOptionList();
		}

		private void _ValidateOptionList()
		{
			// Editor-only repair pass: drop invalid names, dedupe, and reassign corrupted flag bits.
			if(m_isValidatingOptionList)
			{
				return;
			}

			m_isValidatingOptionList = true;

			try
			{
				var removedMessages = new List<string>();
				var fixedMessages = new List<string>();
				var nameSet = new HashSet<string>(StringComparer.Ordinal);

				for(var i=m_optionList.Count-1;i>=0;i--)
				{
					var option = m_optionList[i];
					var name = option.Name;

					if(name.IsEmpty())
					{
						m_optionList.RemoveAt(i);
						removedMessages.Add("Removed option with empty name.");

						continue;
					}

					if(!nameSet.Add(name))
					{
						m_optionList.RemoveAt(i);
						removedMessages.Add($"Removed duplicate option name. [{name}]");
					}
				}

				var usedOrders = new HashSet<int>();

				for(var i=0;i<m_optionList.Count;i++)
				{
					var option = m_optionList[i];
					var needsReassign = !option.Flag.IsSingleBitFlag();

					if(!needsReassign)
					{
						var order = option.Order;

						if(order < 0 || order >= c_maxOptionCount || !usedOrders.Add(order))
						{
							needsReassign = true;
						}
					}

					if(!needsReassign)
					{
						continue;
					}

					if(!_TryAllocateNextFlag(usedOrders,out var newFlag))
					{
						var name = option.Name;

						m_optionList.RemoveAt(i);
						i--;
						removedMessages.Add($"Removed option. Max {c_maxOptionCount} flags. [{name}]");

						continue;
					}

					var oldFlag = option.Flag;

					option._EditorSetFlag(newFlag);
					fixedMessages.Add($"Reassigned flag. [{option.Name}] {oldFlag} -> {newFlag}");
				}

				if(removedMessages.Count == 0 && fixedMessages.Count == 0)
				{
					return;
				}

				EditorUtility.SetDirty(this);

				var message = string.Empty;

				if(removedMessages.Count > 0)
				{
					message += $"Removed:\n{string.Join("\n",removedMessages)}";
				}

				if(fixedMessages.Count > 0)
				{
					if(!message.IsEmpty())
					{
						message += "\n\n";
					}

					message += $"Fixed:\n{string.Join("\n",fixedMessages)}";
				}

				EditorUtility.DisplayDialog("GraphicQualityOption",message,"Ok");
			}
			finally
			{
				m_isValidatingOptionList = false;
			}
		}

		// Editor-only initial seed for a new asset. Runtime does not re-run this; the saved asset is the source of truth.
		protected override void OnCreate()
		{
			_TryAddOption(Global.GlobalTextureMipmapLimit,"0","1",GraphicQualityPresetType.QualityMiddle);
			_TryAddOption(Global.AnisotropicFiltering,"Enable","Disable",GraphicQualityPresetType.QualityHigh);
			_TryAddOption(Global.VerticalSyncCount,"1","0",GraphicQualityPresetType.QualityHighest);
			_TryAddOption(Global.DisableCameraFarHalf,"1.0","0.5",GraphicQualityPresetType.QualityHighest);
			_TryAddOption(Global.LodBias,"1.0","0.5",GraphicQualityPresetType.QualityMiddle);
			_TryAddOption(Global.MaximumLODLevel,"0","1",GraphicQualityPresetType.QualityLow);
			_TryAddOption(Global.AntiAliasing,"2","0",GraphicQualityPresetType.QualityHigh);
			_TryAddOption(Global.SkinWeights,"FourBones","TwoBones",GraphicQualityPresetType.QualityHigh);
			_TryAddOption(Global.ShadowDistance,"80","40",GraphicQualityPresetType.QualityMiddle);
			_TryAddOption(Global.RealtimeReflectionProbes,"1","0",GraphicQualityPresetType.QualityHigh);
		}
#endif

		[SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(_OnChangedOptionList))]
		private List<GraphicQualityInfo> m_optionList = new();

		private void _OnChangedOptionList()
		{
#if UNITY_EDITOR
			_ValidateOptionList();
#endif
			static int _Compare(GraphicQualityInfo infoA,GraphicQualityInfo infoB)
			{
				return infoA.Flag.CompareTo(infoB.Flag);
			}

			m_optionList.Sort(_Compare);
		}

		/// <summary>
		/// Builds a preset bitmask. Options whose MinimumPreset is met by presetType are turned ON.
		/// </summary>
		public long GetGraphicQualityInPreset(GraphicQualityPresetType presetType)
		{
			var graphicsQuality = 0L;

			for(var i=0;i<m_optionList.Count;i++)
			{
				var option = m_optionList[i];

				if(_IsAtLeast(presetType,option.MinimumPreset))
				{
					graphicsQuality = graphicsQuality.AddFlag(option.Flag);
				}
			}

			return graphicsQuality;
		}

		#region Query
		/// <summary>Returns the raw On/Off string for an option. Throws when the option name is missing.</summary>
		public string FindValue(long graphicQuality,string optionName)
		{
			if(TryFindValue(graphicQuality,optionName,out var value))
			{
				return value;
			}

			throw new InvalidOperationException($"Graphic quality option not found. [{optionName}]");
		}

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

		private bool _TryFindOption(string optionName,out GraphicQualityInfo option)
		{
			for(var i=0;i<m_optionList.Count;i++)
			{
				option = m_optionList[i];

				if(string.Equals(option.Name,optionName,StringComparison.Ordinal))
				{
					return true;
				}
			}

			option = null;

			return false;
		}

		/// <summary>
		/// Returns a typed option value. Parsing follows TValue, not option name.
		/// Throws InvalidOperationException (missing/empty) or FormatException (parse failure).
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

		/// <summary>Parses a catalog string into TValue. Caller type selects parsing rules.</summary>
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

		// Preset rank is explicit so enum declaration order can change safely.
		private bool _IsAtLeast(GraphicQualityPresetType presetType,GraphicQualityPresetType minimumPreset)
		{
			return _ToPresetRank(presetType) >= _ToPresetRank(minimumPreset);
		}

		private int _ToPresetRank(GraphicQualityPresetType presetType)
		{
			return presetType switch
			{
				GraphicQualityPresetType.QualityLowest => 0,
				GraphicQualityPresetType.QualityLow => 1,
				GraphicQualityPresetType.QualityMiddle => 2,
				GraphicQualityPresetType.QualityHigh => 3,
				GraphicQualityPresetType.QualityHighest => 4,

				_ => throw new ArgumentOutOfRangeException(nameof(presetType),presetType,null),
			};
		}
	}
}