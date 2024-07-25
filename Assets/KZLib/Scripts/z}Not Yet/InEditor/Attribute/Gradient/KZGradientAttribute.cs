using System;
using UnityEngine;
using System.Diagnostics;

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZGradientAttribute : Attribute { }

#if UNITY_EDITOR
	public class KZGradientAttributeDrawer : KZAttributeDrawer<KZGradientAttribute,Gradient>
	{
		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			//? label
			var rect = DrawPrefixLabel(_label);
			var gradient = ValueEntry.SmartValue;

			if(gradient == null)
			{
				DrawColor(rect,Color.white);

				return;
			}

			for(var i=0.0f;i<rect.width;i++)
			{
				DrawColor(new Rect(rect.x+i,rect.y,1.0f,rect.height),gradient.Evaluate(i/rect.width));
			}
		}
	}
#endif
}