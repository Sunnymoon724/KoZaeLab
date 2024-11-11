#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using ExcelDataReader;
using UnityEngine;

namespace KZLib.KZReader
{
	public class ExcelReader
	{
		[SerializeField]
		private DataSet m_DataSet = null;

		public ExcelReader(string _filePath)
		{
			CommonUtility.IsFileExist(_filePath,true);

			using var stream = new FileStream(_filePath,FileMode.Open,FileAccess.Read);
			using var reader = ExcelReaderFactory.CreateReader(stream);

			m_DataSet = reader.AsDataSet();
		}

		private DataTableCollection TableCollection
		{
			get
			{
				if(m_DataSet == null)
				{
					LogTag.System.E("DataSet is null in ExcelReader");

					return null;
				}

				return m_DataSet.Tables;
			}
		}

		public IEnumerable<string> SheetNameGroup
		{
			get
			{
				var tableCollection = TableCollection;

				for(var i=0;i<tableCollection.Count;i++)
				{
					yield return tableCollection[i].TableName;
				}
			}
		}

		private DataTable GetSheet(string _sheetName)
		{
			var tableCollection = TableCollection;

			if(tableCollection.Contains(_sheetName))
			{
				return tableCollection[_sheetName];
			}
			else
			{
				LogTag.System.E($"The sheet '{_sheetName}' does not exist in tableCollection.");

				return null;
			}
		}

		/// <summary>
		/// empty or #start -> not exist
		/// </summary>
		public bool IsExistRowArray(string _sheetName,int _idx)
		{
			var rowCollection = GetSheet(_sheetName).Rows;

			if(_idx < 0 || _idx >= rowCollection.Count)
			{
				LogTag.System.E($"{_idx} is out of range in rowCollection. [{_sheetName}]");

				return false;
			}

			var cellArray = rowCollection[_idx].ItemArray;

			if(cellArray.IsNullOrEmpty())
			{
				LogTag.System.E($"ItemArray is null for row {_idx} in sheet [{_sheetName}]");

				return false;
			}

			var header = cellArray[0].ToString();

			return !header.Contains("#") && !header.IsEmpty();
		}

		/// <summary>
		/// Get data group in row
		/// </summary>
		public string[] GetRowArray(string _sheetName,int _idx)
		{
			var rowCollection = GetSheet(_sheetName).Rows;

			if(_idx < 0 || _idx >= rowCollection.Count)
			{
				LogTag.System.E($"{_idx} is out of range in rowCollection. [{_sheetName}]");

				return null;
			}

			var cellArray = rowCollection[_idx].ItemArray;

			if(cellArray.IsNullOrEmpty())
			{
				LogTag.System.E($"ItemArray is null for row {_idx} in sheet [{_sheetName}]");

				return null;
			}

			if(!IsExistRowArray(_sheetName,0))
			{
				return null;
			}

			var rowArray = new string[cellArray.Length];

			for(var i=0;i<cellArray.Length;i++)
			{
				rowArray[i] = cellArray[i].ToString();
			}

			return rowArray;
		}

		/// <summary>
		/// Get data group in rows
		/// </summary>
		public string[][] GetRowJaggedArray(string _sheetName,params int[] _idxArray)
		{
			var jaggedList = new List<string[]>(_idxArray.Length);

			for(var i=0;i<_idxArray.Length;i++)
			{
				var rowArray = GetRowArray(_sheetName,_idxArray[i]);

				if(rowArray.IsNullOrEmpty())
				{
					continue;
				}

				jaggedList.Add(rowArray);
			}

			return jaggedList.ToArray();
		}

		/// <summary>
		/// Get data group in column
		/// </summary>
		public string[] GetColumnArray(string _sheetName,int _idx)
		{
			var sheet = GetSheet(_sheetName);

			if(_idx < 0 || _idx >= sheet.Columns.Count)
			{
				LogTag.System.E($"{_idx} is out of range in columnCollection. [{_sheetName}]");

				return null;
			}

			var columnArray = new string[sheet.Columns.Count];

			for(var i=0;i<sheet.Columns.Count;i++)
			{
				columnArray[i] = sheet.Columns[_idx].ToString();
			}

			return columnArray;
		}

		/// <summary>
		/// Get data group in columns
		/// </summary>
		public string[][] GetColumnJaggedArray(string _sheetName,params int[] _idxArray)
		{
			var jaggedArray = new string[_idxArray.Length][];

			for(var i=0;i<_idxArray.Length;i++)
			{
				jaggedArray[i] = GetColumnArray(_sheetName,_idxArray[i]);
			}

			return jaggedArray;
		}

		public IEnumerable<TData> Deserialize<TData>(string _sheetName,int _startRow = 1)
		{
			var type = typeof(TData);

			foreach(var result in Deserialize(_sheetName,type,_startRow))
			{
				yield return (TData) result;
			}
		}

		public IEnumerable<object> Deserialize(string _sheetName,Type _type,int _startRow = 1)
		{
			var sheet = GetSheet(_sheetName);
			var propertyInfoArray = _type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var rowCollection = sheet.Rows;

			var lastRow = rowCollection.Count;
			var startRow = Mathf.Clamp(_startRow,0,lastRow);

			var headerArray = GetRowArray(_sheetName,0);

			for(var i=startRow;i<=lastRow;i++)
			{
				var cellArray = rowCollection[i].ItemArray;

				if(cellArray.IsNullOrEmpty())
				{
					continue;
				}

				var data = Activator.CreateInstance(_type);

				for(var j=0;j<headerArray.Length;j++)
				{
					var cell = cellArray[j];

					if(cell == null)
					{
						continue;
					}

					var index = propertyInfoArray.IndexOf(x=>x.Name.IsEqual(headerArray[j]));

					if(index == -1)
					{
						continue;
					}

					var property = propertyInfoArray[index];

					if(property.CanWrite)
					{
						try
						{
							property.SetValue(data,ConvertData(cell.ToString(),property.PropertyType));
						}
						catch(Exception _ex)
						{
							LogTag.System.E($"There is a problem with the excel file. [sheet : {_sheetName} / error : {_ex.Message} / location : row({i+1})/column({headerArray[j]})]");
						}
					}
				}

				yield return data;
			}
		}

		/// <summary>
		/// Get data group in range (x,y -> start point, w,h -> range size)
		/// </summary>
		public string[,] ConvertToArray(string _sheetName,RectInt _range)
		{
			var sheet = GetSheet(_sheetName);
			var resultArray = new string[_range.width,_range.height];

			for(var i=_range.x;i<_range.x+_range.width;i++)
			{
				var cellArray = sheet.Rows[i].ItemArray;

				for(var j=_range.y;j<_range.y+_range.height;j++)
				{
					resultArray[i,j] = cellArray.IsNullOrEmpty() ? string.Empty : cellArray[j].ToString();
				}
			}

			return resultArray;
		}

		/// <summary>
		/// Get title & index
		/// </summary>
		public IEnumerable<(string Title,int Index)> GetTitleGroup(string _sheetName)
		{
			var headerArray = GetRowArray(_sheetName,0);

			for(var i=0;i<headerArray.Length;i++)
			{
				var header = headerArray[i];

				if(header.IsEmpty())
				{
					continue;
				}

				foreach(var keyword in KEY_WORD_ARRAY)
				{
					if(keyword.IsEqual(header))
					{
						LogTag.System.E($"{header} is invalid title.");

						yield break;
					}
				}

				yield return (header,i);
			}
		}

		private object ConvertData(string _cell,Type _type)
		{
			if(_type == typeof(string))
			{
				return _cell.NormalizeNewLines();
			}
			else if(_type.IsArray)
			{
				var dataArray = _cell.Replace(" ","").TrimEnd('&',' ').Split('&');

				if(dataArray.Length == 0)
				{
					return Array.CreateInstance(_type.GetElementType(),0);
				}

				var type = _type.GetElementType();
				var resultArray = Array.CreateInstance(type,dataArray.Length);

				for(var i=0;i<dataArray.Length;i++)
				{
					resultArray.SetValue(ConvertToObject(dataArray[i],type),i);
				}

				return resultArray;
			}
			else
			{
				return ConvertToObject(_cell,_type);
			}
		}

		private object ConvertToObject(string _text,Type _type)
		{
			if(_type.IsEnum)
			{
				return Enum.Parse(_type,_text);
			}
			else if(_type.Equals(typeof(Vector3)))
			{
				if(_text.TryToVector3(out var _result))
				{
					return _result;
				}
				else
				{
					LogTag.System.E($"{_text} is not vector3.");

					return null;
				}
			}
			else if(_type.IsPrimitive)
			{
				return Convert.ChangeType(_text,_type);
			}

			LogTag.System.E($"There is no type that can be cast from {_type}.");

			return null;
		}

		private static string[] KEY_WORD_ARRAY => new string[]
		{
			"abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", 
			"class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum",
			"event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto",
			"if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long",
			"namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected",
			"public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", 
			"string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
			"ushort", "using", "virtual", "void", "volatile", "while",
		};
	}
}
#endif