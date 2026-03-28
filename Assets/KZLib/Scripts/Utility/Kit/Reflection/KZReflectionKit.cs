using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.Utilities;

public static class KZReflectionKit
{
	private static readonly Dictionary<string,Type> s_typeDict = new();
	private static readonly Lazy<Assembly[]> s_lazyAssemblyArray = new(_GetAssemblyArray);

	private static Assembly[] _GetAssemblyArray()
	{
		return AppDomain.CurrentDomain.GetAssemblies();
	}

	public static Type FindType(string typeName,string namespaceName = null)
	{
		var fullName = namespaceName.IsEmpty() ? typeName : $"{namespaceName}.{typeName}";

		if(s_typeDict.TryGetValue(fullName,out var type))
		{
			return type;
		}

		var assemblyArray = s_lazyAssemblyArray.Value;

		for(var i=0;i<assemblyArray.Length;i++)
		{
			type = assemblyArray[i].GetType(fullName);

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
		var assemblyArray = s_lazyAssemblyArray.Value;

		for(var i=0;i<assemblyArray.Length;i++)
		{
			var typeArray = _GetSafeTypeArray(assemblyArray[i]);

			for(var j=0;j<typeArray.Length;j++)
			{
				var type = typeArray[j];

				if(type.Namespace.IsEqual(namespaceName))
				{
					yield return type;
				}
			}
		}
	}

	public static IEnumerable<Type> FindDerivedTypeGroup(Type type,bool instantiableOnly = false)
	{
		var assemblyArray = s_lazyAssemblyArray.Value;

		for(var i=0;i<assemblyArray.Length;i++)
		{
			foreach(var derivedType in FindDerivedTypeGroup(type,assemblyArray[i],instantiableOnly))
			{
				yield return derivedType;
			}
		}
	}

	public static IEnumerable<Type> FindDerivedTypeGroup(Type type,Assembly assembly,bool instantiableOnly = false)
	{
		return FindDerivedTypeGroup(type,_GetSafeTypeArray(assembly),instantiableOnly);
	}

	public static IEnumerable<Type> FindDerivedTypeGroup(Type type,IEnumerable<Type> assemblyTypeGroup,bool instantiableOnly = false)
	{
		foreach(var assemblyType in assemblyTypeGroup)
		{
			if(!type.IsAssignableFrom(assemblyType) || assemblyType == type)
			{
				continue;
			}

			if(instantiableOnly && (assemblyType.IsAbstract || assemblyType.IsInterface || assemblyType.IsGenericTypeDefinition))
			{
				continue;
			}

			yield return assemblyType;
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
			var attributeArray = attributeProvider.GetCustomAttributes(attributeType,inherit);

			for(var i=0;i<attributeArray.Length;i++)
			{
				yield return attributeArray[i] as TAttribute;
			}
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

	private static Type[] _GetSafeTypeArray(Assembly assembly)
	{
		try
		{
			return assembly.GetTypes();
		}
		catch(ReflectionTypeLoadException exception)
		{
			var typeArray = exception.Types;
			var validTypeList = new List<Type>();

			for(var i=0;i<typeArray.Length;i++)
			{
				if(typeArray[i] != null)
				{
					validTypeList.Add(typeArray[i]);
				}
			}

			return validTypeList.ToArray();
		}
		catch(Exception)
		{
			return Array.Empty<Type>();
		}
	}
}