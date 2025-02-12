using System;
using System.Collections.Generic;
using KZLib.KZUtility;
using Sirenix.OdinInspector;
using UnityEngine;
using KZLib.KZAttribute;

#if UNITY_EDITOR

using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;

#endif

namespace KZLib.KZDevelop
{
	public class GraphicQualityOption : SingletonSO<GraphicQualityOption>
	{
		#region Option Data
		private class OptionData
		{
			[SerializeField]
			private string m_optionName = null;
			[SerializeField]
			private string m_enableValue = null;
			[SerializeField]
			private string m_disableValue = null;

			private readonly Action<string,string,string> m_onAddOption = null;

			public OptionData(Action<string,string,string> onAddOption)
			{
				m_onAddOption = onAddOption;
			}

			[Button("Add",ButtonSizes.Large)]
			protected void OnAddOption()
			{
				m_onAddOption?.Invoke(m_optionName,m_enableValue,m_disableValue);
			}
		}
		#endregion Option Data
		
		#region Graphic Quality Data
		[Serializable]
		public class GraphicQualityData
		{
			[SerializeField,HideInInspector]
			private string m_name = null;

			[SerializeField,HideInInspector]
			private long m_flag = 0L;

			[SerializeField,HideInInspector]
			private string m_enableValue = null;

			[SerializeField,HideInInspector]
			private string m_disableValue = null;

			public GraphicQualityData(string name,long flag,string enableValue,string disableValue)
			{
				m_name = name;
				m_flag = flag;
				m_enableValue = enableValue;
				m_disableValue = disableValue;
			}

			public long Flag => m_flag;
			public int Order => (int) Mathf.Log(m_flag,2);

			[ShowInInspector,HideLabel,KZRichText]
			protected string FlagName => $"{Order} : {m_name}";

			public string Name => m_name;

			public string GetValue(long graphicQuality)
			{
				return graphicQuality.HasFlag(m_flag) ? m_enableValue : m_disableValue;
			}
		}
		#endregion Graphic Quality Data

	#if UNITY_EDITOR
		[HorizontalGroup("Button",Order = 0),Button("Add Option",ButtonSizes.Large)]
		protected void OnAddOption()
		{
			var window = OdinEditorWindow.InspectObject(new OptionData(AddOption));
			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(250,100);
		}

		private void AddOption(string name,string enableValue,string disableValue)
		{
			if(!TryGetOptionFlag(name,out var flag))
			{
				return;
			}

			PresetDict[GraphicQualityPresetType.QualityLowest].Add(new GraphicQualityData(name,flag,enableValue,disableValue));

			OnChangedPreset();
		}
	#endif

		private bool TryGetOptionFlag(string name,out long flag)
		{
			var nameHashSet = new HashSet<string>();
			var orderHashSet = new HashSet<long>();

			foreach(var qualityList in PresetDict.Values)
			{
				foreach(var quality in qualityList)
				{
					nameHashSet.Add(quality.Name);
					orderHashSet.Add(quality.Order);
				}
			}

			if(nameHashSet.Count == 0)
			{
				flag = 1L << 0;

				return true;
			}

			if(nameHashSet.Contains(name))
			{
				flag = -1L;

				return false;
			}

			var order = 0;

			flag = 0L;

			while(orderHashSet.Contains(order))
			{
				order++;
			}

			flag = 1L << order;

			return true;
		}

		[HorizontalGroup("Preset",Order = 1)]
		[HorizontalGroup("Preset/0",Order = 0),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(OnChangedPreset))]
		private List<GraphicQualityData> m_lowestPresetList = new();
		[HorizontalGroup("Preset/0",Order = 0),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(OnChangedPreset))]
		private List<GraphicQualityData> m_lowPresetList = new();
		[HorizontalGroup("Preset/0",Order = 0),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(OnChangedPreset))]
		private List<GraphicQualityData> m_middlePresetList = new();
		[HorizontalGroup("Preset/0",Order = 0),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(OnChangedPreset))]
		private List<GraphicQualityData> m_highPresetList = new();
		[HorizontalGroup("Preset/0",Order = 0),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(OnChangedPreset))]
		private List<GraphicQualityData> m_highestPresetList = new();

		private Dictionary<GraphicQualityPresetType,List<GraphicQualityData>> m_presetDict = null;

		private Dictionary<GraphicQualityPresetType,List<GraphicQualityData>> PresetDict
		{
			get
			{
				m_presetDict ??= new Dictionary<GraphicQualityPresetType,List<GraphicQualityData>>
				{
					{ GraphicQualityPresetType.QualityLowest,	m_lowestPresetList	},
					{ GraphicQualityPresetType.QualityLow,		m_lowPresetList		},
					{ GraphicQualityPresetType.QualityMiddle,	m_middlePresetList	},
					{ GraphicQualityPresetType.QualityHigh,		m_highPresetList	},
					{ GraphicQualityPresetType.QualityHighest,	m_highestPresetList },
				};

				return m_presetDict;
			}
		}

		[SerializeField,HideInInspector]
		private Dictionary<GraphicQualityPresetType,long> m_graphicQualityDict = null;

		private void OnChangedPreset()
		{
			var graphicsQuality = 0L;

			foreach(var presetPair in PresetDict)
			{
				foreach(var preset in presetPair.Value)
				{
					graphicsQuality = graphicsQuality.AddFlag(preset.Flag);
				}

				presetPair.Value.Sort((x,y)=>x.Flag.CompareTo(y.Flag));

				m_graphicQualityDict[presetPair.Key] = graphicsQuality;
			}
		}

		public long GetGraphicQualityInPreset(GraphicQualityPresetType presetType) => m_graphicQualityDict[presetType];

	#if UNITY_EDITOR
		protected override void OnCreate()
		{
			m_graphicQualityDict = new Dictionary<GraphicQualityPresetType,long>
			{
				{ GraphicQualityPresetType.QualityLowest,	0L },
				{ GraphicQualityPresetType.QualityLow,		0L },
				{ GraphicQualityPresetType.QualityMiddle,	0L },
				{ GraphicQualityPresetType.QualityHigh,		0L },
				{ GraphicQualityPresetType.QualityHighest,	0L },
			};

			AddOption(Global.GLOBAL_TEXTURE_MIPMAP_LIMIT,"0","1");
			AddOption(Global.ANISOTROPIC_FILTERING,"Enable","Disable");
			AddOption(Global.VERTICAL_SYNC_COUNT,"1","0");
			AddOption(Global.DISABLE_CAMERA_FAR_HALF,"1.0f","0.5f");
		}
	#endif

		public string FindValue(long graphicQuality,string optionName)
		{
			foreach(var qualityList in PresetDict.Values)
			{
				foreach(var quality in qualityList)
				{
					if(string.Equals(quality.Name,optionName))
					{
						return quality.GetValue(graphicQuality);
					}
				}
			}

			LogTag.System.E($"Not supported GraphicsQuality. [{optionName}]");

			return null;
		} 
	}
}