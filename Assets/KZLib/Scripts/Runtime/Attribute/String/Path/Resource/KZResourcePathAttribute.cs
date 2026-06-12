using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public abstract class KZResourcePathAttribute : KZPathAttribute
	{
		protected KZResourcePathAttribute(bool changePathBtn,bool isIncludeAssets) : base(true,changePathBtn,isIncludeAssets) { }
	}
}