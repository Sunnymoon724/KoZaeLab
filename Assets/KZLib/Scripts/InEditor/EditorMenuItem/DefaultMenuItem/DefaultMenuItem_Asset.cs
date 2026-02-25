#if UNITY_EDITOR
using System;
using KZLib.Utilities;
using KZLib.Windows;
using UnityEditor;
using UnityEngine;

namespace KZLib.EditorInternal.Menus
{
	public static partial class DefaultMenuItem
	{
		private static bool _IsMonoScript()
		{
			return Selection.activeObject as MonoScript;
		}

		private static bool _IsScriptableObject()
		{
			return Selection.activeObject as ScriptableObject;
		}

		private static bool _IsTexture2D()
		{
			return Selection.activeObject as Texture2D;
		}

		private static bool _IsMesh()
		{
			return Selection.activeObject as Mesh;
		}

		private static bool _IsScriptableObjectValidation()
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

			if(typeof(ScriptableObject).IsAssignableFrom(scriptClass))
			{
				return false;
			}
			
			if(scriptClass.IsAbstract)
			{
				return false;
			}

			return true;
		}

		#region Script
		[MenuItem("Assets/KZSubMenu/Script/Create ScriptableObject",false,MenuOrder.Asset.SCRIPT)]
		private static void _OnCreateScriptableObject()
		{
			var script = Selection.activeObject as MonoScript;
			var assetPath = KZFileKit.NormalizePath($"Resources/ScriptableObject/{script.name}.asset");

			if(!KZEditorKit.DisplayCheck("Create scriptableObject",$"Do you want to create scriptableObject? \n Name : {script.name} \n Path : {assetPath}"))
			{
				return;
			}

			if(KZFileKit.IsFileExist(assetPath))
			{
				KZEditorKit.DisplayError(new NullReferenceException($"{assetPath} is exist."));

				return;
			}

			var asset = ScriptableObject.CreateInstance(script.GetClass());

			KZAssetKit.CreateAsset(assetPath,asset,true);
		}

		[MenuItem("Assets/KZSubMenu/Script/Create ScriptableObject",true,MenuOrder.Asset.SCRIPT)]
		private static bool _CanCreateScriptableObject()
		{
			return _IsScriptableObjectValidation();
		}
		#endregion Script

		#region Texture
		[MenuItem("Assets/KZSubMenu/Texture/Open Texture",false,MenuOrder.Asset.TEXTURE)]
		private static void _OnOpenTexture()
		{
			var viewer = EditorWindow.GetWindow<TextureWindow>("Viewer");

			viewer.SetResource(Selection.activeObject as Texture2D);
			viewer.Show();
		}

		[MenuItem("Assets/KZSubMenu/Texture/Open Texture",true,MenuOrder.Asset.TEXTURE)]
		private static bool _CanOpenTexture()
		{
			return _IsTexture2D();
		}
		#endregion Texture

		#region ScriptableObject
		[MenuItem("Assets/KZSubMenu/ScriptableObject/Open ScriptableObject",false,MenuOrder.Asset.SCRIPTABLE_OBJECT)]
		private static void _OnOpenScriptableObject()
		{
			var viewer = EditorWindow.GetWindow<ScriptableObjectWindow>("Viewer");

			viewer.SetResource(Selection.activeObject as ScriptableObject);
			viewer.Show();
		}

		[MenuItem("Assets/KZSubMenu/ScriptableObject/Open ScriptableObject",true,MenuOrder.Asset.SCRIPTABLE_OBJECT)]
		private static bool _CanOpenScriptableObject()
		{
			return _IsScriptableObject();
		}
		#endregion ScriptableObject
	}
}
#endif