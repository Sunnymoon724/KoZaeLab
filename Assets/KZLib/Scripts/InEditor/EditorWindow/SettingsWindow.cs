#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using System.Collections.Generic;
using KZLib.KZEditor;
using UnityEditor;
using System;

namespace KZLib.KZWindow
{
	public partial class SettingsWindow : OdinMenuEditorWindow
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

			tree.Add("게임 설정",GameSettings.In);

			tree.Add("게임 설정/하이라키 커스텀",m_HierarchyCustom);

			AddOdinMenuTree(ref tree);

			AddMetaTable(ref tree);

			return tree;
		}

		private void AddMetaTable(ref OdinMenuTree _tree)
		{
			if(!MetaSettings.HasInstance)
			{
				return;
			}

			var pathList = new List<string>();

			foreach(var tableName in MetaSettings.In.GetMetaTableNameGroup())
			{
				var dataPath = CommonUtility.PathCombine(GameSettings.In.MetaAssetPath,string.Format("{0}.asset",tableName));

				if(!CommonUtility.IsExistFile(dataPath))
				{
					// 생성
					var classType = Type.GetType(string.Format("MetaData.{0}",tableName));

					if(classType == null)
					{
						// 아직 존재하지 않음
						continue;
					}

					CommonUtility.SaveAsset(dataPath,CreateInstance(classType));
				}

				pathList.Add(dataPath);
			}

			foreach(var dataPath in pathList)
			{
				var tableName = CommonUtility.GetOnlyName(dataPath);
				var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(CommonUtility.GetAssetsPath(dataPath));

				_tree.Add(string.Format("메타 설정/{0}",tableName),asset);
			}
		}

		partial void AddOdinMenuTree(ref OdinMenuTree _tree);
	}
}
#endif