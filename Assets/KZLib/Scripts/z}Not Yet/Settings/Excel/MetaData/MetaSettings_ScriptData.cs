#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

/// <summary>
/// 시트 파일 사용 세팅 시
/// </summary>
public partial class MetaSettings : ExcelSettings<MetaSettings>
{
	[Serializable]
	private class ScriptData
	{
		public string SheetName { get; }
		public string ClassName { get; }

		private readonly List<ExcelCellData> m_MemberDataList = null;
		
		public ScriptData(string _className,string _sheetName,List<ExcelCellData> _headerList)
		{
			SheetName = _sheetName;
			ClassName = _className;
			m_MemberDataList = new List<ExcelCellData>(_headerList);
		}

		protected string MergeMemberData(Func<ExcelCellData,string> _onGetData,string _spaceText)
		{
			var builder = new StringBuilder();

			builder.Append(_onGetData(m_MemberDataList[0]));

			for(var i=1;i<m_MemberDataList.Count;i++)
			{
				builder.Append(_spaceText);
				builder.Append(_onGetData(m_MemberDataList[i]));
			}

			return builder.ToString();
		}

		public string MemberFields => MergeMemberData((member)=>{ return member.ToFieldText(); },string.Format("{0}\t\t",Environment.NewLine));
		
		public string MemberProperties => MergeMemberData((member)=>{ return member.ToPropertyText(); },string.Format("{0}{0}\t\t",Environment.NewLine));

		public bool IsInEnum()
		{
			foreach(var data in m_MemberDataList.Where(x=>x.IsEnumType))
			{
				return true;
			}

			return false;
		}

		public void WriteScript(string _scriptPath)
		{
			if(FileUtility.IsExist(_scriptPath))
			{
				LogTag.Editor.W("스크립트가 이미 존재하여 생성을 생략합니다. [경로 :{0}]",_scriptPath);

				return;
			}

			var data = FileUtility.GetTemplateText("MetaTable.txt");

			data = data.Replace("$ClassName",ClassName);
			data = data.Replace("$MemberFields",MemberFields);
			data = data.Replace("$MemberProperties",MemberProperties);
			data = data.Replace("$SheetName",SheetName);

			FileUtility.WriteTextToFile(_scriptPath,data);
		}
	}
}
#endif