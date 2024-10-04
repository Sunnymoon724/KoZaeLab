#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
using System;
using System.Text;

public partial class MetaSettings : ExcelSettings<MetaSettings>
{
	[Serializable]
	private class ExcelCellData
	{
		[SerializeField,HideInInspector] private string m_Name = null;
		[SerializeField,HideInInspector] private DataType m_Type = DataType.String;
		[SerializeField,HideInInspector] private bool m_IsArray = false;

		[HorizontalGroup("0"),HideLabel,ShowInInspector,DisplayAsString]
		public string Name { get => m_Name; private set => m_Name = value; }

		[HorizontalGroup("0"),HideLabel,ShowInInspector]
		private DataType Type
		{
			get => m_Type;
			set => m_Type = Name == META_ID ? DataType.Int : Name == VERSION ? DataType.String : value;
		}
        public bool IsEnumType => Type == DataType.Enum;

		[HorizontalGroup("0"),HideLabel,ShowInInspector,LabelText("배열 여부"),ToggleLeft]
		private bool IsArray
		{
			get => m_IsArray;
			set => m_IsArray = Name != META_ID && Name != VERSION && value;
		}

		public ExcelCellData(string _name,DataType _type,bool _isArray)
		{
			Name = _name;
			IsArray = _isArray;
			Type = _type;
		}

		public string ToFieldText()
		{
			return string.Format("[SerializeField,HideInInspector] private {0} m_{1};",string.Concat(DataTypeToString(),IsArray ? "[]" : ""),Name);
		}

		public string ToPropertyText()
		{
			var builder = new StringBuilder();
			var type = DataTypeToString();

			if(IsArray)
			{
				// Header 추가
				builder.AppendFormat("[HorizontalGroup(\"정보/0\"),LabelText(\"{0}\"),LabelWidth(100),ShowInInspector,PropertyTooltip(\"${0}_ToolTip\"),KZRichText]{1}\t\t",Name,Environment.NewLine);

				// Display 추가
				builder.AppendFormat("private string {0}_Display => {0}.IsNullOrEmpty() ? \"NULL\" : string.Join(\" | \",{0});{1}\t\t",Name,Environment.NewLine);
				// ToolTip 추가
				builder.AppendFormat("private string {0}_ToolTip => {0}_Display.RemoveRichText();{1}{1}\t\t",Name,Environment.NewLine);

				// Property 추가
				builder.AppendFormat("private {0}[] {1} {{ get => m_{1}; set => m_{1} = value; }}{2}\t\t",type,Name,Environment.NewLine);

				// IEnumerable 추가
				builder.AppendFormat("public IEnumerable<{0}> {1}Group => m_{1};",type,Name);
			}
			else
			{
				// Header 추가
				builder.AppendFormat("[HorizontalGroup(\"정보/0\"),LabelText(\"{0}\"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip(\"${0}\")]{1}\t\t",Name,Environment.NewLine);

				// Property 추가
				builder.AppendFormat("public {0} {1} {{ get => m_{1}; private set => m_{1} = value; }}",type,Name);
			}

			return builder.ToString();
		}

		protected string DataTypeToString()
		{
			return Type switch
			{
				DataType.Enum => Name,
				DataType.Vector3 => Type.ToString(),
				_ => Type.ToString().ToLowerInvariant(),
			};
		}
	}
}
#endif