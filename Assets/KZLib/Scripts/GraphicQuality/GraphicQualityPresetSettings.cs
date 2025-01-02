using System;
using System.Collections.Generic;
using KZLib.KZAttribute;
using KZLib.KZUtility;
using Sirenix.OdinInspector;
using UnityEngine;

public class GraphicQualityPresetSettings : InnerBaseSettings<GraphicQualityPresetSettings>
{
	[Serializable]
	private class OptionData
	{
		[SerializeField,HideInInspector]
		private long m_qualityOption = 0L;
		[SerializeField,HideInInspector]
		private string m_qualityType = null;

		[HorizontalGroup("Name",Order = 0),ShowInInspector,HideLabel,KZRichText]
		public string Name => $"{Mathf.Log(m_qualityOption,2)} : {m_qualityType}";

		public long QualityOption => m_qualityOption;

		public OptionData(GraphicQualityType qualityType)
		{
			m_qualityOption = qualityType.QualityOption;
			m_qualityType = qualityType.ToString();
		}
	}

	[HorizontalGroup("Button",Order = 0),Button("Refresh",ButtonSizes.Large)]
	private void OnRefresh()
	{
		var qualityTypeList = new List<GraphicQualityType>(CustomTag.GetCustomTypeGroup<GraphicQualityType>());

		foreach(var optionList in QualityPresetDict.Values)
		{
			foreach(var option in optionList)
			{
				var qualityType = qualityTypeList.Find(x=>x.QualityOption == option.QualityOption);

				if(qualityType != null)
				{
					qualityTypeList.Remove(qualityType);
				}
			}
		}

		foreach(var qualityType in qualityTypeList)
		{
			QualityPresetDict[GraphicsQualityPresetType.QualityLowest].Add(new OptionData(qualityType));
		}

		OnChangedPreset();
	}

	[HorizontalGroup("Preset",Order = 1)]
	[HorizontalGroup("Preset/0",Order = 0),LabelText("Lowest Preset List"),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(OnChangedPreset))]
	private List<OptionData> m_QualityLowestPresetList = new();
	[HorizontalGroup("Preset/0",Order = 0),LabelText("Low Preset List"),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(OnChangedPreset))]
	private List<OptionData> m_QualityLowPresetList = new();
	[HorizontalGroup("Preset/0",Order = 0),LabelText("Middle Preset List"),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(OnChangedPreset))]
	private List<OptionData> m_QualityMiddlePresetList = new();
	[HorizontalGroup("Preset/0",Order = 0),LabelText("High Preset List"),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(OnChangedPreset))]
	private List<OptionData> m_QualityHighPresetList = new();
	[HorizontalGroup("Preset/0",Order = 0),LabelText("Highest Preset List"),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(OnChangedPreset))]
	private List<OptionData> m_QualityHighestPresetList = new();

	private Dictionary<GraphicsQualityPresetType,List<OptionData>> m_qualityPresetDict = null;

	private Dictionary<GraphicsQualityPresetType,List<OptionData>> QualityPresetDict
	{
		get
		{
			m_qualityPresetDict ??= new Dictionary<GraphicsQualityPresetType,List<OptionData>>
			{
				{ GraphicsQualityPresetType.QualityLowest,	m_QualityLowestPresetList },
				{ GraphicsQualityPresetType.QualityLow,		m_QualityLowPresetList },
				{ GraphicsQualityPresetType.QualityMiddle,	m_QualityMiddlePresetList },
				{ GraphicsQualityPresetType.QualityHigh,	m_QualityHighPresetList },
				{ GraphicsQualityPresetType.QualityHighest,	m_QualityHighestPresetList },
			};

			return m_qualityPresetDict;
		}
	}

	[SerializeField,HideInInspector]
	private Dictionary<GraphicsQualityPresetType,long> m_qualityDict = null;

	private Dictionary<GraphicsQualityPresetType,long> QualityDict
	{
		get
		{
			m_qualityDict ??= new Dictionary<GraphicsQualityPresetType,long>
			{
				{ GraphicsQualityPresetType.QualityLowest,	0L },
				{ GraphicsQualityPresetType.QualityLow,		0L },
				{ GraphicsQualityPresetType.QualityMiddle,	0L },
				{ GraphicsQualityPresetType.QualityHigh,	0L },
				{ GraphicsQualityPresetType.QualityHighest,	0L },
			};

			return m_qualityDict;
		}
	}

	private void OnChangedPreset()
	{
		var quality = 0L;

		foreach(var pair in m_qualityPresetDict)
		{
			foreach(var preset in pair.Value)
			{
				quality = quality.AddFlag(preset.QualityOption);
			}

			pair.Value.Sort((x,y)=>x.QualityOption.CompareTo(y.QualityOption));

			QualityDict[pair.Key] = quality;
		}
	}

	public long GetPresetQuality(GraphicsQualityPresetType presetType) => QualityDict[presetType];
}