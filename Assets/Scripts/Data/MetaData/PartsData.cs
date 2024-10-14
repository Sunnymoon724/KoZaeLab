using KZLib;
using KZLib.KZAttribute;
using Sirenix.OdinInspector;
using MessagePack;
using UnityEngine;

namespace MetaData
{
	public record SSS
	{
		public int AAA { get; init; }
	}

	[MessagePackObject]
	public record PartsData : IMetaData
	{
		#region Field
		[SerializeField,HideInInspector,Key(1)] private int m_MetaId;
		[SerializeField,HideInInspector,Key(2)] private string m_Version;

		[SerializeField,HideInInspector,Key(3)] private string m_ModelPath;
		[SerializeField,HideInInspector,Key(4)] private PartsType m_PartsType;
		[SerializeField,HideInInspector,Key(5)] private string m_StatId;
		[SerializeField,HideInInspector,Key(0)] private bool m_Exist;
		#endregion Field

		#region Property
		[BoxGroup("General")]
		[HorizontalGroup("General/0"),LabelText("MetaId"),ShowInInspector,DisplayAsString,PropertyTooltip("$MetaId")]
		public int MetaId { get; init; }
		[HorizontalGroup("General/0"),LabelText("Version"),ShowInInspector,DisplayAsString,PropertyTooltip("$Version")]
		public string Version { get => m_Version; private set => m_Version = value; }


		[HorizontalGroup("General/0"),LabelText("ModelPath"),ShowInInspector,DisplayAsString,PropertyTooltip("$ModelPath")]
		public string ModelPath { get => m_ModelPath; private set => m_ModelPath = value; }

		[HorizontalGroup("General/0"),LabelText("PartsType"),ShowInInspector,DisplayAsString,PropertyTooltip("$PartsType")]
		public PartsType PartsType { get => m_PartsType; private set => m_PartsType = value; }

		[HorizontalGroup("General/0"),LabelText("StatId"),ShowInInspector,DisplayAsString,PropertyTooltip("$StatId")]
		public string StatId { get => m_StatId; private set => m_StatId = value; }
		[HorizontalGroup("General/0"),LabelText("IsExist"),LabelWidth(100),ShowInInspector,KZIsValid("Exist","Error")]
		public bool IsExist => m_Exist;
		#endregion Property

		#region Initialize
		public PartsData() { }

		public PartsData(int _ss)
		{

		}

#if UNITY_EDITOR
		public PartsData Initialize()
		{
			m_Exist = true;

			return this;
		}
#endif
		#endregion Initialize
	}
}