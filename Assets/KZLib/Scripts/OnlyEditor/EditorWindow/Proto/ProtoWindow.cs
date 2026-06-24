#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using KZLib.Attributes;
using KZLib.Data;
using UnityEngine;
using System.Collections;

namespace KZLib.Windows
{
	/// <summary>
	/// Editor window for browsing loaded MemoryPack proto rows grouped by proto type.
	/// Data is read from <see cref="ProtoManager"/>; list items use <see cref="ProtoDrawer"/>.
	/// </summary>
	public class ProtoWindow : OdinEditorWindow
	{
		private Type m_protoType = null;

		[VerticalGroup("0",Order = 0),ShowInInspector,ValueDropdown(nameof(ProtoTypeGroup)),ShowIf(nameof(IsExistProto))]
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

				m_protoList.AddRange(ProtoManager.In.FindGroup(value));
			}
		}

		private bool IsSelected => IsExistProto && ProtoType != null;

		[VerticalGroup("1",Order = 1),LabelText(" "),SerializeField,ListDrawerSettings(ShowFoldout = false,DraggableItems = false,HideAddButton = true,HideRemoveButton = true,ShowIndexLabels = true),ShowIf(nameof(IsSelected))]
		private List<IProto> m_protoList = new();

		[VerticalGroup("1",Order = 1),HideLabel,ShowInInspector,HideIf(nameof(IsExistProto)),KZRichText]
		protected string InfoText => "Proto is empty";

		private bool IsExistProto => ProtoTypeList.Count > 0;

		private List<Type> m_protoTypeList = new();

		private List<Type> ProtoTypeList => ProtoTypeGroup as List<Type>;

		/// <summary>
		/// Lazily reloads proto assets and caches the available proto types for the dropdown.
		/// </summary>
		private IEnumerable ProtoTypeGroup
		{
			get
			{
				if(m_protoTypeList.IsNullOrEmpty())
				{
					ProtoManager.In.Reload();

					m_protoTypeList = new List<Type>();

					foreach(var type in ProtoManager.In.FindTypeGroup())
					{
						m_protoTypeList.Add(type);
					}
				}

				return m_protoTypeList;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			m_protoTypeList.Clear();
		}

		protected override void Initialize()
		{
			base.Initialize();

			if(IsExistProto)
			{
				ProtoType = ProtoTypeList[0];
			}
		}
	}
}
#endif