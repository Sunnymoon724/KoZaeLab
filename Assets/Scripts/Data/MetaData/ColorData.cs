using KZLib;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using MessagePack;
using UnityEngine;
using System.Collections.Generic;

namespace MetaData
{
	[MessagePackObject]
	public class ColorData : IMetaData
	{
		#region Field
		[SerializeField,HideInInspector] private int m_MetaId;
		[SerializeField,HideInInspector] private string m_Version;

		[SerializeField,HideInInspector] private string m_Name;

		[SerializeField,HideInInspector] private Color[] m_ColorArray;

		[SerializeField,HideInInspector] private bool m_Exist;
		#endregion Field

		#region Property
		[VerticalGroup("General")]
		[HorizontalGroup("General/0"),LabelText("MetaId"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$MetaId"),Key(1)]
		public int MetaId { get => m_MetaId; private set => m_MetaId = value; }
		[HorizontalGroup("General/0"),LabelText("Version"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Version"),Key(2)]
		public string Version { get => m_Version; private set => m_Version = value; }

		[VerticalGroup("Info")]
		[HorizontalGroup("Info/0"),LabelText("Name"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Name"),Key(3)]
		public string Name { get => m_Name; private set => m_Name = value; }
		[HorizontalGroup("Info/0"),LabelText("Color"),LabelWidth(100),ShowInInspector,KZColorArray,Key(4)]
		public Color[] ColorArray { get => m_ColorArray; private set => m_ColorArray = value; }

		protected string Color_0 { set => SetColor(0,value); }
		protected string Color_1 { set => SetColor(1,value); }
		protected string Color_2 { set => SetColor(2,value); }
		protected string Color_3 { set => SetColor(3,value); }
		protected string Color_4 { set => SetColor(4,value); }
		protected string Color_5 { set => SetColor(5,value); }
		protected string Color_6 { set => SetColor(6,value); }

		[HorizontalGroup("General/1"),LabelText("IsExist"),LabelWidth(100),ShowInInspector,KZIsValid,Key(0)]
		public bool IsExist => m_Exist;
		#endregion Property

		#region Initialize
		public ColorData() { }
		[SerializationConstructor]
		public ColorData(bool _exist,int _metaId,string _version,string _name,Color[] _colorArray)
		{
			m_MetaId = _metaId;
			m_Version = _version;

			m_Name = _name;

			m_ColorArray ??= new Color[7];

			_colorArray?.CopyTo(m_ColorArray,0);

			m_Exist = _exist;
		}

		private void SetColor(int _index,string _hexColor)
		{
			m_ColorArray ??= new Color[7];

			var color = _index == 0 || m_ColorArray[_index-1] == null ? Color.white : m_ColorArray[_index-1];

			m_ColorArray[_index] = _hexColor.IsEmpty() ? color : _hexColor.ToColor();
		}

		public IMetaData Initialize()
		{
			m_Exist = true;

			return this;
		}
		#endregion Initialize
	}
}