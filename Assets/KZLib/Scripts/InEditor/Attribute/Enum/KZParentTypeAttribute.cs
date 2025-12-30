using System;
using System.Diagnostics;

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZParentTypeAttribute : Attribute
	{
		public int ParentType { get; private set; }

		public KZParentTypeAttribute(int parentType)
		{
			ParentType = parentType;
		}
	}
}