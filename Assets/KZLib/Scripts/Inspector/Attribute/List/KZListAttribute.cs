using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>
	/// Edits a fixed-length list inline with a count field and horizontally laid-out elements.
	/// </summary>
	/// <remarks>Supports <see cref="System.Collections.Generic.List{T}"/> of int, float, and string.</remarks>
	/// <example><code>[KZList] public List&lt;int&gt; values;</code></example>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZListAttribute : Attribute { }
}
