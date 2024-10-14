using System;
using System.Collections.Generic;
using KZLib;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using UnityEngine;

public class GraphicQualityPresetSettings : InnerBaseSettings<GraphicQualityPresetSettings>
{
	[Serializable]
	private class OptionData
	{
		[SerializeField,HideInInspector]
		private long m_QualityOption = 0L;
		[SerializeField,HideInInspector]
		private string m_Tag = null;

		[HorizontalGroup("Name",Order = 0),ShowInInspector,HideLabel,KZRichText]
		public string Name => $"{Mathf.Log(m_QualityOption,2)} : {m_Tag}";

		public long QualityOption => m_QualityOption;

		public OptionData(GraphicQualityTag _tag)
		{
			m_QualityOption = _tag.QualityOption;
			m_Tag = _tag.ToString();
		}
	}

	[HorizontalGroup("Button",Order = 0),Button("Refresh",ButtonSizes.Large)]
	private void OnRefresh()
	{
		var tagList = new List<GraphicQualityTag>(Enumeration.GetEnumerationGroup<GraphicQualityTag>(true));

		foreach(var optionList in QualityPresetDicts.Values)
		{
			foreach(var option in optionList)
			{
				var tag = tagList.Find(x=>x.QualityOption == option.QualityOption);

				if(tag != null)
				{
					tagList.Remove(tag);
				}
			}
		}

		foreach(var tag in tagList)
		{
			QualityPresetDicts[GraphicsQualityPresetType.QualityLowest].Add(new OptionData(tag));
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

	private Dictionary<GraphicsQualityPresetType,List<OptionData>> m_QualityPresetDicts = null;

	private Dictionary<GraphicsQualityPresetType,List<OptionData>> QualityPresetDicts
	{
		get
		{
			m_QualityPresetDicts ??= new Dictionary<GraphicsQualityPresetType,List<OptionData>>
			{
				{ GraphicsQualityPresetType.QualityLowest,	m_QualityLowestPresetList },
				{ GraphicsQualityPresetType.QualityLow,		m_QualityLowPresetList },
				{ GraphicsQualityPresetType.QualityMiddle,	m_QualityMiddlePresetList },
				{ GraphicsQualityPresetType.QualityHigh,	m_QualityHighPresetList },
				{ GraphicsQualityPresetType.QualityHighest,	m_QualityHighestPresetList },
			};

			return m_QualityPresetDicts;
		}
	}

	[SerializeField,HideInInspector]
	private Dictionary<GraphicsQualityPresetType,long> m_QualityDict = null;

	private Dictionary<GraphicsQualityPresetType,long> QualityDict
	{
		get
		{
			m_QualityDict ??= new Dictionary<GraphicsQualityPresetType,long>
			{
				{ GraphicsQualityPresetType.QualityLowest,	0L },
				{ GraphicsQualityPresetType.QualityLow,		0L },
				{ GraphicsQualityPresetType.QualityMiddle,	0L },
				{ GraphicsQualityPresetType.QualityHigh,	0L },
				{ GraphicsQualityPresetType.QualityHighest,	0L },
			};

			return m_QualityDict;
		}
	}

	private void OnChangedPreset()
	{
		var quality = 0L;

		foreach(var pair in m_QualityPresetDicts)
		{
			foreach(var preset in pair.Value)
			{
				quality = quality.AddFlag(preset.QualityOption);
			}

			pair.Value.Sort((x,y)=>x.QualityOption.CompareTo(y.QualityOption));

			QualityDict[pair.Key] = quality;
		}
	}

	public long GetPresetQuality(GraphicsQualityPresetType _type) => QualityDict[_type];
}