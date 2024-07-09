using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NPOI.OpenXmlFormats.Spreadsheet;
using Sirenix.Utilities;

public static class ReflectionUtility
{
	private static readonly object s_SyncRoot = new();
	private static readonly Dictionary<string,Assembly> s_AssemblyDict = new();
	private static readonly Dictionary<string,Type> s_TypeDict = new();

	private static Dictionary<string,Assembly> AssemblyDict
	{
		get
		{
			if(s_AssemblyDict.Count == 0)
			{
				lock(s_SyncRoot)
				{
					foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
					{
						s_AssemblyDict.Add(assembly.FullName,assembly);
					}
				}
			}

			return s_AssemblyDict;
		}
	}

	/// <summary>
	/// 이름으로 타입 찾기
	/// </summary>
	public static Type FindType(string _typeName)
	{
		lock(s_SyncRoot)
		{
			if(s_TypeDict.ContainsKey(_typeName))
			{
				return s_TypeDict[_typeName];
			}
		}

		foreach(var assembly in AssemblyDict.Values)
		{
			var type = assembly.GetType(_typeName);

			if(type != null)
			{
				lock(s_SyncRoot)
				{
					s_TypeDict.Add(_typeName,type);
				}

				return type;
			}
		}

		return null;
	}

	/// <summary>
	/// 이름으로 타입 찾기
	/// </summary>
	public static IEnumerable<Type> FindTypeGroup(string _typeName)
	{
		var typeList = new List<Type>();

		foreach(var assembly in AssemblyDict.Values)
		{
			foreach(var type in assembly.GetTypes())
			{
				if(type.Name.Contains(_typeName))
				{
					if(s_TypeDict.ContainsKey(type.Name))
					{
						s_TypeDict.Add(type.Name,type);
					}

					typeList.Add(type);
				}
			}
		}

		return typeList;
	}

	/// <summary>
	/// 이름으로 타입 찾기
	/// </summary>
	public static Type FindType(string _typeFullName,string _assemblyName)
	{
		return AssemblyDict.TryGetValue(_assemblyName,out var assembly) ? assembly.GetType(_typeFullName) : null;
	}

	public static IEnumerable<Type> FindDerivedTypeGroup(Type _type)
	{
		return FindDerivedTypeGroup(_type,AssemblyDict.Values);
	}

	public static IEnumerable<Type> FindDerivedTypeGroup(Type _type,IEnumerable<Assembly> _assemblyGroup)
	{
		var typeList = new List<Type>();

		foreach(var assembly in _assemblyGroup)
		{
			typeList.AddRange(FindDerivedTypeGroup(_type,assembly));
		}

		return typeList;
	}
	
	public static IEnumerable<Type> FindDerivedTypeGroup(Type _type,Assembly _assembly)
	{
		return FindDerivedTypeGroup(_type,_assembly.GetTypes());
	}
	
	public static IEnumerable<Type> FindDerivedTypeGroup(Type _type,IEnumerable<Type> _typeGroup)
	{
		var typeList = new List<Type>();

		foreach(var type in _typeGroup)
		{
			if(_type.IsAssignableFrom(type) && type != _type)
			{
				typeList.Add(type);
			}
		}

		return typeList;
	}
	
	public static int GetTypeDepth(Type _type)
	{
		var depth = 0;
		var type = _type;

		while(type != null)
		{
			depth++;
			type = type.BaseType;
		}

		return depth;
	}
	
	public static Type GetRootBaseType(Type _type)
	{
		var type = _type;

		while(type.BaseType != null)
		{
			type = type.BaseType;
		}

		return type;
	}

	public static Type GetValueType(MemberInfo _info)
	{
		return _info switch
		{
			FieldInfo field => field.FieldType,
			PropertyInfo property => property.PropertyType,
			_ => throw new NotSupportedException(string.Format("지원하지 않는 타입 입니다.[ 이름 : {0} 타입 : {1}]",_info.Name,_info.GetType())),
		};
	}

	public static object GetValue(MemberInfo _info,object _object)
	{
		return _info switch
		{
			FieldInfo field => field.GetValue(_object),
			PropertyInfo property => property.GetValue(_object),
			_ => throw new NotSupportedException(string.Format("지원하지 않는 타입 입니다.[ 이름 : {0} 타입 : {1}]",_info.Name,_info.GetType())),
		};
	}
	
	public static void SetValue(MemberInfo _info,object _object,object _value)
	{
		switch(_info)
		{
			case FieldInfo field:
				field.SetValue(_object, _value);
				break;
			case PropertyInfo property:
				property.SetValue(_object, _value);
				break;
			default:
				throw new NotSupportedException(string.Format("지원하지 않는 타입 입니다.[ 이름 : {0} 타입 : {1}]",_info.Name,_info.GetType()));
		}
	}
	
	public static IEnumerable<TAttribute> GetAttributeGroup<TAttribute>(ICustomAttributeProvider _info,bool _inherit = false) where TAttribute : Attribute
	{
		return _info.IsDefined(typeof(TAttribute),_inherit) ? _info.GetCustomAttributes(typeof(TAttribute),_inherit).Cast<TAttribute>() : Enumerable.Empty<TAttribute>();
	}

	public static bool IsDefined<TAttribute>(ICustomAttributeProvider _info,bool _inherit = false) where TAttribute : Attribute
	{
		return _info.IsDefined(typeof(TAttribute),_inherit);
	}

	public static TAttribute GetAttribute<TAttribute>(ICustomAttributeProvider _info,bool _inherit = false) where TAttribute : Attribute
	{
		var attributeArray = _info.GetCustomAttributes(typeof(TAttribute),_inherit);

		return attributeArray.Length > 0 ? attributeArray[0] as TAttribute : null;
	}

	public static TMember GetValueInObject<TMember>(string _memberName,object _object,BindingFlags _binding = Flags.InstanceAnyVisibility)
	{
		var type = _object.GetType();

		var propertyInfo = type.GetProperty(_memberName,_binding);

		if(propertyInfo != null)
		{
			return (TMember) propertyInfo.GetValue(_object);
		}

		var fieldInfo = type.GetField(_memberName,_binding);

		if(fieldInfo != null)
		{
			return (TMember) fieldInfo.GetValue(_object);
		}

		return default;
	}

	// public static void ExecuteMethod(object _object,string _name)
	// {
	// 	var methodInfo = _object.GetType().GetMethod(_name,Flags.InstanceAnyVisibility);

	// 	methodInfo?.Invoke(_object,null);
	// }

	public static void ClearCacheData()
	{
		s_AssemblyDict.Clear();
		s_TypeDict.Clear();
	}
}