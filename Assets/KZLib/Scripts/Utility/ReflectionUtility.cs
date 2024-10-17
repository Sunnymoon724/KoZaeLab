using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;

public static class ReflectionUtility
{
	private static readonly Dictionary<string,Type> s_TypeDict = new();

	public static object CreateObject(Type _type,object _data)
	{
		var result = Activator.CreateInstance(_type);


        // foreach(var property in result.GetType().GetProperties())
        // {
        //     var targetProp = target.GetType().GetProperty(prop.Name);

        //     if (targetProp != null && targetProp.CanWrite)
        //     {
        //         var value = prop.GetValue(source);
        //         targetProp.SetValue(target, value);
        //     }
        // }

		return result;
	}

	public static Type FindType(string _typeName,string _namespaceName = null)
	{
		var fullName = _namespaceName.IsEmpty() ? $"{_typeName}" :$"{_namespaceName}.{_typeName}";

		if(s_TypeDict.TryGetValue(fullName,out var type))
		{
			return type;
		}

		foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			type = assembly.GetType(fullName);

			if(type != null)
			{
				s_TypeDict.Add(fullName,type);

				return type;
			}
		}

		return null;
	}

	public static IEnumerable<Type> FindTypeGroup(string _namespaceName)
	{
		foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			foreach(var type in assembly.GetTypes())
			{
				if(type.Namespace.IsEqual(_namespaceName))
				{
					yield return type;
				}
			}
		}
	}

	public static IEnumerable<Type> FindDerivedTypeGroup(Type _type)
	{
		foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			foreach(var derivedType in FindDerivedTypeGroup(_type, assembly))
			{
				yield return derivedType;
			}
		}
	}

	public static IEnumerable<Type> FindDerivedTypeGroup(Type _type,Assembly _assembly)
	{
		return FindDerivedTypeGroup(_type,_assembly.GetTypes());
	}
	
	public static IEnumerable<Type> FindDerivedTypeGroup(Type _type,IEnumerable<Type> _typeGroup)
	{
		foreach(var type in _typeGroup)
		{
			if(_type.IsAssignableFrom(type) && type != _type)
			{
				yield return type;
			}
		}
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

	public static void ClearCacheData()
	{
		s_TypeDict.Clear();
	}
}