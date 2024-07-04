#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using System.Collections.Generic;
using KZLib.KZEditor;
using UnityEditor;

namespace KZLib.KZWindow
{
	public class SettingsWindow : OdinMenuEditorWindow
	{
		protected List<IMetaDataTable> m_TableList = new();

		private readonly HierarchyCustom m_HierarchyCustom = new();

		protected override OdinMenuTree BuildMenuTree()
		{
			m_TableList.Clear();

			var tree = new OdinMenuTree();

			tree.Config.DrawSearchToolbar = true;
			tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
			tree.Selection.SupportsMultiSelect = false;

			AddSettings(tree,new Dictionary<string, string>
			{
				{ "게임 설정", nameof(GameSettings) },
				{ "빌드 설정", nameof(BuildSettings) },
				{ "메타 설정", nameof(MetaSettings) },
				{ "언어 설정", nameof(LanguageSettings) },
				{ "통신 설정", nameof(NetworkSettings) },
			});

			tree.Add("게임 설정/하이라키 커스텀",m_HierarchyCustom);

			AddMetaTable(tree);

			return tree;
		}

		private void AddMetaTable(OdinMenuTree _tree)
		{
			if(!MetaSettings.IsExist)
			{
				return;
			}

			var pathList = new List<string>();

			foreach(var type in MetaSettings.In.GetMetaTableGroup())
			{
				var tableName = type.Name;
				var dataPath = CommonUtility.PathCombine(GameSettings.In.MetaAssetPath,string.Format("{0}.asset",tableName));

				if(!CommonUtility.IsExistFile(dataPath))
				{
					// 생성
					CommonUtility.SaveAsset(dataPath,CreateInstance(type));
				}

				pathList.Add(dataPath);

				var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(CommonUtility.GetAssetsPath(dataPath));

				_tree.Add(string.Format("메타 설정/{0}",tableName),asset);
			}
		}

		private void AddSettings(OdinMenuTree _tree,Dictionary<string,string> _settingsDict)
		{
			foreach(var pair in _settingsDict)
			{
				var settings = CommonUtility.LoadAsset<ScriptableObject>(string.Format("t:ScriptableObject {0}",pair.Value));

				if(settings == null)
				{
					continue;
				}

				_tree.Add(pair.Key,settings);
			}
		}
	}
}
#endif