using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
/// 참고 레퍼런스
/// </summary>

public abstract class Enumeration : IComparable
{
	private const BindingFlags ENUMERATION_FLAG = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

	private readonly string m_Name = null;

	protected Enumeration(string _name)
	{
		m_Name = _name;
	}

	public static IEnumerable<TEnumeration> GetGroup<TEnumeration>(bool _includeDerivedType) where TEnumeration : Enumeration
	{
		var dataList = new List<TEnumeration>();

		var type = typeof(TEnumeration);

		AddValueArray(dataList,type);

		if(_includeDerivedType)
		{
			foreach(var derivedType in ReflectionUtility.FindDerivedTypeGroup(type))
			{
				AddValueArray(dataList,derivedType);
			}
		}

		return dataList;
	}

	private static void AddValueArray<TEnumeration>(List<TEnumeration> _dataList,Type _type) where TEnumeration : Enumeration
	{
		foreach(var fieldInfo in _type.GetFields(ENUMERATION_FLAG))
		{
			_dataList.Add(fieldInfo.GetValue(null) as TEnumeration);
		}
	}

	public static bool IsDefined<TEnumeration>(string _name) where TEnumeration : Enumeration
	{
		return TryParse<TEnumeration>(_name,out _);
	}

	public static TEnumeration Parse<TEnumeration>(string _name) where TEnumeration : Enumeration
	{
		if(TryParse<TEnumeration>(_name,out var data))
		{
			return data;
		}

		throw new InvalidOperationException(string.Format("{0}은 Enum 안에 없습니다.[Enum 종류 : {1}]",_name,nameof(TEnumeration)));
	}

	public static bool TryParse<TEnumeration>(string _name,out TEnumeration _result) where TEnumeration : Enumeration
	{
		foreach(var data in GetGroup<TEnumeration>(true))
		{
			if(data.m_Name.IsEqual(_name))
			{
				_result = data;

				return true;
			}
		}

		_result = null;

		return false;
	}
	public override bool Equals(object _object)
	{
		return _object is Enumeration data && m_Name.IsEqual(data.m_Name);
	}

	public static bool operator ==(Enumeration _left,Enumeration _right)
	{
		if(ReferenceEquals(_left,_right))
		{
			return true;
		}

		if(_left is null || _right is null)
		{
			return false;
		}

		return _left.Equals(_right);
	}

	// public static bool operator ==(Enumeration _left,string _right)
	// {
	// 	return _left.m_Name.IsEqual(_right);
	// }

	public static bool operator !=(Enumeration _left,Enumeration _right)
	{
		return !(_left == _right);
	}

	// public static bool operator !=(Enumeration _left,string _right)
	// {
	// 	return !(_left == _right);
	// }

	public override int GetHashCode() => m_Name.GetHashCode();

	public override string ToString() => m_Name;

	public int CompareTo(object _object)
	{
		return _object is Enumeration data ? m_Name.CompareTo(data.m_Name) : -1;
	}
}