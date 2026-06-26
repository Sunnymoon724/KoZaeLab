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
	public static class EditorCustom
	{
		private const float c_iconSpace = 7.0f;
		private const int c_iconSize = 10;

		private const float c_hierarchyFoldoutWidthFactor = 1.0f;
		private const float c_hierarchyBuiltinIconWidthFactor = 1.0f;
		private const float c_hierarchyMinDrawXFactor = 3.5f;
		private const float c_branchLineThickness = 2.0f;
		private const float c_branchLineHalfHeightDivisor = 2.0f;
		private const float c_branchHorizontalShrinkWithChild = 5.0f;
		private const float c_branchHorizontalExtendWithoutChild = 2.0f;
		private const float c_inactiveIconAlpha = 0.5f;
		private const float c_lightBackgroundGrayscaleThreshold = 0.5f;

		private const int c_prefabRootTreeLevelWithParent = 1;
		private const int c_prefabRootTreeLevelWithoutParent = 0;
		private const int c_sceneRootTreeLevel = 0;
		private const int c_minimumComponentCountForIcon = 2;
		private const int c_firstNonTransformComponentIndex = 1;

		private const string c_editorText = "[Editor] CustomData";
		private const string c_customWindowTitle = "EditorCustomWindow";

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
			[VerticalGroup("Hierarchy/Use",Order = 0),ToggleLeft,ShowInInspector,JsonIgnore,OnValueChanged(nameof(_OnHierarchyOptionChanged))]
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
			[VerticalGroup("Hierarchy/Data/0"),ToggleLeft,ShowInInspector,JsonIgnore,ShowIf(nameof(UseHierarchy)),OnValueChanged(nameof(_OnHierarchyOptionChanged))]
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
			[VerticalGroup("Hierarchy/Data/0"),KZHexColor,ShowInInspector,JsonIgnore,ShowIf(nameof(UseBranchTree)),OnValueChanged(nameof(_OnHierarchyRepaint))]
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
			[VerticalGroup("Hierarchy/Data/1"),ToggleLeft,ShowInInspector,JsonIgnore,ShowIf(nameof(UseHierarchy)),OnValueChanged(nameof(_OnHierarchyOptionChanged))]
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

			[VerticalGroup("Hierarchy/Data/1"),ShowInInspector,JsonIgnore,ShowIf(nameof(UsePrefixDesign)),OnValueChanged(nameof(_OnHierarchyOptionChanged))]
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
			[VerticalGroup("Hierarchy/Data/2"),ToggleLeft,ShowInInspector,JsonIgnore,ShowIf(nameof(UseHierarchy)),OnValueChanged(nameof(_OnHierarchyOptionChanged))]
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

			private void _OnHierarchyOptionChanged()
			{
				_SetHierarchy();
			}

			private void _OnHierarchyRepaint()
			{
				EditorApplication.RepaintHierarchyWindow();
			}
		}
		#endregion Custom Editor Data

		#region Prefix Design Info
		[Serializable]
		private struct PrefixDesignInfo
		{
			[SerializeField,HideInInspector,JsonProperty(nameof(PrefixKey))]
			private string m_prefixKey;
			[ShowInInspector,JsonIgnore,OnValueChanged(nameof(_NotifyDesignInfoChanged))]
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
			[ShowInInspector,JsonIgnore,KZHexColor,OnValueChanged(nameof(_NotifyDesignInfoChanged))]
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
			[ShowInInspector,JsonIgnore,OnValueChanged(nameof(_NotifyDesignInfoChanged))]
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
			[ShowInInspector,JsonIgnore,OnValueChanged(nameof(_NotifyDesignInfoChanged))]
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

			private void _NotifyDesignInfoChanged()
			{
				_OnDesignInfoChanged();
			}
		}
		#endregion Prefix Design Info

		private record HierarchyInfo(int TreeLevel,bool HasChild,bool IsLast);

		private readonly struct PrefixDrawInfo
		{
			public readonly int DesignIndex;
			public readonly string DisplayName;

			public PrefixDrawInfo(int designIndex,string displayName)
			{
				DesignIndex = designIndex;
				DisplayName = displayName;
			}
		}

		private sealed class HierarchyDrawCache
		{
			public HierarchyInfo Tree;
			public PrefixDrawInfo? Prefix;
			public GUIContent Icon;
			public bool ActiveInHierarchy;
			public int ComponentCount;
			public int IconSourceInstanceId;
		}

		private static CustomEditorData s_customData = null;

		private static CustomEditorData CustomData
		{
			get
			{
				return s_customData ??= _LoadInfo<CustomEditorData>();
			}
		}

		private static readonly Dictionary<int,HierarchyDrawCache> s_drawCacheDict = new();
		private static readonly Dictionary<(FontStyle,TextAnchor,bool),GUIStyle> s_prefixStyleCache = new();

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
			ObjectChangeEvents.changesPublished -= _OnObjectChanged;

			if(CustomData.UseHierarchy)
			{
				EditorApplication.hierarchyWindowItemOnGUI += _OnDrawHierarchy;
				EditorApplication.hierarchyChanged += _OnUpdateHierarchy;
				ObjectChangeEvents.changesPublished += _OnObjectChanged;

				_OnUpdateHierarchy();
			}

			EditorApplication.RepaintHierarchyWindow();
		}

		private static TInfo _LoadInfo<TInfo>() where TInfo : new()
		{
			var text = EditorPrefs.GetString(c_editorText,"");

			if(text.IsEmpty())
			{
				return new TInfo();
			}

			try
			{
				var info = JsonConvert.DeserializeObject<TInfo>(text);

				return info ?? new TInfo();
			}
			catch(Exception exception)
			{
				LogChannel.Editor.E($"Get editorPrefs failed. [{c_editorText}/{typeof(TInfo).Name} - {exception.Message}]");

				return new TInfo();
			}
		}

		private static void _OnDrawHierarchy(int instanceId,Rect rect)
		{
			var customData = CustomData;

			if(!customData.UseHierarchy || !s_drawCacheDict.TryGetValue(instanceId,out var cache))
			{
				return;
			}

			if(customData.UsePrefixDesign && cache.Prefix is { } prefix)
			{
				_DrawPrefixDesign(prefix,rect,Selection.activeEntityId == instanceId,customData.UseIcon);
			}

			if(customData.UseIcon)
			{
				_TryRefreshIconCache(instanceId,cache);
				_DrawIcon(cache.Icon,cache.ActiveInHierarchy,rect);
			}

			if(customData.UseBranchTree)
			{
				_DrawBranchTree(cache.Tree,rect);
			}
		}

		private static void _OnUpdateHierarchy()
		{
			if(!CustomData.UseHierarchy)
			{
				return;
			}

			s_drawCacheDict.Clear();

			var prefab = PrefabStageUtility.GetCurrentPrefabStage();

			if(prefab)
			{
				var root = prefab.prefabContentsRoot;
				var level = root.transform.parent ? c_prefabRootTreeLevelWithParent : c_prefabRootTreeLevelWithoutParent;

				_CheckGameObject(root,level,true);

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
					_CheckGameObject(objectArray[j],c_sceneRootTreeLevel,j == objectArray.Length-1);
				}
			}
		}

		private static void _CheckGameObject(GameObject gameObject,int treeLevel,bool isLastChild)
		{
			var instanceId = gameObject.GetInstanceID();

			if(s_drawCacheDict.ContainsKey(instanceId))
			{
				return;
			}

			var childCount = gameObject.transform.childCount;

			var cache = new HierarchyDrawCache()
			{
				Tree = new HierarchyInfo(treeLevel,childCount > 0,isLastChild),
			};

			_ApplyDrawCache(gameObject,cache);
			s_drawCacheDict.Add(instanceId,cache);

			for(var i=0;i<childCount;i++)
			{
				_CheckGameObject(gameObject.transform.GetChild(i).gameObject,treeLevel+1,i == childCount-1);
			}
		}

		private static void _OnDesignInfoChanged()
		{
			if(!CustomData.UseHierarchy)
			{
				return;
			}

			_OnUpdateHierarchy();
			EditorApplication.RepaintHierarchyWindow();
		}

		private static PrefixDrawInfo? _FindPrefixDesign(string instanceName)
		{
			var designInfoList = CustomData.DesignInfoList;
			var matchIndex = -1;
			var matchLength = -1;

			for(var i=0;i<designInfoList.Count;i++)
			{
				var prefixKey = designInfoList[i].PrefixKey;

				if(prefixKey.IsEmpty() || !instanceName.StartsWith(prefixKey,StringComparison.OrdinalIgnoreCase) || prefixKey.Length <= matchLength)
				{
					continue;
				}

				matchIndex = i;
				matchLength = prefixKey.Length;
			}

			if(matchIndex == -1)
			{
				return null;
			}

			return new PrefixDrawInfo(matchIndex,instanceName[matchLength..]);
		}

		private static void _OnObjectChanged(ref ObjectChangeEventStream stream)
		{
			if(!CustomData.UseHierarchy)
			{
				return;
			}

			var repaint = false;

			for(var i=0;i<stream.length;i++)
			{
				var instanceId = _GetChangedInstanceId(ref stream,i);

				if(instanceId == 0)
				{
					continue;
				}

				if(_RefreshDrawCacheEntry(instanceId))
				{
					repaint = true;
				}
			}

			if(repaint)
			{
				EditorApplication.RepaintHierarchyWindow();
			}
		}

		private static int _GetChangedInstanceId(ref ObjectChangeEventStream stream,int index)
		{
			switch(stream.GetEventType(index))
			{
				case ObjectChangeKind.ChangeGameObjectStructure:
				{
					stream.GetChangeGameObjectStructureEvent(index,out var changeEvent);

					return changeEvent.instanceId;
				}
				case ObjectChangeKind.ChangeGameObjectOrComponentProperties:
				{
					stream.GetChangeGameObjectOrComponentPropertiesEvent(index,out var changeEvent);

					return changeEvent.instanceId;
				}
				default:
					return 0;
			}
		}

		private static bool _RefreshDrawCacheEntry(int instanceId)
		{
			if(!s_drawCacheDict.TryGetValue(instanceId,out var cache))
			{
				return false;
			}

			var gameObject = EditorUtility.EntityIdToObject(instanceId) as GameObject;

			if(gameObject == null)
			{
				s_drawCacheDict.Remove(instanceId);

				return true;
			}

			_ApplyDrawCache(gameObject,cache);

			return true;
		}

		private static void _ApplyDrawCache(GameObject gameObject,HierarchyDrawCache cache)
		{
			var customData = CustomData;

			cache.Prefix = customData.UsePrefixDesign ? _FindPrefixDesign(gameObject.name) : null;
			cache.Icon = customData.UseIcon ? _GenerateContent(gameObject,out cache.IconSourceInstanceId) : null;
			cache.ActiveInHierarchy = gameObject.activeInHierarchy;
			cache.ComponentCount = gameObject.GetComponentCount();

			if(!customData.UseIcon)
			{
				cache.IconSourceInstanceId = 0;
			}
		}

		private static void _TryRefreshIconCache(int instanceId,HierarchyDrawCache cache)
		{
			var gameObject = EditorUtility.EntityIdToObject(instanceId) as GameObject;

			if(gameObject == null)
			{
				return;
			}

			if(!CustomData.UseIcon)
			{
				return;
			}

			var componentCount = gameObject.GetComponentCount();
			var activeInHierarchy = gameObject.activeInHierarchy;
			var icon = _GenerateContent(gameObject,out var iconSourceInstanceId);

			if(componentCount == cache.ComponentCount && activeInHierarchy == cache.ActiveInHierarchy && iconSourceInstanceId == cache.IconSourceInstanceId)
			{
				return;
			}

			cache.ComponentCount = componentCount;
			cache.ActiveInHierarchy = activeInHierarchy;
			cache.IconSourceInstanceId = iconSourceInstanceId;
			cache.Icon = icon;
		}

		private static float _GetHierarchyRowLeadingWidth(Rect rect) => rect.height*(c_hierarchyFoldoutWidthFactor+c_hierarchyBuiltinIconWidthFactor);

		private static float _GetHierarchyMinDrawX(Rect rect) => rect.height*c_hierarchyMinDrawXFactor;

		private static Rect _GetPrefixDesignRect(Rect rect,bool useCustomIcon)
		{
			var offset = _GetHierarchyRowLeadingWidth(rect);

			if(useCustomIcon)
			{
				offset += c_iconSpace+c_iconSize+c_iconSpace;
			}

			return new Rect(rect.x+offset,rect.y,Mathf.Max(0.0f,rect.width-offset),rect.height);
		}

		#region Draw Branch Tree
		private static void _DrawBranchTree(HierarchyInfo hierarchyInfo,Rect rect)
		{
			if(!CustomData.UseBranchTree || hierarchyInfo.TreeLevel < 0 || rect.x < _GetHierarchyMinDrawX(rect))
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
			var height = rect.height/c_branchLineHalfHeightDivisor;
			var x = _GetTreeStartX(rect,treeLevel);
			var y = isUpper ? rect.y : (rect.y+height);

			EditorGUI.DrawRect(new Rect(x,y,c_branchLineThickness,height),CustomData.BranchTreeColor);
		}

		/// <summary>
		/// ----
		/// </summary>
		//! Horizontal
		private static void _DrawHorizontalLine(Rect rect,int treeLevel,bool hasChild)
		{
			var x = _GetTreeStartX(rect,treeLevel);
			var y = rect.y+rect.height/c_branchLineHalfHeightDivisor;
			var width = rect.height+(hasChild ? -c_branchHorizontalShrinkWithChild : c_branchHorizontalExtendWithoutChild);

			EditorGUI.DrawRect(new Rect(x,y,width,c_branchLineThickness),CustomData.BranchTreeColor);
		}

		private static float _GetTreeStartX(Rect rect,int treeLevel)
		{
			return _GetHierarchyRowLeadingWidth(rect)+(rect.height-c_branchLineThickness)*treeLevel;
		}
		#endregion Draw Branch Tree

		#region Draw Prefix Design
		private static void _DrawPrefixDesign(PrefixDrawInfo prefixDrawInfo,Rect rect,bool isSelected,bool useCustomIcon)
		{
			var designInfo = CustomData.DesignInfoList[prefixDrawInfo.DesignIndex];
			var backgroundColor = isSelected ? designInfo.BackgroundColor.InvertColor() : designInfo.BackgroundColor;
			var darkText = backgroundColor.grayscale <= c_lightBackgroundGrayscaleThreshold;
			var designRect = _GetPrefixDesignRect(rect,useCustomIcon);

			EditorGUI.DrawRect(rect,backgroundColor);
			EditorGUI.LabelField(designRect,prefixDrawInfo.DisplayName,_GetPrefixStyle(designInfo.Style,designInfo.Alignment,darkText));
		}

		private static GUIStyle _GetPrefixStyle(FontStyle style,TextAnchor alignment,bool darkText)
		{
			var key = (style,alignment,darkText);

			if(!s_prefixStyleCache.TryGetValue(key,out var guiStyle))
			{
				guiStyle = new GUIStyle()
				{
					fontStyle = style,
					alignment = alignment,
					normal = new GUIStyleState() { textColor = darkText ? Color.white : Color.black },
				};

				s_prefixStyleCache[key] = guiStyle;
			}

			return guiStyle;
		}
		#endregion Draw Prefix Design

		#region Draw Icon
		private static void _DrawIcon(GUIContent content,bool activeInHierarchy,Rect rect)
		{
			if(content == null || content.image == null)
			{
				return;
			}

			var cachedColor = GUI.color;

			if(!activeInHierarchy)
			{
				GUI.color = cachedColor.MaskAlpha(c_inactiveIconAlpha);
			}

			var iconRect = new Rect(rect.x+c_iconSpace,rect.y+c_iconSpace,c_iconSize,c_iconSize);

			EditorGUI.LabelField(iconRect,content);

			GUI.color = cachedColor;
		}

		private static GUIContent _GenerateContent(GameObject gameObject,out int sourceInstanceId)
		{
			sourceInstanceId = 0;

			var componentArray = gameObject.GetComponents<Component>();

			if(componentArray == null || componentArray.Length < c_minimumComponentCountForIcon)
			{
				return null;
			}

			var component = componentArray[c_firstNonTransformComponentIndex];

			if(component == null)
			{
				return null;
			}

			var componentType = component.GetType();

			if(componentType == typeof(CanvasRenderer))
			{
				component = componentArray[^1];
				componentType = component.GetType();
			}

			sourceInstanceId = component.GetInstanceID();

			var content = EditorGUIUtility.ObjectContent(component,componentType);
			content.text = null;
			content.tooltip = "";

			return content;
		}
		#endregion Draw Icon

		private static void _ResetCustomData()
		{
			if(!KZEditorKit.DisplayConfirm("Reset CustomData"))
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
				LogChannel.Editor.E($"Set editorPrefs failed. [{c_editorText}/{CustomData} - {exception.Message}]");
			}
		}

		public static void ShowCustom()
		{
			var window = OdinEditorWindow.InspectObject(CustomData);

			window.titleContent = new GUIContent(c_customWindowTitle);
			window.Show();
			window.Focus();
		}
	}
}
#endif