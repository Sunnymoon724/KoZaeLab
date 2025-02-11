#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using KZLib.KZAttribute;
using KZLib.KZData;

namespace KZLib.KZWindow
{
	public class ProtoWindow : OdinEditorWindow
	{
		private Type m_protoType = null;

		[VerticalGroup("0",Order = 0),ShowInInspector,ValueDropdown(nameof(ProtoTypeList)),ShowIf(nameof(IsExistProto))]
		private Type ProtoType
		{
			get => m_protoType;
			set
			{
				if(m_protoType == value)
				{
					return;
				}

				m_protoType = value;

				m_protoList.Clear();

				if(m_protoType == null)
				{
					return;
				}

				m_protoList.AddRange(ProtoManager.In.GetProtoGroup(value));
			}
		}

		private bool IsSelected => IsExistProto && ProtoType != null;

		[VerticalGroup("1",Order = 1),LabelText(" "),SerializeReference,ListDrawerSettings(ShowFoldout = false,DraggableItems = false,HideAddButton = true,HideRemoveButton = true,ShowIndexLabels = true),ShowIf(nameof(IsSelected))]
		private List<IProto> m_protoList = new();

		[VerticalGroup("1",Order = 1),HideLabel,ShowInInspector,HideIf(nameof(IsExistProto)),KZRichText]
		protected string InfoText => "Proto is empty";

		private bool IsExistProto => ProtoTypeList.Count > 0;

		private List<Type> m_protoTypeList = null;

		private List<Type> ProtoTypeList
		{
			get
			{
				if(m_protoTypeList == null)
				{
					ProtoManager.In.Reload();

					m_protoTypeList = new List<Type>();

					foreach(var type in ProtoManager.In.GetProtoTypeGroup())
					{
						m_protoTypeList.Add(type);
					}
				}

				return m_protoTypeList;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();

			if(IsExistProto)
			{
				ProtoType = ProtoTypeList.First();
			}
		}
	}
}
#endif