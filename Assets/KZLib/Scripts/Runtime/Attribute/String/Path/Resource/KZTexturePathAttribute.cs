using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZTexturePathAttribute : KZResourcePathAttribute
	{
		public KZTexturePathAttribute(bool changePathButton = false,bool newLine = false) : base(changePathButton,newLine) { }
	}
}