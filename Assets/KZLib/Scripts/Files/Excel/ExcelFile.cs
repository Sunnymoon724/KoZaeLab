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

namespace KZLib.KZFiles
{
	[Serializable]
	public partial class ExcelFile
	{
		[SerializeField]
		private IWorkbook m_Workbook = null;

		public ExcelFile(string _filePath)
		{
			CommonUtility.IsExistFile(_filePath,true);

			using var stream = new FileStream(_filePath,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
			var extension = CommonUtility.GetExtension(_filePath);

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

			throw new NullReferenceException("파일이 잘못 되었습니다.");
		}

		public IEnumerable<string> SheetNameGroup
		{
			get
			{
				var nameList = new List<string>();

				for(var i=0;i<m_Workbook.NumberOfSheets;i++)
				{
					nameList.Add(m_Workbook.GetSheetName(i));
				}

				return nameList;
			}
		}

		public Dictionary<string,string[]> GetEnumDict(string _sheetName)
		{
			var sheet = GetSheet(_sheetName);
			var explicitDict = new Dictionary<string,string[]>();

			foreach(var data in sheet.GetDataValidations())
			{
				// 데이터 유효성의 목록 가져오기
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

		private string GetHeader(ISheet _sheet,string[] _dataArray)
		{
			var index = 0;

			foreach(var row in GetRowGroup(_sheet.SheetName,1))
			{
				if(!_dataArray.Contains(row))
				{
					index++;

					continue;
				}

				return ParseCell(GetSheet(_sheet.SheetName).GetRow(0).GetCell(index));
			}

			return null;
		}

		private ISheet GetSheet(string _sheetName)
		{
			return m_Workbook.GetSheet(_sheetName) ?? throw new NullReferenceException("시트가 없습니다.");
		}

		public IEnumerable<(string,int)> GetTitleGroup(string _sheetName)
		{
			var titleList = new List<(string,int)>();

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

				titleList.Add((header,i));
			}

			return titleList;
		}

		/// <summary>
		/// _sheetName의 _row번째 행의 값을 가져옵니다.
		/// </summary>
		public IEnumerable<string> GetRowGroup(string _sheetName,int _row)
		{
			var sheet = GetSheet(_sheetName);
			var row = sheet.GetRow(_row) ?? throw new NullReferenceException(string.Format("{0}에서 {1}번째 행의 값이 비어있습니다.",_sheetName,_row));

			var rowList = new List<string>();

			for(var i=0;i<=row.LastCellNum;i++)
			{
				rowList.Add(ParseCell(row.GetCell(i)));
			}

			return rowList;
		}

		/// <summary>
		/// _sheetName의 (_rowArray 번째) 행의 값을 가져옵니다.
		/// </summary>
		public IEnumerable<string>[] GetRowGroupArray(string _sheetName,int[] _rowArray)
		{
			var sheet = GetSheet(_sheetName);
			var resultArray = new List<string>[_rowArray.Length];
			var textList = new List<string>();

			for(var i=0;i<_rowArray.Length;i++)
			{
				var row = sheet.GetRow(_rowArray[i]) ?? throw new NullReferenceException(string.Format("{0}에서 {1}번째 행의 값이 비어있습니다.",_sheetName,_rowArray[i]));

				textList.Clear();

				for(var j=0;j<row.LastCellNum;j++)
				{
					textList.Add(ParseCell(row.GetCell(j)));
				}

				resultArray[i] = new List<string>(textList);
			}

			return resultArray;
		}

		/// <summary>
		/// _sheetName의 _column번째 열의 값을 가져옵니다.
		/// </summary>
		public IEnumerable<string> GetColumnGroup(string _sheetName,int _column)
		{
			var sheet = GetSheet(_sheetName);
			var resultList = new List<string>();

			for(var i=0;i<=sheet.LastRowNum;i++)
			{
				var row = sheet.GetRow(i);

				resultList.Add(row == null ? string.Empty : ParseCell(row.GetCell(_column)));
			}

			return resultList;
		}

		/// <summary>
		/// _sheetName의 (_columnArray 번째) 열의 값을 가져옵니다.
		/// </summary>
		public IEnumerable<string>[] GetColumnGroupArray(string _sheetName,int[] _columnArray)
		{
			var sheet = GetSheet(_sheetName);
			var resultArray = new List<string>[_columnArray.Length];
			var textList = new List<string>();

			for(var i=0;i<_columnArray.Length;i++)
			{
				textList.Clear();

				for(var j=0;j<=sheet.LastRowNum;j++)
				{
					var row = sheet.GetRow(j);

					textList.Add(row == null ? string.Empty : ParseCell(row.GetCell(_columnArray[i])));
				}

				resultArray[i] = new List<string>(textList);
			}

			return resultArray;
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