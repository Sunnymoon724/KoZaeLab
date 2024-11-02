using System;
using UnityEngine;
using System.Diagnostics;
using Sirenix.OdinInspector;

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
		protected override string GetNewPath()
		{
			return CommonUtility.GetFolderPathInPanel("Change new path.");
		}

		protected override Rect OnClickToOpen(Rect _rect,bool _isValid)
		{
			return DrawButton(_rect,SdfIconType.Folder2,_isValid,()=>
			{
				CommonUtility.Open(CommonUtility.GetAbsolutePath(ValueEntry.SmartValue,Attribute.IsIncludeAssets));
			});
		}

		protected override bool IsValidPath()
		{
			return CommonUtility.IsFolderExist(ValueEntry.SmartValue);
		}
    }
#endif
}