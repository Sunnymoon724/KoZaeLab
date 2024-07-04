#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System;
using NPOI.SS.UserModel;
using System.Reflection;

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
					resultArray[i,j] = row == null ? string.Empty : ParseCell(row.GetCell(j));
				}
			}

			return resultArray;
		}

		private object ConvertData(ICell _cell,Type _type)
		{
			var text = GetCellText(_cell);

			if(_type.Equals(typeof(string)))
			{
				return text.NormalizeNewLines();
			}
			else if(_type.IsArray)
			{
				var dataArray = text.Replace(" ","").TrimEnd('&',' ').Split('&');
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
				return ConvertToObject(text,_type);
			}
		}

		private object ConvertToObject(string _text,Type _type)
		{
			if(_type.IsEnum)
			{
				if(Enum.TryParse(_type,_text,out var value))
				{
					return value;
				}

				throw new ArgumentException(string.Format("{0}는 {1}의 enum이 아닙니다.",_text,_type.Name));
			}
			else if(_type.Equals(typeof(Vector3)))
			{
				if(_text.TryToVector3(out var _result))
				{
					return _result;
				}

				throw new ArgumentException(string.Format("{0}는 vector3가 아닙니다.",_text));
			}
			else if(_type.Equals(typeof(short)))
			{
				if(short.TryParse(_text,out var value))
				{
					return value;
				}

				throw new ArgumentException(string.Format("{0}는 short가 아닙니다.",_text));
			}
			else if(_type.Equals(typeof(int)))
			{
				if(int.TryParse(_text,out var value))
				{
					return value;
				}

				throw new ArgumentException(string.Format("{0}는 int가 아닙니다.",_text));
			}
			else if(_type.Equals(typeof(long)))
			{
				if(long.TryParse(_text,out var value))
				{
					return value;
				}

				throw new ArgumentException(string.Format("{0}는 long이 아닙니다.",_text));
			}
			else if(_type.Equals(typeof(float)))
			{
				if(float.TryParse(_text,out var value))
				{
					return value;
				}

				throw new ArgumentException(string.Format("{0}는 float이 아닙니다.",_text));
			}
			else if(_type.Equals(typeof(double)))
			{
				if(double.TryParse(_text,out var value))
				{
					return value;
				}

				throw new ArgumentException(string.Format("{0}는 double이 아닙니다.",_text));
			}
			else if(_type.Equals(typeof(bool)))
			{
				if(bool.TryParse(_text,out var value))
				{
					return value;
				}

				throw new ArgumentException(string.Format("{0}는 bool이 아닙니다.",_text));
			}

			throw new InvalidCastException(string.Format("{0}을 캐스팅 할 수 있는 타입이 없습니다.",_type));
		}

		private string GetCellText(ICell _cell)
		{
			return (_cell.CellType == CellType.Numeric || _cell.CellType == CellType.Formula) ? string.Format("{0}",_cell.NumericCellValue) : _cell.StringCellValue;
		}

		private string ParseCell(ICell _cell)
		{
			return _cell == null ? string.Empty : _cell.ToString();
		}
	}
}
#endif