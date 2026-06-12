using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZRichTextAttribute : Attribute
	{
		public string TextFormat { get; }

		public KZRichTextAttribute(string textFormat = null)
		{
			TextFormat = textFormat;
		}
	}
}