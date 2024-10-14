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
		[BoxGroup("General",ShowLabel = false)]
		[HorizontalGroup("General/0"),LabelText("MetaId"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$MetaId")]
		public int MetaId { get => m_MetaId; private set => m_MetaId = value; }
		[HorizontalGroup("General/0"),LabelText("Version"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Version")]
		public string Version { get => m_Version; private set => m_Version = value; }


		[BoxGroup("Camera",ShowLabel = false)]
		[HorizontalGroup("Camera/0"),LabelText("Orthographic"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Orthographic")]
		public bool Orthographic { get => m_Orthographic; private set => m_Orthographic = value; }
		[HorizontalGroup("Camera/0"),LabelText("NearClipPlane"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$NearClipPlane")]
		public float NearClipPlane { get => m_NearClipPlane; private set => m_NearClipPlane = value; }
		[HorizontalGroup("Camera/0"),LabelText("FarClipPlane"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$FarClipPlane")]
		public float FarClipPlane { get => m_FarClipPlane; private set => m_FarClipPlane = value; }
		[HorizontalGroup("Camera/0"),LabelText("FieldOfView"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$FieldOfView")]
		public float FieldOfView { get => m_FieldOfView; private set => m_FieldOfView = value; }

		[BoxGroup("Transform",ShowLabel = false)]
		[HorizontalGroup("Transform/0"),LabelText("Position"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Position")]
		public Vector3 Position { get => m_Position; private set => m_Position = value; }
		[HorizontalGroup("Transform/0"),LabelText("Rotation"),LabelWidth(100),ShowInInspector,DisplayAsString,PropertyTooltip("$Rotation")]
		public Vector3 Rotation { get => m_Rotation; private set => m_Rotation = value; }


		[HorizontalGroup("General/0"),LabelText("IsExist"),LabelWidth(100),ShowInInspector,KZIsValid]
		public bool IsExist => m_Exist;
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