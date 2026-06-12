using System;
using System.Diagnostics;

namespace KZLib.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZFolderPathAttribute : KZPathAttribute
	{
		public KZFolderPathAttribute(bool _addChangePathBtn = true,bool _includeProject = true) : base(true,_addChangePathBtn,_includeProject) { }
	}
}