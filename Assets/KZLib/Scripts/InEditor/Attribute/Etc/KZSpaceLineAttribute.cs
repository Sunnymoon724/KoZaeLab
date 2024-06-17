using UnityEngine;
using System;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = true,Inherited = true)]
	public class KZSpaceLineAttribute : PropertyAttribute
	{
		public float Space { get; }
		public Color LineColor { get; }

		public KZSpaceLineAttribute(float _space,Color _color)
		{
			Space = _space;
			LineColor = _color;
		}

		public KZSpaceLineAttribute() : this(10.0f,Color.white) { }

		public KZSpaceLineAttribute(float _space) : this(_space,Color.white) { }
		public KZSpaceLineAttribute(Color _color) : this(10.0f,_color) { }
	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(KZSpaceLineAttribute))]
	public class KZSpaceLineAttributeDrawer : DecoratorDrawer
	{
		KZSpaceLineAttribute SpaceLine => attribute as KZSpaceLineAttribute;

		public override void OnGUI(Rect _rect)
		{
			EditorGUI.DrawRect(new Rect(_rect.x,_rect.y+SpaceLine.Space/2.0f,_rect.width,1.0f),SpaceLine.LineColor);
		}

		public override float GetHeight()
		{
			return SpaceLine.Space;
		}
	}
#endif
}