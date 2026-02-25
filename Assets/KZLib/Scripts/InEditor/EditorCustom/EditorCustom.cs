#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using KZLib.Attributes;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KZLib.EditorInternal
{
	[Serializable]
	public static class EditorCustom
	{
		private const float c_headSpace = 37.0f;
		private const float c_iconSpace = 7.0f;
		private const int c_iconSize = 10;

		private const string c_editorText = "[Editor] CustomData";

		#region Custom Editor Data
		[Serializable]
		private class CustomEditorData
		{
			[TitleGroup("Option",BoldTitle = false,Order = 0)]
			[HorizontalGroup("Option/Button",Order = 0),Button("Reset Custom Data",ButtonSizes.Large)]
			protected void _ResetButton()
			{
				_ResetCustomData();
			}
			[HorizontalGroup("Option/Button",Order = 0),Button("Save Custom Data",ButtonSizes.Large)]
			protected void _SaveButton()
			{
				_SaveCustomData();
			}

			[SerializeField,HideInInspector,JsonProperty(nameof(UseHierarchy))]
			private bool m_useHierarchy = false;
			[TitleGroup("Hierarchy",BoldTitle = false,Order = 1)]
			[VerticalGroup("Hierarchy/Use",Order = 0),ToggleLeft,ShowInInspector,JsonIgnore]
			public bool UseHierarchy
			{
				get => m_useHierarchy;
				private set
				{
					if(m_useHierarchy == value)
					{
						return;
					}

					m_useHierarchy = value;
				}
			}


			[SerializeField,HideInInspector,JsonProperty(nameof(UseBranchTree))]
			private bool m_useBranchTree = true;
			[BoxGroup("Hierarchy/Data",Order = 1,ShowLabel = false)]
			[VerticalGroup("Hierarchy/Data/0"),ToggleLeft,ShowInInspector,JsonIgnore,ShowIf(nameof(UseHierarchy))]
			public bool UseBranchTree
			{
				get => UseHierarchy && m_useBranchTree;
				private set
				{
					if(m_useBranchTree == value)
					{
						return;
					}

					m_useBranchTree = value;
				}
			}
			[SerializeField,HideInInspector,JsonProperty(nameof(BranchTreeHexColor))]
			private string m_branchTreeHexColor = "#FF61C2FF";
			[VerticalGroup("Hierarchy/Data/0"),KZHexColor,ShowInInspector,JsonIgnore,ShowIf(nameof(UseBranchTree))]
			private string BranchTreeHexColor
			{
				get => m_branchTreeHexColor;
				set
				{
					if(m_branchTreeHexColor == value)
					{
						return;
					}

					m_branchTreeHexColor = value;
				}
			}
			[JsonIgnore]
			public Color BranchTreeColor => BranchTreeHexColor.ToColor();

			[PropertySpace(10)]

			[SerializeField,HideInInspector,JsonProperty(nameof(UsePrefixDesign))]
			private bool m_usePrefixDesign = true;
			[BoxGroup("Hierarchy/Data",Order = 1,ShowLabel = false)]
			[VerticalGroup("Hierarchy/Data/1"),ToggleLeft,ShowInInspector,JsonIgnore,ShowIf(nameof(UseHierarchy))]
			public bool UsePrefixDesign
			{
				get => UseHierarchy && m_usePrefixDesign;
				private set
				{
					if(m_usePrefixDesign == value)
					{
						return;
					}

					m_usePrefixDesign = value;
				}
			}
			[SerializeField,HideInInspector,JsonProperty(nameof(DesignInfoList))]
			private List<PrefixDesignInfo> m_designInfoList = new();
			[VerticalGroup("Hierarchy/Data/1"),ShowInInspector,JsonIgnore,ShowIf(nameof(UsePrefixDesign))]
			public List<PrefixDesignInfo> DesignInfoList
			{
				get => m_designInfoList;
				private set
				{
					if(m_designInfoList == value)
					{
						return;
					}

					m_designInfoList = value;
				}
			}

			[PropertySpace(10)]

			[SerializeField,HideInInspector,JsonProperty(nameof(UseIcon))]
			private bool m_useIcon = true;
			[VerticalGroup("Hierarchy/Data/2"),ToggleLeft,ShowInInspector,JsonIgnore,ShowIf(nameof(UseHierarchy))]
			public bool UseIcon
			{
				get => UseHierarchy && m_useIcon;
				private set
				{
					if(m_useIcon == value)
					{
						return;
					}

					m_useIcon = value;
				}
			}
		}
		#endregion Custom Editor Data

		#region Prefix Design Info
		[Serializable]
		private struct PrefixDesignInfo
		{
			[SerializeField,HideInInspector,JsonProperty(nameof(PrefixKey))]
			private string m_prefixKey;
			[ShowInInspector,JsonIgnore]
			public string PrefixKey
			{
				get => m_prefixKey;
				private set
				{
					if(m_prefixKey == value)
					{
						return;
					}

					m_prefixKey = value;
				}
			}

			[SerializeField,HideInInspector,JsonProperty(nameof(BackgroundHexColor))]
			private string m_backgroundHexColor;
			[ShowInInspector,JsonIgnore,KZHexColor]
			public string BackgroundHexColor
			{
				get => m_backgroundHexColor;
				private set
				{
					if(m_backgroundHexColor == value)
					{
						return;
					}

					m_backgroundHexColor = value;
				}
			}

			[SerializeField,HideInInspector,JsonProperty(nameof(Alignment))]
			private TextAnchor m_alignment;
			[ShowInInspector,JsonIgnore]
			public TextAnchor Alignment
			{
				get => m_alignment;
				private set
				{
					if(m_alignment == value)
					{
						return;
					}

					m_alignment = value;
				}
			}

			[SerializeField,HideInInspector,JsonProperty(nameof(Style))]
			private FontStyle m_style;
			[ShowInInspector,JsonIgnore]
			public FontStyle Style
			{
				get => m_style;
				private set
				{
					if(m_style == value)
					{
						return;
					}

					m_style = value;
				}
			}

			[JsonIgnore]
			public Color BackgroundColor => BackgroundHexColor.IsEmpty() ? Color.white : BackgroundHexColor.ToColor();
		}
		#endregion Prefix Design Info

		private record HierarchyInfo(int TreeLevel,int TreeGroup,bool HasChild,bool IsLast);

		private static CustomEditorData s_customData = null;

		private static CustomEditorData CustomData
		{
			get
			{
				return s_customData ??= _LoadInfo<CustomEditorData>();
			}
		}

		private static readonly Dictionary<int,HierarchyInfo> s_hierarchyInfoDict = new();

		[InitializeOnLoadMethod]
		private static void _Initialize()
		{
			_SetHierarchy();
		}

		private static void _SetHierarchy()
		{
			// Remove Overlap
			EditorApplication.hierarchyWindowItemOnGUI -= _OnDrawHierarchy;
			EditorApplication.hierarchyChanged -= _OnUpdateHierarchy;

			if(CustomData.UseHierarchy)
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

		private static void _OnDrawHierarchy(int instanceId,Rect rect)
		{
			var customData = CustomData;
			
			if(!customData.UseHierarchy || !s_hierarchyInfoDict.TryGetValue(instanceId,out var hierarchyInfo))
			{
				return;
			}

			if(customData.UsePrefixDesign)
			{
				var instance = EditorUtility.EntityIdToObject(instanceId) as GameObject;

				if(instance != null)
				{
					_DrawPrefixDesign(instance,rect);
				}
			}

			if(customData.UseIcon)
			{
				var instance = EditorUtility.EntityIdToObject(instanceId) as GameObject;

				if(instance != null)
				{
					_DrawIcon(instance,rect);
				}
			}

			if(customData.UseBranchTree)
			{
				_DrawBranchTree(hierarchyInfo,rect);
			}
		}

		private static void _OnUpdateHierarchy()
		{
			if(!CustomData.UseHierarchy)
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

			s_hierarchyInfoDict.Add(instanceId,new HierarchyInfo(treeLevel,treeGroup,childCount > 0,isLastChild));

			for(var i=0;i<childCount;i++)
			{
				_CheckGameObject(gameObject.transform.GetChild(i).gameObject,treeLevel+1,treeGroup,i == childCount-1);
			}
		}

		#region Draw Branch Tree
		private static void _DrawBranchTree(HierarchyInfo hierarchyInfo,Rect rect)
		{
			if(!CustomData.UseBranchTree || hierarchyInfo.TreeLevel < 0 || rect.x < 60)
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

			EditorGUI.DrawRect(new Rect(x,y,2.0f,height),CustomData.BranchTreeColor);
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

			EditorGUI.DrawRect(new Rect(x,y,width,2.0f),CustomData.BranchTreeColor);
		}

		private static float _GetTreeStartX(Rect rect,int treeLevel)
		{
			return c_headSpace+(rect.height-2.0f)*treeLevel;
		}
		#endregion Draw Branch Tree

		#region Draw Prefix Design
		private static void _DrawPrefixDesign(GameObject instance,Rect rect)
		{
			var instanceName = instance.name;
			var designInfoList = CustomData.DesignInfoList;

			bool _FindName(PrefixDesignInfo designInfo)
			{
				var prefixKey = designInfo.PrefixKey;

				return !prefixKey.IsEmpty() && instanceName.StartsWith(prefixKey);
			}

			var index = designInfoList.FindIndex(_FindName);

			if(index != -1)
			{
				var designInfo = designInfoList[index];

				var backgroundColor = Selection.activeGameObject == instance ? designInfo.BackgroundColor.InvertColor() : designInfo.BackgroundColor;
				var newName = instanceName[designInfo.PrefixKey.Length..];

				EditorGUI.DrawRect(rect,backgroundColor);

				EditorGUI.LabelField(rect,newName,new GUIStyle()
				{
					fontStyle = designInfo.Style,
					alignment = designInfo.Alignment,
					normal = new GUIStyleState() { textColor = backgroundColor.grayscale > 0.5f ? Color.black : Color.white },
				});
			}
		}
		#endregion Draw Prefix Design

		#region Draw Icon
		private static void _DrawIcon(GameObject instance,Rect rect)
		{
			var content = _GenerateContent(instance);

			if(content == null || content.image == null)
			{
				return;
			}

			var cachedColor = GUI.color;

			if(!instance.activeInHierarchy)
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

		private static void _ResetCustomData()
		{
			if(!KZEditorKit.DisplayCheckBeforeExecute("Reset CustomData"))
			{
				return;
			}

			EditorPrefs.DeleteKey(c_editorText);

			s_customData = null;

			_Initialize();

			KZEditorKit.DisplayInfo("CustomData reset complete");
		}

		private static void _SaveCustomData()
		{
			try
			{
				var text = JsonConvert.SerializeObject(CustomData);

				EditorPrefs.SetString(c_editorText,text);

				_SetHierarchy();
			}
			catch(Exception exception)
			{
				LogChannel.System.E($"Set editorPrefs failed. [{c_editorText}/{CustomData} - {exception.Message}]");
			}
		}

		public static void ShowCustom()
		{
			var window = OdinEditorWindow.InspectObject(CustomData);

			window.titleContent = new GUIContent("EditorCustomWindow");

			window.Show();
		}
	}
}
#endif