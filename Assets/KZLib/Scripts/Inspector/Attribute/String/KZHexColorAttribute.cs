using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>
	/// Edits a <see cref="string"/> field as a hex color value via ColorField.
	/// </summary>
	/// <example><code>[KZHexColor] public string tintHex;</code></example>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZHexColorAttribute : Attribute { }
}
