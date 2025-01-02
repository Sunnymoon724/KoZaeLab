using System;
using System.Diagnostics;
using Sirenix.OdinInspector;

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZDocumentPathAttribute : KZResourcePathAttribute
	{
		public KZDocumentPathAttribute(bool changePathBtn = true,bool isIncludeAssets = false) : base(changePathBtn,isIncludeAssets) { }
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