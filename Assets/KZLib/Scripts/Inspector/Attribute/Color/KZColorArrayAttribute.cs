using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>
	/// Displays a <see cref="UnityEngine.Color"/> array as horizontal swatches (read-only preview).
	/// </summary>
	/// <example><code>[KZColorArray] public Color[] palette;</code></example>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZColorArrayAttribute : Attribute { }
}
