using KZLib;
using KZLib.KZAttribute;
using MessagePack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MetaData
{
	[MessagePackObject]
	public struct CameraData : IMetaData
	{
		#region Field
		[SerializeField,HideInInspector] private int m_MetaId;
		[SerializeField,HideInInspector] private string m_Version;
		[SerializeField,HideInInspector] private string m_Name;

		[SerializeField,HideInInspector] private bool m_Orthographic;
		[SerializeField,HideInInspector] private float m_NearClipPlane;
		[SerializeField,HideInInspector] private float m_FarClipPlane;
		[SerializeField,HideInInspector] private float m_FieldOfView;
		[SerializeField,HideInInspector] private Vector3 m_Position;
		[SerializeField,HideInInspector] private Vector3 m_Rotation;

		[SerializeField,HideInInspector] private bool m_Exist;
		#endregion Field

		#region Property
		[BoxGroup("기본 정보",ShowLabel = false)]
		[HorizontalGroup("기본 정보/0"),LabelText("MetaId"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$MetaId"),Key(0)]
		public int MetaId { readonly get => m_MetaId; private set => m_MetaId = value; }
		[HorizontalGroup("기본 정보/0"),LabelText("Version"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Version"),Key(1)]
		public string Version { readonly get => m_Version; private set => m_Version = value; }
		[HorizontalGroup("기본 정보/0"),LabelText("Name"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Name"),Key(2)]
		public string Name { readonly get => m_Name; private set => m_Name = value; }


		[BoxGroup("카메라 정보",ShowLabel = false)]
		[HorizontalGroup("카메라 정보/0"),LabelText("Orthographic"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Orthographic"),Key(3)]
		public bool Orthographic { readonly get => m_Orthographic; private set => m_Orthographic = value; }
		[HorizontalGroup("카메라 정보/0"),LabelText("NearClipPlane"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$NearClipPlane"),Key(4)]
		public float NearClipPlane { readonly get => m_NearClipPlane; private set => m_NearClipPlane = value; }
		[HorizontalGroup("카메라 정보/0"),LabelText("FarClipPlane"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$FarClipPlane"),Key(5)]
		public float FarClipPlane { readonly get => m_FarClipPlane; private set => m_FarClipPlane = value; }
		[HorizontalGroup("카메라 정보/0"),LabelText("FieldOfView"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$FieldOfView"),Key(6)]
		public float FieldOfView { readonly get => m_FieldOfView; private set => m_FieldOfView = value; }


		[BoxGroup("변환 정보",ShowLabel = false)]
		[HorizontalGroup("변환 정보/0"),LabelText("Position"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Position"),Key(7)]
		public Vector3 Position { readonly get => m_Position; private set => m_Position = value; }
		[HorizontalGroup("변환 정보/0"),LabelText("Rotation"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Rotation"),Key(8)]
		public Vector3 Rotation { readonly get => m_Rotation; private set => m_Rotation = value; }


		[HorizontalGroup("기본 정보/0"),LabelText("IsExist"),LabelWidth(100),ShowInInspector,KZIsValid]
		public readonly bool IsExist => m_Exist;
		#endregion Property

#if UNITY_EDITOR
		public CameraData Initialize()
		{
			m_Exist = true;

			return this;
		}
#endif
	}
}