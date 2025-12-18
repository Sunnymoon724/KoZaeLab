using System;
using UnityEngine;
using System.Diagnostics;

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZColorArrayAttribute : Attribute { }

#if UNITY_EDITOR
	public class KZColorArrayAttributeDrawer : KZAttributeDrawer<KZColorArrayAttribute,Color[]>
	{
		protected override void _DoDrawPropertyLayout(GUIContent label)
		{
			var rect = _DrawPrefixLabel(label);
			var colorArray = ValueEntry.SmartValue;

			if(colorArray.IsNullOrEmpty())
			{
				return;
			}

			var rectArray = _GetRectArray(rect,colorArray.Length);

			for(var i=0;i<colorArray.Length;i++)
			{
				_DrawColor(rectArray[i],colorArray[i]);
			}
		}
	}
#endif
}