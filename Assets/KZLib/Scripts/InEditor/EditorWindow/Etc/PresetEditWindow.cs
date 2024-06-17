#if UNITY_EDITOR
using System.Collections.Generic;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor.Presets;
using UnityEngine;

namespace KZLib.KZWindow
{
	public class PresetEditWindow : OdinEditorWindow
	{
		private const int TOGGLE_ORDER = 0;
		private const int PRESET_ORDER = 1;

		private enum ShowType { All, OnlyValid, OnlyInvalid, }

		[SerializeField,HideInInspector]
		private ShowType m_CurrentType;

		[HorizontalGroup("Toggle",Order = TOGGLE_ORDER),HideLabel,EnumToggleButtons,ShowInInspector]
		private ShowType CurrentType
		{
			get => m_CurrentType;
			set
			{
				m_CurrentType = value;
				m_IsExistPreset = true;

				m_PresetList.Clear();

				m_PresetList.AddRange(CommonUtility.LoadAssetGroup<Preset>("t:preset"));

				if(m_PresetList.IsNullOrEmpty())
				{
					m_IsExistPreset = false;

					return;
				}

				switch(m_CurrentType)
				{
					case ShowType.OnlyValid:
						m_PresetList.RemoveAll(x=>!x.IsValid());
						break;
					case ShowType.OnlyInvalid:
						m_PresetList.RemoveAll(x=>x.IsValid());
						break;
				}

				if(m_PresetList.IsNullOrEmpty())
				{
					m_IsExistPreset = false;
				}
			}
		}

		[HorizontalGroup("Preset",Order = PRESET_ORDER),ShowInInspector,HideLabel,KZRichText,HideIf(nameof(m_IsExistPreset))]
		private string Exist_Display => "존재하지 않습니다.";

		[HorizontalGroup("Preset",Order = PRESET_ORDER),SerializeField,LabelText("프리셋 리스트"),ListDrawerSettings(ShowFoldout = false),ShowIf(nameof(m_IsExistPreset)),Searchable]
		private List<Preset> m_PresetList = new();

#pragma warning disable 0414
		private bool m_IsExistPreset = false;
#pragma warning restore 0414

		protected override void Initialize()
		{
			CurrentType = ShowType.All;
		}
	}
}
#endif