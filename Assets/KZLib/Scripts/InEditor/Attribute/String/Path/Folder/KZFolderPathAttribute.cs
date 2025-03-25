using System;
using UnityEngine;
using System.Diagnostics;
using Sirenix.OdinInspector;
using KZLib.KZUtility;

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZFolderPathAttribute : KZPathAttribute
	{
		public KZFolderPathAttribute(bool _addChangePathBtn = true,bool _includeProject = true) : base(true,_addChangePathBtn,_includeProject) { }
	}

#if UNITY_EDITOR
	public class KZFolderPathAttributeDrawer : KZPathAttributeDrawer<KZFolderPathAttribute>
	{
		protected override string FindNewPath()
		{
			return CommonUtility.FindFolderPathInPanel("Change new path.");
		}

		protected override Rect OnClickToOpen(Rect rect,bool isValid)
		{
			return DrawButton(rect,SdfIconType.Folder2,isValid,()=>
			{
				CommonUtility.Open(AbsolutePath);
			});
		}

		protected override bool IsValidPath()
		{
			return FileUtility.IsFolderExist(ValueEntry.SmartValue);
		}
	}
#endif
}