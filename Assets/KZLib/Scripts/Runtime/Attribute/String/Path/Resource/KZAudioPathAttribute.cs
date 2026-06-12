using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZAudioClipPathAttribute : KZResourcePathAttribute
	{
		public KZAudioClipPathAttribute(bool changePathButton = false) : base(changePathButton,true) { }
	}
}