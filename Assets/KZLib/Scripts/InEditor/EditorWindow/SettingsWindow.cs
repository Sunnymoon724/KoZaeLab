#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using System.Collections.Generic;
using KZLib.KZEditor;

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

			tree.Add("Game Settings",GameSettings.In);

			AddSettings(tree,new Dictionary<string,string>
			{
				{ "Build Settings", nameof(BuildSettings) },
				{ "Meta Settings", nameof(MetaSettings) },
				{ "Language Settings", nameof(LanguageSettings) },
				{ "Network Settings", nameof(NetworkSettings) },
			});

			tree.Add("Game Settings/Quality Preset",GraphicQualityPresetSettings.In);
			tree.Add("Game Settings/Hierarchy Custom",m_HierarchyCustom);

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

				_tree.Add(string.Format("Meta Settings/{0}",type.Name),new MetaDataTable(type.Name,dataList));
			}
		}

		private void AddSettings(OdinMenuTree _tree,Dictionary<string,string> _settingsDict)
		{
			foreach(var pair in _settingsDict)
			{
				var settings = UnityUtility.LoadAsset<ScriptableObject>(string.Format("t:ScriptableObject {0}",pair.Value));

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

			var asset = UnityUtility.LoadAsset<TObject>(_path);

			if(asset == null && _autoCreate)
			{
				asset = CreateInstance<TObject>();

				var dataPath = FileUtility.PathCombine("Resources/ScriptableObjects",typeof(TObject).Name);

				UnityUtility.SaveAsset(dataPath,asset);
			}

			return asset;
		}
	}
}
#endif