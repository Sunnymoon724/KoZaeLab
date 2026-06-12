using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZScriptableObjectPathAttribute : KZResourcePathAttribute
	{
		public KZScriptableObjectPathAttribute(bool changePathButton = false,bool newLine = false) : base(changePathButton,newLine) { }
	}
}