#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;

namespace KZLib.Attributes
{
	/// <summary><see cref="KZFolderPathAttribute"/> drawer.</summary>
	public class KZFolderPathAttributeDrawer : KZPathAttributeDrawer<KZFolderPathAttribute>
	{
		protected override string FindNewPath() => KZEditorKit.OpenFolderPanel("Change new path.");

		protected override Rect OnClickToOpen(Rect rect,bool isValid)
		{
			void _ClickButton() => KZEditorKit.Open(AbsolutePath);

			return DrawButton(rect,SdfIconType.Folder2,isValid,_ClickButton);
		}

		protected override bool IsValidPath() => KZFileKit.IsFolderExist(ValueEntry.SmartValue);
	}
}
#endif
