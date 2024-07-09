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
		private readonly HierarchyCustom m_HierarchyCustom = new();

		protected override OdinMenuTree BuildMenuTree()
		{
			var tree = new OdinMenuTree();

			tree.Config.DrawSearchToolbar = true;
			tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
			tree.Selection.SupportsMultiSelect = false;

			tree.Add("게임 설정",GameSettings.In);

			AddSettings(tree,new Dictionary<string,string>
			{
				{ "빌드 설정", nameof(BuildSettings) },
				{ "메타 설정", nameof(MetaSettings) },
				{ "언어 설정", nameof(LanguageSettings) },
				{ "통신 설정", nameof(NetworkSettings) },
			});

			tree.Add("게임 설정/퀄리티 프리셋",GraphicQualityPresetSettings.In);
			tree.Add("게임 설정/하이라키 커스텀",m_HierarchyCustom);

			AddMetaData(tree);

			return tree;
		}

		private void AddMetaData(OdinMenuTree _tree)
		{
			MetaDataMgr.In.Load_Editor();

			var dataList = new List<IMetaData>();

			foreach(var type in ReflectionUtility.FindTypeGroup("MetaData.{0}"))
			{
				dataList.Clear();

				var iterator = MetaDataMgr.In.GetContainerIterator(type);

				while(iterator.MoveNext())
				{
					var pair = iterator.Current;

					dataList.Add(pair.Value);
				}

				_tree.Add(string.Format("메타 설정/{0}",type.Name),new MetaDataTable(type.Name,dataList));
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

		private TObject GetAsset<TObject>(bool _autoCreate) where TObject : ScriptableObject
		{
			var _path = string.Format("t:ScriptableObject {0}",typeof(TObject).Name);

			var asset = CommonUtility.LoadAsset<TObject>(_path);

			if(asset == null && _autoCreate)
			{
				asset = CreateInstance<TObject>();

				// 생성
				var dataPath = CommonUtility.PathCombine("Resources/ScriptableObjects",typeof(TObject).Name);

				CommonUtility.SaveAsset(dataPath,asset);
			}

			return asset;
		}
	}
}
#endif