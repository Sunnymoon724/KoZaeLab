#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using System.Collections.Generic;
using System;
using KZLib.KZAttribute;
using KZLib.KZTool;
using UnityEditor;
using Sirenix.Utilities.Editor;

namespace KZLib.KZWindow
{
	public class PlayerPrefsWindow : OdinEditorWindow
	{
		private enum PlayerPrefsType { None, String, Int, Float, }

		#region PlayerPrefsInfo
		[Serializable]
		private record PlayerPrefsInfo
		{
			[SerializeField,HideInInspector]
			private string m_value = null;

			[TitleGroup("$m_key",BoldTitle = false,Subtitle = "$m_type"),HideLabel,ShowInInspector,MultiLineProperty(3)]
			public string Value
			{
				get => m_value;
				private set
				{
					if(m_value.IsEqual(value))
					{
						return;
					}

					switch(m_type)
					{
						case PlayerPrefsType.String:
							PlayerPrefsManager.In.SetString(m_key,value);
							break;
						case PlayerPrefsType.Int:
							if(int.TryParse(value,out var intNumber))
							{
								PlayerPrefsManager.In.SetInt(m_key,intNumber);
							}
							break;
						case PlayerPrefsType.Float:
							if(float.TryParse(value,out var floatNumber))
							{
								PlayerPrefsManager.In.SetFloat(m_key,floatNumber);
							}
							break;
						case PlayerPrefsType.None:
						default:
							break;
					}

					m_value = value;
				}
			}

			private readonly string m_key = string.Empty;
			private readonly PlayerPrefsType m_type = PlayerPrefsType.None;

			public string Key => m_key;

			public PlayerPrefsInfo(string key,PlayerPrefsType type,string value)
			{
				m_key = key;
				m_type = type;
				m_value = value;
			}
		}
		#endregion PlayerPrefsInfo

		[VerticalGroup("1",Order = 1),LabelText("Info List"),SerializeField,ListDrawerSettings(ShowFoldout = false,DraggableItems = false,HideAddButton = true,CustomRemoveIndexFunction = nameof(_OnRemoveInfo),OnTitleBarGUI = nameof(_OnRefreshInfo)),ShowIf(nameof(IsExistInfo)),Searchable]
		private List<PlayerPrefsInfo> m_playerPrefsInfoList = new();

		[VerticalGroup("1",Order = 1),HideLabel,ShowInInspector,HideIf(nameof(IsExistInfo)),KZRichText]
		protected string InfoText => "PlayerPrefs is empty";

		private bool IsExistInfo => m_playerPrefsInfoList.Count > 0;
		
		private bool m_ascending = true;
		private bool m_isShowSystem = false;

		protected override void Initialize()
		{
			base.Initialize();

			_LoadPlayerPrefsInfo();

			_SortInfoList();
		}

		private void _OnRemoveInfo(int index)
		{
			if(!CommonUtility.DisplayCheckBeforeExecute("Remove this playerPrefs"))
			{
				return;
			}

			var info = m_playerPrefsInfoList[index];

			PlayerPrefsManager.In.RemoveKey(info.Key);

			m_playerPrefsInfoList.Remove(info);

			LogSvc.Editor.I($"{info.Key} is removed");
		}

		private void _OnRefreshInfo()
		{
			if(m_ascending)
			{
				if(SirenixEditorGUI.ToolbarButton(SdfIconType.CaretDownFill))
				{
					m_ascending = false;
					_SortInfoList();
				}
			}
			else
			{
				if(SirenixEditorGUI.ToolbarButton(SdfIconType.CaretUpFill))
				{
					m_ascending = true;
					_SortInfoList();
				}
			}

			if(SirenixEditorGUI.ToolbarButton(SdfIconType.ArrowRepeat))
			{
				_LoadPlayerPrefsInfo();
			}
			
			if(m_isShowSystem)
			{
				if(SirenixEditorGUI.ToolbarButton(SdfIconType.EyeSlashFill))
				{
					m_isShowSystem = false;
					_LoadPlayerPrefsInfo();
				}
			}
			else
			{
				if(SirenixEditorGUI.ToolbarButton(SdfIconType.EyeFill))
				{
					m_isShowSystem = true;
					_LoadPlayerPrefsInfo();
				}
			}
		}

		[VerticalGroup("2",Order = 2),Button("Delete All",ButtonHeight = 30),ShowIf(nameof(IsExistInfo))]
		protected void OnDeleteAll()
		{
			if(!CommonUtility.DisplayCheckBeforeExecute("Delete all playerPrefs"))
			{
				return;
			}

			PlayerPrefsManager.In.Clear();

			LogSvc.Editor.I($"PlayerPrefs is deleted");

			m_playerPrefsInfoList.Clear();
		}

		private void _LoadPlayerPrefsInfo()
		{
			var keyArray = PlayerPrefsReader.LoadPlayerPrefsKeyArray(PlayerSettings.companyName,PlayerSettings.productName);

			m_playerPrefsInfoList.Clear();

			for(var i=0;i<keyArray.Length;i++)
			{
				var key = keyArray[i];

				if(!m_isShowSystem)
				{
					if(key.StartsWith("unity.") || key.Equals("UnityGraphicsQuality") || key.Equals("AddressablesRuntimeDataPath"))
					{
						continue;
					}
				}

				if(!_TryGetPlayerValue(key,out var value,out var playerPrefsType))
				{
					continue;
				}

				m_playerPrefsInfoList.Add(new PlayerPrefsInfo(key,playerPrefsType,value));
			}

			_SortInfoList();
		}

		private bool _TryGetPlayerValue(string key,out string value,out PlayerPrefsType playerPrefsType)
		{
			value = string.Empty;
			playerPrefsType = PlayerPrefsType.None;

			if(PlayerPrefs.HasKey(key))
			{
				var intValue = PlayerPrefs.GetInt(key,int.MinValue);

				if(intValue != int.MinValue)
				{
					value = $"{intValue}";
					playerPrefsType = PlayerPrefsType.Int;

					return true;
				}

				var floatValue = PlayerPrefs.GetFloat(key,float.MinValue);

				if(floatValue != float.MinValue)
				{
					value = $"{floatValue}";
					playerPrefsType = PlayerPrefsType.Float;

					return true;
				}

				var stringValue = PlayerPrefs.GetString(key,string.Empty);

				if(!stringValue.IsEmpty())
				{
					value = stringValue;
					playerPrefsType = PlayerPrefsType.String;

					return true;
				}

				PlayerPrefs.DeleteKey(key);
			}

			return false;
		}

		private void _SortInfoList()
		{
			if(m_ascending)
			{
				static int _Compare(PlayerPrefsInfo infoA,PlayerPrefsInfo infoB)
				{
					return string.Compare(infoA.Key,infoB.Key,StringComparison.Ordinal);
				}

				m_playerPrefsInfoList.Sort(_Compare);
			}
			else
			{
				static int _Compare(PlayerPrefsInfo infoA,PlayerPrefsInfo infoB)
				{
					return string.Compare(infoB.Key,infoA.Key,StringComparison.Ordinal);
				}

				m_playerPrefsInfoList.Sort(_Compare);
			}
		}
	}
}
#endif