using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	/// <summary>
	/// Assigns a parent category to an enum field or enum member.
	/// </summary>
	/// <remarks>
	/// On a field: only enum members matching <see cref="ParentType"/> appear in the dropdown.<br/>
	/// On an enum member: marks which parent category the member belongs to.
	/// </remarks>
	/// <example>
	/// <code>
	/// public enum ItemType {
	///     [KZParentType(1)] Weapon_Sword,
	///     [KZParentType(2)] Armor_Helmet,
	/// }
	/// [KZParentType(1)] public ItemType weapon;
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZParentTypeAttribute : Attribute
	{
		/// <summary>Parent category identifier.</summary>
		public int ParentType { get; }

		public KZParentTypeAttribute(int parentType)
		{
			ParentType = parentType;
		}
	}
}
