using System;
using System.Diagnostics;
using Sirenix.OdinInspector;

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZDocumentPathAttribute : KZResourcePathAttribute
	{
		public KZDocumentPathAttribute(bool _changePathBtn = true,bool _includeProject = false) : base(_changePathBtn,_includeProject) { }
	}

#if UNITY_EDITOR
	public class KZDocumentPathAttributeDrawer : KZResourcePathAttributeDrawer<KZDocumentPathAttribute>
	{
		protected override SdfIconType IconType => SdfIconType.FileEarmark;

		protected override string ResourceKind => null;

		protected override void OnOpenResource()
		{
			FileUtility.OpenFile(FileUtility.GetFullPath(ValueEntry.SmartValue));
		}
	}
#endif
}