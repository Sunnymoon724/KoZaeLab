using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.Utilities;

/// <summary>
/// Utility methods for runtime type discovery, attribute inspection, and member value access.
/// </summary>
public static class KZReflectionKit
{
	private static readonly Dictionary<string,Type> s_typeDict = new();
	private static readonly Lazy<Assembly[]> s_lazyAssemblyArray = new(_GetAssemblyArray);

	private static Assembly[] _GetAssemblyArray()
	{
		return AppDomain.CurrentDomain.GetAssemblies();
	}

	/// <summary>
	/// Finds a type by name across all loaded assemblies, optionally prefixed with a namespace.
	/// Results are cached for subsequent lookups.
	/// </summary>
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

	/// <summary>
	/// Yields all types in loaded assemblies that belong to the given namespace.
	/// </summary>
	public static IEnumerable<Type> FindTypeGroup(string namespaceName)
	{
		var assemblyArray = s_lazyAssemblyArray.Value;

		for(var i=0;i<assemblyArray.Length;i++)
		{
			var typeArray = _GetSafeTypeArray(assemblyArray[i]);

			for(var j=0;j<typeArray.Length;j++)
			{
				var type = typeArray[j];

				if(type.Namespace != null && type.Namespace.IsEqual(namespaceName))
				{
					yield return type;
				}
			}
		}
	}

	/// <summary>
	/// Yields all types assignable from the given base type across all loaded assemblies.
	/// </summary>
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

	/// <summary>
	/// Yields all types assignable from the given base type within a single assembly.
	/// </summary>
	public static IEnumerable<Type> FindDerivedTypeGroup(Type type,Assembly assembly,bool instantiableOnly = false)
	{
		return FindDerivedTypeGroup(type,_GetSafeTypeArray(assembly),instantiableOnly);
	}

	/// <summary>
	/// Yields all types assignable from the given base type within the provided type collection.
	/// </summary>
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

	/// <summary>
	/// Returns the inheritance depth of the given type, counting the type itself.
	/// </summary>
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

	/// <summary>
	/// Returns the root base type in the inheritance chain (the type with no base type).
	/// </summary>
	public static Type FindRootBaseType(Type type)
	{
		while(type.BaseType != null)
		{
			type = type.BaseType;
		}

		return type;
	}

	/// <summary>
	/// Yields all attributes of type TAttribute defined on the given provider.
	/// </summary>
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

	/// <summary>
	/// Returns whether the given provider defines an attribute of type TAttribute.
	/// </summary>
	public static bool IsDefined<TAttribute>(ICustomAttributeProvider attributeProvider,bool inherit = false) where TAttribute : Attribute
	{
		return attributeProvider.IsDefined(typeof(TAttribute),inherit);
	}

	/// <summary>
	/// Returns the first attribute of type TAttribute defined on the given provider, or null if none exist.
	/// </summary>
	public static TAttribute FindAttribute<TAttribute>(ICustomAttributeProvider attributeProvider,bool inherit = false) where TAttribute : Attribute
	{
		var attributeArray = attributeProvider.GetCustomAttributes(typeof(TAttribute),inherit);

		return attributeArray.Length > 0 ? attributeArray[0] as TAttribute : null;
	}

	/// <summary>
	/// Reads a property or field value from an object by member name and casts it to TMember.
	/// </summary>
	public static TMember FindValueInObject<TMember>(string memberName,object value,BindingFlags bindingFlag = Flags.InstanceAnyVisibility)
	{
		if(value == null)
		{
			return default;
		}

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

	/// <summary>
	/// Safely retrieves all types from an assembly, returning only successfully loaded types on partial load failures.
	/// </summary>
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
