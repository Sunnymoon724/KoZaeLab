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
		protected override void _DrawPropertyLayout(GUIContent label)
		{
			var rect = DrawPrefixLabel(label);
			var colorArray = ValueEntry.SmartValue;

			if(colorArray.IsNullOrEmpty())
			{
				return;
			}

			var rectArray = GetRectArray(rect,colorArray.Length);

			for(var i=0;i<colorArray.Length;i++)
			{
				DrawColor(rectArray[i],colorArray[i]);
			}
		}
	}
#endif
}