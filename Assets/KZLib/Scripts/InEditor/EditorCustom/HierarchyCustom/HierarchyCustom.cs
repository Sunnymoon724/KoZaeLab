#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KZLib
{
	public class HierarchyCustom : EditorCustom
	{
		private const float c_head_space = 37.0f;

		private const string c_hierarchy_data = "[Custom] HierarchyData";

		[Serializable]
		private class HierarchyData
		{
			public bool UseHierarchy { get; set; } = false;

			public bool UseBranchTree { get; set; } = true;
			public string BranchTreeHexColor { get; set; } = "#FF61C2FF";

			public bool UseCategoryLine { get; set; } = true;
			public string CategoryHexColor { get; set; } = "#877FE9FF";

			public bool UseIcon { get; set; } = true;
		}

		private static HierarchyData s_Hierarchy = null;
		private static HierarchyData Hierarchy => s_Hierarchy ??= LoadData<HierarchyData>(c_hierarchy_data);

		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			CommonUtility.AddTag(Global.CATEGORY_TAG);

			SetHierarchy();
		}

		protected override void _ResetCustom()
		{
			if(CommonUtility.DisplayCheck("Hierarchy Custom Reset","Hierarchy Custom Reset?"))
			{
				EditorPrefs.DeleteKey(c_hierarchy_data);
				s_Hierarchy = null;

				CommonUtility.DisplayInfo("Hierarchy Custom Reset Complete");
			}
		}

		[TitleGroup("Hierarchy",BoldTitle = false,Order = 1)]
		[VerticalGroup("Hierarchy/Use",Order = 0),LabelText("Use Hierarchy"),ToggleLeft,ShowInInspector]
		private static bool UseHierarchy
		{
			get => Hierarchy.UseHierarchy;
			set
			{
				if(Hierarchy.UseHierarchy == value)
				{
					return;
				}

				Hierarchy.UseHierarchy = value;

				SaveHierarchy();
			}
		}

		[BoxGroup("Hierarchy/Data",Order = 1,ShowLabel = false)]
		[HorizontalGroup("Hierarchy/Data/0"),LabelText("Use Branch Tree"),ToggleLeft,ShowInInspector,ShowIf(nameof(UseHierarchy))]
		private static bool UseBranchTree
		{
			get => Hierarchy.UseBranchTree && UseHierarchy;
			set
			{
				if(Hierarchy.UseBranchTree == value)
				{
					return;
				}

				Hierarchy.UseBranchTree = value;

				SaveHierarchy();
			}
		}

		[HorizontalGroup("Hierarchy/Data/0"),HideLabel,KZHexColor,ShowInInspector,ShowIf(nameof(UseBranchTree))]
		private static string BranchTreeHexColor
		{
			get => Hierarchy.BranchTreeHexColor;
			set
			{
				if(Hierarchy.BranchTreeHexColor == value)
				{
					return;
				}

				Hierarchy.BranchTreeHexColor = value;

				SaveHierarchy();
			}
		}
		private static Color BranchTreeColor => BranchTreeHexColor.ToColor();

		[HorizontalGroup("Hierarchy/Data/1"),LabelText("Use Category Line"),ToggleLeft,ShowInInspector,ShowIf(nameof(UseHierarchy))]
		private static bool UseCategoryLine
		{
			get => Hierarchy.UseCategoryLine && UseHierarchy;
			set
			{
				if(Hierarchy.UseCategoryLine == value)
				{
					return;
				}

				Hierarchy.UseCategoryLine = value;

				SaveHierarchy();
			}
		}

		[HorizontalGroup("Hierarchy/Data/1"),HideLabel,KZHexColor,ShowInInspector,ShowIf(nameof(UseCategoryLine))]
		private static string CategoryHexColor
		{
			get => Hierarchy.CategoryHexColor;
			set
			{
				if(Hierarchy.CategoryHexColor == value)
				{
					return;
				}

				Hierarchy.CategoryHexColor = value;

				SaveHierarchy();
			}
		}
		private static Color CategoryColor => CategoryHexColor.ToColor();

		[HorizontalGroup("Hierarchy/Data/2"),LabelText("Use Icon"),ToggleLeft,ShowInInspector,ShowIf(nameof(UseHierarchy))]
		private static bool UseIcon
		{
			get => Hierarchy.UseIcon && UseHierarchy;
			set
			{
				if(Hierarchy.UseIcon == value)
				{
					return;
				}

				Hierarchy.UseIcon = value;

				SaveHierarchy();
			}
		}

		private static readonly Dictionary<int,InstanceData> s_instanceDataDict = new();

		private static void SaveHierarchy()
		{
			SaveData(c_hierarchy_data,Hierarchy);

			SetHierarchy();
		}

		private static void SetHierarchy()
		{
			if(!UseHierarchy)
			{
				return;
			}

			// Remove Overlap
			EditorApplication.hierarchyWindowItemOnGUI -= OnDrawHierarchy;
			EditorApplication.hierarchyChanged -= OnUpdateHierarchy;

			EditorApplication.hierarchyWindowItemOnGUI += OnDrawHierarchy;
			EditorApplication.hierarchyChanged += OnUpdateHierarchy;

			OnUpdateHierarchy();

			EditorApplication.RepaintHierarchyWindow();
		}

		private static void OnDrawHierarchy(int instanceId,Rect rect)
		{
			if(!UseHierarchy || !s_instanceDataDict.TryGetValue(instanceId,out var data))
			{
				return;
			}

			if(UseCategoryLine && data.IsCategory)
			{
				DrawCategory(data,rect,instanceId);
			}
			else if(UseIcon)
			{
				DrawIcon(rect,instanceId);
			}

			DrawBranchTree(data,rect,data.IsCategory);
		}
		private static void OnUpdateHierarchy()
		{
			if(!UseHierarchy)
			{
				return;
			}

			s_instanceDataDict.Clear();

			var prefab = PrefabStageUtility.GetCurrentPrefabStage();

			if(prefab)
			{
				var root = prefab.prefabContentsRoot;
				var level = root.transform.parent ? 1 : 0;

				CheckGameObject(root,level,0,true);

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
					CheckGameObject(objectArray[j],0,j,j == objectArray.Length-1);
				}
			}
		}

		private static void CheckGameObject(GameObject gameObject,int treeLevel,int treeGroup,bool isLastChild)
		{
			var instanceId = gameObject.GetInstanceID();

			if(s_instanceDataDict.ContainsKey(instanceId))
			{
				return;
			}

			var childCount = gameObject.transform.childCount;

			s_instanceDataDict.Add(instanceId,new InstanceData(treeLevel,treeGroup,childCount > 0,isLastChild,gameObject.CompareTag(Global.CATEGORY_TAG)));

			for(var i=0;i<childCount;i++)
			{
				CheckGameObject(gameObject.transform.GetChild(i).gameObject,treeLevel+1,treeGroup,i == childCount-1);
			}
		}

		#region Draw Branch Tree
		private static void DrawBranchTree(InstanceData instanceData,Rect rect,bool isCategory)
		{
			if(!UseBranchTree || instanceData.TreeLevel < 0 || rect.x < 60 || isCategory)
			{
				return;
			}

			if(instanceData.IsLast)
			{
				DrawHalfVerticalAndHorizontalLine(rect,instanceData.TreeLevel,instanceData.HasChild);
			}
			else
			{
				DrawVerticalAndHorizontalLine(rect,instanceData.TreeLevel,instanceData.HasChild);
			}
		}

		/// <summary>
		/// |
		/// |----
		/// |
		/// </summary>
		private static void DrawVerticalAndHorizontalLine(Rect rect,int treeLevel,bool hasChild)
		{
			DrawHalfVerticalLine(rect,true,treeLevel);
			DrawHorizontalLine(rect,treeLevel,hasChild);
			DrawHalfVerticalLine(rect,false,treeLevel);
		}

		/// <summary>
		/// |
		/// |----
		/// </summary>
		private static void DrawHalfVerticalAndHorizontalLine(Rect rect,int treeLevel,bool hasChild)
		{
			DrawHalfVerticalLine(rect,true,treeLevel);
			DrawHorizontalLine(rect,treeLevel,hasChild);
		}

		/// <summary>
		/// |
		/// </summary>
		//! Half-Vertical
		private static void DrawHalfVerticalLine(Rect rect,bool isUpper,int treeLevel)
		{
			var height = rect.height/2.0f;
			var x = GetTreeStartX(rect,treeLevel);
			var y = isUpper ? rect.y : (rect.y+height);

			EditorGUI.DrawRect(new Rect(x,y,2.0f,height),BranchTreeColor);
		}

		/// <summary>
		/// ----
		/// </summary>
		//! Horizontal
		private static void DrawHorizontalLine(Rect rect,int treeLevel,bool hasChild)
		{
			var x = GetTreeStartX(rect,treeLevel);
			var y = rect.y + rect.height/2.0f;
			var width = rect.height + (hasChild ? -5.0f :  2.0f);

			EditorGUI.DrawRect(new Rect(x,y,width,2.0f),BranchTreeColor);
		}

		private static float GetTreeStartX(Rect rect,int treeLevel)
		{
			return c_head_space+(rect.height-2.0f)*treeLevel;
		}
		#endregion Draw Branch Tree

		#region Draw Category
		private static void DrawCategory(InstanceData instanceData,Rect rect,int instanceId)
		{
			var newRect = new Rect(c_head_space,rect.y,rect.width+25.0f+instanceData.TreeLevel*14.0f,rect.height);
			var current = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
			var color = Selection.activeGameObject == current ? CategoryColor.InvertColor() : CategoryColor;

			var text = current == null ? "" : current.name;

			EditorGUI.DrawRect(newRect,color);
			EditorGUI.LabelField(newRect,text,new GUIStyle()
			{
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleCenter,
				normal = new GUIStyleState() { textColor = color.grayscale > 0.5f ? Color.black : Color.white },
			});
		}
		#endregion Draw Category

		#region Draw Icon
		private static void DrawIcon(Rect rect,int instanceId)
		{
			var current = EditorUtility.InstanceIDToObject(instanceId) as GameObject;

			if(current == null)
			{
				return;
			}

			var content = GetContent(current);

			if(content == null || content.image == null)
			{
				return;
			}

			var cached = GUI.color;

			if(!current.activeInHierarchy)
			{
				GUI.color = cached.MaskAlpha(0.5f);
			}

			var newRect = new Rect(rect.x+7,rect.y+7,10,10);

			EditorGUI.LabelField(newRect,content);

			GUI.color = cached;
		}

		private static GUIContent GetContent(GameObject gameObject)
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

			var type = component.GetType();

			if(type == typeof(CanvasRenderer))
			{
				component = componentArray[^1];
			}

			var content = EditorGUIUtility.ObjectContent(component,type);
			content.text = null;
            content.tooltip = "";

			return content;
		}
		#endregion	Draw Icon

		private record InstanceData(int TreeLevel,int TreeGroup,bool HasChild,bool IsLast,bool IsCategory);
	}
}
#endif