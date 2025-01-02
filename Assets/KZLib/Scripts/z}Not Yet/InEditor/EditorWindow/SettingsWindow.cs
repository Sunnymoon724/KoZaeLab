#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace KZLib.Window
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
				{ "Build Settings",		nameof(BuildSettings)		},
				// { "Meta Settings",		nameof(MetaSettings)		},
				// { "Config Settings",	nameof(ConfigSettings)		},
				// { "Language Settings",	nameof(LanguageSettings)	},
				{ "Network Settings",	nameof(NetworkSettings)		},
			});

			tree.Add("Game Settings/Quality Preset",GraphicQualityPresetSettings.In);
			tree.Add("Game Settings/Hierarchy Custom",m_HierarchyCustom);

			// AddMetaData(tree);

			return tree;
		}

		// private void AddMetaData(OdinMenuTree _tree)
		// {
		// 	if(!MetaSettings.IsExist)
		// 	{
		// 		return;
		// 	}

		// 	MetaDataMgr.In.Load_Editor();

		// 	foreach(var type in MetaSettings.In.GetMetaTypeGroup())
		// 	{
		// 		_tree.Add($"Meta Settings/{type.Name}",new MetaDataTable(type));
		// 	}
		// }

		private void AddSettings(OdinMenuTree _tree,Dictionary<string,string> _settingsDict)
		{
			foreach(var pair in _settingsDict)
			{
				var settings = AssetDatabase.LoadAssetAtPath<ScriptableObject>($"t:ScriptableObject {pair.Value}");

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