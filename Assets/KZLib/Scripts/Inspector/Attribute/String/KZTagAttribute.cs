using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>
	/// Edits a <see cref="string"/> field via the Unity project Tag dropdown.
	/// </summary>
	/// <example><code>[KZTag] public string layerTag;</code></example>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZTagAttribute : Attribute { }
}
