#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KZLib.Tet
{
	[Serializable]
	public static class EditorCustom
	{
		private const float c_headSpace = 37.0f;
		private const float c_iconSpace = 7.0f;
		private const int c_iconSize = 10;

		private const string c_editorText = "[Editor] CustomInfo";

		[Serializable]
		private record CustomInfo
		{
			public bool UseHierarchy { get; set; } = false;

			public bool UseBranchTree { get; set; } = true;
			public string BranchTreeHexColor { get; set; } = "#FF61C2FF";

			public bool UseCategoryLine { get; set; } = true;
			public string CategoryHexColor { get; set; } = "#877FE9FF";

			public bool UseIcon { get; set; } = true;

			[JsonIgnore]
			public Color BranchTreeColor => BranchTreeHexColor.ToColor();
			[JsonIgnore]
			public Color CategoryColor => CategoryHexColor.ToColor();
		}

		private record HierarchyInfo(int TreeLevel,int TreeGroup,bool HasChild,bool IsLast,bool IsCategory);

		private static CustomInfo s_customInfo = null;

		private static readonly Dictionary<int,HierarchyInfo> s_hierarchyInfoDict = new();

		[InitializeOnLoadMethod]
		private static void _Initialize()
		{
			s_customInfo = _LoadInfo<CustomInfo>();

			_InitializeHierarchy();
		}

		private static void _InitializeHierarchy()
		{
			CommonUtility.AddTag("Category");

			_SetHierarchy();
		}

		private static void _SetHierarchy()
		{
			// Remove Overlap
			EditorApplication.hierarchyWindowItemOnGUI -= _OnDrawHierarchy;
			EditorApplication.hierarchyChanged -= _OnUpdateHierarchy;

			if(s_customInfo.UseHierarchy)
			{
				EditorApplication.hierarchyWindowItemOnGUI += _OnDrawHierarchy;
				EditorApplication.hierarchyChanged += _OnUpdateHierarchy;

				_OnUpdateHierarchy();
			}

			EditorApplication.RepaintHierarchyWindow();
		}

		private static TInfo _LoadInfo<TInfo>() where TInfo : new()
		{
			var text = EditorPrefs.GetString(c_editorText,"");

			return text.IsEmpty() ? new TInfo() : JsonConvert.DeserializeObject<TInfo>(text);
		}

		public static void SetCustomInfo(string key,object value)
		{
			if(!_TryGetPropertyInfo(key,out var propertyInfo))
			{
				return;
			}

			propertyInfo.SetValue(s_customInfo,value);

			_SaveCustomInfo();
		}

		public static TValue GetCustomInfo<TValue>(string key)
		{
			if(!_TryGetPropertyInfo(key,out var propertyInfo))
			{
				return default;
			}

			return (TValue) propertyInfo.GetValue(s_customInfo);
		}

		private static bool _TryGetPropertyInfo(string key,out PropertyInfo propertyInfo)
		{
			var customType = typeof(CustomInfo);
			propertyInfo = customType.GetProperty(key);

			if(propertyInfo == null)
			{
				LogSvc.System.E($"{key} is not exist in custom info");
			}

			return propertyInfo != null;
		}

		private static void _SaveCustomInfo()
		{
			try
			{
				EditorPrefs.SetString(c_editorText,JsonConvert.SerializeObject(s_customInfo));

				_SetHierarchy();
			}
			catch(Exception exception)
			{
				LogSvc.System.E($"Set editorPrefs failed. [{c_editorText}/{s_customInfo} - {exception.Message}]");
			}
		}

		private static void _OnDrawHierarchy(int instanceId,Rect rect)
		{
			if(!s_customInfo.UseHierarchy || !s_hierarchyInfoDict.TryGetValue(instanceId,out var hierarchyInfo))
			{
				return;
			}

			if(s_customInfo.UseCategoryLine && hierarchyInfo.IsCategory)
			{
				_DrawCategory(hierarchyInfo,rect,instanceId);
			}

			if(s_customInfo.UseIcon)
			{
				_DrawIcon(rect,instanceId);
			}

			if(s_customInfo.UseBranchTree)
			{
				_DrawBranchTree(hierarchyInfo,rect,hierarchyInfo.IsCategory);
			}
		}

		private static void _OnUpdateHierarchy()
		{
			if(!s_customInfo.UseHierarchy)
			{
				return;
			}

			s_hierarchyInfoDict.Clear();

			var prefab = PrefabStageUtility.GetCurrentPrefabStage();

			if(prefab)
			{
				var root = prefab.prefabContentsRoot;
				var level = root.transform.parent ? 1 : 0;

				_CheckGameObject(root,level,0,true);

				return;
			}

			for(var i=0;i<SceneManager.sceneCount;i++)
			{
				var scene = SceneManager.GetSceneAt(i);

				if(!scene.isLoaded)
				{
					continue;
				}

				var objectArray = scene.GetRootGameObjects();

				for(var j=0;j<objectArray.Length;j++)
				{
					_CheckGameObject(objectArray[j],0,j,j == objectArray.Length-1);
				}
			}
		}

		private static void _CheckGameObject(GameObject gameObject,int treeLevel,int treeGroup,bool isLastChild)
		{
			var instanceId = gameObject.GetInstanceID();

			if(s_hierarchyInfoDict.ContainsKey(instanceId))
			{
				return;
			}

			var childCount = gameObject.transform.childCount;

			s_hierarchyInfoDict.Add(instanceId,new HierarchyInfo(treeLevel,treeGroup,childCount > 0,isLastChild,gameObject.CompareTag("Category")));

			for(var i=0;i<childCount;i++)
			{
				_CheckGameObject(gameObject.transform.GetChild(i).gameObject,treeLevel+1,treeGroup,i == childCount-1);
			}
		}

		#region Draw Branch Tree
		private static void _DrawBranchTree(HierarchyInfo hierarchyInfo,Rect rect,bool isCategory)
		{
			if(!s_customInfo.UseBranchTree || hierarchyInfo.TreeLevel < 0 || rect.x < 60 || isCategory)
			{
				return;
			}

			if(hierarchyInfo.IsLast)
			{
				_DrawHalfVerticalAndHorizontalLine(rect,hierarchyInfo.TreeLevel,hierarchyInfo.HasChild);
			}
			else
			{
				_DrawVerticalAndHorizontalLine(rect,hierarchyInfo.TreeLevel,hierarchyInfo.HasChild);
			}
		}

		/// <summary>
		/// |
		/// |----
		/// |
		/// </summary>
		private static void _DrawVerticalAndHorizontalLine(Rect rect,int treeLevel,bool hasChild)
		{
			_DrawHalfVerticalLine(rect,true,treeLevel);
			_DrawHorizontalLine(rect,treeLevel,hasChild);
			_DrawHalfVerticalLine(rect,false,treeLevel);
		}

		/// <summary>
		/// |
		/// |----
		/// </summary>
		private static void _DrawHalfVerticalAndHorizontalLine(Rect rect,int treeLevel,bool hasChild)
		{
			_DrawHalfVerticalLine(rect,true,treeLevel);
			_DrawHorizontalLine(rect,treeLevel,hasChild);
		}

		/// <summary>
		/// |
		/// </summary>
		//! Half-Vertical
		private static void _DrawHalfVerticalLine(Rect rect,bool isUpper,int treeLevel)
		{
			var height = rect.height/2.0f;
			var x = _GetTreeStartX(rect,treeLevel);
			var y = isUpper ? rect.y : (rect.y+height);

			EditorGUI.DrawRect(new Rect(x,y,2.0f,height),s_customInfo.BranchTreeColor);
		}

		/// <summary>
		/// ----
		/// </summary>
		//! Horizontal
		private static void _DrawHorizontalLine(Rect rect,int treeLevel,bool hasChild)
		{
			var x = _GetTreeStartX(rect,treeLevel);
			var y = rect.y+rect.height/2.0f;
			var width = rect.height+(hasChild ? -5.0f :  2.0f);

			EditorGUI.DrawRect(new Rect(x,y,width,2.0f),s_customInfo.BranchTreeColor);
		}

		private static float _GetTreeStartX(Rect rect,int treeLevel)
		{
			return c_headSpace+(rect.height-2.0f)*treeLevel;
		}
		#endregion Draw Branch Tree

		#region Draw Category
		private static void _DrawCategory(HierarchyInfo hierarchyInfo,Rect rect,int instanceId)
		{
			var categoryRect = new Rect(c_headSpace,rect.y,rect.width+25.0f+hierarchyInfo.TreeLevel*14.0f,rect.height);
			var currentObject = EditorUtility.EntityIdToObject(instanceId) as GameObject;
			var categoryColor = Selection.activeGameObject == currentObject ? s_customInfo.CategoryColor.InvertColor() : s_customInfo.CategoryColor;

			var currentName = currentObject == null ? "" : currentObject.name;

			EditorGUI.DrawRect(categoryRect,categoryColor);
			EditorGUI.LabelField(categoryRect,currentName,new GUIStyle()
			{
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleCenter,
				normal = new GUIStyleState() { textColor = categoryColor.grayscale > 0.5f ? Color.black : Color.white },
			});
		}
		#endregion Draw Category

		#region Draw Icon
		private static void _DrawIcon(Rect rect,int instanceId)
		{
			var currentObject = EditorUtility.EntityIdToObject(instanceId) as GameObject;

			if(currentObject == null)
			{
				return;
			}

			var content = _GenerateContent(currentObject);

			if(content == null || content.image == null)
			{
				return;
			}

			var cachedColor = GUI.color;

			if(!currentObject.activeInHierarchy)
			{
				GUI.color = cachedColor.MaskAlpha(0.5f);
			}

			var iconRect = new Rect(rect.x+c_iconSpace,rect.y+c_iconSpace,c_iconSize,c_iconSize);

			EditorGUI.LabelField(iconRect,content);

			GUI.color = cachedColor;
		}

		private static GUIContent _GenerateContent(GameObject gameObject)
		{
			var componentArray = gameObject.GetComponents<Component>();

			if(componentArray == null || componentArray.Length < 2)
			{
				return null;
			}

			var component = componentArray[1];

			if(component == null)
			{
				return null;
			}

			var componentType = component.GetType();

			if(componentType == typeof(CanvasRenderer))
			{
				component = componentArray[^1];
			}

			var content = EditorGUIUtility.ObjectContent(component,componentType);
			content.text = null;
            content.tooltip = "";

			return content;
		}
		#endregion Draw Icon

		public static void ResetCustom()
		{
			if(!CommonUtility.DisplayCheckBeforeExecute("Hierarchy custom reset"))
			{
				return;
			}

			EditorPrefs.DeleteKey(c_editorText);

			s_customInfo = null;

			_Initialize();

			CommonUtility.DisplayInfo("Hierarchy custom reset complete");
		}
	}
}
#endif