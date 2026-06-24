using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>
	/// Displays a suffix label to the right of a numeric or string field.
	/// </summary>
	/// <example><code>[KZSuffix("px")] public int width;</code></example>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZSuffixAttribute : Attribute
	{
		/// <summary>Text shown to the right of the field.</summary>
		public string SuffixText { get; }

		public KZSuffixAttribute(string suffixText)
		{
			SuffixText = suffixText;
		}
	}
}
