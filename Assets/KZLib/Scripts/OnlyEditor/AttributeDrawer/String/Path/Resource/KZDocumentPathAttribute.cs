#if UNITY_EDITOR
using Sirenix.OdinInspector;

namespace KZLib.Attributes
{
	/// <summary><see cref="KZDocumentPathAttribute"/> drawer. Opens the document with the OS default app.</summary>
	public class KZDocumentPathAttributeDrawer : KZResourcePathAttributeDrawer<KZDocumentPathAttribute>
	{
		protected override SdfIconType IconType => SdfIconType.FileEarmark;

		protected override string ResourceKind => null;

		protected override void OnOpenResource()
		{
			KZEditorKit.Open(AbsolutePath);
		}
	}
}
#endif