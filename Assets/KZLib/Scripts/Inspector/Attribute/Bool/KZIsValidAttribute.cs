using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>
	/// Displays a <see cref="bool"/> field as a text label (e.g. O/X). Does not allow editing.
	/// </summary>
	/// <example><code>[KZIsValid("Yes", "No")] public bool isReady;</code></example>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZIsValidAttribute : Attribute
	{
		/// <summary>Text when value is <c>true</c>.</summary>
		public string CorrectText { get; }
		/// <summary>Text when value is <c>false</c>.</summary>
		public string WrongText { get; }
		/// <summary>Text color (hex) when <c>false</c>. Null uses <see cref="Global.WrongHexColor"/>.</summary>
		public string WrongHexColor { get; }

		public KZIsValidAttribute(string correctText = "O",string wrongText = "X",string wrongHexColor = null)
		{
			CorrectText = correctText;
			WrongText = wrongText;
			WrongHexColor = wrongHexColor;
		}
	}
}
