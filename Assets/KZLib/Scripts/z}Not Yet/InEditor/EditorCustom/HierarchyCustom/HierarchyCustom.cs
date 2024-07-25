#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KZLib.KZEditor
{
	public class HierarchyCustom : EditorCustom
	{
		private const float HEAD_SPACE = 37.0f;

		private const string HIERARCHY_DATA = "[Custom] HierarchyData";

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
		private static HierarchyData Hierarchy => s_Hierarchy ??= LoadData<HierarchyData>(HIERARCHY_DATA);

		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			UnityUtility.AddTag(Global.CATEGORY_TAG);

			SetHierarchy();
		}

		protected override void DoResetCustom()
		{
			if(UnityUtility.DisplayCheck("하이라키 커스텀 초기화","하이라키 커스텀을 초기화 하시겠습니까?"))
			{
				EditorPrefs.DeleteKey(HIERARCHY_DATA);
				s_Hierarchy = null;

				UnityUtility.DisplayInfo("하이라키 커스텀을 초기화 했습니다.");
			}
		}

		[TitleGroup("하이라키 설정",BoldTitle = false,Order = 1)]
		[VerticalGroup("하이라키 설정/사용",Order = 0),LabelText("하이라키 사용"),ToggleLeft,ShowInInspector]
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

		[BoxGroup("하이라키 설정/데이터",Order = 1,ShowLabel = false)]
		[HorizontalGroup("하이라키 설정/데이터/0"),LabelText("브랜치 트리 사용"),ToggleLeft,ShowInInspector,ShowIf(nameof(UseHierarchy))]
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

		[HorizontalGroup("하이라키 설정/데이터/0"),HideLabel,KZHexColor,ShowInInspector,ShowIf(nameof(UseBranchTree))]
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

		[HorizontalGroup("하이라키 설정/데이터/1"),LabelText("카테고리 라인 사용"),ToggleLeft,ShowInInspector,ShowIf(nameof(UseHierarchy))]
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

		[HorizontalGroup("하이라키 설정/데이터/1"),HideLabel,KZHexColor,ShowInInspector,ShowIf(nameof(UseCategoryLine))]
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

		[HorizontalGroup("하이라키 설정/데이터/2"),LabelText("아이콘 사용"),ToggleLeft,ShowInInspector,ShowIf(nameof(UseHierarchy))]
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

		private static readonly Dictionary<int,InstanceData> s_InstanceDataDict = new();

		private static void SaveHierarchy()
		{
			SaveData(HIERARCHY_DATA,Hierarchy);

			SetHierarchy();
		}

		private static void SetHierarchy()
		{
			if(!UseHierarchy)
			{
				return;
			}

			// 중복 세팅 제거 용
			EditorApplication.hierarchyWindowItemOnGUI -= OnDrawHierarchy;
			EditorApplication.hierarchyChanged -= OnUpdateHierarchy;

			EditorApplication.hierarchyWindowItemOnGUI += OnDrawHierarchy;
			EditorApplication.hierarchyChanged += OnUpdateHierarchy;

			OnUpdateHierarchy();

			EditorApplication.RepaintHierarchyWindow();
		}

		private static void OnDrawHierarchy(int _instanceID,Rect _rect)
		{
			if(!UseHierarchy || !s_InstanceDataDict.ContainsKey(_instanceID))
			{
				return;
			}

			var data = s_InstanceDataDict[_instanceID];
			var category = UseCategoryLine && data.IsCategory;

			if(category)
			{
				DrawCategory(data,_rect,_instanceID);
			}
			else if(UseIcon)
			{
				DrawIcon(_rect,_instanceID);
			}

			
			DrawBranchTree(data,_rect,category);
		}

		private static void OnUpdateHierarchy()
		{
			if(!UseHierarchy)
			{
				return;
			}

			s_InstanceDataDict.Clear();

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

		private static void CheckGameObject(GameObject _object,int _treeLevel,int _treeGroup,bool _isLastChild)
		{
			var instanceId = _object.GetInstanceID();

			if(s_InstanceDataDict.ContainsKey(instanceId))
			{
				return;
			}

			var childCount = _object.transform.childCount;

			s_InstanceDataDict.Add(instanceId,new InstanceData(_treeLevel,_treeGroup,childCount > 0,_isLastChild,_object.CompareTag(Global.CATEGORY_TAG)));

			for(var i=0;i<childCount;i++)
			{
				CheckGameObject(_object.transform.GetChild(i).gameObject,_treeLevel+1,_treeGroup,i == childCount-1);
			}
		}

		#region Draw Branch Tree
		private static void DrawBranchTree(InstanceData _data,Rect _rect,bool _isCategory)
		{
			if(UseBranchTree && _data.TreeLevel >= 0)
			{
				if(_data.TreeLevel < 0 || _rect.x < 60 || _isCategory)
				{
					return;
				}

				if(_data.IsLast)
				{
					DrawHVHLine(_rect,_data.TreeLevel,_data.HasChild);
				}
				else
				{
					DrawVHLine(_rect,_data.TreeLevel,_data.HasChild);
				}
			}
		}

		/// <summary>
		/// |
		/// |----
		/// |
		/// </summary>
		//! Vertical-Horizontal
		private static void DrawVHLine(Rect _rect,int _treeLevel,bool _hasChild)
		{
			DrawHalfVerticalLine(_rect,true,_treeLevel);
			DrawHorizontalLine(_rect,_treeLevel,_hasChild);
			DrawHalfVerticalLine(_rect,false,_treeLevel);
		}

		/// <summary>
		/// |
		/// |----
		/// </summary>
		//! Half-Vertical-Horizontal
		private static void DrawHVHLine(Rect _rect,int _treeLevel,bool _hasChild)
		{
			DrawHalfVerticalLine(_rect,true,_treeLevel);
			DrawHorizontalLine(_rect,_treeLevel,_hasChild);
		}

		/// <summary>
		/// |
		/// </summary>
		//! Half-Vertical
		private static void DrawHalfVerticalLine(Rect _rect,bool _isUp,int _treeLevel)
		{
			var height = _rect.height/2.0f;
			var x = GetTreeStartX(_rect,_treeLevel);
			var y = _isUp ? _rect.y : (_rect.y+height);

			EditorGUI.DrawRect(new Rect(x,y,2.0f,height),BranchTreeColor);
		}

		/// <summary>
		/// ----
		/// </summary>
		//! Horizontal
		private static void DrawHorizontalLine(Rect _rect,int _treeLevel,bool _hasChild)
		{
			var x = GetTreeStartX(_rect,_treeLevel);
			var y = _rect.y + _rect.height/2.0f;
			var width = _rect.height + (_hasChild ? -5.0f :  2.0f);

			EditorGUI.DrawRect(new Rect(x,y,width,2.0f),BranchTreeColor);
		}

		private static float GetTreeStartX(Rect _rect,int _treeLevel)
		{
			return HEAD_SPACE+(_rect.height-2.0f)*_treeLevel;
		}
		#endregion Draw Branch Tree

		#region Draw Category
		private static void DrawCategory(InstanceData _data,Rect _rect,int _instanceID)
		{
			var rect = new Rect(HEAD_SPACE,_rect.y,_rect.width+25.0f+_data.TreeLevel*14.0f,_rect.height);
			var current = EditorUtility.InstanceIDToObject(_instanceID) as GameObject;
			var color = Selection.activeGameObject == current ? CategoryColor.InvertColor() : CategoryColor;

			var text = current == null ? "" : current.name;

			EditorGUI.DrawRect(rect,color);
			EditorGUI.LabelField(rect,text,new GUIStyle()
			{
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleCenter,
				normal = new GUIStyleState() { textColor = color.grayscale > 0.5f ? Color.black : Color.white },
			});
		}
		#endregion Draw Category

		#region Draw Icon
		private static void DrawIcon(Rect _rect,int _instanceID)
		{
			var current = EditorUtility.InstanceIDToObject(_instanceID) as GameObject;

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

			var rect = new Rect(_rect.x+7,_rect.y+7,10,10);

			EditorGUI.LabelField(rect,content);

			GUI.color = cached;
		}

		private static GUIContent GetContent(GameObject _object)
		{
			var componentArray = _object.GetComponents<Component>();

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