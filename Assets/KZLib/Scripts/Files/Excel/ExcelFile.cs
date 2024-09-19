#if UNITY_EDITOR
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Text.RegularExpressions;
using Sirenix.Utilities;

namespace KZLib.KZFiles
{
	[Serializable]
	public class ExcelFile
	{
		[SerializeField]
		private IWorkbook m_Workbook = null;

		public ExcelFile(string _filePath)
		{
			FileUtility.IsExist(_filePath,true);

			using var stream = new FileStream(_filePath,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
			var extension = FileUtility.GetExtension(_filePath);

			if(extension.IsEqual(".xls"))
			{
				m_Workbook = new HSSFWorkbook(stream);

				return;
			}
			else if(extension.IsEqual(".xlsx") || extension.IsEqual(".xlsm"))
			{
				m_Workbook = new XSSFWorkbook(stream);

				return;
			}

			throw new ArgumentException("파일이 잘못 되었습니다.");
		}

		public IEnumerable<string> SheetNameGroup
		{
			get
			{
				for(var i=0;i<m_Workbook.NumberOfSheets;i++)
				{
					yield return m_Workbook.GetSheetName(i);
				}
			}
		}

		/// <summary>
		/// 엑셀 시트의 데이터 유효성을 열거형 딕셔너리로 반환합니다.
		/// </summary>
		public Dictionary<string,string[]> GetEnumDict(string _sheetName)
		{
			var sheet = GetSheet(_sheetName);
			var explicitDict = new Dictionary<string,string[]>();

			foreach(var data in sheet.GetDataValidations())
			{
				var explicitArray = data.ValidationConstraint.ExplicitListValues;

				if(explicitArray.IsNullOrEmpty())
				{
					continue;
				}

				for(var i=0;i<explicitArray.Length;i++)
				{
					explicitArray[i] = Regex.Replace(explicitArray[i],@"[^0-9a-zA-Z_]+",string.Empty);
				}

				var header = GetHeader(sheet,explicitArray);

				if(header == null || explicitDict.ContainsKey(header))
				{
					continue;
				}

				explicitDict.Add(header,explicitArray);
			}

			return explicitDict;
		}

		/// <summary>
		/// 엑셀 시트의 데이터 유효성을 1행으로 확인하여, 해당 열의 header를 반환합니다.
		/// </summary>
		private string GetHeader(ISheet _sheet,string[] _dataArray)
		{
			var row = GetSheet(_sheet.SheetName).GetRow(1);

			for(var i=0;i<row.LastCellNum;i++)
			{
				if(!_dataArray.Contains(ParseCell(row.GetCell(i))))
				{
					continue;
				}

				return ParseCell(GetSheet(_sheet.SheetName).GetRow(0).GetCell(i));
			}

			return null;
		}

		private ISheet GetSheet(string _sheetName)
		{
			return m_Workbook.GetSheet(_sheetName) ?? throw new NullReferenceException("시트가 없습니다.");
		}

		/// <summary>
		/// 열의 제목과 인덱스를 반환합니다.
		/// </summary>
		public IEnumerable<(string,int)> GetTitleGroup(string _sheetName)
		{
			var headerList = GetRowGroup(_sheetName,0).ToList();

			for(var i=0;i<headerList.Count;i++)
			{
				var header = headerList[i];

				if(header.IsEmpty())
				{
					continue;
				}

				if(KEY_WORD_ARRAY.Any(x=>x.IsEqual(header.ToLowerInvariant())))
				{
					LogTag.File.W("{0}는 헤더로 사용할 수 없습니다.",header);

					continue;
				}

				yield return (header,i);
			}
		}

		/// <summary>
		/// sheetName의 row번째 행의 값을 가져옵니다.
		/// </summary>
		public IEnumerable<string> GetRowGroup(string _sheetName,int _row)
		{
			var sheet = GetSheet(_sheetName);
			var row = sheet.GetRow(_row) ?? throw new NullReferenceException(string.Format("{0}에서 {1}번째 행의 값이 비어있습니다.",_sheetName,_row));

            return Enumerable.Range(0,row.LastCellNum).Select(i => ParseCell(row.GetCell(i)));
		}

		/// <summary>
		/// _sheetName의 (_rowArray 번째) 행의 값을 가져옵니다.
		/// </summary>
		public IEnumerable<string>[] GetRowGroupArray(string _sheetName,int[] _rowArray)
		{
			var sheet = GetSheet(_sheetName);

			return _rowArray.Select(i => sheet.GetRow(i) ?? throw new NullReferenceException(string.Format("{0}에서 {1}번째 행의 값이 비어있습니다.",_sheetName,i))).Select(row => Enumerable.Range(0,row.LastCellNum).Select(j => ParseCell(row.GetCell(j))).ToArray()).ToArray();
		}

		/// <summary>
		/// _sheetName의 _column번째 열의 값을 가져옵니다.
		/// </summary>
		public IEnumerable<string> GetColumnGroup(string _sheetName,int _column)
		{
			var sheet = GetSheet(_sheetName);

			return Enumerable.Range(0,sheet.LastRowNum+1).Select(i =>
			{
				var row = sheet.GetRow(i);
				return row == null ? string.Empty : ParseCell(row.GetCell(_column));
			});
		}

		/// <summary>
		/// _sheetName의 (_columnArray 번째) 열의 값을 가져옵니다.
		/// </summary>
		public IEnumerable<string>[] GetColumnGroupArray(string _sheetName,int[] _columnArray)
		{
			var sheet = GetSheet(_sheetName);

			return _columnArray.Select(column => Enumerable.Range(0,sheet.LastRowNum+1).Select(rowNum =>
			{
				var row = sheet.GetRow(rowNum);
				return row == null ? string.Empty : ParseCell(row.GetCell(column));
			})).ToArray();
		}

		public IEnumerable<TData> Deserialize<TData>(string _sheetName,int _startRow = 1)
		{
			var sheet = GetSheet(_sheetName);
			var propertyInfoArray = typeof(TData).GetProperties(Flags.InstanceAnyVisibility);
			var lastRow = sheet.LastRowNum;
			var startRow = Mathf.Clamp(_startRow,0,lastRow);

			var headerRow = sheet.GetRow(0);
			var headerArray = Enumerable.Range(0,headerRow.LastCellNum).Select(i => ParseCell(headerRow.GetCell(i))).ToArray();

			for(var i=startRow;i<=lastRow;i++)
			{
				var row = sheet.GetRow(i);

				if(row == null)
				{
					continue;
				}

				var data = Activator.CreateInstance<TData>();

				for(var j=0;j<headerArray.Length;j++)
				{
					var cell = row.GetCell(j);

					if(cell == null || cell.CellType == CellType.Blank)
					{
						continue;
					}

					var index = propertyInfoArray.FindIndex(x=>x.Name.IsEqual(headerArray[j]));

					if(index == -1)
					{
						continue;
					}

					var property = propertyInfoArray[index];

					if(property.CanWrite)
					{
						try
						{
							property.SetValue(data,ConvertData(cell,property.PropertyType),null);
						}
						catch(Exception _ex)
						{
							LogTag.File.E(string.Format("엑셀 파일에 문제가 있습니다. 시트:{0} 오류:{1} 위치: {2}",_sheetName,_ex.Message,string.Format("행[{0}]/열[{1}]",i+1,headerArray[j])));
						}
					}
				}

				yield return data;
			}
		}

		/// <summary>
		/// sheetName의 시트를 range로 지정한 구역만큼 가지고 옵니다.(range에서 x,y는 시작 지점 w,h는 크기)
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

		/// <summary>
		/// 엑셀 셀의 데이터를 파싱하여, type 형식에 맞게 반환합니다.
		/// </summary>
		private object ConvertData(ICell _cell,Type _type)
		{
			var text = GetCellText(_cell);

			if(_type == typeof(string))
			{
				return text.NormalizeNewLines();
			}
			else if(_type.IsArray)
			{
				var dataArray = text.Replace(" ","").TrimEnd('&',' ').Split('&');

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
				return ConvertToObject(text,_type);
			}
		}

        /// <summary>
        /// text를 type 형식에 맞게 반환합니다.
        /// </summary>
		private object ConvertToObject(string _text,Type _type)
		{
			if(_type.IsEnum)
			{
				return Enum.Parse(_type,_text);
			}
			else if(_type.Equals(typeof(Vector3)))
			{
				return _text.TryToVector3(out var _result) ? _result : throw new ArgumentException(string.Format("{0}는 vector3가 아닙니다.",_text));
			}
			else if(_type.IsPrimitive)
			{
				return Convert.ChangeType(_text,_type);
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