using System;
using UnityEngine;
using System.Diagnostics;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib.KZAttribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = false,Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class KZRichTextAttribute : Attribute
	{
		public string TextFormat { get; }

		public KZRichTextAttribute(string _textFormat = null)
		{
			TextFormat = _textFormat;
		}
	}

#if UNITY_EDITOR
	public abstract class KZRichTextAttributeDrawer<TValue> : KZAttributeDrawer<KZRichTextAttribute,TValue>
	{
		protected string m_TextFormat;

		protected override void Initialize()
		{
			base.Initialize();

			m_TextFormat = Attribute.TextFormat.IsEmpty() ? DefaultFormat : Attribute.TextFormat;
		}

		protected override void DoDrawPropertyLayout(GUIContent _label)
		{
			var rect = DrawPrefixLabel(_label);

			var style = LABEL_STYLE;
			style.richText = true;

			EditorGUI.LabelField(rect,Label,style);
		}

		protected virtual string DefaultFormat => "{0}";
		protected virtual string Label => string.Format(m_TextFormat,ValueEntry.SmartValue);
	}

	public class KZRichTextStringAttributeDrawer : KZRichTextAttributeDrawer<string>
	{
		protected override string Label
		{
			get
			{
				var text = ValueEntry.SmartValue.IsEmpty() ? string.Empty : ValueEntry.SmartValue;

				if(text.Contains(Environment.NewLine))
				{
					text = text.Replace(Environment.NewLine,"\\n");
				}

				return string.Format(m_TextFormat,text);
			}
		}
	}

	public class KZRichTextIntAttributeDrawer : KZRichTextAttributeDrawer<int> { }

	public class KZRichTextFloatAttributeDrawer : KZRichTextAttributeDrawer<float> { }

	public class KZRichTextBoolAttributeDrawer : KZRichTextAttributeDrawer<bool> { }

	public class KZRichTextVector2AttributeDrawer : KZRichTextAttributeDrawer<Vector2>
	{
		protected override string DefaultFormat => "x : {0:F3} / y : {1:F3}";

		protected override string Label => string.Format(m_TextFormat,ValueEntry.SmartValue.x,ValueEntry.SmartValue.y);
	}

	public class KZRichTextVector3AttributeDrawer : KZRichTextAttributeDrawer<Vector3>
	{
		protected override string DefaultFormat => "x : {0:F3} / y : {1:F3} / z : {2:F3}";

		protected override string Label => string.Format(m_TextFormat,ValueEntry.SmartValue.x,ValueEntry.SmartValue.y,ValueEntry.SmartValue.z);
	}

	public class KZRichTextVector4IntAttributeDrawer : KZRichTextAttributeDrawer<Vector4>
	{
		protected override string DefaultFormat => "x : {0:F3} / y : {1:F3} / z : {2:F3} / w : {3:F3}";

		protected override string Label => string.Format(m_TextFormat,ValueEntry.SmartValue.x,ValueEntry.SmartValue.y,ValueEntry.SmartValue.z,ValueEntry.SmartValue.w);
	}

	public class KZRichTextVector2IntAttributeDrawer : KZRichTextAttributeDrawer<Vector2Int>
	{
		protected override string DefaultFormat => "x : {0} / y : {1}";

		protected override string Label => string.Format(m_TextFormat,ValueEntry.SmartValue.x,ValueEntry.SmartValue.y);
	}

	public class KZRichTextVector3IntAttributeDrawer : KZRichTextAttributeDrawer<Vector3Int>
	{
		protected override string DefaultFormat => "x : {0} / y : {1} / z : {2}";

		protected override string Label => string.Format(m_TextFormat,ValueEntry.SmartValue.x,ValueEntry.SmartValue.y,ValueEntry.SmartValue.z);
	}
#endif
}