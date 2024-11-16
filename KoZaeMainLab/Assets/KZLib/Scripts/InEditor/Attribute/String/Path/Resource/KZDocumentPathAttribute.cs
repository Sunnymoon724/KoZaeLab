using System;
using System.Diagnostics;
using Sirenix.OdinInspector;

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZDocumentPathAttribute : KZResourcePathAttribute
	{
		public KZDocumentPathAttribute(bool _changePathBtn = true,bool _isIncludeAssets = false) : base(_changePathBtn,_isIncludeAssets) { }
	}

#if UNITY_EDITOR
	public class KZDocumentPathAttributeDrawer : KZResourcePathAttributeDrawer<KZDocumentPathAttribute>
	{
		protected override SdfIconType IconType => SdfIconType.FileEarmark;

		protected override string ResourceKind => null;

		protected override void OnOpenResource()
		{
			CommonUtility.Open(CommonUtility.GetAbsolutePath(ValueEntry.SmartValue,Attribute.IsIncludeAssets));
		}
	}
#endif
}