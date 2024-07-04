using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.Utilities;

public static partial class CommonUtility
{
	/// <summary>
	/// 이름으로 타입 찾기
	/// </summary>
	public static Type FindType(string _typeFullName)
	{
		foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			foreach(var type in assembly.GetTypes())
			{
				if(type.FullName.IsEmpty() || type.FullName.IsEqual(_typeFullName))
				{
					return type;
				}
			}
		}

		return null;
	}

	/// <summary>
	/// 이름으로 타입 찾기
	/// </summary>
	public static Type FindType(string _typeFullName,string _assemblyName)
	{
		var assembly = Assembly.Load(_assemblyName);

		if(assembly != null)
		{
			return assembly.GetType(_typeFullName);
		}

		return null;
	}

	public static IEnumerable<Type> FindDerivedTypeGroup(Type _type)
	{
		return FindDerivedTypeGroup(_type,AppDomain.CurrentDomain.GetAssemblies());
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
			if(_type.IsAssignableFrom(type))
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

		while(type != null)
		{
			type = type.BaseType;
		}

		return type;
	}

	public static Type GetValueType(MemberInfo _info)
	{
		var field = _info as FieldInfo;

		if(field != null)
		{
			return field.FieldType;
		}

		var property = _info as PropertyInfo;

		if(property != null)
		{
			return property.PropertyType;
		}

		throw new NotSupportedException(string.Format("지원하지 않는 타입 입니다.[ 이름 : {0} 타입 : {1}]",_info.Name,_info.GetType()));
	}

	public static object GetValue(MemberInfo _info,object _object)
	{
		var field = _info as FieldInfo;

		if(field != null)
		{
			return field.GetValue(_object);
		}

		var property = _info as PropertyInfo;

		if(property != null)
		{
			return property.GetValue(_object);
		}

		throw new NotSupportedException(string.Format("지원하지 않는 타입 입니다.[ 이름 : {0} 타입 : {1}]",_info.Name,_info.GetType()));
	}
	
	public static void SetValue(MemberInfo _info,object _object,object _value)
	{
		var field = _info as FieldInfo;

		if(field != null)
		{
			field.SetValue(_object,_value);

			return;
		}

		var property = _info as PropertyInfo;

		if(property != null)
		{
			property.SetValue(_object,_value);

			return;
		}

		throw new NotSupportedException(string.Format("지원하지 않는 타입 입니다.[ 이름 : {0} 타입 : {1}]",_info.Name,_info.GetType()));
	}
	
	public static IEnumerable<TAttribute> GetAttributeGroup<TAttribute>(ICustomAttributeProvider _info,bool _inherit = false) where TAttribute : Attribute
	{
		var attributeGroup = new List<TAttribute>();

		if(_info.IsDefined(typeof(TAttribute),_inherit))
		{
			foreach(var attribute in _info.GetCustomAttributes(typeof(TAttribute),_inherit))
			{
				attributeGroup.Add(attribute as TAttribute);
			}
		}

		return attributeGroup;
	}

	public static bool IsDefined<TAttribute>(ICustomAttributeProvider _info,bool _inherit = false) where TAttribute : Attribute
	{
		return _info.IsDefined(typeof(TAttribute),_inherit);
	}

	public static TAttribute GetAttribute<TAttribute>(ICustomAttributeProvider _info,bool _inherit = false) where TAttribute : Attribute
	{
		var attributeArray = _info.GetCustomAttributes(typeof(TAttribute),_inherit);

		return attributeArray.IsNullOrEmpty() ? null : attributeArray[0] as TAttribute;
	}

	public static TMember GetValueInObject<TMember>(string _memberName,object _object)
	{
		if(!_memberName.IsEmpty())
		{
			var type = _object.GetType();

			var propertyInfo = type.GetProperty(_memberName,Flags.InstanceAnyVisibility);

			if(propertyInfo != null)
			{
				return (TMember) propertyInfo.GetValue(_object);
			}

			var fieldInfo = type.GetField(_memberName,Flags.InstanceAnyVisibility);

			if(fieldInfo != null)
			{
				return (TMember) fieldInfo.GetValue(_object);
			}
		}

		return default;
	}

	public static void ExecuteMethod(object _object,string _name)
	{
		var info = _object.GetType().GetMethod(_name);

		info?.Invoke(_object,null);
	}
}