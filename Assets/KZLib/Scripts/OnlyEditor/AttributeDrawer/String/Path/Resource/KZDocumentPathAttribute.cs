#if UNITY_EDITOR
using Sirenix.OdinInspector;

namespace KZLib.Attributes
{
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