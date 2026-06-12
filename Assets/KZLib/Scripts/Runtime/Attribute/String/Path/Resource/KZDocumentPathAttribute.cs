using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZDocumentPathAttribute : KZResourcePathAttribute
	{
		public KZDocumentPathAttribute(bool changePathBtn = true,bool isIncludeAssets = false) : base(changePathBtn,isIncludeAssets) { }
	}
}