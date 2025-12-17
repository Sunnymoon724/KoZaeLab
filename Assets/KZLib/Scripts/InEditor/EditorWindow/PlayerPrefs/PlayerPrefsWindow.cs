#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using System.Collections.Generic;
using System;
using KZLib.KZAttribute;
using UnityEditor;
using Sirenix.Utilities.Editor;
using Microsoft.Win32;

namespace KZLib.KZWindow
{
	public class PlayerPrefsWindow : OdinEditorWindow
	{
		#region PlayerPrefsInfo
		[Serializable]
		private record PlayerPrefsInfo
		{
			[SerializeField,HideInInspector]
			private string m_value = null;

			[TitleGroup("$m_key",BoldTitle = false),HideLabel,ShowInInspector,MultiLineProperty(3)]
			public string Value
			{
				get => m_value;
				private set
				{
					if(m_value.IsEqual(value))
					{
						return;
					}

					PlayerPrefsManager.In.SetString(m_key,value);

					m_value = value;
				}
			}

			private readonly string m_key = null;

			public string Key => m_key;

			public PlayerPrefsInfo(string key,string value)
			{
				m_key = key;
				m_value = value;
			}
		}
		#endregion PlayerPrefsInfo

		[VerticalGroup("1",Order = 1),LabelText("Info List"),SerializeField,ListDrawerSettings(ShowFoldout = false,DraggableItems = false,HideAddButton = true,CustomRemoveIndexFunction = nameof(_OnRemoveInfo),OnTitleBarGUI = nameof(_OnRefreshInfo)),ShowIf(nameof(IsExistInfo))]
		private List<PlayerPrefsInfo> m_playerPrefsInfoList = new();

		[VerticalGroup("1",Order = 1),HideLabel,ShowInInspector,HideIf(nameof(IsExistInfo)),KZRichText]
		protected string InfoText => "PlayerPrefs is empty";

		private bool IsExistInfo => m_playerPrefsInfoList.Count > 0;

		protected override void Initialize()
		{
			base.Initialize();

			_LoadPlayerPrefsInfo();
		}

		private void _OnRemoveInfo(int index)
		{
			if(!CommonUtility.DisplayCheck("Remove playerPrefs","Are you sure?"))
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
			if(SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
			{
				_LoadPlayerPrefsInfo();
			}
		}

		[VerticalGroup("2",Order = 2),Button("Delete All",ButtonHeight = 30),ShowIf(nameof(IsExistInfo))]
		protected void OnDeleteAll()
		{
			if(!CommonUtility.DisplayCheck("Delete all playerPrefs","Are you sure?"))
			{
				return;
			}

			PlayerPrefsManager.In.Clear();

			LogSvc.Editor.I($"PlayerPrefs is deleted");

			m_playerPrefsInfoList.Clear();
		}

		private void _LoadPlayerPrefsInfo()
		{
			var keyArray = _LoadPlayerPrefsKey(PlayerSettings.companyName,PlayerSettings.productName);

			m_playerPrefsInfoList.Clear();

			foreach(var key in keyArray)
			{
				if(!_TryGetPlayerValue(key,out var value))
				{
					continue;
				}

				m_playerPrefsInfoList.Add(new PlayerPrefsInfo(key,value));
			}
		}

		private bool _TryGetPlayerValue(string key,out string value)
		{
			value = string.Empty;

			if(!PlayerPrefs.HasKey(key))
			{
				return false;
			}

			var intValue = PlayerPrefs.GetInt(key,0);

			if(intValue != 0)
			{
				value = $"{intValue}";

				return true;
			}

			var floatValue = PlayerPrefs.GetFloat(key,0.0f);

			if(floatValue != 0.0f)
			{
				value = $"{floatValue}";

				return true;
			}

			var stringValue = PlayerPrefs.GetString(key,"");

			if(!stringValue.IsEmpty())
			{
				value = stringValue;

				return true;
			}

			PlayerPrefs.DeleteKey(key);

			return false;
		}

		private static string[] _LoadPlayerPrefsKey(string companyName,string productName)
		{
			var resultList = new List<string>();
#if UNITY_EDITOR_WIN
			var prefsPath = @$"SOFTWARE\Unity\UnityEditor\{companyName}\{productName}";

			using var registryKey = Registry.CurrentUser.OpenSubKey(prefsPath);

			if(registryKey != null)
			{
				foreach(var key in registryKey.GetValueNames())
				{
					if(key.StartsWith("unity.") || key.StartsWith("UnityGraphicsQuality"))
					{
						continue;
					}

					resultList.Add(key[..key.LastIndexOf("_h",StringComparison.Ordinal)]);
				}
			}
#elif UNITY_EDITOR_OSX
			var prefsPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),"Library/Preferences"),$"unity.{companyName}.{productName}.plist");
			var dictionary = PropertyListParser.Parse(new FileInfo(prefsPath)) as NSDictionary ?? throw new InvalidOperationException($"{prefsPath} is not valid");

			foreach(var pair in dictionary)
			{
				resultList.Add(pair.Key);
			}
#endif
			return resultList.ToArray();
		}
	}
}
#endif