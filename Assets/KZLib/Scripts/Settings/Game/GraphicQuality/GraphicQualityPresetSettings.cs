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

		[HorizontalGroup("0")]
		[HorizontalGroup("0/0",Order = 0),ShowInInspector,HideLabel,KZRichText]
		public string Name => string.Format("{0} : {1}",Mathf.Log(m_QualityOption,2),m_Tag);

		public long QualityOption => m_QualityOption;

		public OptionData(GraphicQualityTag _tag)
		{
			m_QualityOption = _tag.QualityOption;
			m_Tag = _tag.ToString();
		}
	}

	[HorizontalGroup("버튼",Order = 0),Button("새로 고침",ButtonSizes.Large)]
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

	[HorizontalGroup("프리셋",Order = 1)]
	[HorizontalGroup("프리셋/0",Order = 0),LabelText("가장 낮은 옵션 리스트"),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(OnChangedPreset))]
	private List<OptionData> m_QualityLowestPresetList = new();
	[HorizontalGroup("프리셋/0",Order = 0),LabelText("낮은  옵션 리스트"),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(OnChangedPreset))]
	private List<OptionData> m_QualityLowPresetList = new();
	[HorizontalGroup("프리셋/0",Order = 0),LabelText("중간 옵션 리스트"),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(OnChangedPreset))]
	private List<OptionData> m_QualityMiddlePresetList = new();
	[HorizontalGroup("프리셋/0",Order = 0),LabelText("높은 옵션 리스트"),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(OnChangedPreset))]
	private List<OptionData> m_QualityHighPresetList = new();
	[HorizontalGroup("프리셋/0",Order = 0),LabelText("가장 높은 옵션 리스트"),SerializeField,ListDrawerSettings(ShowFoldout = false,HideAddButton = true),OnValueChanged(nameof(OnChangedPreset))]
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