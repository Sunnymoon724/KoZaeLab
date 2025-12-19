#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using KZLib.KZAttribute;
using KZLib.Tet;

namespace KZLib.KZWindow
{
	public class EditorCustomWindow : OdinEditorWindow
	{
		[TitleGroup("Option",BoldTitle = false,Order = 0)]
		[VerticalGroup("Option/Button",Order = 0),Button("Reset Custom",ButtonSizes.Large)]
		protected void OnResetCustom()
		{
			EditorCustom.ResetCustom();
		}

		[TitleGroup("Hierarchy",BoldTitle = false,Order = 1)]
		[VerticalGroup("Hierarchy/Use",Order = 0),ToggleLeft,ShowInInspector]
		protected bool UseHierarchy { get => EditorCustom.GetCustomInfo<bool>(nameof(UseHierarchy)); set => EditorCustom.SetCustomInfo(nameof(UseHierarchy),value); }

		[BoxGroup("Hierarchy/Data",Order = 1,ShowLabel = false)]
		[HorizontalGroup("Hierarchy/Data/0"),ToggleLeft,ShowInInspector,ShowIf(nameof(UseHierarchy))]
		protected bool UseBranchTree { get => EditorCustom.GetCustomInfo<bool>(nameof(UseBranchTree)) && UseHierarchy; set => EditorCustom.SetCustomInfo(nameof(UseBranchTree),value); }

		[HorizontalGroup("Hierarchy/Data/0"),HideLabel,KZHexColor,ShowInInspector,ShowIf(nameof(UseBranchTree))]
		protected string BranchTreeHexColor { get => EditorCustom.GetCustomInfo<string>(nameof(BranchTreeHexColor)); set => EditorCustom.SetCustomInfo(nameof(BranchTreeHexColor),value); }

		[HorizontalGroup("Hierarchy/Data/1"),ToggleLeft,ShowInInspector,ShowIf(nameof(UseHierarchy))]
		protected bool UseCategoryLine { get => EditorCustom.GetCustomInfo<bool>(nameof(UseCategoryLine)) && UseHierarchy; set => EditorCustom.SetCustomInfo(nameof(UseCategoryLine),value); }
		[HorizontalGroup("Hierarchy/Data/1"),HideLabel,KZHexColor,ShowInInspector,ShowIf(nameof(UseCategoryLine))]
		protected string CategoryHexColor { get => EditorCustom.GetCustomInfo<string>(nameof(CategoryHexColor)); set => EditorCustom.SetCustomInfo(nameof(CategoryHexColor),value); }

		[HorizontalGroup("Hierarchy/Data/2"),ToggleLeft,ShowInInspector,ShowIf(nameof(UseHierarchy))]
		protected bool UseIcon{ get => EditorCustom.GetCustomInfo<bool>(nameof(UseIcon)) && UseHierarchy; set => EditorCustom.SetCustomInfo(nameof(UseIcon),value); }
	}
}
#endif