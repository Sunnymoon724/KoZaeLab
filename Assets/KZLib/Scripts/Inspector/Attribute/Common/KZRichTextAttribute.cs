using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>
	/// Renders a field as a read-only rich-text label. Does not allow editing.
	/// </summary>
	/// <remarks>
	/// When <see cref="TextFormat"/> is null, the drawer uses a type-specific default format.
	/// </remarks>
	/// <example><code>[KZRichText("HP: {0}")] public int hp;</code></example>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZRichTextAttribute : Attribute
	{
		/// <summary><see cref="string.Format(string,object)"/> pattern. Null uses drawer default.</summary>
		public string TextFormat { get; }

		public KZRichTextAttribute(string textFormat = null)
		{
			TextFormat = textFormat;
		}
	}
}
