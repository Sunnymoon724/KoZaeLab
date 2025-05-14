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
		#region Option Handler
		private class OptionHandler
		{
			private string m_optionName = null;
			private string m_enableValue = null;
			private string m_disableValue = null;

			[HorizontalGroup("0",Order = 0),ShowInInspector]
			private string OptionName
			{
				get => m_optionName;
				set
				{
					ErrorText = string.Empty;
					m_optionName = value;
				}
			}

			[HorizontalGroup("1",Order = 1),ShowInInspector]
			private string EnableValue
			{
				get => m_enableValue;
				set
				{
					ErrorText = string.Empty;
					m_enableValue = value;
				}
			}
			
			[HorizontalGroup("2",Order = 2),ShowInInspector]
			private string DisableValue
			{
				get => m_disableValue;
				set
				{
					ErrorText = string.Empty;
					m_disableValue = value;
				}
			}

			[HorizontalGroup("4",Order = 4),ShowInInspector,KZRichText,HideLabel]
			protected string ErrorText { get; private set; }

			private readonly Func<string,string,string,bool> m_onAddOption = null;

			private bool IsValidAddOption => !OptionName.IsEmpty() && !EnableValue.IsEmpty() && !DisableValue.IsEmpty();

			public OptionHandler(Func<string,string,string,bool> onAddOption)
			{
				m_onAddOption = onAddOption;
			}

#if UNITY_EDITOR
			private OdinEditorWindow m_optionWindow = null;

			[HorizontalGroup("5",Order = 5),Button("Add",ButtonSizes.Large),EnableIf(nameof(IsValidAddOption))]
			protected void OnAddOption()
			{
				var result = m_onAddOption.Invoke(OptionName,EnableValue,DisableValue);

				if(result)
				{
					m_optionWindow.Close();
				}
				else
				{
					ErrorText = "<color=red>already exist.</color>";
				}
			}

			public void SetWindow(OdinEditorWindow window)
			{
				m_optionWindow = window;
			}
#endif
		}
		#endregion Option Window
		
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
			protected string FlagName => $"{Order} : {m_name} [{m_enableValue}/{m_disableValue}]";

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
			var handler = new OptionHandler(_TryAddOption);
			var window = OdinEditorWindow.InspectObject(handler);

			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(250,150);

			handler.SetWindow(window);
		}

		private bool _TryAddOption(string optionName,string enableValue,string disableValue)
		{
			if(!_TryGetOptionFlag(optionName,out var flag))
			{
				return false;
			}

			PresetDict[GraphicQualityPresetType.QualityLowest].Add(new GraphicQualityData(optionName,flag,enableValue,disableValue));

			_OnChangedPreset();

			return true;
		}

		private bool _TryGetOptionFlag(string name,out long flag)
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

			while(orderHashSet.Contains(order))
			{
				order++;
			}

			flag = 1L << order;

			return true;
		}
#endif

		[HorizontalGroup("Preset",Order = 1)]
		[HorizontalGroup("Preset/0",Order = 0),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(_OnChangedPreset))]
		private List<GraphicQualityData> m_lowestPresetList = new();
		[HorizontalGroup("Preset/0",Order = 0),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(_OnChangedPreset))]
		private List<GraphicQualityData> m_lowPresetList = new();
		[HorizontalGroup("Preset/0",Order = 0),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(_OnChangedPreset))]
		private List<GraphicQualityData> m_middlePresetList = new();
		[HorizontalGroup("Preset/0",Order = 0),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(_OnChangedPreset))]
		private List<GraphicQualityData> m_highPresetList = new();
		[HorizontalGroup("Preset/0",Order = 0),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(_OnChangedPreset))]
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

		private void _OnChangedPreset()
		{
			var graphicsQuality = 0L;

			foreach(var presetPair in PresetDict)
			{
				foreach(var preset in presetPair.Value)
				{
					graphicsQuality = graphicsQuality.AddFlag(preset.Flag);
				}

				presetPair.Value.Sort((x,y)=>x.Flag.CompareTo(y.Flag));
			}
		}

		public long GetGraphicQualityInPreset(GraphicQualityPresetType presetType)
		{
			var graphicsQuality = 0L;
			var presetList = PresetDict[presetType];

			for(var i=0;i<presetList.Count;i++)
			{
				graphicsQuality = graphicsQuality.AddFlag(presetList[i].Flag);
			}

			return graphicsQuality;
		}

#if UNITY_EDITOR
		protected override void OnCreate()
		{
			_TryAddOption(Global.GLOBAL_TEXTURE_MIPMAP_LIMIT,"0","1");
			_TryAddOption(Global.ANISOTROPIC_FILTERING,"Enable","Disable");
			_TryAddOption(Global.VERTICAL_SYNC_COUNT,"1","0");
			_TryAddOption(Global.DISABLE_CAMERA_FAR_HALF,"1.0","0.5");
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

			KZLogType.System.E($"Not supported GraphicsQuality. [{optionName}]");

			return null;
		} 
	}
}