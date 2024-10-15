using KZLib;
using KZLib.KZAttribute;
using MessagePack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MetaData
{
	[MessagePackObject]
	public class CameraData : IMetaData
	{
		#region Field
		[SerializeField,HideInInspector] private int m_MetaId;
		[SerializeField,HideInInspector] private string m_Version;

		[SerializeField,HideInInspector] private bool m_Orthographic;
		[SerializeField,HideInInspector] private float m_NearClipPlane;
		[SerializeField,HideInInspector] private float m_FarClipPlane;
		[SerializeField,HideInInspector] private float m_FieldOfView;

		[SerializeField,HideInInspector] private Vector3 m_Position;
		[SerializeField,HideInInspector] private Vector3 m_Rotation;

		[SerializeField,HideInInspector] private bool m_Exist;
		#endregion Field

		#region Property
		[VerticalGroup("General")]
		[HorizontalGroup("General/0"),LabelText("MetaId"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$MetaId"),Key(1)]
		public int MetaId { get => m_MetaId; private set => m_MetaId = value; }
		[HorizontalGroup("General/0"),LabelText("Version"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Version"),Key(2)]
		public string Version { get => m_Version; private set => m_Version = value; }

		[VerticalGroup("Info")]
		[HorizontalGroup("Info/0"),LabelText("Orthographic"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Orthographic"),Key(3)]
		public bool Orthographic { get => m_Orthographic; private set => m_Orthographic = value; }
		[HorizontalGroup("Info/0"),LabelText("NearClipPlane"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$NearClipPlane"),Key(4)]
		public float NearClipPlane { get => m_NearClipPlane; private set => m_NearClipPlane = value; }
		[HorizontalGroup("Info/0"),LabelText("FarClipPlane"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$FarClipPlane"),Key(5)]
		public float FarClipPlane { get => m_FarClipPlane; private set => m_FarClipPlane = value; }
		[HorizontalGroup("Info/0"),LabelText("FieldOfView"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$FieldOfView"),Key(6)]
		public float FieldOfView { get => m_FieldOfView; private set => m_FieldOfView = value; }

		[VerticalGroup("Transform")]
		[HorizontalGroup("Transform/0"),LabelText("Position"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Position"),Key(7)]
		public Vector3 Position { get => m_Position; private set => m_Position = value; }
		[HorizontalGroup("Transform/0"),LabelText("Rotation"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Rotation"),Key(8)]
		public Vector3 Rotation { get => m_Rotation; private set => m_Rotation = value; }

		[HorizontalGroup("General/1"),LabelText("IsExist"),LabelWidth(100),ShowInInspector,KZIsValid,Key(0)]
		public bool IsExist => m_Exist;
		#endregion Property

		#region Initialize
		public CameraData() { }
		[SerializationConstructor]
		public CameraData(bool _exist,int _metaId,string _version,bool _orthographic,float _nearClipPlane,float _farClipPlane,float _fieldOfView,Vector3 _position,Vector3 _rotation)
		{
			m_MetaId = _metaId;
			m_Version = _version;

			m_Orthographic = _orthographic;
			m_NearClipPlane = _nearClipPlane;
			m_FarClipPlane = _farClipPlane;
			m_FieldOfView = _fieldOfView;

			m_Position = _position;
			m_Rotation = _rotation;
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