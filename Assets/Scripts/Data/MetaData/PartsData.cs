using KZLib;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using MessagePack;
using UnityEngine;
using Newtonsoft.Json;

namespace MetaData
{
	[MessagePackObject]
	public record PartsData : IMetaData
	{
		#region Field
		[SerializeField,HideInInspector] private int m_MetaId;
		[SerializeField,HideInInspector] private string m_Version;

		[SerializeField,HideInInspector] private string m_ModelPath;
		[SerializeField,HideInInspector] private PartsType m_PartsType;
		[SerializeField,HideInInspector] private string m_StatId;

		[SerializeField,HideInInspector] private bool m_Exist;
		#endregion Field

		#region Property
		[VerticalGroup("General")]
		[HorizontalGroup("General/0"),LabelText("MetaId"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$MetaId"),Key(1)]
		public int MetaId { get => m_MetaId; private set => m_MetaId = value; }
		[HorizontalGroup("General/0"),LabelText("Version"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Version"),Key(2)]
		public string Version { get => m_Version; private set => m_Version = value; }

		[VerticalGroup("Info")]
		[HorizontalGroup("Info/0"),LabelText("ModelPath"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$ModelPath"),Key(3)]
		public string ModelPath { get => m_ModelPath; private set => m_ModelPath = value; }
		[HorizontalGroup("Info/0"),LabelText("PartsType"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$PartsType"),Key(4)]
		public PartsType PartsType { get => m_PartsType; private set => m_PartsType = value; }
		[HorizontalGroup("Info/0"),LabelText("StatId"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$StatId"),Key(5)]
		public string StatId { get => m_StatId; private set => m_StatId = value; }

		[HorizontalGroup("General/1"),LabelText("IsExist"),LabelWidth(100),ShowInInspector,KZIsValid,Key(0)]
		public bool IsExist => m_Exist;
		#endregion Property

		#region Initialize
		public PartsData() { }
		[SerializationConstructor]
		public PartsData(bool _exist,int _metaId,string _version,string _modelPath,PartsType _partsType,string _statId)
		{
			m_MetaId = _metaId;
			m_Version = _version;

			m_ModelPath = _modelPath;
			m_PartsType = _partsType;
			m_StatId = _statId;

			m_Exist = _exist;
		}

		public IMetaData Initialize()
		{
			m_Exist = true;

			return this;
		}
		#endregion Initialize
	}
}