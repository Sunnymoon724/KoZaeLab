using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
/// 참고 레퍼런스
/// </summary>

namespace KZLib.KZDevelop
{
	public abstract class Enumeration : IComparable
	{
		private readonly string m_Name = null;

		protected Enumeration(string _name)
		{
			m_Name = _name;
		}

		public static List<string> GetNames<TEnumeration>() where TEnumeration : Enumeration
		{
			return GetDataList<string,TEnumeration>(x=>x.ToString());
		}

		public static List<TEnumeration> GetValueList<TEnumeration>() where TEnumeration : Enumeration
		{
			return GetDataList<TEnumeration,TEnumeration>(x=>x);
		}

		private static List<TValue> GetDataList<TValue,TEnumeration>(Func<TEnumeration,TValue> _condition) where TEnumeration : Enumeration
		{
			var dataList = new List<TValue>();

			foreach(var fieldInfo in GetFieldInfoArray<TEnumeration>())
			{
				var value = fieldInfo.GetValue(null) as TEnumeration;

				if(value != null)
				{
					dataList.Add(_condition(value));
				}
			}

			return dataList;
		}

		private static FieldInfo[] GetFieldInfoArray<TEnumeration>() where TEnumeration : Enumeration
		{
			return typeof(TEnumeration).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
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
			var dataList = GetValueList<TEnumeration>();

			var index = dataList.FindIndex(x=>x.m_Name.IsEqual(_name));

			if(index == -1)
			{
				_result = null;

				return false;
			}
			else
			{
				_result = dataList[index];

				return true;
			}
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

		public static bool operator !=(Enumeration _left,Enumeration _right)
		{
			return !(_left == _right);
		}

		public override int GetHashCode() => m_Name.GetHashCode();

		public override string ToString() => m_Name;

		public int CompareTo(object _object)
		{
			return _object is Enumeration data ? m_Name.CompareTo(data.m_Name) : -1;
		}
	}
}