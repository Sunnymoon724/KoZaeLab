#if UNITY_EDITOR
using System;
using KZLib.KZAttribute;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using YamlDotNet.Serialization;

namespace KZLib.KZWindow
{
	public class TemplateViewerWindow : OdinEditorWindow
	{
		private string m_typeName = null;
		private string m_yamlText = null;
		private string m_jsonText = null;

		[VerticalGroup("0",Order = 0),ShowInInspector]
		private string TypeName
		{
			get => m_typeName;
			set
			{
				if(m_typeName == value)
				{
					return;
				}

				m_typeName = value;

				var type = Type.GetType(value);

				if(type == null)
				{
					return;
				}

				var data = Activator.CreateInstance(type);

				var serializer = new SerializerBuilder().IncludeNonPublicProperties().Build();

				m_yamlText = serializer.Serialize(data,type);
				m_jsonText = JsonConvert.SerializeObject(data);
			}
		}

		private bool IsValid => !m_typeName.IsEmpty() && Type.GetType(TypeName) != null;

		[VerticalGroup("1",Order = 1),ShowInInspector,HideIf(nameof(IsValid)),KZRichText]
		protected string DisableText => "Not Exist Type";

		[VerticalGroup("2",Order = 2),ShowInInspector,ShowIf(nameof(IsValid))]
		protected string YamlText => m_yamlText;
		[VerticalGroup("2",Order = 2),ShowInInspector,ShowIf(nameof(IsValid))]
		protected string JsonText => m_jsonText;
	}
}
#endif