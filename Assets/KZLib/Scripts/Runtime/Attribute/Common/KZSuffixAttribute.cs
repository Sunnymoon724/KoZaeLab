using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>
	/// For suffix
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZSuffixAttribute : Attribute
	{
		public string SuffixText { get; }

		public KZSuffixAttribute(string suffixText)
		{
			SuffixText = suffixText;
		}
	}
}