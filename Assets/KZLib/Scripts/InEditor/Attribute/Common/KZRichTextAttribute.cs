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

		public KZRichTextAttribute(string textFormat = null)
		{
			TextFormat = textFormat;
		}
	}

#if UNITY_EDITOR
	public abstract class KZRichTextAttributeDrawer<TValue> : KZAttributeDrawer<KZRichTextAttribute,TValue>
	{
		protected string m_textFormat = null;

		protected override void Initialize()
		{
			base.Initialize();

			m_textFormat = Attribute.TextFormat.IsEmpty() ? DefaultFormat : Attribute.TextFormat;
		}

		protected override void _DrawPropertyLayout(GUIContent label)
		{
			var rect = DrawPrefixLabel(label);

			var style = new GUIStyle(GUI.skin.label)
			{
				richText = true
			};

			EditorGUI.LabelField(rect,LabelText,style);
		}

		protected virtual string DefaultFormat => "{0}";
		protected virtual string LabelText => string.Format(m_textFormat,ValueEntry.SmartValue);
	}

	public class KZRichTextStringAttributeDrawer : KZRichTextAttributeDrawer<string>
	{
		protected override string LabelText
		{
			get
			{
				var text = ValueEntry.SmartValue.IsEmpty() ? string.Empty : ValueEntry.SmartValue;

				if(text.Contains(Environment.NewLine))
				{
					text = text.Replace(Environment.NewLine,"\\n");
				}

				return string.Format(m_textFormat,text);
			}
		}
	}

	public class KZRichTextIntAttributeDrawer : KZRichTextAttributeDrawer<int> { }

	public class KZRichTextFloatAttributeDrawer : KZRichTextAttributeDrawer<float> { }

	public class KZRichTextBoolAttributeDrawer : KZRichTextAttributeDrawer<bool> { }

	public class KZRichTextVector2AttributeDrawer : KZRichTextAttributeDrawer<Vector2>
	{
		protected override string DefaultFormat => "x : {0:F3} / y : {1:F3}";

		protected override string LabelText => string.Format(m_textFormat,ValueEntry.SmartValue.x,ValueEntry.SmartValue.y);
	}

	public class KZRichTextVector3AttributeDrawer : KZRichTextAttributeDrawer<Vector3>
	{
		protected override string DefaultFormat => "x : {0:F3} / y : {1:F3} / z : {2:F3}";

		protected override string LabelText => string.Format(m_textFormat,ValueEntry.SmartValue.x,ValueEntry.SmartValue.y,ValueEntry.SmartValue.z);
	}

	public class KZRichTextVector4IntAttributeDrawer : KZRichTextAttributeDrawer<Vector4>
	{
		protected override string DefaultFormat => "x : {0:F3} / y : {1:F3} / z : {2:F3} / w : {3:F3}";

		protected override string LabelText => string.Format(m_textFormat,ValueEntry.SmartValue.x,ValueEntry.SmartValue.y,ValueEntry.SmartValue.z,ValueEntry.SmartValue.w);
	}

	public class KZRichTextVector2IntAttributeDrawer : KZRichTextAttributeDrawer<Vector2Int>
	{
		protected override string DefaultFormat => "x : {0} / y : {1}";

		protected override string LabelText => string.Format(m_textFormat,ValueEntry.SmartValue.x,ValueEntry.SmartValue.y);
	}

	public class KZRichTextVector3IntAttributeDrawer : KZRichTextAttributeDrawer<Vector3Int>
	{
		protected override string DefaultFormat => "x : {0} / y : {1} / z : {2}";

		protected override string LabelText => string.Format(m_textFormat,ValueEntry.SmartValue.x,ValueEntry.SmartValue.y,ValueEntry.SmartValue.z);
	}

	public class KZRichTextQuaternionIntAttributeDrawer : KZRichTextAttributeDrawer<Quaternion>
	{
		protected override string DefaultFormat => "x : {0:F3} / y : {1:F3} / z : {2:F3} / w : {3:F3}";

		protected override string LabelText => string.Format(m_textFormat,ValueEntry.SmartValue.x,ValueEntry.SmartValue.y,ValueEntry.SmartValue.z,ValueEntry.SmartValue.w);
	}
#endif
}