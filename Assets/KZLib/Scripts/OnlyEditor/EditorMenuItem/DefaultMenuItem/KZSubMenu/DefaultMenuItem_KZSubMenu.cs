#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KZLib.EditorInternal.Menus
{
	/// <summary>
	/// <c>Assets/KZSubMenu</c> shared validators and prefab mesh scan helpers.
	/// </summary>
	public static partial class DefaultMenuItem
	{
		private static bool _IsScriptableObjectAsset()
		{
			return Selection.activeObject is ScriptableObject;
		}

		private static bool _IsTexture2D()
		{
			return Selection.activeObject is Texture2D;
		}

		private static bool _IsPrefabAsset()
		{
			if(Selection.activeObject is not GameObject gameObject)
			{
				return false;
			}

			return PrefabUtility.IsPartOfPrefabAsset(gameObject);
		}

		private static bool _IsCreatableScriptableObjectScript()
		{
			if(Selection.activeObject is not MonoScript script)
			{
				return false;
			}

			var scriptClass = script.GetClass();

			if(scriptClass == null)
			{
				return false;
			}

			if(!typeof(ScriptableObject).IsAssignableFrom(scriptClass))
			{
				return false;
			}

			if(scriptClass.IsAbstract)
			{
				return false;
			}

			return true;
		}

		private static Dictionary<string,List<string>> _CollectMeshNameDict(GameObject root)
		{
			var textListDict = new Dictionary<string,List<string>>();

			if(!root)
			{
				return textListDict;
			}

			var meshFilterArray = root.GetComponentsInChildren<MeshFilter>();

			for(var i=0;i<meshFilterArray.Length;i++)
			{
				var meshFilter = meshFilterArray[i];

				if(!meshFilter || !meshFilter.sharedMesh)
				{
					continue;
				}

				textListDict.AddOrCreate(meshFilter.sharedMesh.name,meshFilter.transform.BuildHierarchyPath());
			}

			var skinnedMeshRendererArray = root.GetComponentsInChildren<SkinnedMeshRenderer>();

			for(var i=0;i<skinnedMeshRendererArray.Length;i++)
			{
				var skinnedMeshRenderer = skinnedMeshRendererArray[i];

				if(!skinnedMeshRenderer || !skinnedMeshRenderer.sharedMesh)
				{
					continue;
				}

				textListDict.AddOrCreate(skinnedMeshRenderer.sharedMesh.name,skinnedMeshRenderer.transform.BuildHierarchyPath());
			}

			return textListDict;
		}

		private static void _LogMeshNameList(GameObject root)
		{
			var textListDict = _CollectMeshNameDict(root);

			if(textListDict.Count == 0)
			{
				LogChannel.Editor.I("Mesh is not found.");

				return;
			}

			LogChannel.Editor.I("Mesh list");

			_LogGroupedPathList(textListDict);
		}
	}
}
#endif