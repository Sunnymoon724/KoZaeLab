// using System;
// using System.Collections.Generic;
// using System.Reflection;
// using KZLib.KZDevelop;

// public static partial class CommonUtility
// {
	

// 	public static IEnumerable<TEnumeration> GetEnumerationGroup<TEnumeration>() where TEnumeration : Enumeration
// 	{
// 		var dataList = new List<TEnumeration>();

// 		var type = typeof(TEnumeration);
// 		var fieldInfo = type.GetField("m_Name",ENUMERATION_FLAG);

// 		if(fieldInfo != null)
// 		{
// 			dataList.Add(fieldInfo.GetValue(null) as TEnumeration);
// 		}

// 		return dataList;
// 	}

	

		
// }