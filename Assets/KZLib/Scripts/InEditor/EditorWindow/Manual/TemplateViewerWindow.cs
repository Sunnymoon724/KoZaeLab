#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using KZLib.KZAttribute;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using YamlDotNet.Serialization;

namespace KZLib.KZWindow
{
	public class TemplateViewerWindow : OdinEditorWindow
	{
		private string m_yamlText = null;
		private string m_jsonText = null;

		[VerticalGroup("0",Order = 0),ShowInInspector,ValueDropdown(nameof(AssemblyNameList))]
		private string AssemblyName { get; set; }
		[VerticalGroup("0",Order = 0),ShowInInspector]
		private string NamespaceName { get; set; }
		[VerticalGroup("0",Order = 0),ShowInInspector]
		private string TypeName { get; set; }

		private Type m_type = null;

		private bool IsValid => m_type != null;

		[VerticalGroup("1",Order = 1),Button("Convert")]
		protected void OnConvert()
		{
			if(TypeName.IsEmpty())
			{
				return;
			}

			m_type = Type.GetType($"{(NamespaceName.IsEmpty() ? $"{TypeName}" : $"{NamespaceName}.{TypeName}")}, {AssemblyName}");

			if(m_type == null)
			{
				return;
			}

			var instance = _GenerateInstance();

			var serializer = new SerializerBuilder().IncludeNonPublicProperties().Build();

			m_yamlText = serializer.Serialize(instance,m_type);
			m_jsonText = JsonConvert.SerializeObject(instance);
		}

		[VerticalGroup("2",Order = 2),ShowInInspector,HideIf(nameof(IsValid)),KZRichText]
		protected string DisableText => "Not Exist Type";

		[VerticalGroup("3",Order = 3),ShowInInspector,ShowIf(nameof(IsValid))]
		protected string YamlText => m_yamlText;
		[VerticalGroup("3",Order = 3),ShowInInspector,ShowIf(nameof(IsValid))]
		protected string JsonText => m_jsonText;

		private object _GenerateInstance()
		{
			var constructor = m_type.GetConstructor(Type.EmptyTypes);
			var instance = constructor != null ? Activator.CreateInstance(m_type) : FormatterServices.GetUninitializedObject(m_type);

			foreach(var propertyInfo in m_type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var setMethod = propertyInfo.SetMethod ?? propertyInfo.GetSetMethod(true);
                if (setMethod == null)
                {
                    continue;
                }

                var propertyType = propertyInfo.PropertyType;
                propertyInfo.SetValue(instance, _GenerateValue(propertyType));
            }

            return instance;
		}

		private object _GenerateValue(Type type)
		{
			if(type == typeof(string))
			{
				return "Dummy";
			}

			if(type.IsEnum)
			{
				return Enum.GetValues(type).GetValue(0);
			}

			if(type.IsArray)
			{
				var elementType = type.GetElementType();
				var arrayInstance = Array.CreateInstance(elementType,2);

				for(var i=0;i<2;i++)
				{
					arrayInstance.SetValue(_GenerateValue(elementType),i);
				}

				return arrayInstance;
			}

			var infoArray = type.GetConstructors();

			if(infoArray != null && infoArray.Length > 0)
			{
				var parameters = infoArray[0].GetParameters();
				var argumentArray = new object[parameters.Length];

				for(var i=0;i<argumentArray.Length;i++)
				{
					argumentArray[i] = _GenerateValue(parameters[i].ParameterType);
				}

				return Activator.CreateInstance(type,argumentArray);
			}

			try
			{
				return Activator.CreateInstance(type);
			}
			catch
			{
				try
				{
					return FormatterServices.GetUninitializedObject(type);
				}
				catch
				{
					return null;
				}
			}
		}

		private static readonly List<string> s_assemblyNameList = new();

		private static List<string> AssemblyNameList
		{
			get
			{
				if(s_assemblyNameList.IsNullOrEmpty())
				{
					foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
					{
						s_assemblyNameList.Add(assembly.GetName().Name);
					}
				}

				return s_assemblyNameList;

				
			}
		}
	}
}
#endif