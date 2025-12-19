using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.Utilities;

public static partial class CommonUtility
{
	private static readonly Dictionary<string,Type> s_typeDict = new();

	public static Type FindType(string typeName,string namespaceName = null)
	{
		var fullName = namespaceName.IsEmpty() ? typeName : $"{namespaceName}.{typeName}";

		if(s_typeDict.TryGetValue(fullName,out var type))
		{
			return type;
		}

		foreach(var assembly in _GetAllAssemblyArray())
		{
			type = assembly.GetType(fullName);

			if(type != null)
			{
				s_typeDict.Add(fullName,type);

				return type;
			}
		}

		return null;
	}

	public static IEnumerable<Type> FindTypeGroup(string namespaceName)
	{
		foreach(var assembly in _GetAllAssemblyArray())
		{
			foreach(var type in assembly.GetTypes())
			{
				if(type.Namespace.IsEqual(namespaceName))
				{
					yield return type;
				}
			}
		}
	}

	public static IEnumerable<Type> FindDerivedTypeGroup(Type type)
	{
		foreach(var assembly in _GetAllAssemblyArray())
		{
			foreach(var derivedType in FindDerivedTypeGroup(type,assembly))
			{
				yield return derivedType;
			}
		}
	}

	public static IEnumerable<Type> FindDerivedTypeGroup(Type type,Assembly assembly)
	{
		return FindDerivedTypeGroup(type,assembly.GetTypes());
	}
	
	public static IEnumerable<Type> FindDerivedTypeGroup(Type type,IEnumerable<Type> assemblyTypeGroup)
	{
		foreach(var assemblyType in assemblyTypeGroup)
		{
			if(type.IsAssignableFrom(assemblyType) && assemblyType != type)
			{
				yield return assemblyType;
			}
		}
	}

	public static int CalculateTypeDepth(Type type)
	{
		var depth = 0;

		while(type != null)
		{
			depth++;
			type = type.BaseType;
		}

		return depth;
	}

	public static Type FindRootBaseType(Type type)
	{
		while(type.BaseType != null)
		{
			type = type.BaseType;
		}

		return type;
	}

	public static IEnumerable<TAttribute> FindAttributeGroup<TAttribute>(ICustomAttributeProvider attributeProvider,bool inherit = false) where TAttribute : Attribute
	{
		var attributeType = typeof(TAttribute);

		if(attributeProvider.IsDefined(attributeType,inherit))
		{
			foreach(var attribute in attributeProvider.GetCustomAttributes(attributeType,inherit))
			{
				yield return attribute as TAttribute;
			}
		}
		else
		{
			yield break;
		}
	}

	public static bool IsDefined<TAttribute>(ICustomAttributeProvider attributeProvider,bool inherit = false) where TAttribute : Attribute
	{
		return attributeProvider.IsDefined(typeof(TAttribute),inherit);
	}

	public static TAttribute FindAttribute<TAttribute>(ICustomAttributeProvider attributeProvider,bool inherit = false) where TAttribute : Attribute
	{
		var attributeArray = attributeProvider.GetCustomAttributes(typeof(TAttribute),inherit);

		return attributeArray.Length > 0 ? attributeArray[0] as TAttribute : null;
	}

	public static TMember FindValueInObject<TMember>(string memberName,object value,BindingFlags bindingFlag = Flags.InstanceAnyVisibility)
	{
		var valueType = value.GetType();

		var propertyInfo = valueType.GetProperty(memberName,bindingFlag);

		if(propertyInfo != null)
		{
			return (TMember) propertyInfo.GetValue(value);
		}

		var fieldInfo = valueType.GetField(memberName,bindingFlag);

		if(fieldInfo != null)
		{
			return (TMember) fieldInfo.GetValue(value);
		}

		return default;
	}

	public static void ClearCache()
	{
		s_typeDict.Clear();
	}
	
	private static Assembly[] _GetAllAssemblyArray()
	{
		return AppDomain.CurrentDomain.GetAssemblies();
	}
}