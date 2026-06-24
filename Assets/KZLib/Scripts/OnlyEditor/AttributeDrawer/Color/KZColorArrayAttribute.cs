#if UNITY_EDITOR
using UnityEngine;

namespace KZLib.Attributes
{
	/// <summary><see cref="KZColorArrayAttribute"/> drawer. Displays colors as horizontal swatches.</summary>
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
}
#endif
