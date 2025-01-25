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
		protected bool UseHierarchy { get => EditorCustom.GetCustomData<bool>(nameof(UseHierarchy)); set => EditorCustom.SetCustomData(nameof(UseHierarchy),value); }

		[BoxGroup("Hierarchy/Data",Order = 1,ShowLabel = false)]
		[HorizontalGroup("Hierarchy/Data/0"),ToggleLeft,ShowInInspector,ShowIf(nameof(UseHierarchy))]
		protected bool UseBranchTree { get => EditorCustom.GetCustomData<bool>(nameof(UseBranchTree)) && UseHierarchy; set => EditorCustom.SetCustomData(nameof(UseBranchTree),value); }

		[HorizontalGroup("Hierarchy/Data/0"),HideLabel,KZHexColor,ShowInInspector,ShowIf(nameof(UseBranchTree))]
		protected string BranchTreeHexColor { get => EditorCustom.GetCustomData<string>(nameof(BranchTreeHexColor)); set => EditorCustom.SetCustomData(nameof(BranchTreeHexColor),value); }

		[HorizontalGroup("Hierarchy/Data/1"),ToggleLeft,ShowInInspector,ShowIf(nameof(UseHierarchy))]
		protected bool UseCategoryLine { get => EditorCustom.GetCustomData<bool>(nameof(UseCategoryLine)) && UseHierarchy; set => EditorCustom.SetCustomData(nameof(UseCategoryLine),value); }
		[HorizontalGroup("Hierarchy/Data/1"),HideLabel,KZHexColor,ShowInInspector,ShowIf(nameof(UseCategoryLine))]
		protected string CategoryHexColor { get => EditorCustom.GetCustomData<string>(nameof(CategoryHexColor)); set => EditorCustom.SetCustomData(nameof(CategoryHexColor),value); }

		[HorizontalGroup("Hierarchy/Data/2"),ToggleLeft,ShowInInspector,ShowIf(nameof(UseHierarchy))]
		protected bool UseIcon{ get => EditorCustom.GetCustomData<bool>(nameof(UseIcon)) && UseHierarchy; set => EditorCustom.SetCustomData(nameof(UseIcon),value); }
	}
}
#endif