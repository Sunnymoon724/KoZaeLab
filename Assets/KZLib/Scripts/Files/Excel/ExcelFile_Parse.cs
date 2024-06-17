#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System;
using NPOI.SS.UserModel;
using System.Reflection;
using System.ComponentModel;
using System.Linq;

namespace KZLib.KZFiles
{
	public partial class ExcelFile
	{
		public IEnumerable<TData> Deserialize<TData>(string _sheetName,int _startRow = 1)
		{
			var sheet = GetSheet(_sheetName);
			var dataList = new List<TData>();
			var propertyArray = typeof(TData).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			var lastRow = sheet.LastRowNum;
			var startRow = Mathf.Clamp(_startRow,0,lastRow);

			var columnCount = sheet.GetRow(0).LastCellNum; // 헤드는 중간에 빈칸이 없으므로.
			var headerRow = sheet.GetRow(0);

			for(var i=startRow;i<=lastRow;i++)
			{
				var row = sheet.GetRow(i);

				if(row == null)
				{
					continue;
				}

				var data = Activator.CreateInstance<TData>();
				var flag = false;

				for(var j=0;j<=columnCount;j++)
				{
					var cell = row.GetCell(j);

					if(cell == null || cell.CellType == CellType.Blank)
					{
						//? 빈 셀 스킵
						continue;
					}

					var header = ParseCell(headerRow.GetCell(j));
					var index = propertyArray.FindIndex(x=>x.Name.IsEqual(header));

					if(index == -1)
					{
						continue;
					}

					var property = propertyArray[index];

					if(property.CanWrite)
					{
						try
						{
							flag = true;
							property.SetValue(data,ConvertData(cell,property.PropertyType),null);
						}
						catch(Exception _ex)
						{
							Log.Files.F(string.Format("엑셀 파일에 문제가 있습니다. 시트:{0} 오류:{1} 위치: {2}",_sheetName,_ex.Message,string.Format("행[{0}]/열[{1}]",i+1,header)));
						}
					}
				}

				if(flag)
				{
					dataList.Add(data);
				}
			}

			return dataList;
		}

		/// <summary>
		/// _sheetName의 시트를 _range로 지정한 구역만큼 가지고 옵니다.(_range에서 x,y는 시작 지점 w,h는 크기)
		/// </summary>
		public string[,] ConvertToArray(string _sheetName,RectInt _range)
		{
			var sheet = GetSheet(_sheetName);
			var resultArray = new string[_range.width,_range.height];

			for(var i=_range.x;i<_range.x+_range.width;i++)
			{
				var row = sheet.GetRow(i);

				for(var j=_range.y;j<_range.y+_range.height;j++)
				{
					if(row == null)
					{
						resultArray[i,j] = string.Empty;
					}
					else
					{
						resultArray[i,j] = ParseCell(row.GetCell(j));
					}
				}
			}

			return resultArray;
		}

		private object ConvertData(ICell _cell,Type _type)
		{
			var data = ConvertCell(_cell,_type) ?? throw new InvalidCastException(string.Format("{0}을 캐스팅 할 수 있는 타입이 없습니다.",_type));

			if(_type.IsGenericType && _type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
			{
				return new NullableConverter(_type).ConvertFrom(data);
			}
			else if(_type.IsEnum)
			{
				if(Enum.TryParse(_type,_cell.StringCellValue,out var value))
				{
					return value;
				}

				throw new InvalidCastException(string.Format("{0}을 캐스팅 하는데 문제가 있습니다. [{1}]",_type,_cell.StringCellValue));
			}
			else if(_type.Equals(typeof(Vector3)))
			{
				return ConvertVector3(data);
			}
			else if(_type.IsArray)
			{
				return ConvertArrayData(data,_type.GetElementType());
			}

			return Convert.ChangeType(data,_type);
		}

		private object ConvertCell(ICell _cell,Type _type)
		{
			if(_type.Equals(typeof(float)) || _type.Equals(typeof(double)) || _type.Equals(typeof(short)) || _type.Equals(typeof(int)) || _type.Equals(typeof(long)))
			{
				if(_cell.CellType == CellType.Numeric || _cell.CellType == CellType.Formula)
				{
					return ConvertObject(_cell.NumericCellValue,_type);
				}
				else if(_cell.CellType == CellType.String)
				{
					return ConvertObject(_cell.StringCellValue,_type);
				}
			}
			else if(_type.Equals(typeof(Vector3)))
			{
				return _cell.StringCellValue;
			}
			else if(_type.Equals(typeof(string)) || _type.IsArray)
			{
				return (_cell.CellType == CellType.Numeric) ? _cell.NumericCellValue : _cell.StringCellValue;
			}
			else if(_type.Equals(typeof(bool)))
			{
				return _cell.BooleanCellValue;
			}

			return null;
		}

		private object ConvertObject(object _object,Type _type)
		{
			if(_object != null)
			{
				if(_type.Equals(typeof(float)))
				{
					return Convert.ToSingle(_object);
				}

				if(_type.Equals(typeof(double)))
				{
					return Convert.ToDouble(_object);
				}

				if(_type.Equals(typeof(short)))
				{
					return Convert.ToInt16(_object);
				}

				if(_type.Equals(typeof(int)))
				{
					return Convert.ToInt32(_object);
				}

				if(_type.Equals(typeof(long)))
				{
					return Convert.ToInt64(_object);
				}
			}

			throw new FormatException(string.Format("바꿀 수 있는 포맷이 없습니다. [오브젝트 : {0}/ 타입 : {1}]",_object,_type));
		}

		private object ConvertArrayData(object _object,Type _type)
		{
			if(_object != null)
			{
				var dataArray = _object.ToString().Replace(" ","").TrimEnd('&',' ').Split('&');

				if(_type.Equals(typeof(float)))
				{
					return dataArray.Select(x=>Convert.ToSingle(x)).ToArray();
				}

				if(_type.Equals(typeof(double)))
				{
					return dataArray.Select(x=>Convert.ToDouble(x)).ToArray();
				}

				if(_type.Equals(typeof(short)))
				{
					return dataArray.Select(x=>Convert.ToInt16(x)).ToArray();
				}

				if(_type.Equals(typeof(int)))
				{
					return dataArray.Select(x=>Convert.ToInt32(x)).ToArray();
				}

				if(_type.Equals(typeof(long)))
				{
					return dataArray.Select(x=>Convert.ToInt64(x)).ToArray();
				}

				if(_type.Equals(typeof(string)))
				{
					return dataArray;
				}
			}

			throw new FormatException(string.Format("바꿀 수 있는 포맷이 없습니다. [오브젝트 : {0}/ 타입 : {1}]",_object,_type));
		}

		private object ConvertVector3(object _object)
		{
			return _object.ToString().ToVector3();
		}

		private string ParseCell(ICell _cell)
		{
			return _cell == null ? string.Empty : _cell.ToString();
		}
	}
}
#endif